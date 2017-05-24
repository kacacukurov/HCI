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
    /// Interaction logic for DodavanjeSmera.xaml
    /// </summary>
    public partial class DodavanjeSmera : Window
    {
        private Smer smer;
        private RacunarskiCentar racunarskiCentar;
        private ObservableCollection<Smer> tabelaSmerova;

        public DodavanjeSmera(RacunarskiCentar racunarskiCentar, ObservableCollection<Smer> smerovi)
        {
            smer = new Smer();
            this.racunarskiCentar = racunarskiCentar;
            tabelaSmerova = smerovi;
            InitializeComponent();
            OznakaSmera.Focus();
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

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void finishClick(object sender, RoutedEventArgs e)
        {
            if(validacijaDodavanjaSmera())
            {
                smer.Naziv = NazivSmera.Text;
                smer.Oznaka = OznakaSmera.Text;
                smer.Opis = OpisSmera.Text;
                smer.Datum = DateTime.Parse(DatumUvodjenja.Text);

                tabelaSmerova.Add(smer);
                racunarskiCentar.DodajSmer(smer);
                this.Close();
            }
            
        }

        private bool validacijaDodavanjaSmera()
        {
            if (racunarskiCentar.Smerovi.ContainsKey(OznakaSmera.Text))
            {
                if (racunarskiCentar.Smerovi[OznakaSmera.Text].Obrisan)
                    racunarskiCentar.Smerovi.Remove(OznakaSmera.Text);
                else
                {
                    MessageBox.Show("Smer sa ovom oznakom vec postoji!");
                    OznakaSmera.Text = "";
                    OznakaSmera.Focus();
                    return false;
                }
            }
            else if (NazivSmera.Text == "" || OznakaSmera.Text == "" || OpisSmera.Text == "" || DatumUvodjenja.Text == "")
            {
                MessageBox.Show("Niste popunili sva polja!");
                if (OznakaSmera.Text == "")
                    OznakaSmera.Focus();
                else if (NazivSmera.Text == "")
                    NazivSmera.Focus();
                else if (DatumUvodjenja.Text == "")
                    DatumUvodjenja.Focus();
                else if (OpisSmera.Text == "")
                    OpisSmera.Focus();
                return false;
            }
            return true;
        }

        private void otvoriDatum(object sender, RoutedEventArgs e)
        {
            DatumUvodjenja.IsDropDownOpen = true;
        }

        private void otvori(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Tab)
            {
                if (!((Keyboard.Modifiers & (ModifierKeys.Shift)) == ModifierKeys.Shift))
                    DatumUvodjenja.IsDropDownOpen = true;
            }
                
        }

        private void otvoriUnazad(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Tab && (Keyboard.Modifiers & ( ModifierKeys.Shift)) == ModifierKeys.Shift)
                DatumUvodjenja.IsDropDownOpen = true;
        }
    }
}