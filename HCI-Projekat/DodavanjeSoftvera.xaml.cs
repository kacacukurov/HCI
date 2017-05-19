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
        private Softver noviSoftver;
        private RacunarskiCentar racunarskiCentar;

        public DodavanjeSoftvera(RacunarskiCentar racunarskiCentar)
        {
            InitializeComponent();
            this.racunarskiCentar = racunarskiCentar;
            noviSoftver = new Softver();
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
            noviSoftver.Oznaka = oznakaSoftver.Text;
            noviSoftver.Naziv = nazivSoftver.Text;
            noviSoftver.Opis = opisSoftver.Text;
            noviSoftver.GodIzdavanja = int.Parse(godinaSoftver.Text);
            noviSoftver.Cena = double.Parse(cenaSoftver.Text);
            if ((bool)WindowsOSSoftver.IsChecked)
                noviSoftver.OperativniSistem = "Windows";
            else if ((bool)LinusOSSoftver.IsChecked)
                noviSoftver.OperativniSistem = "Linux";
            else
                noviSoftver.OperativniSistem = "Linux i Windows";
            noviSoftver.Proizvodjac = proizvodjacSoftver.Text;
            noviSoftver.Sajt = sajtSoftver.Text;

            racunarskiCentar.DodajSoftver(noviSoftver);
            this.Close();
        }
    }
}
