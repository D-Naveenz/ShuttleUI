using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using ShuttleUI.Helpers;
using Windows.Media.Core;
using Windows.Storage;

namespace ShuttleUI.Controls;

[TemplatePart(Name = BackgroundHolder, Type = typeof(Grid))]
[TemplatePart(Name = ImagePresenter, Type = typeof(Image))]
[TemplatePart(Name = VideoPresenter, Type = typeof(MediaPlayerElement))]
[TemplateVisualState(Name = NoBackgroundState, GroupName = BackgroundStatesGroup)]
[TemplateVisualState(Name = ImageState, GroupName = BackgroundStatesGroup)]
[TemplateVisualState(Name = VideoState, GroupName = BackgroundStatesGroup)]
public partial class MediaBackgroundPanel : ContentControl
{
    protected const string BackgroundStatesGroup = "BackgroundStates";

    protected const string NoBackgroundState = "NoBackgroundState";
    protected const string ImageState = "ImageState";
    protected const string VideoState = "VideoState";

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
        }
        else
        {
            BackgroundType = MediaBackgroundType.Unknown;
            SourceFile = null;
        }

        ChangeBackgroundContent();
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
        if (BackgroundType == MediaBackgroundType.Unknown)
        {
            return;
        }

        var _backgroundContent = new Uri(SourceFile!.Path);
        var imagePresenter = GetTemplateChild(ImagePresenter) as Image;
        var videoPresenter = GetTemplateChild(VideoPresenter) as MediaPlayerElement;

        // Reset source of the background presenters
        imagePresenter!.Source = null;
        videoPresenter!.Source = null;

        // Update the visual states
        UpdateVisualStates();

        // Set the source of the background presenters
        if (BackgroundType == MediaBackgroundType.Image)
        {
            imagePresenter!.Source = new BitmapImage(_backgroundContent);
        }
        else if (BackgroundType == MediaBackgroundType.Video)
        {
            videoPresenter!.Source = MediaSource.CreateFromUri(_backgroundContent);
            videoPresenter.MediaPlayer.IsLoopingEnabled = IsVideoLoopingEnabled;
            videoPresenter.MediaPlayer.Play();
        }
    }

    private void UpdateVisualStates()
    {
        if (BackgroundType == MediaBackgroundType.Image)
        {
            VisualStateManager.GoToState(this, ImageState, true);
        }
        else if (BackgroundType == MediaBackgroundType.Video)
        {
            VisualStateManager.GoToState(this, VideoState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, NoBackgroundState, true);
        }
    }
}