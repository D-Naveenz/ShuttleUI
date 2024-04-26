using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;

namespace ShuttleUI.Controls;

/// <summary>
/// AutomationPeer for MediaBackgroundPanel
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MediaBackgroundPanel"/> class.
/// </remarks>
/// <param name="owner">SettingsCard</param>
public class MediaBackgroundPanelAutomationPeer(MediaBackgroundPanel owner) : FrameworkElementAutomationPeer(owner)
{
    /// <summary>
    /// Gets the control type for the element that is associated with the UI Automation peer.
    /// </summary>
    /// <returns>The control type.</returns>
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Pane;
    }

    /// <summary>
    /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType,
    /// differentiates the control represented by this AutomationPeer.
    /// </summary>
    /// <returns>The string that contains the name.</returns>
    protected override string GetClassNameCore()
    {
        return Owner.GetType().Name;
    }

    protected override string GetNameCore()
    {
        if (Owner is MediaBackgroundPanel backgroundPanel)
        {
            var name = AutomationProperties.GetName(backgroundPanel);
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }
            else
            {
                if (backgroundPanel.BackgroundType != MediaBackgroundType.Unknown)
                {
                    return $"{backgroundPanel.BackgroundType}Background";
                }
            }
        }

        return base.GetNameCore();
    }
}