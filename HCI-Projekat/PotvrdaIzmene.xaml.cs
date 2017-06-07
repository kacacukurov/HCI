using System.Windows;

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for PotvrdaIzmene.xaml
    /// </summary>
    public partial class PotvrdaIzmene : Window
    {
        public bool daKlik;

        public PotvrdaIzmene()
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
