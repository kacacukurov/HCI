﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for IzmenaSoftvera.xaml
    /// </summary>
    public partial class IzmenaSoftvera : Window
    {
        public bool potvrdaIzmena;

        public IzmenaSoftvera()
        {
            InitializeComponent();
            this.potvrdaIzmena = false;
        }
    }
}