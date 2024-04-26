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

    public MediaBackgroundType BackgroundType
    {
        get; set;
    } = MediaBackgroundType.Unknown;

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is string && container is ContentPresenter contentPresenter)
        {
            if (BackgroundType == MediaBackgroundType.Image && ImageTemplate != null)
            {
                return ImageTemplate;
            }
            else if (BackgroundType == MediaBackgroundType.Video && MediaPlayerTemplate != null)
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