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
    /// Interaction logic for DodavanjeSoftvera.xaml
    /// </summary>
    public partial class DodavanjeSoftvera : Window
    {
        public DodavanjeSoftvera()
        {
            InitializeComponent();
        }

        private void nextClick(object sender, RoutedEventArgs e)
        {
            Korak2Softver.Focus();
        }

        private void backClick(object sender, RoutedEventArgs e)
        {
            Korak1Softver.Focus();
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void finishClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Zavrsi");
            this.Close();
        }
    }
}
