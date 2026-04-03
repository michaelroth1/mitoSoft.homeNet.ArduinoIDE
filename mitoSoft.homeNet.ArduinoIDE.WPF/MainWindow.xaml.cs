using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;
using mitoSoft.homeNet.ArduinoIDE.WPF.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HomeNet = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.HomeNet;

namespace mitoSoft.homeNet.ArduinoIDE.WPF;

public partial class MainWindow : Window
{
    private string _currentFilePath = string.Empty;
    private string _searchText = string.Empty;

    private readonly SettingsService _settingsService;
    private readonly FileService _fileService;
    private readonly TextEditorService _textEditorService;
    private DocumentService _documentService = null!;

    public ICommand FindCommand { get; }

    public MainWindow()
    {
        InitializeComponent();
        FindCommand = new RelayCommand(FindMenuItem_Click);
        DataContext = this;

        _settingsService = new SettingsService();
        _fileService = new FileService();
        _textEditorService = new TextEditorService();

        LoadSettings();
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
            ApplyZoom();
        }
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _documentService = new DocumentService(DocumentPane, ZoomSlider.Value);
        YamlTextBox.Text = _settingsService.GetYamlContent();
        UpdateControllerList();
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _settingsService.SaveWindowSettings(Width, Height, ZoomSlider.Value);
    }

    private void YamlTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        SaveYamlInSettings();
        UpdateControllerList();
    }

    private void SaveYamlInSettings()
    {
        _settingsService.SaveYamlContent(YamlTextBox.Text);
    }

    private void UpdateControllerList()
    {
        var controllers = new TextCrawler(YamlTextBox.Text).ParseHomeNetControllers();

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
        CheckErrors();
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

        var parser = new YamlParser(YamlTextBox.Text);
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
        CheckErrors();

        var controller = GetController();

        if (controller == null)
        {
            return;
        }

        var config = new YamlParser(YamlTextBox.Text).Parse(controller.UniqueId);

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

    private void SaveOutputToFile(string controllerName, string content)
    {
        _fileService.SaveOutputFile(controllerName, content);
    }

    private HomeNet.Controller? GetController()
    {
        string? controllerName = ControllerListBox.SelectedItem as string;

        if (string.IsNullOrWhiteSpace(controllerName))
        {
            return null;
        }

        var controller = new YamlParser(YamlTextBox.Text).GetController(controllerName);
        return controller;
    }

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var filePath = _fileService.OpenYamlFile();
        if (filePath != null)
        {
            _currentFilePath = filePath;
            YamlTextBox.Text = _fileService.ReadFile(_currentFilePath);
            StatusText.Text = $"Opened: {_currentFilePath}";
        }
    }

    private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var activeDocument = _documentService.GetActiveDocument();

        if (activeDocument == null)
            return;

        if (activeDocument.Title == "YAML Editor")
        {
            SaveYamlFile();
        }
        else if (activeDocument.Title.StartsWith("Output:"))
        {
            var controllerName = activeDocument.Title.Replace("Output: ", "");
            var dockPanel = activeDocument.Content as DockPanel;
            var textBox = dockPanel?.Children.OfType<TextBox>().FirstOrDefault();
            if (textBox != null)
            {
                SaveOutputToFile(controllerName, textBox.Text);
            }
        }
    }

    private void SaveYamlFile()
    {
        var filePath = _fileService.SaveYamlFile(_currentFilePath);
        if (filePath != null)
        {
            _currentFilePath = filePath;
            _fileService.WriteFile(_currentFilePath, YamlTextBox.Text);
            StatusText.Text = $"Saved: {_currentFilePath}";
        }
    }

    private void CopyToClipboardMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var activeDocument = _documentService.GetActiveDocument();

        if (activeDocument == null)
            return;

        string? contentToCopy = null;

        if (activeDocument.Title == "YAML Editor")
        {
            contentToCopy = YamlTextBox.Text;
        }
        else if (activeDocument.Title.StartsWith("Output:"))
        {
            var dockPanel = activeDocument.Content as DockPanel;
            var textBox = dockPanel?.Children.OfType<TextBox>().FirstOrDefault();
            contentToCopy = textBox?.Text;
        }

        if (!string.IsNullOrEmpty(contentToCopy))
        {
            Clipboard.SetText(contentToCopy);
            StatusText.Text = "Content copied to clipboard";
        }
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void CommentMenuItem_Click(object sender, RoutedEventArgs e)
    {
        CommentSelectedLines();
    }

    private void CommentButton_Click(object sender, RoutedEventArgs e)
    {
        CommentSelectedLines();
    }

    private void CommentSelectedLines()
    {
        _textEditorService.CommentLines(YamlTextBox);
    }

    private void UncommentMenuItem_Click(object sender, RoutedEventArgs e)
    {
        UncommentSelectedLines();
    }

    private void UncommentButton_Click(object sender, RoutedEventArgs e)
    {
        UncommentSelectedLines();
    }

    private void UncommentSelectedLines()
    {
        _textEditorService.UncommentLines(YamlTextBox);
    }

    private void SelectHomeNetNodeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        SelectYamlNode("homeNet");
    }

    private void SelectYamlNode(string key)
    {
        _textEditorService.SelectYamlNode(YamlTextBox, key);
    }

    private void CreateHomeNetElementsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var newConfig = new YamlParser(YamlTextBox.Text).AddHomeNetElements();

        var outputWindow = new OutputWindow("Missing HomeNet elements", newConfig);
        outputWindow.ShowDialog();
    }

    private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ApplyZoom();
    }

    private void ApplyZoom()
    {
        if (YamlTextBox != null)
        {
            YamlTextBox.FontSize = 12 * ZoomSlider.Value;
        }

        if (_documentService != null)
        {
            _documentService.UpdateZoomForOutputDocuments(ZoomSlider.Value);
        }
    }

    private void FindMenuItem_Click(object sender, RoutedEventArgs e)
    {
        FindMenuItem_Click(sender);
    }

    private void FindMenuItem_Click(object? sender)
    {
        var findDialog = new FindDialog(_searchText);
        if (findDialog.ShowDialog() == true)
        {
            _searchText = findDialog.SearchText;
            FindNext();
        }
    }

    private void FindNext()
    {
        if (string.IsNullOrEmpty(_searchText))
            return;

        var activeDocument = _documentService.GetActiveDocument();
        if (activeDocument == null)
            return;

        TextBox? targetTextBox = null;

        if (activeDocument.Title == "YAML Editor")
        {
            targetTextBox = YamlTextBox;
        }
        else if (activeDocument.Title.StartsWith("Output:"))
        {
            var dockPanel = activeDocument.Content as DockPanel;
            targetTextBox = dockPanel?.Children.OfType<TextBox>().FirstOrDefault();
        }

        if (targetTextBox == null)
            return;

        if (!_textEditorService.FindNext(targetTextBox, _searchText))
        {
            MessageBox.Show("Not found.", "Find", MessageBoxButton.OK, MessageBoxImage.Information);
        }
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