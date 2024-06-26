﻿using Microsoft.UI.Xaml.Controls;

using ShuttleUI.Gallery.ViewModels;

namespace ShuttleUI.Gallery.Views;

public sealed partial class ContentGridPage : Page
{
    public ContentGridViewModel ViewModel
    {
        get;
    }

    public ContentGridPage()
    {
        ViewModel = App.GetService<ContentGridViewModel>();
        InitializeComponent();
    }
}
