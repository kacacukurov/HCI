using System;
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
    /// Interaction logic for PotvrdaOdustajanja.xaml
    /// </summary>
    public partial class PotvrdaOdustajanja : Window
    {
        public bool daKlik;

        public PotvrdaOdustajanja()
        {
            InitializeComponent();
            noDeleteButton.Focus();
            this.daKlik = false;
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void finishClick(object sender, RoutedEventArgs e)
        {
            daKlik = true;
            this.Close();
        }
    }
}
