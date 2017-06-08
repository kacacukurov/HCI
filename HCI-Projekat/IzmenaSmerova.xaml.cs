using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Specialized;

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for IzmenaSmerova.xaml
    /// </summary>
    public partial class IzmenaSmerova : Window
    {
        private RacunarskiCentar racunarskiCentar;
        private ObservableCollection<Smer> tabelaSmerova;
        private List<int> indeksi;
        public bool potvrdaIzmena;
        private UndoRedoStack stekStanja;
        OrderedDictionary prethodnaStanjaAplikacije;
        private Notifier notifierError;
        private Notifier notifierMainWindow;

        public IzmenaSmerova(RacunarskiCentar racunarskiCentar, ObservableCollection<Smer> smerovi, List<int> indeksi,
            UndoRedoStack stek, OrderedDictionary prethodnaStanja, Notifier mainWindowNotifier)
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

            InitializeComponent();
            this.potvrdaIzmena = false;
            this.stekStanja = stek;
            this.prethodnaStanjaAplikacije = prethodnaStanja;
            this.racunarskiCentar = racunarskiCentar;
            this.indeksi = indeksi;
            this.notifierMainWindow = mainWindowNotifier;
            tabelaSmerova = smerovi;
            InitializeComponent();
            NazivSmera.Focus();
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

        private void finishClick(object sender, RoutedEventArgs e)
        {
            if (validacijaPraznihPolja())
            {
                // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                StanjeAplikacije staroStanje = new StanjeAplikacije();
                staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                staroStanje.TipPodataka = "smer";
                staroStanje.Kolicina = indeksi.Count;
                staroStanje.TipPromene = "izmena";
                foreach(int index in indeksi)
                {
                    staroStanje.Oznake.Add(tabelaSmerova[index].Oznaka);
                }

                foreach (int index in indeksi)
                {
                    Smer smerIzmena = racunarskiCentar.Smerovi[tabelaSmerova[index].Oznaka];
                    bool nazivPromenjen = false;

                    if(NazivSmera.Text.Trim() != "")
                    {
                        smerIzmena.Naziv = NazivSmera.Text.Trim();
                        nazivPromenjen = true;
                    }
                    if (OpisSmera.Text.Trim() != "")
                    {
                        smerIzmena.Opis = OpisSmera.Text.Trim();
                    }
                    if(DatumUvodjenja.Text.Trim() != "")
                        smerIzmena.Datum = DateTime.Parse(DatumUvodjenja.Text.Trim()).ToString("dd/MM/yyyy");

                    if (smerIzmena.Predmeti.Count > 0 && nazivPromenjen)
                    {
                        foreach (string predmet in smerIzmena.Predmeti)
                        {
                            racunarskiCentar.Predmeti[predmet].SmerDetalji = "Oznaka: " + smerIzmena.Oznaka + "\nNaziv: " + smerIzmena.Naziv;
                        }
                    }

                    tabelaSmerova[index] = smerIzmena;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierMainWindow.ShowSuccess("Uspešno ste izmenili smerove!");
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
                potvrdaIzmena = true;

                this.Close();
            }
        }
        
        private bool validacijaPraznihPolja()
        {
            if (NazivSmera.Text.Trim() == "" && OpisSmera.Text.Trim() == "" && DatumUvodjenja.Text.Trim() == "")
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Niste popunili nijedno polje!");
                });
                if (NazivSmera.Text.Trim() == "")
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

        private void otvoriHelp(object sender, RoutedEventArgs e)
        {
            IInputElement focusedControl = FocusManager.GetFocusedElement(Application.Current.Windows[0]);
            if (focusedControl is DependencyObject)
            {
                string str = HelpProvider.GetHelpKey((DependencyObject)focusedControl);
                HelpProvider.ShowHelp("izmenaSmerova", this);
            }
        }
    }
}
