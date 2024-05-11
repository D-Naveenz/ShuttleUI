using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ShuttleUI.Controls;

public class BackgroundTemplateSelector : DataTemplateSelector
{
    public DataTemplate? ImageTemplate
    {
        get; set;
    }

    public DataTemplate? MediaPlayerTemplate
    {
        get; set;
    }

    public BackgroundTypeCallback? GetBackgroundType
    {
        get; set;
    }

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is string && container is ContentPresenter contentPresenter && GetBackgroundType != null)
        {
            var _backgroundType = GetBackgroundType();

            if (_backgroundType == MediaBackgroundType.Image && ImageTemplate != null)
            {
                return ImageTemplate;
            }
            else if (_backgroundType == MediaBackgroundType.Video && MediaPlayerTemplate != null)
            {
                return MediaPlayerTemplate;
            }
            else
            {
                contentPresenter.Content = null;
                return default;
            }
        }

        return base.SelectTemplateCore(item, container);
    }
}

public delegate MediaBackgroundType BackgroundTypeCallback();