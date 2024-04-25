# ShuttleUI

**Current Stage:** Work in Progress (WIP)/Pre-release

Shuttle UI is a library designed to accelerate the development process of WinUI 3 applications. It offers advanced UI controls and addresses limitations present in some XAML elements provided by WinUI. Additionally, it includes enhancements for the WinUI template provided by TemplateStudio.

### Components
1) Behaviors
    - LayoutBehavior: This behavior allows for runtime adjustment of page layout spacing. It addresses the fixed margin issue in the NavigationView frame by providing control over spacing to individual pages. It also allows selecting any element as the root control for spacing and provides flexibility in choosing whether the space should be treated as margin or padding.
2) Controls
    - MediaBackgroundPanel: This control functions like a Grid and facilitates adding dynamic backgrounds behind its content. It automatically detects whether the media source is an image or a video and loads it at runtime.
    - SettingsCard: This control implements a card layout similar to the one found in Windows 11 UI.
3) Helpers
    - StringExtensions: Includes the **TryToEnum\<T>** method, which extracts the enum value of a given string.
    - StyleExtensions: Overcomes the limitation of nested styles in XAML styles.

### How to Use
Shuttle UI is developed using the latest WindowsAppSDK (1.5.240404000) and DotNet 8.0. While it is optimized for use with the TemplateStudio template, this is not a strict requirement.

### License
Shuttle UI is released under the MIT License. For more details, see the [LICENSE.md](LICENSE.md) file included in the package.