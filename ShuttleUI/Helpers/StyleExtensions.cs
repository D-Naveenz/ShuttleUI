using System.Linq;
using Microsoft.UI.Xaml;

namespace ShuttleUI.Helpers;

// Adapted from https://github.com/rudyhuyn/XamlPlus
public static partial class StyleExtensions
{
    public static ResourceDictionary GetResources(Style obj)
    {
        return (ResourceDictionary)obj.GetValue(ResourcesProperty);
    }

    public static void SetResources(Style obj, ResourceDictionary value)
    {
        obj.SetValue(ResourcesProperty, value);
    }

    public static readonly DependencyProperty ResourcesProperty =
        DependencyProperty.RegisterAttached("Resources", typeof(ResourceDictionary), typeof(StyleExtensions), new PropertyMetadata(null, ResourcesChanged));

    private static void ResourcesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not FrameworkElement frameworkElement)
        {
            return;
        }

        var resource = frameworkElement.Resources;
        if (resource == null)
        {
            return;
        }

        if (e.NewValue is ResourceDictionary newResource)
        {
            // Replace each item from the resource dictionary and add it to the new one
            newResource.Select(kvp => (kvp.Key, kvp.Value)).ToList().ForEach(kvp => 
            {
                resource[kvp.Key] = kvp.Value;
            });
        }

        if (frameworkElement.IsLoaded)
        {
            // Only force if the style was applied after the control was loaded
            ForceControlToReloadThemeResources(frameworkElement);
        }
    }

    private static void ForceControlToReloadThemeResources(FrameworkElement frameworkElement)
    {
        // To force the refresh of all resource references.
        // Note: Doesn't work when in high-contrast.
        var currentRequestedTheme = frameworkElement.RequestedTheme;
        frameworkElement.RequestedTheme = currentRequestedTheme == ElementTheme.Dark
            ? ElementTheme.Light
            : ElementTheme.Dark;
        frameworkElement.RequestedTheme = currentRequestedTheme;
    }
}