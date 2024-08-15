using System;
using System.Collections.Generic;
using System.Windows.Controls;
using PRBD_Framework;
using MyPoll.Model;
using MyPoll.ViewModel;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MyPoll.View;

/// <summary>
/// Logique d'interaction pour AdminView.xaml
/// </summary>
public partial class AdminView : UserControlBase {

    private readonly AdminViewModel _vm;

    public AdminView(User user)
    {
        InitializeComponent();
        DataContext = _vm = new AdminViewModel(user);

    }
}
