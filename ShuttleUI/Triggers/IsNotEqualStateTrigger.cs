using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace ShuttleUI.Triggers;

/// <summary>
/// Enables a state if the value is not equal to another value
/// </summary>
internal class IsNotEqualStateTrigger : StateTriggerBase
{
    private void UpdateTrigger() => SetActive(!IsEqualStateTrigger.AreValuesEqual(Value, To, true));

    /// <summary>
    /// Gets or sets the value for comparison.
    /// </summary>
    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Value"/> DependencyProperty
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(object), typeof(IsNotEqualStateTrigger), new PropertyMetadata(null, OnValuePropertyChanged));

    private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var obj = (IsNotEqualStateTrigger)d;
        obj.UpdateTrigger();
    }

    /// <summary>
    /// Gets or sets the value to compare inequality to.
    /// </summary>
    public object To
    {
        get => GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="To"/> DependencyProperty
    /// </summary>
    public static readonly DependencyProperty ToProperty =
                DependencyProperty.Register(nameof(To), typeof(object), typeof(IsNotEqualStateTrigger), new PropertyMetadata(null, OnValuePropertyChanged));
}
