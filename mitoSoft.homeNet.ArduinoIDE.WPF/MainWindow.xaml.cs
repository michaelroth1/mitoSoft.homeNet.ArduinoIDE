using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;
using mitoSoft.homeNet.ArduinoIDE.WPF.Services;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using HomeNet = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.HomeNet;

namespace mitoSoft.homeNet.ArduinoIDE.WPF;

public partial class MainWindow : Window
{
    private string _currentFilePath = string.Empty;
    private string _searchText = string.Empty;

    private readonly SettingsService _settingsService;
    private readonly FileService _fileService;
    private readonly TextEditorService _textEditorService;
    private readonly GpioOverviewService _gpioOverviewService;
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
        _gpioOverviewService = new GpioOverviewService();

        LoadSettings();
        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
    }

    private void LoadYamlSyntaxHighlighting()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "mitoSoft.homeNet.ArduinoIDE.WPF.Editor.YAML.xshd";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new XmlTextReader(stream))
                    {
                        YamlTextBox.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading YAML syntax highlighting: {ex.Message}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
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
        _documentService = new DocumentService(DocumentPane, () => ZoomSlider.Value);

        // Load YAML syntax highlighting
        LoadYamlSyntaxHighlighting();

        YamlTextBox.Text = _settingsService.GetYamlContent();

        // Subscribe to TextChanged event for AvalonEdit
        YamlTextBox.TextChanged += (s, args) =>
        {
            SaveYamlInSettings();
            UpdateControllerList();
        };

        // Enable touch and manipulation for YAML Editor
        YamlTextBox.IsManipulationEnabled = true;
        YamlTextBox.ManipulationBoundaryFeedback += (s, e) => { e.Handled = true; };

        // Improve YAML Editor scrolling with mouse wheel
        YamlTextBox.PreviewMouseWheel += (s, e) =>
        {
            var editor = s as TextEditor;
            if (editor != null && editor.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                var scrollViewer = FindScrollViewer(editor);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3.0);
                    e.Handled = true;
                }
            }
        };

        // Enable touch scrolling for YAML Editor
        YamlTextBox.Loaded += (s, e) =>
        {
            var scrollViewer = FindScrollViewer(YamlTextBox);
            if (scrollViewer != null)
            {
                scrollViewer.PanningMode = PanningMode.VerticalOnly;
                scrollViewer.PanningDeceleration = 0.001;
                scrollViewer.PanningRatio = 1.0;
            }
        };

        // Apply zoom after UI is fully initialized
        ApplyZoom();

        UpdateControllerList();
    }

    private ScrollViewer? FindScrollViewer(DependencyObject obj)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child is ScrollViewer scrollViewer)
                return scrollViewer;

            var result = FindScrollViewer(child);
            if (result != null)
                return result;
        }
        return null;
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _settingsService.SaveWindowSettings(Width, Height, ZoomSlider.Value);
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
            var textEditor = GetActiveTextEditor();
            if (textEditor != null)
            {
                SaveOutputToFile(controllerName, textEditor.Text);
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
        var textEditor = GetActiveTextEditor();
        if (textEditor != null)
        {
            var contentToCopy = textEditor.Text;
            if (!string.IsNullOrEmpty(contentToCopy))
            {
                Clipboard.SetText(contentToCopy);
                StatusText.Text = "Content copied to clipboard";
            }
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
        var activeDocument = _documentService.GetActiveDocument();

        // Check if active document is GPIO Documentation
        if (activeDocument?.Title == "GPIO Documentation")
        {
            var dockPanel = activeDocument.Content as DockPanel;
            var scrollViewer = dockPanel?.Children.OfType<ScrollViewer>().FirstOrDefault();
            var stackPanel = scrollViewer?.Content as StackPanel;

            if (stackPanel != null)
            {
                var findDialog = new FindDialog(_searchText);
                if (findDialog.ShowDialog() == true)
                {
                    _searchText = findDialog.SearchText;
                    _documentService.FindInGpioDocumentation(stackPanel, _searchText);
                }
                return;
            }
        }

        // Default behavior for TextEditor-based documents
        var findDialog2 = new FindDialog(_searchText);
        if (findDialog2.ShowDialog() == true)
        {
            _searchText = findDialog2.SearchText;
            FindNext();
        }
    }

    private TextEditor? GetActiveTextEditor()
    {
        var activeDocument = _documentService.GetActiveDocument();
        if (activeDocument == null)
            return null;

        // Find the TextEditor in the active document
        if (activeDocument.Title == "YAML Editor")
        {
            return YamlTextBox;
        }
        else if (activeDocument.Title.StartsWith("Output:"))
        {
            var dockPanel = activeDocument.Content as DockPanel;
            return dockPanel?.Children.OfType<TextEditor>().FirstOrDefault();
        }

        return null;
    }

    private void FindNext()
    {
        if (string.IsNullOrEmpty(_searchText))
            return;

        var textEditor = GetActiveTextEditor();

        // Perform search if TextEditor was found
        if (textEditor != null)
        {
            if (!_textEditorService.FindNext(textEditor, _searchText))
            {
                MessageBox.Show("Not found.", "Find", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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

    private void CreateGpioDocumentationMenuItem_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var overviews = _gpioOverviewService.GenerateOverview(YamlTextBox.Text);

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