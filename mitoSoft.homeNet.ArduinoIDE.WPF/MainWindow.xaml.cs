using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;
using HomeNet = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.HomeNet;
using Xceed.Wpf.AvalonDock.Layout;

namespace mitoSoft.homeNet.ArduinoIDE.WPF;

public partial class MainWindow : Window
{
    private string _currentFilePath = string.Empty;
    private string _searchText = string.Empty;

    public ICommand FindCommand { get; }

    public MainWindow()
    {
        InitializeComponent();
        FindCommand = new RelayCommand(FindMenuItem_Click);
        DataContext = this;
        LoadSettings();
        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
    }

    private void LoadSettings()
    {
        if (Properties.Settings.Default.WindowWidth > 0 && Properties.Settings.Default.WindowHeight > 0)
        {
            Width = Properties.Settings.Default.WindowWidth;
            Height = Properties.Settings.Default.WindowHeight;
        }

        if (Properties.Settings.Default.ZoomFactor > 0)
        {
            ZoomSlider.Value = Properties.Settings.Default.ZoomFactor;
            ApplyZoom();
        }
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        YamlTextBox.Text = Properties.Settings.Default.YamlContent;
        UpdateControllerList();
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        Properties.Settings.Default.WindowWidth = Width;
        Properties.Settings.Default.WindowHeight = Height;
        Properties.Settings.Default.ZoomFactor = ZoomSlider.Value;
        Properties.Settings.Default.Save();
    }

