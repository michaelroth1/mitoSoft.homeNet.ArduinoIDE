using mitoSoft.homeNet.ArduinoIDE.Models;
using mitoSoft.homeNet.ArduinoIDE.Views;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

namespace mitoSoft.homeNet.ArduinoIDE.Services;

public class MainPanelService
{
    private readonly LayoutDocumentPane _documentPane;
    private readonly Func<double> _getCurrentZoomFactor;
    private readonly Func<object?> _getActiveContent;

    public MainPanelService(LayoutDocumentPane documentPane, Func<double> getCurrentZoomFactor, Func<object?> getActiveContent)
    {
        _documentPane = documentPane;
        _getCurrentZoomFactor = getCurrentZoomFactor;
        _getActiveContent = getActiveContent;
    }

    public event EventHandler<FrameworkElement>? MainPanelViewAdded;

    public FrameworkElement CreateOrUpdateOutputDocument(string controllerName, string content)
    {
        var existingDoc = _documentPane.Children.OfType<LayoutDocument>()
            .FirstOrDefault(d => d.Title == $"Output: {controllerName}");

        if (existingDoc != null)
        {
            return this.UpdateExistingDocument(existingDoc, content);
        }

        return this.CreateNewDocument(controllerName, content);
    }

    private FrameworkElement UpdateExistingDocument(LayoutDocument document, string content)
    {
        var view = document.Content as OutputView;
        if (view != null)
        {
            view.SetContent(content);
        }
        document.IsSelected = true;
        document.IsActive = true;
        return view!;
    }

    private FrameworkElement CreateNewDocument(string controllerName, string content)
    {
        var view = new OutputView(controllerName);
        view.SetContent(content);
        view.SetZoomFactor(_getCurrentZoomFactor());

        var document = new LayoutDocument
        {
            Title = $"Output: {controllerName}",
            CanClose = true,
            Content = view
        };

        _documentPane.Children.Add(document);
        document.IsSelected = true;
        document.IsActive = true;
        MainPanelViewAdded?.Invoke(this, view);
        return view;
    }

    public LayoutDocument? GetActiveDocument()
    {
        var activeContent = _getActiveContent();
        if (activeContent != null)
        {
            var doc = _documentPane.Children.OfType<LayoutDocument>()
                .FirstOrDefault(d => d.Content == activeContent);
            if (doc != null)
                return doc;
        }

        return _documentPane.Children.OfType<LayoutDocument>().FirstOrDefault(d => d.IsActive);
    }

    public IEnumerable<LayoutDocument> GetAllDocuments()
    {
        return _documentPane.Children.OfType<LayoutDocument>();
    }

    public void UpdateZoomForAllEditorViews(double zoomFactor)
    {
        foreach (var document in _documentPane.Children.OfType<LayoutDocument>())
        {
            if (document.Content is IEditorView editorView)
            {
                editorView.SetZoomFactor(zoomFactor);
            }
        }
    }

    public FrameworkElement CreateOrUpdateGpioDocumentation(List<ControllerGpioOverview> overviews)
    {
        var existingDoc = _documentPane.Children.OfType<LayoutDocument>()
            .FirstOrDefault(d => d.Title == "GPIO Documentation");

        if (existingDoc != null)
        {
            return this.UpdateExistingGpioDocument(existingDoc, overviews);
        }

        return this.CreateNewGpioDocument(overviews);
    }

    private FrameworkElement UpdateExistingGpioDocument(LayoutDocument document, List<ControllerGpioOverview> overviews)
    {
        var view = document.Content as DocumentionView;
        if (view != null)
        {
            view.SetContent(overviews);
        }

        document.IsSelected = true;
        document.IsActive = true;
        return view!;
    }

    private FrameworkElement CreateNewGpioDocument(List<ControllerGpioOverview> overviews)
    {
        var view = new DocumentionView();
        view.SetContent(overviews);

        var gpioDocument = new LayoutDocument
        {
            Title = "GPIO Documentation",
            CanClose = true,
            Content = view
        };

        _documentPane.Children.Add(gpioDocument);
        gpioDocument.IsSelected = true;
        gpioDocument.IsActive = true;
        MainPanelViewAdded?.Invoke(this, view);
        return view;
    }

    public FrameworkElement CreateOrUpdateMissingHomeNetElementsDocument(string content)
    {
        const string title = "Missing homeNet elements";

        var existingDoc = _documentPane.Children.OfType<LayoutDocument>()
            .FirstOrDefault(d => d.Title == title);

        if (existingDoc != null)
        {
            var existingView = existingDoc.Content as MissingHomeNetElementsView;
            existingView?.SetContent(content);
            existingDoc.IsSelected = true;
            existingDoc.IsActive = true;
            return existingView!;
        }

        var view = new MissingHomeNetElementsView();
        view.SetContent(content);
        view.SetZoomFactor(_getCurrentZoomFactor());

        var document = new LayoutDocument
        {
            Title = title,
            CanClose = true,
            Content = view
        };

        _documentPane.Children.Add(document);
        document.IsSelected = true;
        document.IsActive = true;
        MainPanelViewAdded?.Invoke(this, view);
        return view;
    }
}