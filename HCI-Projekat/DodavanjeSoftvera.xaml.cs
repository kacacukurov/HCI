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
            BackStepMenuItem.IsEnabled = false;
        }

        private void cutClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cut");
        }

        private void copyClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Copy");
        }

        private void pasteClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Paste");
        }

        private void undoClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Undo");
        }

        private void redoClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Redo");
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
            NextStepMenuItem.IsEnabled = false;
            BackStepMenuItem.IsEnabled = true;
            Korak2Softver.Focus();
        }

        public void backClick(object sender, RoutedEventArgs e)
        {
            BackStepMenuItem.IsEnabled = false;
            NextStepMenuItem.IsEnabled = true;
            Korak1Softver.Focus();
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void finishClick(object sender, RoutedEventArgs e)
        {
            if (validacijaNovogSoftvera())
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

        private bool validacijaNovogSoftvera()
        {
            if (racunarskiCentar.Softveri.ContainsKey(oznakaSoftver.Text))
            {
                if (racunarskiCentar.Softveri[oznakaSoftver.Text].Obrisan)
                    racunarskiCentar.Softveri.Remove(oznakaSoftver.Text);
                else
                {
                    MessageBox.Show("Softver sa ovom oznakom vec postoji!");
                    oznakaSoftver.Text = "";
                    oznakaSoftver.Focus();
                    return false;
                }
            }
            if (oznakaSoftver.Text == "" || nazivSoftver.Text == "" || opisSoftver.Text == "" || godinaSoftver.Text == "" ||
                cenaSoftver.Text == "" || proizvodjacSoftver.Text == "" || sajtSoftver.Text == "")
            {
                MessageBox.Show("Niste popunili sva polja!");
                if (oznakaSoftver.Text == "")
                    oznakaSoftver.Focus();
                else if (nazivSoftver.Text == "")
                    nazivSoftver.Focus();
                else if (opisSoftver.Text == "")
                    opisSoftver.Focus();
                else if (godinaSoftver.Text == "")
                    godinaSoftver.Focus();
                else if (cenaSoftver.Text == "")
                    cenaSoftver.Focus();
                else if (proizvodjacSoftver.Text == "")
                    proizvodjacSoftver.Focus();
                else if (sajtSoftver.Text == "")
                    sajtSoftver.Focus();
                return false;
            }
            int godina;
            if (!int.TryParse(godinaSoftver.Text, out godina))
            {
                MessageBox.Show("Godina nije dobro unesena, unesite broj!");
                godinaSoftver.Text = "";
                godinaSoftver.Focus();
                return false;
            }
            double cena;
            if (!double.TryParse(cenaSoftver.Text, out cena))
            {
                MessageBox.Show("Cena nije dobro unesena, unesite broj!");
                cenaSoftver.Text = "";
                cenaSoftver.Focus();
                return false;
            }
            return true;
        }
    }
}
