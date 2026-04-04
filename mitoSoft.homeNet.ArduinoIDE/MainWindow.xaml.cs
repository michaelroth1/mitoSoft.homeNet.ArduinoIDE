using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;
using mitoSoft.homeNet.ArduinoIDE.Services;
using mitoSoft.homeNet.ArduinoIDE.Views;
using Res = mitoSoft.homeNet.ArduinoIDE.Properties.Resources;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using HomeNet = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.HomeNet;

namespace mitoSoft.homeNet.ArduinoIDE;

public partial class MainWindow : Window
{
    private string _currentFileName = string.Empty;

    private readonly DispatcherTimer _statusClearTimer = new();

    private readonly SettingsService _settingsService;
    private readonly FileService _fileService;
    private readonly FocusService _viewFocusService = new();
    private MainPanelService _documentService = null!;

    public ICommand FindCommand { get; }
    public ICommand SaveCommand { get; }

    public MainWindow()
    {
        InitializeComponent();

        _statusClearTimer.Interval = TimeSpan.FromSeconds(5);
        _statusClearTimer.Tick += StatusClearTimer_Tick;

        FindCommand = new RelayCommand(_ => _viewFocusService.FocusedEditorView?.ShowFindBar(string.Empty));
        SaveCommand = new RelayCommand(_ => this.Save());
        DataContext = this;

        _settingsService = new SettingsService();
        _fileService = new FileService();

        this.LoadSettings();
        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
    }

    private void LoadSettings()
    {
        _settingsService.LoadWindowSettings(out double width, out double height, out double zoomFactor);

        if (width > 0 && height > 0)
        {
            Width = width;
            Height = height;
        }

        if (zoomFactor > 0)
        {
            ZoomSlider.Value = zoomFactor;
            // Note: ApplyZoom will be called in MainWindow_Loaded after UI is initialized
        }
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _documentService = new MainPanelService(DocumentPane, () => ZoomSlider.Value, () => DockingManager.ActiveContent);
        _documentService.DocumentViewAdded += (s, view) => _viewFocusService.Track(view);

        _viewFocusService.SetInitialView(YamlView);

        var lastFile = _settingsService.GetLastOpenedYamlFile();
        if (!string.IsNullOrEmpty(lastFile) && File.Exists(lastFile))
        {
            _currentFileName = lastFile;
            YamlView.Text = _fileService.ReadFile(_currentFileName);
            SetStatusText(string.Format(Res.Msg_Opened, _currentFileName));
        }

        // Subscribe to TextChanged event
        YamlView.TextChanged += (s, args) => this.UpdateControllerList();

        // Apply zoom after UI is fully initialized
        this.ApplyZoom();

        this.UpdateControllerList();
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _settingsService.SaveWindowSettings(Width, Height, ZoomSlider.Value);
    }

    private void SetStatusText(string text)
    {
        StatusText.Text = text;
        _statusClearTimer.Stop();
        _statusClearTimer.Start();
    }

    private void StatusClearTimer_Tick(object? sender, EventArgs e)
    {
        _statusClearTimer.Stop();
        StatusText.Text = string.Empty;
    }

    private void UpdateControllerList()
    {
        var controllers = new TextCrawler(YamlView.Text).ParseHomeNetControllers();

        var selectedItem = ControllerListBox.SelectedItem;

        ControllerListBox.Items.Clear();
        foreach (var controller in controllers)
        {
            ControllerListBox.Items.Add(controller);
        }

        if (selectedItem != null && ControllerListBox.Items.Contains(selectedItem))
        {
            ControllerListBox.SelectedItem = selectedItem;
        }
    }

