using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for BrisanjePoljaKalendara.xaml
    /// </summary>
    public partial class BrisanjePoljaKalendara : Window
    {
        private ObservableCollection<PoljaKalendaraBrisanje> predmetUkalendaru;
        public bool daKlik;

        public BrisanjePoljaKalendara()
        {
            InitializeComponent();
            noDeleteButton.Focus();
            this.daKlik = false;
            predmetUkalendaru = new ObservableCollection<PoljaKalendaraBrisanje>();
        }

        public ObservableCollection<PoljaKalendaraBrisanje> PredmetUkalendaru
        {
            get { return predmetUkalendaru; }
            set { predmetUkalendaru = value; }
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
