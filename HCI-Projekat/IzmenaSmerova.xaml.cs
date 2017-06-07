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
        private Notifier notifierError;

        public IzmenaSmerova(RacunarskiCentar racunarskiCentar, ObservableCollection<Smer> smerovi, List<int> indeksi)
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
            this.racunarskiCentar = racunarskiCentar;
            this.indeksi = indeksi;
            tabelaSmerova = smerovi;
            InitializeComponent();
            NazivSmera.Focus();
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

        private void finishClick(object sender, RoutedEventArgs e)
        {
            if (validacijaPraznihPolja())
            {
                foreach (int index in indeksi)
                {
                    Smer smerIzmena = racunarskiCentar.Smerovi[tabelaSmerova[index].Oznaka];
                    bool nazivPromenjen = false;
                    bool opisPromenjen = false;

                    if(NazivSmera.Text.Trim() != "")
                    {
                        smerIzmena.Naziv = NazivSmera.Text.Trim();
                        nazivPromenjen = true;
                    }
                    if (OpisSmera.Text.Trim() != "")
                    {
                        smerIzmena.Opis = OpisSmera.Text.Trim();
                        opisPromenjen = true;
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
    }
}