    private void ControllerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ErrorTextBox.Text = string.Empty;
    }

    private void CheckButton_Click(object sender, RoutedEventArgs e)
    {
        this.CheckErrors();
    }

    private void CheckErrors()
    {
        ErrorTextBox.Text = string.Empty;

        string? controllerName = ControllerListBox.SelectedItem as string;

        if (string.IsNullOrWhiteSpace(controllerName))
        {
            ErrorTextBox.Text = Res.Msg_NoControllerSelected;
            return;
        }

        var parser = new YamlParser(YamlView.Text);
        var controller = parser.GetController(controllerName);

        if (controller == null)
        {
            ErrorTextBox.Text = Res.Msg_ControllerNotFound;
            return;
        }

        var controllerConfig = parser.Parse(controller.UniqueId);

        ErrorTextBox.Text += controllerConfig.GetGpioErrors();
        ErrorTextBox.Text += controllerConfig.GetPartnerWarnings();

        if (string.IsNullOrWhiteSpace(ErrorTextBox.Text))
        {
            ErrorTextBox.Text = Res.Msg_NoWarningsFound;
        }
    }

    private void BuildButton_Click(object sender, RoutedEventArgs e)
    {
        this.CheckErrors();

        var controller = this.GetController();

        if (controller == null)
        {
            return;
        }

        try
        {
            var config = new YamlParser(YamlView.Text).Parse(controller.UniqueId);

            var programBuilder = new ProgramTextBuilder(
                controller.Name,
                controller.IPAddress,
                controller.MacAddress,
                controller.BrokerIPAddress,
                controller.BrokerUserName,
                controller.BrokerPassword,
                controller.GpioMode,
                controller.SubscribedTopic,
                controller.AdditionalDeclaration,
                controller.AdditionalSetup,
                controller.AdditionalCode);

            var program = programBuilder.Build(config);

            var view = _documentService.CreateOrUpdateOutputDocument(controller.Name, program);
            _viewFocusService.SetFocusedView(view);

            programBuilder.Check();

            SetStatusText(string.Format(Res.Msg_BuildSuccessful, controller.Name));
        }
        catch (Exception ex)
        {
            SetStatusText(string.Format(Res.Msg_BuildFailed, controller.Name));
            throw new InvalidOperationException(string.Format(Res.Msg_BuildError, controller.Name, ex.Message), ex);
        }
    }

    private HomeNet.Controller? GetController()
    {
        string? controllerName = ControllerListBox.SelectedItem as string;

        if (string.IsNullOrWhiteSpace(controllerName))
        {
            return null;
        }

        var controller = new YamlParser(YamlView.Text).GetController(controllerName);
        return controller;
    }

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var filePath = _fileService.OpenYamlFile();
        if (filePath != null)
        {
            _currentFileName = filePath;
            YamlView.Text = _fileService.ReadFile(_currentFileName);
            _settingsService.SaveLastOpenedYamlFile(_currentFileName);
            SetStatusText(string.Format(Res.Msg_Opened, _currentFileName));
        }
    }

    private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
    {
        this.Save();
    }

    private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        this.SaveAs();
    }

    private void Save()
    {
        if (_viewFocusService.FocusedView is YamlView &&
            !string.IsNullOrEmpty(_currentFileName) &&
            File.Exists(_currentFileName))
        {
            _fileService.WriteFile(_currentFileName, YamlView.Text);
            SetStatusText(string.Format(Res.Msg_Saved, _currentFileName));
        }
        else
        {
            this.SaveAs();
        }
    }

    private void SaveAs()
    {
        if (_viewFocusService.FocusedView is YamlView)
        {
            var saved = this.SaveToFile(
                filter: Res.Filter_Yaml,
                defaultFileName: "config.yaml",
                content: YamlView.Text,
                currentFilePath: _currentFileName);

            if (saved != null)
                _currentFileName = saved;
        }
        else if (_viewFocusService.FocusedView is OutputView outputView)
        {
            var doc = _documentService.GetAllDocuments().FirstOrDefault(d => d.Content == outputView);
            var controllerName = doc?.Title.Replace("Output: ", "") ?? "output";
            this.SaveToFile(
                filter: Res.Filter_Arduino,
                defaultFileName: $"{controllerName}.ino",
                content: outputView.GetTextEditor().Text);
        }
    }

    private string? SaveToFile(string filter,
        string defaultFileName,
        string content,
        string? currentFilePath = null)
    {
        var filePath = _fileService.ShowSaveDialog(
            filter: filter,
            defaultFileName: defaultFileName,
            currentFilePath: currentFilePath);

        if (filePath != null)
        {
            _fileService.WriteFile(filePath, content);
            SetStatusText(string.Format(Res.Msg_Saved, filePath));
        }

        return filePath;
    }

    private void CopyToClipboardMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var textEditor = _viewFocusService.FocusedEditorView?.GetTextEditor();
        if (!string.IsNullOrEmpty(textEditor?.Text))
        {
            Clipboard.SetText(textEditor.Text);
            SetStatusText(Res.Msg_CopiedToClipboard);
        }
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void MissingHomeNetElementsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var newConfig = new YamlParser(YamlView.Text).AddHomeNetElements();

        _documentService.CreateOrUpdateMissingHomeNetElementsDocument(newConfig);
    }

    private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        this.ApplyZoom();
    }

    private void ApplyZoom()
    {
        _documentService?.UpdateZoomForAllEditorViews(ZoomSlider.Value);
    }

    private void FindMenuItem_Click(object sender, RoutedEventArgs e)
    {
        _viewFocusService.FocusedEditorView?.ShowFindBar(string.Empty);
    }

    private void ViewControllersMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (ViewControllersMenuItem.IsChecked)
        {
            ControllersAnchorable.Show();
        }
        else
        {
            ControllersAnchorable.Hide();
        }
    }

    private void ViewErrorsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (ViewErrorsMenuItem.IsChecked)
        {
            ErrorsAnchorable.Show();
        }
        else
        {
            ErrorsAnchorable.Hide();
        }
    }

    private void GpioDocumentationMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var overviews = DocumentationService.GenerateOverview(YamlView.Text);

        if (overviews.Count == 0)
        {
            throw new Exception(Res.Msg_NoControllersWithGpios);
        }

        _documentService.CreateOrUpdateGpioDocumentation(overviews);
        SetStatusText(Res.Msg_GpioDocumentationCreated);
    }

    private void LayoutAnchorable_Hiding(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (sender == ControllersAnchorable)
        {
            ViewControllersMenuItem.IsChecked = false;
        }
        else if (sender == ErrorsAnchorable)
        {
            ViewErrorsMenuItem.IsChecked = false;
        }
    }
}