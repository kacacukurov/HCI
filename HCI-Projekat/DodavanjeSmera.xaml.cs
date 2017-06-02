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
        private bool izmena;
        public int indeks;

        public DodavanjeSmera(RacunarskiCentar racunarskiCentar, ObservableCollection<Smer> smerovi, bool izmena)
        {
            smer = new Smer();
            this.racunarskiCentar = racunarskiCentar;
            this.izmena = izmena;
            tabelaSmerova = smerovi;
            InitializeComponent();
            if(!izmena)
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
            if (izmena)
            {
                izmeniSmer();
                return;
            }
            if(validacijaDodavanjaSmera())
            {
                smer.Naziv = NazivSmera.Text.Trim();
                smer.Oznaka = OznakaSmera.Text.Trim();
                smer.Opis = OpisSmera.Text.Trim();
                string datum = DateTime.Parse(DatumUvodjenja.Text.Trim()).ToString("dd/MM/yyyy");
                smer.Datum = DateTime.Parse(datum);

                tabelaSmerova.Add(smer);
                racunarskiCentar.DodajSmer(smer);
                this.Close();
            }
        }

        private bool validacijaDodavanjaSmera()
        {
            if (racunarskiCentar.Smerovi.ContainsKey(OznakaSmera.Text.Trim()))
            {
                if (racunarskiCentar.Smerovi[OznakaSmera.Text.Trim()].Obrisan)
                    racunarskiCentar.Smerovi.Remove(OznakaSmera.Text.Trim());
                else
                {
                    MessageBox.Show("Smer sa unetom oznakom već postoji!");
                    OznakaSmera.Focus();
                    return false;
                }
            }
            else if (!validacijaPraznihPolja())
            {
                return false;
            }
            return true;
        }

        private void izmeniSmer()
        {
            if (validacijaPraznihPolja())
            {
                Smer smerIzmena = racunarskiCentar.Smerovi[OznakaSmera.Text.Trim()];

                smerIzmena.Naziv = NazivSmera.Text.Trim();
                smerIzmena.Opis = OpisSmera.Text.Trim();
                smerIzmena.Datum = DateTime.Parse(DatumUvodjenja.Text.Trim());

                if (smerIzmena.Predmeti.Count > 0)
                {
                    foreach (string predmet in smerIzmena.Predmeti)
                    {
                        racunarskiCentar.Predmeti[predmet].SmerDetalji = "Oznaka: " + smerIzmena.Oznaka + "\nNaziv: " + smerIzmena.Naziv;
                    }
                }

                tabelaSmerova[indeks] = smerIzmena;
                this.Close();
            }
        }

        private bool validacijaPraznihPolja()
        {
            if (NazivSmera.Text.Trim() == "" || OznakaSmera.Text.Trim() == "" || OpisSmera.Text.Trim() == "" || DatumUvodjenja.Text.Trim() == "")
            {
                MessageBox.Show("Niste popunili sva polja!");
                if (OznakaSmera.Text.Trim() == "")
                    OznakaSmera.Focus();
                else if (NazivSmera.Text.Trim() == "")
                    NazivSmera.Focus();
                else if (DatumUvodjenja.Text.Trim() == "")
                {
                    DatumUvodjenja.Focus();
                    DatumUvodjenja.IsDropDownOpen = true;
                }
                else if (OpisSmera.Text.Trim() == "")
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