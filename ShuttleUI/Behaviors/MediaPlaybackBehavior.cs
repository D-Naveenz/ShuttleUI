using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using Windows.Media.Core;

namespace ShuttleUI.Behaviors;

internal class MediaPlaybackBehavior : Behavior<MediaPlayerElement>
{
    /// <summary>
    /// Gets or sets the source of the media playback.
    /// </summary>
    public Uri? Source
    {
        get => (Uri)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register("Source", typeof(Uri), typeof(MediaPlaybackBehavior), new PropertyMetadata(null, OnSourcePropertyChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the media should repeat.
    /// </summary>
    public bool Repeat
    {
        get => (bool)GetValue(RepeatProperty);
        set => SetValue(RepeatProperty, value);
    }

    // Using a DependencyProperty as the backing store for Repeat.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty RepeatProperty =
        DependencyProperty.Register("Repeat", typeof(bool), typeof(MediaPlaybackBehavior), new PropertyMetadata(false));

    /// <summary>
    /// Gets the media source.
    /// </summary>
    public MediaSource? MediaSource
    {
        get; private set;
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.Loaded += MediaPlayer_OnLoaded;
    }

    private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MediaPlaybackBehavior behavior && behavior.AssociatedObject != null)
        {
            behavior.MediaSource = MediaSource.CreateFromUri(behavior.Source);
        }
    }

    private void MediaPlayer_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (MediaSource != null)
        {
            AssociatedObject.Source = MediaSource;
            AssociatedObject.MediaPlayer.IsLoopingEnabled = Repeat;
            AssociatedObject.MediaPlayer.Play();
        }
    }
}