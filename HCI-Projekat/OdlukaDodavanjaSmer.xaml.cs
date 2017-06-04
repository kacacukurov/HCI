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
    /// Interaction logic for OdlukaDodavanjaSmer.xaml
    /// </summary>
    public partial class OdlukaDodavanjaSmer : Window
    {
        public bool potvrdaNovogUnosa;

        public OdlukaDodavanjaSmer()
        {
            InitializeComponent();
            potvrdiUnos.Focus();
            potvrdaNovogUnosa = true;
        }

        public void noviUnos(object sender, EventArgs e)
        {
            potvrdaNovogUnosa = true;
            this.Close();
        }

        public void stariUnos(object sender, EventArgs e)
        {
            potvrdaNovogUnosa = false;
            this.Close();
        }
    }
}
