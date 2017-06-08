using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
        OrderedDictionary prethodnaStanjaAplikacije;
        StanjeAplikacije staroStanje;
        public bool potvrdio;
        private UndoRedoStack stekStanja;

        public DodavanjeSmera(RacunarskiCentar racunarskiCentar, ObservableCollection<Smer> smerovi, bool izmena, string oznaka, 
            Notifier notifierMainWindow, UndoRedoStack stack, OrderedDictionary prethodnaStanja)
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

            this.prethodnaStanjaAplikacije = prethodnaStanja;
            this.staroStanje = null;
            this.potvrdio = false;
            this.stekStanja = stack;
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
                // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                staroStanje = new StanjeAplikacije(DeepClone(racunarskiCentar), "Dodat novi smer sa oznakom " + OznakaSmera.Text.Trim(), "smer");

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

                // na undo stek treba da upisemo staro stanje aplikacije
                // generisemo neki novi kljuc pod kojim cemo cuvati prethodno stanje na steku
                string kljuc = Guid.NewGuid().ToString();
                // proveravamo da li vec ima 10 koraka za undo operaciju, ako ima, izbacujemo prvi koji je ubacen kako bismo 
                // i dalje imali 10 mogucih koraka, ali ukljucujuci i ovaj novi
                if (prethodnaStanjaAplikacije.Count >= 2)
                    prethodnaStanjaAplikacije.RemoveAt(0);
                prethodnaStanjaAplikacije.Add(kljuc, staroStanje);
                stekStanja.GetUndo().Push(kljuc);
                // postavljamo flag na true, da bismo mogli da omogucimo klik na dugme za undo operaciju
                potvrdio = true;

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

                // na undo stek treba da upisemo staro stanje aplikacije
                // generisemo neki novi kljuc pod kojim cemo cuvati prethodno stanje na steku
                string kljuc = Guid.NewGuid().ToString();
                // proveravamo da li vec ima 10 koraka za undo operaciju, ako ima, izbacujemo prvi koji je ubacen kako bismo 
                // i dalje imali 10 mogucih koraka, ali ukljucujuci i ovaj novi
                if (prethodnaStanjaAplikacije.Count >= 2)
                    prethodnaStanjaAplikacije.RemoveAt(0);
                prethodnaStanjaAplikacije.Add(kljuc, staroStanje);
                stekStanja.GetUndo().Push(kljuc);
                // postavljamo flag na true, da bismo mogli da omogucimo klik na dugme za undo operaciju
                potvrdio = true;

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
                        // pamtimo staro stanje aplikacije zbog undo redo mehanizma
                        staroStanje = new StanjeAplikacije(DeepClone(racunarskiCentar), "Aktiviran logički obrisan smer sa oznakom " + smer.Oznaka, "smer");

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
                // pamtimo staro stanje aplikacije zbog undo redo mehanizma
                staroStanje = new StanjeAplikacije(DeepClone(racunarskiCentar), "Izmenjen smer sa oznakom " + oznakaSmeraZaIzmenu, "smer");

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

                // na undo stek treba da upisemo staro stanje aplikacije
                // generisemo neki novi kljuc pod kojim cemo cuvati prethodno stanje na steku
                string kljuc = Guid.NewGuid().ToString();
                // proveravamo da li vec ima 10 koraka za undo operaciju, ako ima, izbacujemo prvi koji je ubacen kako bismo 
                // i dalje imali 10 mogucih koraka, ali ukljucujuci i ovaj novi
                if (prethodnaStanjaAplikacije.Count >= 2)
                    prethodnaStanjaAplikacije.RemoveAt(0);
                prethodnaStanjaAplikacije.Add(kljuc, staroStanje);
                stekStanja.GetUndo().Push(kljuc);
                // postavljamo flag na true, da bismo mogli da omogucimo klik na dugme za undo operaciju
                potvrdio = true;

                this.Close();
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

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}