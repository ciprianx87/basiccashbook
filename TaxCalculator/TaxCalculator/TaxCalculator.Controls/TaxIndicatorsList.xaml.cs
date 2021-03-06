﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaxCalculator.ViewModel.ViewModels;
using TaxCalculator.ViewModel.Base;

namespace TaxCalculator.Controls
{
    /// <summary>
    /// Interaction logic for TaxIndicatorsList.xaml
    /// </summary>
    public partial class TaxIndicatorsList : UserControl, IDisposable
    {
        public TaxIndicatorsList()
        {
            InitializeComponent();
            this.DataContext = new TaxIndicatorsListVm();
        }

        private void DeleteCommand_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            (this.DataContext as TaxIndicatorsListVm).Delete(but.CommandParameter);
        }
        private void EditCommand_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            (this.DataContext as TaxIndicatorsListVm).Edit(but.CommandParameter);
        }

        private void SetAsDefaultCommand_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            (this.DataContext as TaxIndicatorsListVm).SetAsDefault(but.CommandParameter);
        }
       

        private void EditIndicatorsCommand_Click(object sender, RoutedEventArgs e)
        {
            var but = (sender as Button);
            (this.DataContext as TaxIndicatorsListVm).EditIndicators(but.CommandParameter);
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }
    }
}
