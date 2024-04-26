using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;

namespace ShuttleUI.Controls;

public partial class MediaBackgroundPanel : ContentControl
{
    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> for the image.
    /// </summary>
    public DataTemplate? ImageTemplate
    {
        get => (DataTemplate)GetValue(ImageTemplateProperty);
        set => SetValue(ImageTemplateProperty, value);
    }

    // Using a DependencyProperty as the backing store for ImageTemplate.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ImageTemplateProperty =
        DependencyProperty.Register(
            "ImageTemplate",
            typeof(DataTemplate),
            typeof(MediaBackgroundPanel),
            new PropertyMetadata(null, (d, e) => ((MediaBackgroundPanel)d).OnImageTemplatePropertyChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> for the video.
    /// </summary>
    public DataTemplate? MediaPlayerTemplate
    {
        get => (DataTemplate)GetValue(MediaPlayerTemplateProperty);
        set => SetValue(MediaPlayerTemplateProperty, value);
    }

    // Using a DependencyProperty as the backing store for MediaControlTemplate.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MediaPlayerTemplateProperty =
        DependencyProperty.Register(
            "MediaPlayerTemplate",
            typeof(DataTemplate),
            typeof(MediaBackgroundPanel),
            new PropertyMetadata(null, (d, e) => ((MediaBackgroundPanel)d).OnMediaPlayerTemplatePropertyChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));

    /// <summary>
    /// Gets or sets the source of the background.
    /// </summary>
    public object? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(
            "Source", 
            typeof(object), 
            typeof(MediaBackgroundPanel), 
            new PropertyMetadata(null, OnSourcePropertyChangedAsync));

    /// <summary>
    /// Gets or sets the <see cref="MediaBackgroundType"/> of the selected background source.
    /// </summary>
    public MediaBackgroundType BackgroundType
    {
        get => (MediaBackgroundType)GetValue(BackgroundTypeProperty);
        set => SetValue(BackgroundTypeProperty, value);
    }

    // Using a DependencyProperty as the backing store for BackgroundType.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty BackgroundTypeProperty =
        DependencyProperty.Register("BackgroundType", typeof(MediaBackgroundType), typeof(MediaBackgroundPanel), new PropertyMetadata(MediaBackgroundType.Unknown));



    /// <summary>
    /// Gets oe sets the element of the background described in  <see cref="MediaBackgroundType"/>.
    /// </summary>
    public FrameworkElement? BackgroundContent
    {
        get => (FrameworkElement?)GetValue(BackgroundContentProperty);
        set => SetValue(BackgroundContentProperty, value);
    }

    // Using a DependencyProperty as the backing store for BackgroundContent.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty BackgroundContentProperty =
        DependencyProperty.Register("BackgroundContent", typeof(FrameworkElement), typeof(MediaBackgroundPanel), new PropertyMetadata(null));

    /// <summary>
    /// Gets the <see cref="StorageFile"/> of the source, if the source is recognized as the current background.
    /// </summary>
    public StorageFile? SourceFile
    {
        get;
        private set;
    }

    private static async void OnSourcePropertyChangedAsync(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MediaBackgroundPanel backgroundPanel)
        {
            await backgroundPanel.SourceChangedAsync(e.NewValue);
        }
    }

    public virtual void OnImageTemplatePropertyChanged(DataTemplate? oldValue, DataTemplate? newValue)
    {
        ImageTemplateChanged();
    }

    public virtual void OnMediaPlayerTemplatePropertyChanged(DataTemplate? oldValue, DataTemplate? newValue)
    {
        MediaPlayerTemplateChanged();
    }
}

/// <summary>
/// Defines the type of background. It can be an image or a video, or none.
/// </summary>
public enum MediaBackgroundType
{
    Unknown, // Default value, not forsing any background
    Image,
    Video
}