    private void YamlTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        SaveYamlInSettings();
        UpdateControllerList();
    }

    private void SaveYamlInSettings()
    {
        Properties.Settings.Default.YamlContent = YamlTextBox.Text;
        Properties.Settings.Default.Save();
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

        CreateOutputDocument(controller.Name, program);

        programBuilder.Check();
    }

    private void CreateOutputDocument(string controllerName, string content)
    {
        var existingDoc = DocumentPane.Children.OfType<LayoutDocument>()
            .FirstOrDefault(d => d.Title == $"Output: {controllerName}");

        if (existingDoc != null)
        {
            var dockPanel = existingDoc.Content as DockPanel;
            var textBox = dockPanel?.Children.OfType<TextBox>().FirstOrDefault();
            if (textBox != null)
            {
                textBox.Text = content;
            }
            existingDoc.IsSelected = true;
            existingDoc.IsActive = true;
            return;
        }

        var outputDocument = new LayoutDocument
        {
            Title = $"Output: {controllerName}",
            CanClose = true,
            Content = CreateOutputContent(controllerName, content)
        };

        DocumentPane.Children.Add(outputDocument);
        outputDocument.IsSelected = true;
        outputDocument.IsActive = true;
    }

    private DockPanel CreateOutputContent(string controllerName, string content)
    {
        var dockPanel = new DockPanel();

        var toolBar = new ToolBar();
        DockPanel.SetDock(toolBar, Dock.Top);

        var copyButton = new Button
        {
            Content = "Copy to Clipboard",
            Padding = new Thickness(10, 2, 10, 2)
        };
        copyButton.Click += (s, e) =>
        {
            if (!string.IsNullOrEmpty(content))
            {
                Clipboard.SetText(content);
                MessageBox.Show("Content copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        };

        toolBar.Items.Add(copyButton);

        var textBox = new TextBox
        {
            Text = content,
            IsReadOnly = true,
            AcceptsReturn = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            FontFamily = new System.Windows.Media.FontFamily("Consolas"),
            FontSize = 11 * ZoomSlider.Value,
            Background = System.Windows.Media.Brushes.White
        };

        dockPanel.Children.Add(toolBar);
        dockPanel.Children.Add(textBox);

        return dockPanel;
    }

    private void SaveOutputToFile(string controllerName, string content)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Arduino files (*.ino)|*.ino|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FileName = controllerName + ".ino"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            File.WriteAllText(saveFileDialog.FileName, content);
            MessageBox.Show($"File saved to: {saveFileDialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
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
        var openFileDialog = new OpenFileDialog
        {
            Filter = "YAML files (*.yaml;*.yml)|*.yaml;*.yml|All files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            _currentFilePath = openFileDialog.FileName;
            YamlTextBox.Text = File.ReadAllText(_currentFilePath);
            StatusText.Text = $"Opened: {_currentFilePath}";
        }
    }

    private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var activeDocument = DocumentPane.Children.OfType<LayoutDocument>().FirstOrDefault(d => d.IsActive);

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
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "YAML files (*.yaml)|*.yaml|All files (*.*)|*.*",
            FileName = !string.IsNullOrEmpty(_currentFilePath) ? Path.GetFileName(_currentFilePath) : "config.yaml"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            _currentFilePath = saveFileDialog.FileName;
            File.WriteAllText(_currentFilePath, YamlTextBox.Text);
            StatusText.Text = $"Saved: {_currentFilePath}";
        }
    }

    private void CopyToClipboardMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var activeDocument = DocumentPane.Children.OfType<LayoutDocument>().FirstOrDefault(d => d.IsActive);

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
        int selectionStart = YamlTextBox.SelectionStart;
        int selectionLength = YamlTextBox.SelectionLength;

        if (selectionLength == 0) return;

        string text = YamlTextBox.Text;
        int lineStart = text.LastIndexOf('\n', selectionStart) + 1;
        int lineEnd = text.IndexOf('\n', selectionStart + selectionLength);
        if (lineEnd == -1) lineEnd = text.Length;

        string selectedText = text[lineStart..lineEnd];
        string commentedText = string.Join("\n", selectedText
            .Split(["\r\n", "\n"], StringSplitOptions.None)
            .Select(line => "#" + line));

        YamlTextBox.Text = text[..lineStart] + commentedText + text[lineEnd..];
        YamlTextBox.SelectionStart = lineStart;
        YamlTextBox.SelectionLength = commentedText.Length;
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
        int selectionStart = YamlTextBox.SelectionStart;
        int selectionLength = YamlTextBox.SelectionLength;

        if (selectionLength == 0) return;

        string text = YamlTextBox.Text;
        int lineStart = text.LastIndexOf('\n', selectionStart) + 1;
        int lineEnd = text.IndexOf('\n', selectionStart + selectionLength);
        if (lineEnd == -1) lineEnd = text.Length;

        string selectedText = text[lineStart..lineEnd];
        string uncommentedText = string.Join("\n", selectedText
            .Split(["\r\n", "\n"], StringSplitOptions.None)
            .Select(line => line.StartsWith('#') ? line[1..] : line));

        YamlTextBox.Text = text[..lineStart] + uncommentedText + text[lineEnd..];
        YamlTextBox.SelectionStart = lineStart;
        YamlTextBox.SelectionLength = uncommentedText.Length;
    }

    private void SelectHomeNetNodeMenuItem_Click(object sender, RoutedEventArgs e)
    {
        SelectYamlNode("homeNet");
    }

    private void SelectYamlNode(string key)
    {
        string text = YamlTextBox.Text;
        int index = text.IndexOf(key, StringComparison.OrdinalIgnoreCase);

        if (index >= 0)
        {
            YamlTextBox.Focus();
            YamlTextBox.SelectionStart = index;
            YamlTextBox.SelectionLength = key.Length;
            YamlTextBox.ScrollToLine(GetLineNumber(text, index));
        }
    }

    private int GetLineNumber(string text, int index)
    {
        return text[..index].Count(c => c == '\n');
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

        if (DocumentPane?.Children != null)
        {
            foreach (var document in DocumentPane.Children.OfType<LayoutDocument>())
            {
                if (document.Title.StartsWith("Output:"))
                {
                    var dockPanel = document.Content as DockPanel;
                    var textBox = dockPanel?.Children.OfType<TextBox>().FirstOrDefault();
                    if (textBox != null)
                    {
                        textBox.FontSize = 11 * ZoomSlider.Value;
                    }
                }
            }
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

        var activeDocument = DocumentPane.Children.OfType<LayoutDocument>().FirstOrDefault(d => d.IsActive);
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

        int startPosition = targetTextBox.SelectionStart + targetTextBox.SelectionLength;
        int foundIndex = targetTextBox.Text.IndexOf(_searchText, startPosition, StringComparison.OrdinalIgnoreCase);

        if (foundIndex != -1)
        {
            targetTextBox.Focus();
            targetTextBox.SelectionStart = foundIndex;
            targetTextBox.SelectionLength = _searchText.Length;
            targetTextBox.ScrollToLine(GetLineNumber(targetTextBox.Text, foundIndex));
        }
        else
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