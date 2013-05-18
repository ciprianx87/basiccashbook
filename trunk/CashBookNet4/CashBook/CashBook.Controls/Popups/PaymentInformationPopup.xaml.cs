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
using System.Windows.Shapes;
using CashBook.ViewModels;

namespace CashBook.Controls.Popups
{ 
    public partial class PaymentInformationPopup : Window
    {
        public PaymentInformationPopup()
        {
            InitializeComponent();
            this.Loaded += Window_Loaded;
            this.DataContext = new PaymentInformationDialogVM();
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= Window_Loaded;
            bool res = this.Activate();
            this.Topmost = true;
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }
    }
}
