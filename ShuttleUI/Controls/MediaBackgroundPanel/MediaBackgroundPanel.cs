using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using ShuttleUI.Helpers;
using Windows.Storage;

namespace ShuttleUI.Controls;

[TemplatePart(Name = BackgroundHolder, Type = typeof(ContentPresenter))]
[TemplatePart(Name = ImagePresenter, Type = typeof(Image))]
[TemplatePart(Name = VideoPresenter, Type = typeof(MediaPlayerElement))]
public partial class MediaBackgroundPanel : ContentControl
{
    protected const string BackgroundHolder = "PART_BackgroundHolder";
    protected const string ImagePresenter = "PART_ImagePresenter";
    protected const string VideoPresenter = "PART_VideoPresenter";

    public MediaBackgroundPanel()
    {
        DefaultStyleKey = typeof(MediaBackgroundPanel);
    }

    public async Task SourceChangedAsync(object? source)
    {
        if (source != null)
        {
            var file = await GetFileFromSourceAsync(source);
            var contentType = file.ContentType.Split('/')[0];

            MediaBackgroundType fileType;
            if (contentType != null && contentType.TryToEnum(out fileType))
            {
                // Change the background data
                BackgroundType = fileType;
                SourceFile = file;
            }

            ChangeBackgroundContent();
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        BackgroundSelector = new BackgroundTemplateSelector
        {
            ImageTemplate = ImageTemplate,
            MediaPlayerTemplate = MediaPlayerTemplate,
            GetBackgroundType = () => BackgroundType
        };
    }

    protected static async Task<StorageFile> GetFileFromSourceAsync(object sourceObj)
    {
        Uri source;

        if (sourceObj is string sourceStr)
        {
            // Load storage file from the string
            try
            {
                source = new Uri(sourceStr);
            }
            catch (UriFormatException)
            {
                try
                {
                    var executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    source = new Uri(Path.Combine(executingLocation!, sourceStr.GetPathChunk()));
                }
                catch (UriFormatException)
                {
                    throw new UriFormatException("The source string is not a valid uri or a valid path.");
                }
            }
        }
        else if (sourceObj is Uri sourceUri)
        {
            // Load storage file from the uri
            source = sourceUri;
        }
        else
        {
            throw new InvalidDataException("The source must be a string or a uri.");
        }

        if (source.Scheme != "file")
        {
            return await StorageFile.GetFileFromApplicationUriAsync(source);
        }
        else
        {
            return await StorageFile.GetFileFromPathAsync(source.LocalPath);
        }
    }

    /// <summary>
    /// Creates AutomationPeer
    /// </summary>
    /// <returns>An automation peer for <see cref="MediaBackgroundPanel"/>.</returns>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new MediaBackgroundPanelAutomationPeer(this);
    }

    private void ChangeBackgroundContent()
    {
        if (Source == null)
        {
            BackgroundContent = null;
            return;
        }

        if (BackgroundType == MediaBackgroundType.Unknown)
        {
            throw new InvalidOperationException("Source must be either image or video");
        }

        BackgroundContent = new Uri(SourceFile!.Path);
    }
}