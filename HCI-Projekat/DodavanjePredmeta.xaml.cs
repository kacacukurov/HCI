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
    /// Interaction logic for DodavanjePredmeta.xaml
    /// </summary>
    public partial class DodavanjePredmeta : Window
    {
        public DodavanjePredmeta()
        {
            InitializeComponent();
        }

        private void nextClickPredmet(object sender, RoutedEventArgs e)
        {
            Korak2Predmet.Focus();
        }

        private void backClickPredmet(object sender, RoutedEventArgs e)
        {
            Korak1Predmet.Focus();
        }

        private void exitClickPredmet(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void finishClickPredmet(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Kraj");
            this.Close();
        }
    }
}
