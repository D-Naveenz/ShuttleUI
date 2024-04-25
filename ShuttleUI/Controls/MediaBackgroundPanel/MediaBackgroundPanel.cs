using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using MimeMapping;
using ShuttleUI.Helpers;
using Windows.Media.Core;

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

    /// <inheritdoc/>>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }

    private static Uri? GetFileFromSource(object sourceObj)
    {
        Uri? source = null;

        if (sourceObj is string sourceStr)
        {
            // Load storage file from the string
            try
            {
                source = new Uri(sourceStr);
            }
            catch (UriFormatException)
            {
                var localFolderPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
                var path = Path.GetFullPath(Path.Combine(localFolderPath, sourceStr));

                try
                {
                    source = new Uri(path);
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

        return source;
    }

    private void SourceChangedAsync(object? source)
    {
        if (source != null)
        {
            var uri = GetFileFromSource(source);
            var contentType = MimeUtility.GetMimeMapping(uri?.LocalPath).Split('/')[0];
            MediaBackgroundType fileType;

            if (uri != null && contentType != null && contentType.TryToEnum(out fileType))
            {
                // Change the background data
                BackgroundType = fileType;
                SourceUri = uri;
            }

            ChangeBackgroundContent();
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
                image.Source = new BitmapImage(SourceUri);

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
                mediaPlayer.Source = MediaSource.CreateFromUri(SourceUri);
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
            Source = MediaSource.CreateFromUri(SourceUri),
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
            _imagePresenter.Source = new BitmapImage(SourceUri);

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
                _videoPresenter.MediaPlayer.Source = MediaSource.CreateFromUri(SourceUri);
            }

            BackgroundContent = _videoPresenter;
            _videoPresenter.MediaPlayer.Play();
        }
        else
        {
            throw new InvalidOperationException("The template of the MediaBackgroundPanel must contain a ContentPresenter with the name PART_BackgroundPresenter.");
        }
    }
}