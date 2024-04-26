using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using ShuttleUI.Helpers;
using Windows.Media.Core;
using Windows.Storage;

namespace ShuttleUI.Controls;

[TemplatePart(Name = BackgroundHolder, Type = typeof(ContentPresenter))]
[TemplatePart(Name = ImagePresenter, Type = typeof(Image))]
[TemplatePart(Name = VideoPresenter, Type = typeof(MediaPlayerElement))]
[TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
[TemplateVisualState(Name = ImageBackgroundState, GroupName = CommonStates)]
[TemplateVisualState(Name = MediaBackgroundState, GroupName = CommonStates)]
public partial class MediaBackgroundPanel : ContentControl
{
    protected const string CommonStates = "CommonStates";

    protected const string NormalState = "Normal";
    protected const string ImageBackgroundState = "ImageBackground";
    protected const string MediaBackgroundState = "MediaBackground";

    protected const string BackgroundHolder = "PART_BackgroundHolder";
    protected const string ImagePresenter = "PART_ImagePresenter";
    protected const string VideoPresenter = "PART_VideoPresenter";

    private Image? _imagePresenter;
    private MediaPlayerElement? _videoPresenter;

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
            if (file != null && contentType != null && contentType.TryToEnum(out fileType))
            {
                // Change the background data
                BackgroundType = fileType;
                SourceFile = file;
            }

            ChangeBackgroundContent();
        }
    }

    /// <inheritdoc/>>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }

    private static async Task<StorageFile> GetFileFromSourceAsync(object sourceObj)
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

    private void ImageTemplateChanged()
    {
        if (ImageTemplate != null)
        {
            // Change the template of the image presenter
            if (ImageTemplate.LoadContent() is Image image)
            {
                image.Name = ImagePresenter;
                if (SourceFile != null)
                {
                    image.Source = new BitmapImage(new Uri(SourceFile.Path));
                }

                _imagePresenter = image;
            }

            throw new InvalidDataException("The Image Template must contain an Image");
        }
        else
        {
            CreateImagePresenter();
        }

        ChangeBackgroundContent();
    }

    private Image CreateImagePresenter()
    {
        var image = new Image
        {
            Name = ImagePresenter,
        };

        return image;
    }

    private void MediaPlayerTemplateChanged()
    {
        if (MediaPlayerTemplate != null)
        {
            // Change the template of the video presenter
            if (MediaPlayerTemplate.LoadContent() is MediaPlayerElement mediaPlayer)
            {
                mediaPlayer.Name = VideoPresenter;
                mediaPlayer.Source = MediaSource.CreateFromStorageFile(SourceFile);
                mediaPlayer.MediaPlayer.IsLoopingEnabled = true;

                _videoPresenter = mediaPlayer;
            }

            throw new InvalidDataException("The Media Player Template must contain a MediaPlayerElement");
        }
        else
        {
            CreateVideoPresenter();
        }

        ChangeBackgroundContent();
    }

    private MediaPlayerElement CreateVideoPresenter()
    {
        var mediaPlayer = new MediaPlayerElement()
        {
            Name = VideoPresenter,
            Source = MediaSource.CreateFromStorageFile(SourceFile),
            MediaPlayer = { IsLoopingEnabled = true }
        };

        return mediaPlayer;
    }

    private void ChangeBackgroundContent()
    {
        if (Source == null)
        {
            BackgroundContent = null;
            return;
        }

        if (BackgroundType == MediaBackgroundType.Image)
        {
            // Load the image presenter as the background content if the background type is a image
            _imagePresenter ??= CreateImagePresenter();
            if (SourceFile != null)
            {
                _imagePresenter.Source = new BitmapImage(new Uri(SourceFile.Path));
            }

            BackgroundContent = _imagePresenter;
        }
        else if (BackgroundType == MediaBackgroundType.Video)
        {
            // Load the video presenter as the background content if the background type is a video
            if (_videoPresenter == null)
            {
                _videoPresenter = CreateVideoPresenter();
            }
            else
            {
                _videoPresenter.MediaPlayer.Source = MediaSource.CreateFromStorageFile(SourceFile);
            }

            BackgroundContent = _videoPresenter;
            _videoPresenter.MediaPlayer.Play();
        }
        else
        {
            throw new InvalidOperationException("The template of the MediaBackgroundPanel must contain a ContentPresenter with the name PART_BackgroundPresenter.");
        }
    }

    //private void UpdateVisualState()
    //{

    //    if (BackgroundType == MediaBackgroundType.Image)
    //    {

    //        VisualStateManager.GoToState(this, ImageBackgroundState, true);
    //    }
    //    else if (BackgroundType == MediaBackgroundType.Video)
    //    {

    //        VisualStateManager.GoToState(this, MediaBackgroundState, true);
    //    }
    //    else
    //    {

    //        VisualStateManager.GoToState(this, NormalState, true);
    //    }
    //}

    /// <summary>
    /// Creates AutomationPeer
    /// </summary>
    /// <returns>An automation peer for <see cref="MediaBackgroundPanel"/>.</returns>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new MediaBackgroundPanelAutomationPeer(this);
    }
}