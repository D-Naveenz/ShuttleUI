using Windows.Foundation.Metadata;

namespace ShuttleUI.Helpers;

internal static partial class ControlHelpers
{
    internal static bool IsXamlRootAvailable { get; } = ApiInformation.IsPropertyPresent("Windows.UI.Xaml.UIElement", "XamlRoot");
}