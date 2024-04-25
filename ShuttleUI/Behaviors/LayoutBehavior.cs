using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Xaml.Interactivity;

namespace ShuttleUI.Behaviors;

public class LayoutBehavior : Behavior<Frame>
{
    private static LayoutBehavior? _instance;

    public Thickness DefaultThickness
    {
        get => (Thickness)GetValue(DefaultThicknessProperty);
        set => SetValue(DefaultThicknessProperty, value);
    }

    public static readonly DependencyProperty DefaultThicknessProperty =
        DependencyProperty.Register("DefaultThickness", typeof(Thickness), typeof(LayoutBehavior), new PropertyMetadata(null));

    public FrameThicknessType DefaultThicknessType
    {
        get => (FrameThicknessType)GetValue(DefaultThicknessTypeProperty);
        set => SetValue(DefaultThicknessTypeProperty, value);
    }

    public static readonly DependencyProperty DefaultThicknessTypeProperty =
        DependencyProperty.Register("DefaultThicknessType", typeof(FrameThicknessType), typeof(LayoutBehavior), new PropertyMetadata(FrameThicknessType.Margin));

    public static Thickness? GetThickness(FrameworkElement element) => (Thickness?)element.GetValue(ThicknessProperty);

    public static void SetThickness(FrameworkElement element, Thickness? value) => element.SetValue(ThicknessProperty, value);

    public static readonly DependencyProperty ThicknessProperty =
        DependencyProperty.RegisterAttached(
            "Thickness",
            typeof(Thickness),
            typeof(LayoutBehavior),
            new PropertyMetadata(null, OnUpdateThickness));

    public static FrameThicknessType? GetThicknessType(FrameworkElement element) => (FrameThicknessType?)element.GetValue(ThicknessTypeProperty);

    public static void SetThicknessType(FrameworkElement element, FrameThicknessType? value) => element.SetValue(ThicknessTypeProperty, value);

    public static readonly DependencyProperty ThicknessTypeProperty =
        DependencyProperty.RegisterAttached(
            "ThicknessType",
            typeof(FrameThicknessType?),
            typeof(LayoutBehavior),
            new PropertyMetadata(null, OnUpdateThicknessType));

    public FrameworkElement? AttachedElement
    {
        get;
        private set;
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        // Set the instance
        if (_instance == null)
        {
            _instance = this;
        }

        AssociatedObject.Navigated += OnNavigated;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        // Reset the instance
        _instance = null;
        AttachedElement = null;

        AssociatedObject.Navigating -= OnNavigating;
        AssociatedObject.Navigated -= OnNavigated;
    }

    private void OnNavigating(object sender, NavigatingCancelEventArgs e)
    {
        AttachedElement = null;

        AssociatedObject.Navigating -= OnNavigating;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        // Navigated event fires after the page is loaded.
        // So, we have to subcribe to the Navigating event after the page is loaded.
        // Otherwise the Attached element is always null.
        AssociatedObject.Navigating += OnNavigating;

        if (AttachedElement == null)
        {
            SetThicknessOfSelectedType(AssociatedObject);
        }
    }

    private static void OnUpdateThickness(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var element = d as FrameworkElement;
        if (element != null)
        {
            _instance?.SetThicknessOfSelectedType(element);
        }
    }

    private static void OnUpdateThicknessType(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var element = d as FrameworkElement;
        if (element != null)
        {
            _instance?.SetThicknessOfSelectedType(element);
        }
    }

    private void SetThicknessOfSelectedType(FrameworkElement element)
    {
        AttachedElement = element;

        var thickness = GetThickness(element).Equals(new Thickness(0)) ? null : GetThickness(element);
        thickness ??= DefaultThickness;

        var thicknessType = GetThicknessType(element) ?? DefaultThicknessType;

        // Reset frame margin
        AssociatedObject.Margin = new(0);

        if (thicknessType == FrameThicknessType.None)
        {
            if (element != null)
            {
                SetPadding(element, new(0));
            }
        }
        else if (thicknessType == FrameThicknessType.Margin)
        {
            // Change the margin of the frame(Page)
            AssociatedObject.Margin = (Thickness)thickness;
        }
        else if (element != null)
        {
            SetPadding(element, (Thickness)thickness);
        }
    }

    private static void SetPadding(FrameworkElement element, Thickness thickness)
    {
        if (element is Control controlElement)
        {
            controlElement.Padding = thickness;
        }
        else if (element is Grid gridElement)
        {
            gridElement.Padding = thickness;
        }
        else
        {
            throw new ArgumentException("Couldn't find an element to apply padding.");
        }
    }
}

public enum FrameThicknessType
{
    None,
    Margin,
    Padding
}