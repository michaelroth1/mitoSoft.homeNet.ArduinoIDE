using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using mitoSoft.homeNet.ArduinoIDE.WPF.Models;
using System.Printing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;
using Xceed.Wpf.AvalonDock.Layout;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Services;

public class DocumentService
{
    private readonly LayoutDocumentPane _documentPane;
    private readonly Func<double> _getCurrentZoomFactor;
    private IHighlightingDefinition? _cppHighlighting;

    public DocumentService(LayoutDocumentPane documentPane, Func<double> getCurrentZoomFactor)
    {
        _documentPane = documentPane;
        _getCurrentZoomFactor = getCurrentZoomFactor;
        LoadCppSyntaxHighlighting();
    }

    private void LoadCppSyntaxHighlighting()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "mitoSoft.homeNet.ArduinoIDE.WPF.Editor.Arduino.xshd";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new XmlTextReader(stream))
                    {
                        _cppHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
            }
        }
        catch
        {
            // Silently fail if highlighting cannot be loaded
            _cppHighlighting = null;
        }
    }

    public void CreateOrUpdateOutputDocument(string controllerName, string content)
    {
        var existingDoc = _documentPane.Children.OfType<LayoutDocument>()
            .FirstOrDefault(d => d.Title == $"Output: {controllerName}");

        if (existingDoc != null)
        {
            UpdateExistingDocument(existingDoc, content);
            return;
        }

        CreateNewDocument(controllerName, content);
    }

    private void UpdateExistingDocument(LayoutDocument document, string content)
    {
        var dockPanel = document.Content as DockPanel;
        var textEditor = dockPanel?.Children.OfType<TextEditor>().FirstOrDefault();
        if (textEditor != null)
        {
            textEditor.Text = content;
        }
        document.IsSelected = true;
        document.IsActive = true;
    }

    private void CreateNewDocument(string controllerName, string content)
    {
        var outputDocument = new LayoutDocument
        {
            Title = $"Output: {controllerName}",
            CanClose = true,
            Content = CreateOutputContent(controllerName, content)
        };

        _documentPane.Children.Add(outputDocument);
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

        var textEditor = new TextEditor
        {
            Text = content,
            IsReadOnly = true,
            ShowLineNumbers = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            FontFamily = new FontFamily("Consolas"),
            FontSize = 11 * _getCurrentZoomFactor(),
            Background = Brushes.White,
            IsManipulationEnabled = true
        };

        if (_cppHighlighting != null)
        {
            textEditor.SyntaxHighlighting = _cppHighlighting;
        }

        // Enable touch and manipulation
        textEditor.ManipulationBoundaryFeedback += (s, e) => { e.Handled = true; };

        // Improve scrolling with mouse wheel for Output panels
        textEditor.PreviewMouseWheel += (s, e) =>
        {
            var editor = s as TextEditor;
            if (editor != null)
            {
                var scrollViewer = FindScrollViewer(editor);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3.0);
                    e.Handled = true;
                }
            }
        };

        // Enable touch scrolling
        textEditor.Loaded += (s, e) =>
        {
            var scrollViewer = FindScrollViewer(textEditor);
            if (scrollViewer != null)
            {
                scrollViewer.PanningMode = PanningMode.VerticalOnly;
                scrollViewer.PanningDeceleration = 0.001;
                scrollViewer.PanningRatio = 1.0;
            }
        };

        dockPanel.Children.Add(toolBar);
        dockPanel.Children.Add(textEditor);

        return dockPanel;
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

    public LayoutDocument? GetActiveDocument()
    {
        return _documentPane.Children.OfType<LayoutDocument>().FirstOrDefault(d => d.IsActive);
    }

    public void UpdateZoomForOutputDocuments(double zoomFactor)
    {
        foreach (var document in _documentPane.Children.OfType<LayoutDocument>())
        {
            if (document.Title.StartsWith("Output:") || document.Title == "GPIO Documentation")
            {
                var dockPanel = document.Content as DockPanel;
                var textEditor = dockPanel?.Children.OfType<TextEditor>().FirstOrDefault();
                if (textEditor != null)
                {
                    textEditor.FontSize = 11 * zoomFactor;
                }
            }
        }
    }

    public void CreateOrUpdateGpioDocumentation(List<ControllerGpioOverview> overviews)
    {
        var existingDoc = _documentPane.Children.OfType<LayoutDocument>()
            .FirstOrDefault(d => d.Title == "GPIO Documentation");

        if (existingDoc != null)
        {
            UpdateExistingGpioDocument(existingDoc, overviews);
            return;
        }

        CreateNewGpioDocument(overviews);
    }

    private void UpdateExistingGpioDocument(LayoutDocument document, List<ControllerGpioOverview> overviews)
    {
        var dockPanel = document.Content as DockPanel;
        var scrollViewer = dockPanel?.Children.OfType<ScrollViewer>().FirstOrDefault();
        var stackPanel = scrollViewer?.Content as StackPanel;

        if (stackPanel != null)
        {
            stackPanel.Children.Clear();
            PopulateGpioContent(stackPanel, overviews);
        }

        document.IsSelected = true;
        document.IsActive = true;
    }

    private void CreateNewGpioDocument(List<ControllerGpioOverview> overviews)
    {
        var gpioDocument = new LayoutDocument
        {
            Title = "GPIO Documentation",
            CanClose = true,
            Content = CreateGpioDocumentContent(overviews)
        };

        _documentPane.Children.Add(gpioDocument);
        gpioDocument.IsSelected = true;
        gpioDocument.IsActive = true;
    }

    private DockPanel CreateGpioDocumentContent(List<ControllerGpioOverview> overviews)
    {
        var dockPanel = new DockPanel();

        var toolBar = new ToolBar();
        DockPanel.SetDock(toolBar, Dock.Top);

        var printButton = new Button
        {
            Content = "Print / Export",
            Padding = new Thickness(10, 2, 10, 2),
            Margin = new Thickness(0, 0, 5, 0)
        };
        printButton.Click += (s, e) => PrintGpioDocumentation(overviews);

        var searchButton = new Button
        {
            Content = "Find (Ctrl+F)",
            Padding = new Thickness(10, 2, 10, 2),
            Margin = new Thickness(0, 0, 5, 0)
        };

        var clearHighlightsButton = new Button
        {
            Content = "Clear Highlights",
            Padding = new Thickness(10, 2, 10, 2)
        };

        toolBar.Items.Add(printButton);
        toolBar.Items.Add(searchButton);
        toolBar.Items.Add(clearHighlightsButton);
        dockPanel.Children.Add(toolBar);

        var scrollViewer = new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            Background = Brushes.White
        };

        // Improve mouse wheel scrolling
        scrollViewer.PreviewMouseWheel += (s, e) =>
        {
            var scroller = s as ScrollViewer;
            if (scroller != null)
            {
                scroller.ScrollToVerticalOffset(scroller.VerticalOffset - e.Delta / 3.0);
                e.Handled = true;
            }
        };

        var stackPanel = new StackPanel
        {
            Margin = new Thickness(10)
        };

        PopulateGpioContent(stackPanel, overviews);

        scrollViewer.Content = stackPanel;
        dockPanel.Children.Add(scrollViewer);

        // Enable Find functionality
        searchButton.Click += (s, e) => ShowFindInGpioDoc(stackPanel);
        clearHighlightsButton.Click += (s, e) => ClearHighlightsInGpioDoc(stackPanel);

        return dockPanel;
    }

    private void PopulateGpioContent(StackPanel stackPanel, List<ControllerGpioOverview> overviews)
    {
        var title = new TextBlock
        {
            Text = "GPIO Übersicht - Schaltschrank Dokumentation",
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 20)
        };
        stackPanel.Children.Add(title);

        if (overviews.Count == 0)
        {
            var noData = new TextBlock
            {
                Text = "Keine Controller mit GPIOs gefunden.",
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.Gray
            };
            stackPanel.Children.Add(noData);
            return;
        }

        foreach (var overview in overviews)
        {
            var controllerBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(230, 240, 255)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(100, 150, 200)),
                BorderThickness = new Thickness(2),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 10, 0, 5),
                CornerRadius = new CornerRadius(5)
            };

            var controllerStack = new StackPanel();

            var controllerTitle = new TextBlock
            {
                Text = $"Controller: {overview.ControllerName} (ID: {overview.ControllerId})",
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };
            controllerStack.Children.Add(controllerTitle);

            var controllerInfo = new TextBlock
            {
                Text = $"IP: {overview.IPAddress} | MAC: {overview.MacAddress} | Verwendete GPIOs: {overview.TotalGpioCount}",
                FontSize = 11,
                Foreground = Brushes.DarkBlue,
                Margin = new Thickness(0, 3, 0, 0)
            };
            controllerStack.Children.Add(controllerInfo);

            controllerBorder.Child = controllerStack;
            stackPanel.Children.Add(controllerBorder);

            if (overview.Items.Count == 0)
            {
                var noItems = new TextBlock
                {
                    Text = "  Keine Geräte konfiguriert",
                    FontStyle = FontStyles.Italic,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(10, 5, 0, 10)
                };
                stackPanel.Children.Add(noItems);
                continue;
            }

            var dataGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                IsReadOnly = true,
                GridLinesVisibility = DataGridGridLinesVisibility.All,
                HeadersVisibility = DataGridHeadersVisibility.All,
                AlternatingRowBackground = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
                RowBackground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 15),
                FontSize = 11,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                CanUserResizeColumns = true
            };

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Typ",
                Binding = new System.Windows.Data.Binding("Type"),
                Width = 80,
                MinWidth = 10
            });

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Name",
                Binding = new System.Windows.Data.Binding("Name"),
                Width = 180,
                MinWidth = 10
            });

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Beschreibung",
                Binding = new System.Windows.Data.Binding("Description"),
                Width = 200,
                MinWidth = 10
            });

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "GPIO Pins",
                Binding = new System.Windows.Data.Binding("GpioPins"),
                Width = 250,
                MinWidth = 10
            });

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Zusatzinfo",
                Binding = new System.Windows.Data.Binding("AdditionalInfo"),
                Width = 120,
                MinWidth = 10
            });

            dataGrid.ItemsSource = overview.Items;
            stackPanel.Children.Add(dataGrid);
        }

        var timestamp = new TextBlock
        {
            Text = $"Erstellt am: {DateTime.Now:dd.MM.yyyy HH:mm:ss}",
            FontSize = 10,
            Foreground = Brushes.Gray,
            Margin = new Thickness(0, 10, 0, 0)
        };
        stackPanel.Children.Add(timestamp);
    }

    private void PrintGpioDocumentation(List<ControllerGpioOverview> overviews)
    {
        try
        {
            var printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                var doc = new FlowDocument
                {
                    PagePadding = new Thickness(50),
                    ColumnWidth = printDialog.PrintableAreaWidth
                };

                var title = new Paragraph(new Run("GPIO Übersicht - Schaltschrank Dokumentation"))
                {
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                doc.Blocks.Add(title);

                foreach (var overview in overviews)
                {
                    var controllerHeader = new Paragraph(new Run($"Controller: {overview.ControllerName} (ID: {overview.ControllerId})"))
                    {
                        FontSize = 14,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 10, 0, 5),
                        Background = new SolidColorBrush(Color.FromRgb(230, 240, 255))
                    };
                    doc.Blocks.Add(controllerHeader);

                    var controllerInfo = new Paragraph(new Run($"IP: {overview.IPAddress} | MAC: {overview.MacAddress} | Verwendete GPIOs: {overview.TotalGpioCount}"))
                    {
                        FontSize = 10,
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                    doc.Blocks.Add(controllerInfo);

                    var table = new Table
                    {
                        CellSpacing = 0,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };

                    table.Columns.Add(new TableColumn { Width = new GridLength(60) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(150) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(150) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(180) });
                    table.Columns.Add(new TableColumn { Width = new GridLength(100) });

                    var rowGroup = new TableRowGroup();

                    var headerRow = new TableRow { Background = new SolidColorBrush(Color.FromRgb(200, 200, 200)) };
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Typ")) { FontWeight = FontWeights.Bold, Margin = new Thickness(2) }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Name")) { FontWeight = FontWeights.Bold, Margin = new Thickness(2) }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Beschreibung")) { FontWeight = FontWeights.Bold, Margin = new Thickness(2) }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("GPIO Pins")) { FontWeight = FontWeights.Bold, Margin = new Thickness(2) }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Zusatzinfo")) { FontWeight = FontWeights.Bold, Margin = new Thickness(2) }));
                    rowGroup.Rows.Add(headerRow);

                    foreach (var item in overview.Items)
                    {
                        var row = new TableRow();
                        row.Cells.Add(new TableCell(new Paragraph(new Run(item.Type)) { Margin = new Thickness(2), FontSize = 10 }));
                        row.Cells.Add(new TableCell(new Paragraph(new Run(item.Name)) { Margin = new Thickness(2), FontSize = 10 }));
                        row.Cells.Add(new TableCell(new Paragraph(new Run(item.Description)) { Margin = new Thickness(2), FontSize = 10 }));
                        row.Cells.Add(new TableCell(new Paragraph(new Run(item.GpioPins)) { Margin = new Thickness(2), FontSize = 10 }));
                        row.Cells.Add(new TableCell(new Paragraph(new Run(item.AdditionalInfo)) { Margin = new Thickness(2), FontSize = 10 }));
                        rowGroup.Rows.Add(row);
                    }

                    table.RowGroups.Add(rowGroup);
                    doc.Blocks.Add(table);
                }

                var timestampPara = new Paragraph(new Run($"Erstellt am: {DateTime.Now:dd.MM.yyyy HH:mm:ss}"))
                {
                    FontSize = 10,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                doc.Blocks.Add(timestampPara);

                var documentPaginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
                printDialog.PrintDocument(documentPaginator, "GPIO Documentation");

                MessageBox.Show("Dokument wurde zum Drucker gesendet.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Drucken: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ShowFindInGpioDoc(StackPanel stackPanel)
    {
        var findDialog = new FindDialog("");
        if (findDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(findDialog.SearchText))
        {
            HighlightSearchInGpioDoc(stackPanel, findDialog.SearchText);
        }
    }

    public void FindInGpioDocumentation(StackPanel stackPanel, string searchText)
    {
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            HighlightSearchInGpioDoc(stackPanel, searchText);
        }
    }

    private void HighlightSearchInGpioDoc(StackPanel stackPanel, string searchText)
    {
        bool found = false;

        // Clear previous highlights
        ClearHighlightsInGpioDoc(stackPanel);

        foreach (var child in stackPanel.Children)
        {
            if (child is DataGrid dataGrid)
            {
                // Search in DataGrid
                foreach (var item in dataGrid.Items)
                {
                    if (item is GpioOverviewItem gpioItem)
                    {
                        if (gpioItem.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            gpioItem.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            gpioItem.GpioPins.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                            gpioItem.Type.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        {
                            found = true;
                            // Highlight the row
                            var row = dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                            if (row != null)
                            {
                                row.Background = new SolidColorBrush(Color.FromRgb(255, 255, 150)); // Yellow highlight
                            }
                        }
                    }
                }

                if (found)
                {
                    dataGrid.ScrollIntoView(dataGrid.Items[0]);
                }
            }
            else if (child is Border border)
            {
                // Search in controller headers
                var stack = border.Child as StackPanel;
                if (stack != null)
                {
                    foreach (var textBlock in stack.Children.OfType<TextBlock>())
                    {
                        if (textBlock.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        {
                            found = true;
                            border.Background = new SolidColorBrush(Color.FromRgb(255, 255, 150)); // Yellow highlight
                            break;
                        }
                    }
                }
            }
            else if (child is TextBlock textBlock)
            {
                if (textBlock.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    textBlock.Background = new SolidColorBrush(Color.FromRgb(255, 255, 150)); // Yellow highlight
                }
            }
        }

        if (!found)
        {
            MessageBox.Show($"'{searchText}' nicht gefunden.", "Find", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ClearHighlightsInGpioDoc(StackPanel stackPanel)
    {
        foreach (var child in stackPanel.Children)
        {
            if (child is DataGrid dataGrid)
            {
                foreach (var item in dataGrid.Items)
                {
                    var row = dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    if (row != null)
                    {
                        row.ClearValue(DataGridRow.BackgroundProperty);
                    }
                }
            }
            else if (child is Border border)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(230, 240, 255)); // Restore original color
            }
            else if (child is TextBlock textBlock)
            {
                textBlock.ClearValue(TextBlock.BackgroundProperty);
            }
        }
    }
}
