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
    /// Interaction logic for DodavanjeSoftvera.xaml
    /// </summary>
    public partial class DodavanjeSoftvera : Window
    {
        private Softver noviSoftver;
        private RacunarskiCentar racunarskiCentar;
        private ObservableCollection<Softver> tabelaSoftvera;

        public DodavanjeSoftvera(RacunarskiCentar racunarskiCentar, ObservableCollection<Softver> softveri)
        {
            InitializeComponent();
            this.racunarskiCentar = racunarskiCentar;
            tabelaSoftvera = softveri;
            noviSoftver = new Softver();
            oznakaSoftver.Focus();
        }

        public void nextStep(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            nextClick(sender, e);
        }

        public void backStep(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            backClick(sender, e);
        }

        public void nextClick(object sender, RoutedEventArgs e)
        {
            Korak2Softver.Focus();
        }

        public void backClick(object sender, RoutedEventArgs e)
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

            tabelaSoftvera.Add(noviSoftver);
            racunarskiCentar.DodajSoftver(noviSoftver);
            this.Close();
        }
    }
}
