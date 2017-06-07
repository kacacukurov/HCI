using System.Windows;

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for PotvrdaBrisanja.xaml
    /// </summary>
    public partial class PotvrdaBrisanja : Window
    {
        public bool daKlik;

        public PotvrdaBrisanja()
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
