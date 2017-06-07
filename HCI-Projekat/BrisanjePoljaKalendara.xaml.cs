using System.Collections.ObjectModel;
using System.Windows;

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
