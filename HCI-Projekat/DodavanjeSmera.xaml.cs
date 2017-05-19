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
    /// Interaction logic for DodavanjeSmera.xaml
    /// </summary>
    public partial class DodavanjeSmera : Window
    {
        public DodavanjeSmera()
        {
            InitializeComponent();
        }

        private void exitClickSmer(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void finishClickSmer(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Kraj");
            this.Close();
        }
    }
}