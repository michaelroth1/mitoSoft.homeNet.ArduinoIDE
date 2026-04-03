using mitoSoft.homeNet.ArduinoIDE.Models;
using Res = mitoSoft.homeNet.ArduinoIDE.Properties.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace mitoSoft.homeNet.ArduinoIDE.Views;

public partial class DocumentionView : UserControl
{
    private List<ControllerGpioOverview> _overviews = new();

    public DocumentionView()
    {
        InitializeComponent();
        this.SetupScrollViewer();
    }

    public void SetContent(List<ControllerGpioOverview> overviews)
    {
        _overviews = overviews;
        this.PopulateContent();
    }

    private void SetupScrollViewer()
    {
        // Improve mouse wheel scrolling
        MainScrollViewer.PreviewMouseWheel += (s, e) =>
        {
            var scroller = s as ScrollViewer;
            if (scroller != null)
            {
                scroller.ScrollToVerticalOffset(scroller.VerticalOffset - e.Delta / 3.0);
                e.Handled = true;
            }
        };
    }

    private void PopulateContent()
    {
        ContentPanel.Children.Clear();

        var title = new TextBlock
        {
            Text = Res.Doc_Title,
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 20)
        };
        ContentPanel.Children.Add(title);

        if (_overviews.Count == 0)
        {
            var noData = new TextBlock
            {
                Text = Res.Doc_NoData,
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.Gray
            };
            ContentPanel.Children.Add(noData);
            return;
        }

        foreach (var overview in _overviews)
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
                Text = string.Format(Res.Doc_ControllerTitle, overview.ControllerName, overview.ControllerId),
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };
            controllerStack.Children.Add(controllerTitle);

            var controllerInfo = new TextBlock
            {
                Text = string.Format(Res.Doc_ControllerInfo, overview.IPAddress, overview.MacAddress, overview.TotalGpioCount),
                FontSize = 11,
                Foreground = Brushes.DarkBlue,
                Margin = new Thickness(0, 3, 0, 0)
            };
            controllerStack.Children.Add(controllerInfo);

            controllerBorder.Child = controllerStack;
            ContentPanel.Children.Add(controllerBorder);

            if (overview.Items.Count == 0)
            {
                var noItems = new TextBlock
                {
                    Text = Res.Doc_NoDevices,
                    FontStyle = FontStyles.Italic,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(10, 5, 0, 10)
                };
                ContentPanel.Children.Add(noItems);
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
                Header = Res.Col_Type,
                Binding = new System.Windows.Data.Binding("Type"),
                Width = 80,
                MinWidth = 10
            });

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = Res.Col_Name,
                Binding = new System.Windows.Data.Binding("Name"),
                Width = 180,
                MinWidth = 10
            });

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = Res.Col_Description,
                Binding = new System.Windows.Data.Binding("Description"),
                Width = 200,
                MinWidth = 10
            });

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = Res.Col_GpioPins,
                Binding = new System.Windows.Data.Binding("GpioPins"),
                Width = 250,
                MinWidth = 10
            });

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = Res.Col_AdditionalInfo,
                Binding = new System.Windows.Data.Binding("AdditionalInfo"),
                Width = 120,
                MinWidth = 10
            });

            dataGrid.ItemsSource = overview.Items;
            ContentPanel.Children.Add(dataGrid);
        }

        var timestamp = new TextBlock
        {
            Text = string.Format(Res.Doc_CreatedAt, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")),
            FontSize = 10,
            Foreground = Brushes.Gray,
            Margin = new Thickness(0, 10, 0, 0)
        };
        ContentPanel.Children.Add(timestamp);
    }

    private void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        this.PrintGpioDocumentation();
    }

    private void PrintGpioDocumentation()
    {
        try
        {
            var printDialog = new System.Windows.Controls.PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                var doc = new FlowDocument
                {
                    PagePadding = new Thickness(50),
                    ColumnWidth = printDialog.PrintableAreaWidth
                };

                var title = new Paragraph(new Run(Res.Doc_Title))
                {
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                doc.Blocks.Add(title);

                foreach (var overview in _overviews)
                {
                    var controllerHeader = new Paragraph(new Run(string.Format(Res.Doc_ControllerTitle, overview.ControllerName, overview.ControllerId)))
                    {
                        FontSize = 14,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 10, 0, 5),
                        Background = new SolidColorBrush(Color.FromRgb(230, 240, 255))
                    };
                    doc.Blocks.Add(controllerHeader);

                    var controllerInfo = new Paragraph(new Run(string.Format(Res.Doc_ControllerInfo, overview.IPAddress, overview.MacAddress, overview.TotalGpioCount)))
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
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run(Res.Col_Type)) { FontWeight = FontWeights.Bold, Margin = new Thickness(2) }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run(Res.Col_Name)) { FontWeight = FontWeights.Bold, Margin = new Thickness(2) }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run(Res.Col_Description)) { FontWeight = FontWeights.Bold, Margin = new Thickness(2) }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run(Res.Col_GpioPins)) { FontWeight = FontWeights.Bold, Margin = new Thickness(2) }));
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run(Res.Col_AdditionalInfo)) { FontWeight = FontWeights.Bold, Margin = new Thickness(2) }));
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

                var timestampPara = new Paragraph(new Run(string.Format(Res.Doc_CreatedAt, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"))))
                {
                    FontSize = 10,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                doc.Blocks.Add(timestampPara);

                var documentPaginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
                printDialog.PrintDocument(documentPaginator, "GPIO Documentation");

                MessageBox.Show(Res.Msg_PrintSuccess, Res.Msg_PrintSuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(string.Format(Res.Msg_PrintError, ex.Message));
        }
    }
}