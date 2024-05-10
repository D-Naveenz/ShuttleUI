using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Xaml.Interactivity;

namespace ShuttleUI.Behaviors;

public class LayoutBehavior : Behavior<Frame>
{
    private static LayoutBehavior? _instance;

    /// <summary>
    /// Gets or sets the default <see cref="Thickness"/> for spacing.
    /// </summary>
    public Thickness DefaultThickness
    {
        get => (Thickness)GetValue(DefaultThicknessProperty);
        set => SetValue(DefaultThicknessProperty, value);
    }

    public static readonly DependencyProperty DefaultThicknessProperty =
        DependencyProperty.Register("DefaultThickness", typeof(Thickness), typeof(LayoutBehavior), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the default thickness type. This is a <see cref="LayoutThicknessType"/> and <c>Margin</c> is the default value
    /// </summary>
    public LayoutThicknessType DefaultThicknessType
    {
        get => (LayoutThicknessType)GetValue(DefaultThicknessTypeProperty);
        set => SetValue(DefaultThicknessTypeProperty, value);
    }

    public static readonly DependencyProperty DefaultThicknessTypeProperty =
        DependencyProperty.Register("DefaultThicknessType", typeof(LayoutThicknessType), typeof(LayoutBehavior), new PropertyMetadata(LayoutThicknessType.Margin));

    /// <summary>
    /// Getter of the <see cref="ThicknessProperty"/>. This is a <see cref="Thickness"/>
    /// </summary>
    /// <param name="element">Attached element of the property</param>
    /// <returns></returns>
    public static Thickness? GetThickness(FrameworkElement element) => (Thickness?)element.GetValue(ThicknessProperty);

    /// <summary>
    /// Setter of the <see cref="ThicknessProperty"/>. This is a <see cref="LayoutThicknessType"/>
    /// </summary>
    /// <param name="element">Attached element of the property</param>
    /// <param name="value">Thickness value</param>
    public static void SetThickness(FrameworkElement element, Thickness? value) => element.SetValue(ThicknessProperty, value);

    public static readonly DependencyProperty ThicknessProperty =
        DependencyProperty.RegisterAttached(
            "Thickness",
            typeof(Thickness),
            typeof(LayoutBehavior),
            new PropertyMetadata(null, OnUpdateThickness));

    /// <summary>
    /// Getter of the <see cref="ThicknessTypeProperty"/>. This is a <see cref="LayoutThicknessType"/> and <c>Margin</c> is the default value
    /// </summary>
    /// <param name="element">Attached element of the property</param>
    /// <returns></returns>
    public static LayoutThicknessType? GetThicknessType(FrameworkElement element) => (LayoutThicknessType?)element.GetValue(ThicknessTypeProperty);

    /// <summary>
    /// Setter of the <see cref="ThicknessTypeProperty"/>. This is a <see cref="LayoutThicknessType"/>
    /// </summary>
    /// <param name="element">Attached element of the property</param>
    /// <param name="value">Thickness Type. It can be a <c>Margin</c>, <c>Padding</c> or set it to <c>None</c> to disable spacing.</param>
    public static void SetThicknessType(FrameworkElement element, LayoutThicknessType? value) => element.SetValue(ThicknessTypeProperty, value);

    public static readonly DependencyProperty ThicknessTypeProperty =
        DependencyProperty.RegisterAttached(
            "ThicknessType",
            typeof(LayoutThicknessType?),
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

        if (thicknessType == LayoutThicknessType.None)
        {
            if (element != null)
            {
                SetPadding(element, new(0));
            }
        }
        else if (thicknessType == LayoutThicknessType.Margin)
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

public enum LayoutThicknessType
{
    None,
    Margin,
    Padding
}