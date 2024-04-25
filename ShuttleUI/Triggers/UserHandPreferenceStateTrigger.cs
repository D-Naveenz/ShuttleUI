using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;

namespace ShuttleUI.Triggers;

/// <summary>
/// Trigger for switching UI based on whether the user favors their left or right hand.
/// </summary>
public class UserHandPreferenceStateTrigger : StateTriggerBase
{
    private static readonly HandPreference handPreference;

    static UserHandPreferenceStateTrigger()
    {
#if HAS_UNO
        handPreference = HandPreference.RightHanded;
#else
        handPreference = new UISettings().HandPreference;
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserHandPreferenceStateTrigger"/> class.
    /// </summary>
    public UserHandPreferenceStateTrigger()
    {
        SetActive(handPreference == HandPreference.RightHanded);
    }

    /// <summary>
    /// Gets or sets the hand preference to trigger on.
    /// </summary>
    /// <value>A value from the <see cref="HandPreference"/> enum.</value>
    public HandPreference HandPreference
    {
        get => (HandPreference)GetValue(HandPreferenceProperty);
        set => SetValue(HandPreferenceProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="HandPreference"/> DependencyProperty
    /// </summary>
    public static readonly DependencyProperty HandPreferenceProperty =
        DependencyProperty.Register(nameof(HandPreference), typeof(HandPreference), typeof(UserHandPreferenceStateTrigger), new PropertyMetadata(HandPreference.RightHanded, OnHandPreferencePropertyChanged));

    private static void OnHandPreferencePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var obj = (UserHandPreferenceStateTrigger)d;
        var val = (HandPreference)e.NewValue;
        obj.SetActive(handPreference == val);
    }
}