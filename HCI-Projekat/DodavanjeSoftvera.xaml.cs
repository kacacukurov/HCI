using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

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
        private bool izmena;
        public int indeks;
        private bool dodavanjeSoftveraIzborStarogUnosa;

        public DodavanjeSoftvera(RacunarskiCentar racunarskiCentar, ObservableCollection<Softver> softveri, bool izmena)
        {
            InitializeComponent();
            this.racunarskiCentar = racunarskiCentar;
            this.izmena = izmena;
            this.dodavanjeSoftveraIzborStarogUnosa = false;
            tabelaSoftvera = softveri;
            noviSoftver = new Softver();
            if(!izmena)
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
            if (izmena)
            {
                izmeniSoftver();
                return;
            }
            if (validacijaNovogSoftvera() && !dodavanjeSoftveraIzborStarogUnosa)
            {
                noviSoftver.Oznaka = oznakaSoftver.Text.Trim();
                noviSoftver.Naziv = nazivSoftver.Text.Trim();
                noviSoftver.Opis = opisSoftver.Text.Trim();
                noviSoftver.GodIzdavanja = int.Parse(godinaSoftver.Text.Trim());
                noviSoftver.Cena = double.Parse(cenaSoftver.Text.Trim());
                if ((bool)WindowsOSSoftver.IsChecked)
                    noviSoftver.OperativniSistem = "Windows";
                else if ((bool)LinuxOSSoftver.IsChecked)
                    noviSoftver.OperativniSistem = "Linux";
                else
                    noviSoftver.OperativniSistem = "Windows i Linux";
                noviSoftver.Proizvodjac = proizvodjacSoftver.Text.Trim();
                noviSoftver.Sajt = sajtSoftver.Text.Trim();

                tabelaSoftvera.Add(noviSoftver);
                racunarskiCentar.DodajSoftver(noviSoftver);
                this.Close();
            }
            else if (dodavanjeSoftveraIzborStarogUnosa)
            {
                // ukoliko postoji softver (logicki neaktivan) sa istom oznakom
                // kao sto je uneta, ponovo aktiviramo taj softver (postaje logicki aktivan)
                tabelaSoftvera.Add(racunarskiCentar.Softveri[oznakaSoftver.Text.Trim()]);
                this.Close();
            }
        }

        private bool validacijaNovogSoftvera()
        {
            if (racunarskiCentar.Softveri.ContainsKey(oznakaSoftver.Text.Trim()))
            {
                if (racunarskiCentar.Softveri[oznakaSoftver.Text.Trim()].Obrisan)
                {
                    dodavanjeSoftveraIzborStarogUnosa = false;
                    Softver softver = racunarskiCentar.Softveri[oznakaSoftver.Text.Trim()];

                    // vec postoji softver sa tom oznakom, ali je logicki obrisan
                    OdlukaDodavanjaSoftver odluka = new OdlukaDodavanjaSoftver();
                    odluka.Oznaka.Text = "Oznaka: " + softver.Oznaka;
                    odluka.Naziv.Text = "Naziv: " + softver.Naziv;
                    odluka.OperativniSistem.Text = "Operativni sistem: " + softver.OperativniSistem;
                    odluka.Proizvodjac.Text = "Proizvođač: " + softver.Proizvodjac;
                    odluka.GodinaIzdavanja.Text = "Godina izdavanja: " + softver.GodIzdavanja.ToString();
                    odluka.Sajt.Text = "Sajt: " + softver.Sajt;
                    odluka.Cena.Text = "Cena: " + softver.Cena.ToString();
                    odluka.ShowDialog();

                    if (odluka.potvrdaNovogUnosa)
                        // ukoliko je korisnik potvrdio da zeli da unese nove podatke, gazimo postojeci neaktivan softver
                        racunarskiCentar.Softveri.Remove(oznakaSoftver.Text.Trim());
                    else {
                        // vracamo logicki obrisan softver da bude aktivan
                        softver.Obrisan = false;
                        dodavanjeSoftveraIzborStarogUnosa = true;
                    }
                }
                else
                {
                    MessageBox.Show("Softver sa unetom oznakom već postoji!");
                    vratiNaKorak1();
                    UpdateLayout();
                    oznakaSoftver.Focus();
                    return false;
                }
            }
            if (!validacijaPodataka())
                return false;
            return true;
        }

        private void vratiNaKorak1()
        {
            Keyboard.ClearFocus();
            BackStepMenuItem.IsEnabled = false;
            NextStepMenuItem.IsEnabled = true;
            Korak1Softver.Focus();
        }

        private void vratiNaKorak2()
        {
            Keyboard.ClearFocus();
            BackStepMenuItem.IsEnabled = true;
            NextStepMenuItem.IsEnabled = false;
            Korak2Softver.Focus();
        }

        private bool validacijaPodataka()
        {
            if (oznakaSoftver.Text.Trim() == "" || nazivSoftver.Text.Trim() == "" || proizvodjacSoftver.Text.Trim() == "" || opisSoftver.Text.Trim() == "" 
                || godinaSoftver.Text.Trim() == "" || cenaSoftver.Text.Trim() == "" || sajtSoftver.Text.Trim() == "")
            {
                MessageBox.Show("Niste popunili sva polja!");
                if (oznakaSoftver.Text.Trim() == "")
                {
                    vratiNaKorak1();
                    UpdateLayout();
                    oznakaSoftver.Focus();
                }
                else if (nazivSoftver.Text.Trim() == "")
                {
                    vratiNaKorak1();
                    UpdateLayout();
                    nazivSoftver.Focus();
                }
                else if (proizvodjacSoftver.Text.Trim() == "")
                {
                    vratiNaKorak1();
                    UpdateLayout();
                    proizvodjacSoftver.Focus();
                }
                else if (sajtSoftver.Text.Trim() == "")
                {
                    vratiNaKorak2();
                    UpdateLayout();
                    sajtSoftver.Focus();
                }
                else if (godinaSoftver.Text.Trim() == "")
                {
                    vratiNaKorak2();
                    UpdateLayout();
                    godinaSoftver.Focus();
                }
                else if (cenaSoftver.Text.Trim() == "")
                {
                    vratiNaKorak2();
                    UpdateLayout();
                    cenaSoftver.Focus();
                }
                else if (opisSoftver.Text.Trim() == "")
                {
                    vratiNaKorak2();
                    UpdateLayout();
                    opisSoftver.Focus();
                }
                
                return false;
            }

            int godina;
            if (!int.TryParse(godinaSoftver.Text.Trim(), out godina))
            {
                MessageBox.Show("Godina nije dobro unesena, unesite broj!");
                godinaSoftver.Text = "";
                vratiNaKorak2();
                UpdateLayout();
                godinaSoftver.Focus();
                return false;
            }

            double cena;
            if (!double.TryParse(cenaSoftver.Text.Trim(), out cena))
            {
                MessageBox.Show("Cena nije dobro unesena, unesite broj!");
                cenaSoftver.Text = "";
                vratiNaKorak2();
                UpdateLayout();
                cenaSoftver.Focus();
                return false;
            }

            return true;
        }

        private void izmeniSoftver()
        {
            if (validacijaPodataka())
            {
                Softver softverIzmena = racunarskiCentar.Softveri[oznakaSoftver.Text.Trim()];
                bool promenilaSeOznaka = false;
                bool promenioSeNaziv = false;
                bool promenioSeOpis = false;

                if (softverIzmena.Naziv != nazivSoftver.Text.Trim())
                    promenioSeNaziv = true;
                softverIzmena.Naziv = nazivSoftver.Text.Trim();

                if (softverIzmena.Opis != opisSoftver.Text.Trim())
                    promenioSeOpis = true;
                softverIzmena.Opis = opisSoftver.Text.Trim();

                softverIzmena.GodIzdavanja = int.Parse(godinaSoftver.Text.Trim());
                softverIzmena.Cena = double.Parse(cenaSoftver.Text.Trim());

                if ((bool)WindowsOSSoftver.IsChecked)
                    softverIzmena.OperativniSistem = "Windows";
                else if ((bool)LinuxOSSoftver.IsChecked)
                    softverIzmena.OperativniSistem = "Linux";
                else
                    softverIzmena.OperativniSistem = "Windows i Linux";

                softverIzmena.Proizvodjac = proizvodjacSoftver.Text.Trim();
                softverIzmena.Sajt = sajtSoftver.Text.Trim();

                // DODATI AZURIRANJE STRINGA U UCIONICI/PREDMETU UKOLIKO SE PROMENIO NAZIV, OZNAKA ILI OPIS SOFTVERA
                /*
                if(promenilaSeOznaka || promenioSeNaziv || promenioSeOpis)
                {
                    // naci povezane ucionice i predmete i azurirati im ispis
                }
                */

                tabelaSoftvera[indeks] = softverIzmena;
                this.Close();
            }
        }
    }
}
