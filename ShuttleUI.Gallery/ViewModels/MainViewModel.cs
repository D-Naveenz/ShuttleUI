using CommunityToolkit.Mvvm.ComponentModel;

namespace ShuttleUI.Gallery.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    private Uri? _backgroundSource;

    public MainViewModel()
    {
        BackgroundSource = new Uri("ms-appx:///Assets/videos/sample_video.mp4");
    }
}
