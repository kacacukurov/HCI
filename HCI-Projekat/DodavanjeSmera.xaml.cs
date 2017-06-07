using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;

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
        private bool unosPrviPut;
        private string oznakaSmeraZaIzmenu;
        public int indeks;
        private bool dodavanjeSmeraIzborStarogUnosa;
        private Notifier notifierError;
        private Notifier notifierMainWindow;

        public DodavanjeSmera(RacunarskiCentar racunarskiCentar, ObservableCollection<Smer> smerovi, bool izmena, string oznaka, 
            Notifier notifierMainWindow)
        {
            notifierError = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: this,
                    corner: Corner.TopRight,
                    offsetX: 20,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: System.TimeSpan.FromSeconds(5),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(1));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
            this.notifierMainWindow = notifierMainWindow;
            smer = new Smer();
            this.racunarskiCentar = racunarskiCentar;
            this.izmena = izmena;
            this.unosPrviPut = true;
            this.oznakaSmeraZaIzmenu = oznaka;
            this.dodavanjeSmeraIzborStarogUnosa = false;
            tabelaSmerova = smerovi;
            InitializeComponent();
            if (!izmena)
                OznakaSmera.Focus();
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

        private void resetujBojuOkvira(object sender, EventArgs e)
        {
            try
            {
                TextBox t = (TextBox)sender;
                t.ClearValue(Border.BorderBrushProperty);
            }
            catch
            {
                DatePicker d = (DatePicker)sender;
                d.ClearValue(Border.BorderBrushProperty);
            }
        }

        private void UnetaOznakaSmera(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (!izmena)
            {
                if (racunarskiCentar.Smerovi.ContainsKey(t.Text.Trim()))
                    GreskaOznakaSmera.Text = "Oznaka zauzeta!";
                else
                    GreskaOznakaSmera.Text = "";
            }
            else if (!unosPrviPut && izmena)
            {
                if (racunarskiCentar.Smerovi.ContainsKey(t.Text.Trim()) && !t.Text.Trim().Equals(oznakaSmeraZaIzmenu))
                    GreskaOznakaSmera.Text = "Oznaka zauzeta!";
                else
                    GreskaOznakaSmera.Text = "";
            }
            unosPrviPut = false;
        }

        private void finishClick(object sender, RoutedEventArgs e)
        {
            if (izmena)
            {
                izmeniSmer();
                return;
            }
            if (validacijaDodavanjaSmera() && !dodavanjeSmeraIzborStarogUnosa)
            {
                smer.Naziv = NazivSmera.Text.Trim();
                smer.Oznaka = OznakaSmera.Text.Trim();
                smer.Opis = OpisSmera.Text.Trim();
                string datum = DateTime.Parse(DatumUvodjenja.Text.Trim()).ToString("dd/MM/yyyy");
                smer.Datum = datum;

                tabelaSmerova.Add(smer);
                racunarskiCentar.DodajSmer(smer);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierMainWindow.ShowSuccess("Uspešno ste dodali novi smer!");
                });
                this.Close();
            }
            else if (dodavanjeSmeraIzborStarogUnosa)
            {
                // ukoliko postoji smer (logicki neaktivan) sa istom oznakom
                // kao sto je uneta, ponovo aktiviramo taj smer (postaje logicki aktivan)
                tabelaSmerova.Add(racunarskiCentar.Smerovi[OznakaSmera.Text.Trim()]);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierMainWindow.ShowSuccess("Uspešno ste aktivirali smer!");
                });
                this.Close();
            }
        }

        private bool validacijaDodavanjaSmera()
        {
            if (!validacijaPraznihPolja())
            {
                return false;
            }
            else if (racunarskiCentar.Smerovi.ContainsKey(OznakaSmera.Text.Trim()))
            {

                if (racunarskiCentar.Smerovi[OznakaSmera.Text.Trim()].Obrisan)
                {
                    dodavanjeSmeraIzborStarogUnosa = false;
                    Smer smer = racunarskiCentar.Smerovi[OznakaSmera.Text.Trim()];

                    // vec postoji smer sa tom oznakom, ali je logicki obrisan
                    OdlukaDodavanjaSmer odluka = new OdlukaDodavanjaSmer();
                    odluka.Oznaka.Text = "Oznaka: " + smer.Oznaka;
                    odluka.Naziv.Text = "Naziv: " + smer.Naziv;
                    odluka.DatumUvodjenja.Text = "Datum uvođenja: " + smer.Datum;
                    odluka.ShowDialog();

                    if (odluka.potvrdaNovogUnosa)
                        // ukoliko je korisnik potvrdio da zeli da unese nove podatke, gazimo postojeci neaktivan smer
                        racunarskiCentar.Smerovi.Remove(OznakaSmera.Text.Trim());
                    else {
                        // vracamo logicki obrisan smer da bude aktivan
                        smer.Obrisan = false;
                        dodavanjeSmeraIzborStarogUnosa = true;
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        notifierError.ShowError("Smer sa unetom oznakom već postoji!");
                    });
                    OznakaSmera.Focus();
                    return false;
                }
            }
            return true;
        }

        private void izmeniSmer()
        {
            if (validacijaPraznihPolja())
            {
                Smer smerIzmena = racunarskiCentar.Smerovi[oznakaSmeraZaIzmenu];
                string staraOznaka = smerIzmena.Oznaka;
                bool oznakaIzmenjena = false;

                if (!staraOznaka.Equals(OznakaSmera.Text.Trim()))
                    oznakaIzmenjena = true;

                smerIzmena.Oznaka = OznakaSmera.Text.Trim();
                smerIzmena.Naziv = NazivSmera.Text.Trim();
                smerIzmena.Opis = OpisSmera.Text.Trim();
                string datum = DateTime.Parse(DatumUvodjenja.Text.Trim()).ToString("dd/MM/yyyy");
                smerIzmena.Datum = datum;

                if (smerIzmena.Predmeti.Count > 0)
                {
                    foreach (string predmet in smerIzmena.Predmeti)
                    {
                        racunarskiCentar.Predmeti[predmet].Smer = smerIzmena.Oznaka;
                        racunarskiCentar.Predmeti[predmet].SmerDetalji = "Oznaka: " + smerIzmena.Oznaka + "\nNaziv: " + smerIzmena.Naziv;
                    }
                }

                if (oznakaIzmenjena)
                {
                    racunarskiCentar.Smerovi.Remove(staraOznaka);
                    racunarskiCentar.Smerovi.Add(smerIzmena.Oznaka, smerIzmena);
                }

                tabelaSmerova[indeks] = smerIzmena;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierMainWindow.ShowSuccess("Uspešno ste izmenili smer!");
                });
            }
        }

        private bool validacijaPraznihPolja()
        {
            if (NazivSmera.Text.Trim() == "" || OznakaSmera.Text.Trim() == "" || OpisSmera.Text.Trim() == "" || DatumUvodjenja.Text.Trim() == "")
            {
                //povera paznih polja kako bi se uokvirila crvenim
                if (OznakaSmera.Text.Trim() == "")
                    OznakaSmera.BorderBrush = System.Windows.Media.Brushes.Red;
                if (NazivSmera.Text.Trim() == "")
                    NazivSmera.BorderBrush = System.Windows.Media.Brushes.Red;
                if (OpisSmera.Text.Trim() == "")
                    OpisSmera.BorderBrush = System.Windows.Media.Brushes.Red;
                if (DatumUvodjenja.Text.Trim() == "")
                    DatumUvodjenja.BorderBrush = System.Windows.Media.Brushes.Red;

                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Niste popunili sva polja!!");
                });
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
            if (e.Key == Key.Tab)
            {
                if (!((Keyboard.Modifiers & (ModifierKeys.Shift)) == ModifierKeys.Shift))
                    DatumUvodjenja.IsDropDownOpen = true;
            }

        }

        private void otvoriUnazad(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && (Keyboard.Modifiers & (ModifierKeys.Shift)) == ModifierKeys.Shift)
                DatumUvodjenja.IsDropDownOpen = true;
        }
    }
}