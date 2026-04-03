using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;
using mitoSoft.homeNet.ArduinoIDE.WPF.Services;
using mitoSoft.homeNet.ArduinoIDE.WPF.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HomeNet = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.HomeNet;

namespace mitoSoft.homeNet.ArduinoIDE.WPF;

public partial class MainWindow : Window
{
    private string _currentFilePath = string.Empty;

    private readonly SettingsService _settingsService;
    private readonly FileService _fileService;
    private readonly DocumentationService _gpioDocumentationService;
    private readonly FocusService _viewFocusService = new();
    private MainPanelService _documentService = null!;

    public ICommand FindCommand { get; }

    public MainWindow()
    {
        InitializeComponent();
        FindCommand = new RelayCommand(_ => _viewFocusService.FocusedEditorView?.ShowFindBar(string.Empty));
        DataContext = this;

        _settingsService = new SettingsService();
        _fileService = new FileService();
        _gpioDocumentationService = new DocumentationService();

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

        YamlView.Text = _settingsService.GetYamlContent();

        // Subscribe to TextChanged event
        YamlView.TextChanged += (s, args) =>
        {
            this.SaveYamlInSettings();
            this.UpdateControllerList();
        };

        // Apply zoom after UI is fully initialized
        this.ApplyZoom();

        this.UpdateControllerList();
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _settingsService.SaveWindowSettings(Width, Height, ZoomSlider.Value);
    }

    private void SaveYamlInSettings()
    {
        _settingsService.SaveYamlContent(YamlView.Text);
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
            ErrorTextBox.Text = "No controller selected...";
            return;
        }

        var parser = new YamlParser(YamlView.Text);
        var controller = parser.GetController(controllerName);

        if (controller == null)
        {
            ErrorTextBox.Text = "Controller not found...";
            return;
        }

        var controllerConfig = parser.Parse(controller.UniqueId);

        ErrorTextBox.Text += controllerConfig.GetGpioErrors();
        ErrorTextBox.Text += controllerConfig.GetPartnerWarnings();

        if (string.IsNullOrWhiteSpace(ErrorTextBox.Text))
        {
            ErrorTextBox.Text = "No warnings found...";
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

        _documentService.CreateOrUpdateOutputDocument(controller.Name, program);

        programBuilder.Check();
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
            _currentFilePath = filePath;
            YamlView.Text = _fileService.ReadFile(_currentFilePath);
            StatusText.Text = $"Opened: {_currentFilePath}";
        }
    }

    private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (_viewFocusService.FocusedView is YamlView)
        {
            var saved = this.SaveToFile(
                filter: "YAML files (*.yaml)|*.yaml|All files (*.*)|*.*",
                defaultFileName: "config.yaml",
                content: YamlView.Text,
                currentFilePath: _currentFilePath);

            if (saved != null)
                _currentFilePath = saved;
        }
        else if (_viewFocusService.FocusedView is OutputView outputView)
        {
            var doc = _documentService.GetAllDocuments().FirstOrDefault(d => d.Content == outputView);
            var controllerName = doc?.Title.Replace("Output: ", "") ?? "output";
            this.SaveToFile(
                filter: "Arduino files (*.ino)|*.ino|Text files (*.txt)|*.txt|All files (*.*)|*.*",
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
            StatusText.Text = $"Saved: {filePath}";
        }

        return filePath;
    }

    private void CopyToClipboardMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var textEditor = _viewFocusService.FocusedEditorView?.GetTextEditor();
        if (!string.IsNullOrEmpty(textEditor?.Text))
        {
            Clipboard.SetText(textEditor.Text);
            StatusText.Text = "Content copied to clipboard";
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
        try
        {
            var overviews = _gpioDocumentationService.GenerateOverview(YamlView.Text);

            if (overviews.Count == 0)
            {
                MessageBox.Show("Keine Controller mit GPIOs gefunden.\n\nBitte stellen Sie sicher, dass Ihr YAML-File gültige homeNet-Controller mit Cover- oder Light-Konfigurationen enthält.",
                    "Keine GPIOs gefunden", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _documentService.CreateOrUpdateGpioDocumentation(overviews);
            StatusText.Text = "GPIO Documentation created";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Erstellen der GPIO-Dokumentation: {ex.Message}",
                "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            StatusText.Text = "Error creating GPIO documentation";
        }
    }

    private void LayoutAnchorable_Hiding(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (sender is Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable anchorable)
        {
            if (anchorable.Title == "Controllers")
            {
                ViewControllersMenuItem.IsChecked = false;
            }
            else if (anchorable.Title == "Errors / Warnings")
            {
                ViewErrorsMenuItem.IsChecked = false;
            }
        }
    }
}