﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
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
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for Tutorijal.xaml
    /// </summary>
    public partial class Tutorijal : Window
    {
        private Softver noviSoftver;
        private RacunarskiCentar racunarskiCentar;
        private ObservableCollection<Softver> tabelaSoftvera;
        private bool izmena;
        private bool unosPrviPut;
        private string oznakaSoftveraZaIzmenu;
        public int indeks;
        private bool dodavanjeSoftveraIzborStarogUnosa;
        private ToastNotifications.Notifier notifierError;
        private Notifier notifierMainWindow;
        private UndoRedoStack stekStanja;
        OrderedDictionary prethodnaStanjaAplikacije;
        StanjeAplikacije staroStanje;
        public bool potvrdio;
        Popup pop = null;
        FrameworkElement fokusiran = null;

        public Tutorijal(RacunarskiCentar racunarskiCentar, ObservableCollection<Softver> softveri, bool izmena, string oznaka,
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
            this.potvrdio = false;
            this.stekStanja = stack;
            this.notifierMainWindow = notifierMainWindow;
            InitializeComponent();
            this.racunarskiCentar = racunarskiCentar;
            this.izmena = izmena;
            this.unosPrviPut = true;
            this.oznakaSoftveraZaIzmenu = oznaka;
            this.dodavanjeSoftveraIzborStarogUnosa = false;
            tabelaSoftvera = softveri;
            noviSoftver = new Softver();
            if (!izmena)
                oznakaSoftver.Focus();
            BackStepMenuItem.IsEnabled = false;
            this.prethodnaStanjaAplikacije = prethodnaStanja;
            this.staroStanje = null;

            //ukljuci prvi pop-up
            popupOznaka.IsOpen = true;
            pop = popupOznaka;
            Window w = this;
            if (null != w)
            {
                w.LocationChanged += delegate (object sender, EventArgs e)
                {
                    var offset = popupOznaka.HorizontalOffset;
                    popupOznaka.HorizontalOffset = offset + 1;
                    popupOznaka.HorizontalOffset = offset;

                    offset = popupNaziv.HorizontalOffset;
                    popupNaziv.HorizontalOffset = offset + 1;
                    popupNaziv.HorizontalOffset = offset;

                    offset = popupOS.HorizontalOffset;
                    popupOS.HorizontalOffset = offset + 1;
                    popupOS.HorizontalOffset = offset;

                    offset = popupProizvodjac.HorizontalOffset;
                    popupProizvodjac.HorizontalOffset = offset + 1;
                    popupProizvodjac.HorizontalOffset = offset;

                    offset = popupNastavi.HorizontalOffset;
                    popupNastavi.HorizontalOffset = offset + 1;
                    popupNastavi.HorizontalOffset = offset;

                    offset = popupSajt.HorizontalOffset;
                    popupSajt.HorizontalOffset = offset + 1;
                    popupSajt.HorizontalOffset = offset;

                    offset = popupGodina.HorizontalOffset;
                    popupGodina.HorizontalOffset = offset + 1;
                    popupGodina.HorizontalOffset = offset;

                    offset = popupCena.HorizontalOffset;
                    popupCena.HorizontalOffset = offset + 1;
                    popupCena.HorizontalOffset = offset;

                    offset = popupOpis.HorizontalOffset;
                    popupOpis.HorizontalOffset = offset + 1;
                    popupOpis.HorizontalOffset = offset;

                    offset = popupZavrsi.HorizontalOffset;
                    popupZavrsi.HorizontalOffset = offset + 1;
                    popupZavrsi.HorizontalOffset = offset;
                };
                
                w.SizeChanged += delegate (object sender, SizeChangedEventArgs e)
                {
                    var offset = popupOznaka.HorizontalOffset;
                    popupOznaka.HorizontalOffset = offset + 1;
                    popupOznaka.HorizontalOffset = offset;
                    
                    offset = popupNaziv.HorizontalOffset;
                    popupNaziv.HorizontalOffset = offset + 1;
                    popupNaziv.HorizontalOffset = offset;
                    
                    offset = popupOS.HorizontalOffset;
                    popupOS.HorizontalOffset = offset + 1;
                    popupOS.HorizontalOffset = offset;
                    
                    offset = popupProizvodjac.HorizontalOffset;
                    popupProizvodjac.HorizontalOffset = offset + 1;
                    popupProizvodjac.HorizontalOffset = offset;
                    
                    offset = popupNastavi.HorizontalOffset;
                    popupNastavi.HorizontalOffset = offset + 1;
                    popupNastavi.HorizontalOffset = offset;
                    
                    offset = popupSajt.HorizontalOffset;
                    popupSajt.HorizontalOffset = offset + 1;
                    popupSajt.HorizontalOffset = offset;
                    
                    offset = popupGodina.HorizontalOffset;
                    popupGodina.HorizontalOffset = offset + 1;
                    popupGodina.HorizontalOffset = offset;
                    
                    offset = popupCena.HorizontalOffset;
                    popupCena.HorizontalOffset = offset + 1;
                    popupCena.HorizontalOffset = offset;
                    
                    offset = popupOpis.HorizontalOffset;
                    popupOpis.HorizontalOffset = offset + 1;
                    popupOpis.HorizontalOffset = offset;
                    
                    offset = popupZavrsi.HorizontalOffset;
                    popupZavrsi.HorizontalOffset = offset + 1;
                    popupZavrsi.HorizontalOffset = offset;
                };
            }
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
            PotvrdaOdustajanja potvrda = new PotvrdaOdustajanja();
            potvrda.ShowDialog();
            if (potvrda.daKlik)
                this.Close();
        }

        private void resetujBojuOkvira(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.ClearValue(Border.BorderBrushProperty);
        }

        private void UnetaOznakaSoftvera(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (racunarskiCentar.Softveri.ContainsKey(t.Text.Trim()) && !racunarskiCentar.Softveri[t.Text.Trim()].Obrisan)
                GreskaOznakaSoftvera.Text = "Oznaka zauzeta!";
            else
                GreskaOznakaSoftvera.Text = "";
            if (t.Text.Trim().Equals("") || !GreskaOznakaSoftvera.Text.Equals(""))
            {
                nextButton.IsEnabled = false;
                Korak2Softver.IsEnabled = false;
            }else
            {
                if(stakRadioButton.IsEnabled && proizvodjacSoftver.IsEnabled)
                {
                    nextButton.IsEnabled = true;
                    Korak2Softver.IsEnabled = true;
                }
            }
        }

        private void proveriUnetuGodinu(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (t.Text.Trim().Equals(string.Empty))
                GreskaGodinaSoftver.Text = "";
            else
            {
                try
                {
                    int godina = Int32.Parse(t.Text.Trim());
                    if (godina <= 0)
                        GreskaGodinaSoftver.Text = "Unesite ceo pozitivan broj!";
                    else
                        GreskaGodinaSoftver.Text = "";
                }
                catch
                {
                    GreskaGodinaSoftver.Text = "Unesite ceo pozitivan broj!";
                }
            }

            if (t.Text.Trim().Equals("") || !GreskaGodinaSoftver.Text.Equals(""))
            {
                finishButton.IsEnabled = false;
            }
            else
            {
                if(!cenaSoftver.Text.Trim().Equals("") && opisSoftver.IsEnabled)
                    finishButton.IsEnabled = true;
            }
        }

        private void proveriUnetuCenu(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (t.Text.Trim().Equals(string.Empty))
                GreskaCenaSoftver.Text = "";
            else
            {
                try
                {
                    double cena = Double.Parse(t.Text.Trim());
                    if (cena <= 0)
                        GreskaCenaSoftver.Text = "Unesite realan pozitivan broj!";
                    else
                        GreskaCenaSoftver.Text = "";
                }
                catch
                {
                    GreskaCenaSoftver.Text = "Unesite realan pozitivan broj!";
                }
            }
            if (t.Text.Trim().Equals("") || !GreskaCenaSoftver.Text.Equals(""))
            {
                finishButton.IsEnabled = false;
            }
            else
            {
                if(!opisSoftver.Text.Trim().Equals(""))
                    finishButton.IsEnabled = true;
            }
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
                // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                staroStanje = new StanjeAplikacije();
                staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                staroStanje.TipPodataka = "softver";
                staroStanje.Kolicina = 1;
                staroStanje.TipPromene = "brisanje";
                staroStanje.Oznake.Add(oznakaSoftver.Text.Trim());

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
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierMainWindow.ShowSuccess("Uspešno ste dodali novi softver!");
                });

                // na undo stek treba da upisemo staro stanje aplikacije
                // generisemo neki novi kljuc pod kojim cemo cuvati prethodno stanje na steku
                string kljuc = Guid.NewGuid().ToString();
                // proveravamo da li vec ima 10 koraka za undo operaciju, ako ima, izbacujemo prvi koji je ubacen kako bismo 
                // i dalje imali 10 mogucih koraka, ali ukljucujuci i ovaj novi
                if (prethodnaStanjaAplikacije.Count >= 3)
                    prethodnaStanjaAplikacije.RemoveAt(0);
                prethodnaStanjaAplikacije.Add(kljuc, staroStanje);
                stekStanja.GetUndo().Push(kljuc);
                // postavljamo flag na true, da bismo mogli da omogucimo klik na dugme za undo operaciju
                potvrdio = true;

                this.Close();
            }
            else if (dodavanjeSoftveraIzborStarogUnosa)
            {
                // ukoliko postoji softver (logicki neaktivan) sa istom oznakom
                // kao sto je uneta, ponovo aktiviramo taj softver (postaje logicki aktivan)
                tabelaSoftvera.Add(racunarskiCentar.Softveri[oznakaSoftver.Text.Trim()]);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierMainWindow.ShowSuccess("Uspešno ste aktivirali postojeći softver!");
                });

                // na undo stek treba da upisemo staro stanje aplikacije
                // generisemo neki novi kljuc pod kojim cemo cuvati prethodno stanje na steku
                string kljuc = Guid.NewGuid().ToString();
                // proveravamo da li vec ima 10 koraka za undo operaciju, ako ima, izbacujemo prvi koji je ubacen kako bismo 
                // i dalje imali 10 mogucih koraka, ali ukljucujuci i ovaj novi
                if (prethodnaStanjaAplikacije.Count >= 3)
                    prethodnaStanjaAplikacije.RemoveAt(0);
                prethodnaStanjaAplikacije.Add(kljuc, staroStanje);
                stekStanja.GetUndo().Push(kljuc);
                // postavljamo flag na true, da bismo mogli da omogucimo klik na dugme za undo operaciju
                potvrdio = true;

                this.Close();
            }
        }

        private bool validacijaNovogSoftvera()
        {
            if (!validacijaPodataka())
                return false;
            else if (racunarskiCentar.Softveri.ContainsKey(oznakaSoftver.Text.Trim()))
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
                    else
                    {
                        // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                        staroStanje = new StanjeAplikacije();
                        staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                        staroStanje.TipPodataka = "softver";
                        staroStanje.Kolicina = 1;
                        staroStanje.TipPromene = "brisanje";
                        staroStanje.Oznake.Add(softver.Oznaka);

                        // vracamo logicki obrisan softver da bude aktivan
                        softver.Obrisan = false;
                        dodavanjeSoftveraIzborStarogUnosa = true;
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        notifierError.ShowError("Softver sa unetom oznakom već postoji!");
                    });
                    vratiNaKorak1();
                    UpdateLayout();
                    oznakaSoftver.Focus();
                    return false;
                }
            }
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
                //provera praznih polja kako bi se uokvirili u crveno
                if (oznakaSoftver.Text.Trim() == "")
                    oznakaSoftver.BorderBrush = System.Windows.Media.Brushes.Red;
                if (nazivSoftver.Text.Trim() == "")
                    nazivSoftver.BorderBrush = System.Windows.Media.Brushes.Red;
                if (proizvodjacSoftver.Text.Trim() == "")
                    proizvodjacSoftver.BorderBrush = System.Windows.Media.Brushes.Red;
                if (sajtSoftver.Text.Trim() == "")
                    sajtSoftver.BorderBrush = System.Windows.Media.Brushes.Red;
                if (godinaSoftver.Text.Trim() == "")
                    godinaSoftver.BorderBrush = System.Windows.Media.Brushes.Red;
                if (cenaSoftver.Text.Trim() == "")
                    cenaSoftver.BorderBrush = System.Windows.Media.Brushes.Red;
                if (opisSoftver.Text.Trim() == "")
                    opisSoftver.BorderBrush = System.Windows.Media.Brushes.Red;


                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Niste popunili sva polja!");
                });
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
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Godina nije dobro unesena, unesite broj!");
                });
                godinaSoftver.Text = "";
                vratiNaKorak2();
                UpdateLayout();
                godinaSoftver.Focus();
                return false;
            }

            double cena;
            if (!double.TryParse(cenaSoftver.Text.Trim(), out cena))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Cena nije dobro unesena, unesite broj!");
                });
                cenaSoftver.Text = "";
                vratiNaKorak2();
                UpdateLayout();
                cenaSoftver.Focus();
                return false;
            }

            return true;
        }

        private bool proveraIzmeneOS(string oznaka)
        {
            Softver softver = racunarskiCentar.Softveri[oznaka];
            string zeljeniOS = "";
            if ((bool)WindowsOSSoftver.IsChecked)
                zeljeniOS = "Windows";
            else if ((bool)LinuxOSSoftver.IsChecked)
                zeljeniOS = "Linux";
            else
                zeljeniOS = "Windows i Linux";

            bool nadjenPredmetKomSmetaOS = false;
            bool nadjenaUcionicaKojojSmetaOS = false;

            foreach (Predmet p in racunarskiCentar.Predmeti.Values)
            {
                if (zeljeniOS == "Windows")
                {
                    if (p.OperativniSistem == "Linux" && p.Softveri.Contains(oznaka) && !p.Obrisan)
                    {
                        // nije dozvoljena promena OS softvera na windows, ako medju predmetima koji ga koriste
                        // postoji neki koji ima OS linux
                        nadjenPredmetKomSmetaOS = true;
                        break;
                    }
                }
                else if (zeljeniOS == "Linux")
                {
                    if (p.OperativniSistem == "Windows" && p.Softveri.Contains(oznaka) && !p.Obrisan)
                    {
                        // nije dozvoljena promena OS softvera na linux, ako medju predmetima koji ga koriste
                        // postoji neki koji ima OS windows
                        nadjenPredmetKomSmetaOS = true;
                        break;
                    }
                }
            }

            if (nadjenPredmetKomSmetaOS)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Ne možete promeniti operativni sistem softvera \njer se on koristi na predmetu gde nije podržan \nizabrani operativni sistem!");
                });
                return false;
            }
            else
            {
                // ukoliko ne postoji problem za izmenu OS softvera u pogledu sa poklapanjem OS na predmetima
                // koji koriste taj softver, proveravamo poklapanje izmedju softvera i ucionica koje ga koriste
                foreach (Ucionica u in racunarskiCentar.Ucionice.Values)
                {
                    if (zeljeniOS == "Windows")
                    {
                        if (u.OperativniSistem == "Linux" && u.InstaliraniSoftveri.Contains(oznaka) && !u.Obrisan)
                        {
                            // nije dozvoljena promena OS softvera na windows, ako medju ucionicama u kojima se koristi
                            // postoji neka koja ima OS linux
                            nadjenaUcionicaKojojSmetaOS = true;
                            break;
                        }
                    }
                    else if (zeljeniOS == "Linux")
                    {
                        if (u.OperativniSistem == "Windows" && u.InstaliraniSoftveri.Contains(oznaka) && !u.Obrisan)
                        {
                            // nije dozvoljena promena OS softvera na linux, ako medju ucionicama u kojima se koristi
                            // postoji neka koja ima OS windows
                            nadjenaUcionicaKojojSmetaOS = true;
                            break;
                        }
                    }
                }

                if (nadjenaUcionicaKojojSmetaOS)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        notifierError.ShowError("Ne možete promeniti operativni sistem softvera \njer se on koristi u učionici gde nije intaliran \nizabrani operativni sistem!");
                    });
                    return false;
                }
                // ako nema ni predmeta ni ucionice kojoj smeta promenja OS softvera, onda je data dozvola za izmenu
                return true;
            }
        }

        private void izmeniSoftver()
        {
            string novaOznaka = oznakaSoftver.Text.Trim();
            if (novaOznaka != oznakaSoftveraZaIzmenu && racunarskiCentar.Softveri.ContainsKey(novaOznaka))
            {
                notifierError.ShowError("Softver sa unetom oznakom već postoji u bazi!");
                oznakaSoftver.Focus();
                return;
            }

            if (validacijaPodataka() && proveraIzmeneOS(oznakaSoftveraZaIzmenu))
            {
                // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                staroStanje = new StanjeAplikacije();
                staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                staroStanje.TipPodataka = "softver";
                staroStanje.Kolicina = 1;
                staroStanje.TipPromene = "izmena";
                staroStanje.Oznake.Add(oznakaSoftveraZaIzmenu);

                Softver softverIzmena = racunarskiCentar.Softveri[oznakaSoftveraZaIzmenu];
                bool promenilaSeOznaka = false;
                bool promenioSeNaziv = false;
                bool promenioSeOpis = false;
                string staraOznaka = softverIzmena.Oznaka;

                if ((bool)WindowsOSSoftver.IsChecked)
                    softverIzmena.OperativniSistem = "Windows";
                else if ((bool)LinuxOSSoftver.IsChecked)
                    softverIzmena.OperativniSistem = "Linux";
                else
                    softverIzmena.OperativniSistem = "Windows i Linux";

                if (!softverIzmena.Oznaka.Equals(oznakaSoftver.Text.Trim()))
                    promenilaSeOznaka = true;
                softverIzmena.Oznaka = oznakaSoftver.Text.Trim();

                if (!softverIzmena.Naziv.Equals(nazivSoftver.Text.Trim()))
                    promenioSeNaziv = true;
                softverIzmena.Naziv = nazivSoftver.Text.Trim();

                if (!softverIzmena.Opis.Equals(opisSoftver.Text.Trim()))
                    promenioSeOpis = true;
                softverIzmena.Opis = opisSoftver.Text.Trim();

                softverIzmena.GodIzdavanja = int.Parse(godinaSoftver.Text.Trim());
                softverIzmena.Cena = double.Parse(cenaSoftver.Text.Trim());

                softverIzmena.Proizvodjac = proizvodjacSoftver.Text.Trim();
                softverIzmena.Sajt = sajtSoftver.Text.Trim();


                // azurira se oznaka u listi instaliranih softvera/neophodnih softvera u ucionici/predmetu
                // azurira se i ispis softvera za ucionicu/predmet
                StringBuilder sb = new StringBuilder();
                if (promenilaSeOznaka || promenioSeNaziv || promenioSeOpis)
                {
                    List<string> ucioniceZaIzmenu = new List<string>();
                    foreach (Ucionica u in racunarskiCentar.Ucionice.Values)
                    {
                        if (u.InstaliraniSoftveri.Contains(staraOznaka))
                        {
                            if (promenilaSeOznaka)
                            {
                                // ukoliko se promenila oznaka softvera, uklanjamo staru iz odgovarajcue liste u ucionici
                                // u kojoj je instaliran i pamtimo oznaku ucionice u koju treba da dodamo promenjenu oznaku softvera
                                u.InstaliraniSoftveri.Remove(staraOznaka);
                                ucioniceZaIzmenu.Add(u.Oznaka);
                            }
                        }
                    }
                    // idemo kroz sve ucionice u kojima treba azurirati stanje softvera i menjamo staru oznaku novom (izbacili smo
                    // staru i sad ubacujemo novu), azuriramo ispis
                    foreach (string oznaka in ucioniceZaIzmenu)
                    {
                        Ucionica u = racunarskiCentar.Ucionice[oznaka];
                        u.InstaliraniSoftveri.Add(softverIzmena.Oznaka);

                        foreach (string s in u.InstaliraniSoftveri)
                        {
                            if (s.Equals(softverIzmena.Oznaka))
                            {
                                if (u.InstaliraniSoftveri.IndexOf(s) != 0)
                                    sb.Append("\n");
                                sb.Append("Oznaka: " + softverIzmena.Oznaka);
                                sb.Append("\nNaziv: " + softverIzmena.Naziv);
                                sb.Append("\nOpis: " + softverIzmena.Opis + "\n");
                            }
                            else
                            {
                                Softver softver = racunarskiCentar.Softveri[s];
                                if (u.InstaliraniSoftveri.IndexOf(s) != 0)
                                    sb.Append("\n");
                                sb.Append("Oznaka: " + softver.Oznaka);
                                sb.Append("\nNaziv: " + softver.Naziv);
                                sb.Append("\nOpis: " + softver.Opis + "\n");
                            }
                        }
                        u.SoftveriLista = sb.ToString();
                        sb.Clear();
                    }


                    List<string> predmetiZaIzmenu = new List<string>();
                    foreach (Predmet p in racunarskiCentar.Predmeti.Values)
                    {
                        if (p.Softveri.Contains(staraOznaka))
                        {
                            if (promenilaSeOznaka)
                            {
                                // ukoliko se promenila oznaka softvera, uklanjamo staru iz odgovarajcue liste u predmetu
                                // koji koristi ovaj softver i pamtimo oznaku predmeta u koji treba da dodamo promenjenu oznaku predmeta
                                p.Softveri.Remove(staraOznaka);
                                predmetiZaIzmenu.Add(p.Oznaka);
                            }
                        }
                    }
                    // idemo kroz sve predmete u kojima treba azurirati stanje softvera i menjamo staru oznaku novom (izbacili smo
                    // staru i sad ubacujemo novu), azuriramo ispis
                    foreach (string oznaka in predmetiZaIzmenu)
                    {
                        Predmet p = racunarskiCentar.Predmeti[oznaka];
                        p.Softveri.Add(softverIzmena.Oznaka);

                        foreach (string s in p.Softveri)
                        {
                            if (s.Equals(softverIzmena.Oznaka))
                            {
                                if (p.Softveri.IndexOf(s) != 0)
                                    sb.Append("\n");
                                sb.Append("Oznaka: " + softverIzmena.Oznaka);
                                sb.Append("\nNaziv: " + softverIzmena.Naziv);
                                sb.Append("\nOpis: " + softverIzmena.Opis + "\n");
                            }
                            else
                            {
                                Softver softver = racunarskiCentar.Softveri[s];
                                if (p.Softveri.IndexOf(s) != 0)
                                    sb.Append("\n");
                                sb.Append("Oznaka: " + softver.Oznaka);
                                sb.Append("\nNaziv: " + softver.Naziv);
                                sb.Append("\nOpis: " + softver.Opis + "\n");
                            }
                        }
                        p.SoftveriLista = sb.ToString();
                        sb.Clear();
                    }
                }

                if (promenilaSeOznaka)
                {
                    racunarskiCentar.Softveri.Remove(staraOznaka);
                    racunarskiCentar.Softveri.Add(softverIzmena.Oznaka, softverIzmena);
                }

                tabelaSoftvera[indeks] = softverIzmena;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierMainWindow.ShowSuccess("Uspešno ste izmenili softver!");
                });

                // na undo stek treba da upisemo staro stanje aplikacije
                // generisemo neki novi kljuc pod kojim cemo cuvati prethodno stanje na steku
                string kljuc = Guid.NewGuid().ToString();
                // proveravamo da li vec ima 10 koraka za undo operaciju, ako ima, izbacujemo prvi koji je ubacen kako bismo 
                // i dalje imali 10 mogucih koraka, ali ukljucujuci i ovaj novi
                if (prethodnaStanjaAplikacije.Count >= 3)
                    prethodnaStanjaAplikacije.RemoveAt(0);
                prethodnaStanjaAplikacije.Add(kljuc, staroStanje);
                stekStanja.GetUndo().Push(kljuc);
                // postavljamo flag na true, da bismo mogli da omogucimo klik na dugme za undo operaciju
                potvrdio = true;

                this.Close();
            }
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
                HelpProvider.ShowHelp("dodavanjeSoftvera", this);
            }
        }

        private void prekiniTutorijal(object sender, EventArgs e)
        {
            PotvrdaOdustajanja potvrda = new PotvrdaOdustajanja();
            potvrda.ShowDialog();
            if (potvrda.daKlik)
                this.Close();
        }

        private void oznakaIzgubilaFocus(object sender, RoutedEventArgs e)
        {
            if (!(GreskaOznakaSoftvera.Text.Equals("Oznaka zauzeta!".Trim()) || oznakaSoftver.Text.Trim().Equals("")))
            {
                popupOznaka.IsOpen = false;
                pop = null;
            }
            resetujBojuOkvira(sender, e);
        }

        private void oznakaDobilaFokus(object sender, EventArgs e)
        {
            if (popupNaziv.IsOpen)
                nazivSoftver.Focus();
            else if (popupOS.IsOpen)
                stakRadioButton.Focus();
            else if (popupProizvodjac.IsOpen)
                proizvodjacSoftver.Focus();
            else
            {
                popupOznaka.IsOpen = true;
                pop = popupOznaka;
            }
            fokusiran = oznakaSoftver;
        }

        private void nazivIzgubioFokus(object sender, RoutedEventArgs e)
        {
            if (!(nazivSoftver.Text.Trim().Equals("")))
            {
                popupNaziv.IsOpen = false;
                pop = null;
            }
            resetujBojuOkvira(sender, e);
        }

        private void nazivDobioFokus(object sender, EventArgs e)
        {
            if (popupOznaka.IsOpen)
                oznakaSoftver.Focus();
            else if (popupProizvodjac.IsOpen)
                proizvodjacSoftver.Focus();
            else
            {
                popupNaziv.IsOpen = true;
                pop = popupNaziv;
                stakRadioButton.IsEnabled = true;
            }
            fokusiran = nazivSoftver;
        }

        private void nazivPromenioTekst(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (!(t.Text.Trim().Equals("")))
            {
                if (stakRadioButton.IsEnabled && proizvodjacSoftver.IsEnabled)
                {
                    nextButton.IsEnabled = true;
                    Korak2Softver.IsEnabled = true;
                }
            }
            else
            {
                Korak2Softver.IsEnabled = false;
                nextButton.IsEnabled = false;
            }
        }

        private void OSizgubioFokus(object sender, RoutedEventArgs e)
        {
            popupOS.IsOpen = false;
            pop = null;
        }

        private void OSdobioFockus(object sender, EventArgs e)
        {
            if (popupNaziv.IsOpen)
                nazivSoftver.Focus();
            else if (popupProizvodjac.IsOpen)
                proizvodjacSoftver.Focus();
            else if (popupOznaka.IsOpen)
                oznakaSoftver.Focus();
            else
            {
                if (popupOS.IsOpen == false)
                {
                    popupOS.IsOpen = true;
                    pop = popupOS;
                }
                proizvodjacSoftver.IsEnabled = true;
            }
            fokusiran = stakRadioButton;
        }

        private void proizvodjacIzgubioFokus(object sender, RoutedEventArgs e)
        {
            if (!(proizvodjacSoftver.Text.Trim().Equals("")))
            {
                popupProizvodjac.IsOpen = false;
                pop = null;
            }
            resetujBojuOkvira(sender, e);
        }

        private void proizvodjacDobioFokus(object sender, EventArgs e)
        {
            if (popupOznaka.IsOpen)
                oznakaSoftver.Focus();
            else if (popupNaziv.IsOpen)
                nazivSoftver.Focus();
            else
            {
                popupProizvodjac.IsOpen = true;
                pop = popupProizvodjac;
            }
            fokusiran = proizvodjacSoftver;
        }

        private void UnetProizvodjac(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (!(t.Text.Trim().Equals("")))
            {
                Korak2Softver.IsEnabled = true;
                nextButton.IsEnabled = true;
            }
            else
            {
                Korak2Softver.IsEnabled = false;
                nextButton.IsEnabled = false;
            }
        }

        private void nastaviIzgubioFokus(object sender, RoutedEventArgs e)
        {
            popupNastavi.IsOpen = false;
            pop = null;
        }

        private void nastavioDobioFokus(object sender, EventArgs e)
        {
            if (popupOznaka.IsOpen)
                oznakaSoftver.Focus();
            else if (popupNaziv.IsOpen)
                nazivSoftver.Focus();
            else if (popupProizvodjac.IsOpen)
                proizvodjacSoftver.Focus();
            else
            {
                popupNastavi.IsOpen = true;
                pop = popupNastavi;
            }
            fokusiran = nextButton;
        }


        private void otvorenDrugiTab(object sender, EventArgs e)
        {
            if (!(sajtSoftver.Text.Trim().Equals("")))
            {
                popupSajt.IsOpen = false;
                if (sajtSoftver.IsFocused)
                {
                    popupSajt.IsOpen = true;
                    pop = popupSajt;
                }
            }else
            {
                popupSajt.IsOpen = true;
                pop = popupSajt;
                sajtSoftver.Focus();
            }
        }

        private void sajtIzgubioFokus(object sender, RoutedEventArgs e)
        {
            if (!(sajtSoftver.Text.Trim().Equals("")))
            {
                popupSajt.IsOpen = false;
                pop = null;
            }
            resetujBojuOkvira(sender, e);
        }

        private void sajtDobioFokus(object sender, EventArgs e)
        {
            if (popupGodina.IsOpen)
                godinaSoftver.Focus();
            else if (popupCena.IsOpen)
                cenaSoftver.Focus();
            else if (popupOpis.IsOpen)
                opisSoftver.Focus();
            else
            {
                popupSajt.IsOpen = true;
                pop = popupSajt;
            }
            fokusiran = sajtSoftver;
        }

        private void godinaIzgubilaFokus(object sender, RoutedEventArgs e)
        {
            if (!(godinaSoftver.Text.Trim().Equals("") || !GreskaGodinaSoftver.Text.Equals("")))
            {
                popupGodina.IsOpen = false;
                pop = null;
            }
            resetujBojuOkvira(sender, e);
        }

        private void godinaDobilaFokus(object sender, EventArgs e)
        {
            if (popupSajt.IsOpen)
                sajtSoftver.Focus();
            else if (popupCena.IsOpen)
                cenaSoftver.Focus();
            else if (popupOpis.IsOpen)
                opisSoftver.Focus();
            else
            {
                popupGodina.IsOpen = true;
                pop = popupGodina;
                cenaSoftver.IsEnabled = true;
            }
            fokusiran = godinaSoftver;
        }

        private void cenaIzgubilaFokus(object sender, RoutedEventArgs e)
        {
            if (!(cenaSoftver.Text.Trim().Equals("") || !GreskaCenaSoftver.Text.Equals("")))
            {
                popupCena.IsOpen = false;
                pop = null;
            }
            resetujBojuOkvira(sender, e);
        }

        private void cenaDobilaFokus(object sender, EventArgs e)
        {
            if (popupSajt.IsOpen)
                sajtSoftver.Focus();
            else if (popupGodina.IsOpen)
                godinaSoftver.Focus();
            else if (popupOpis.IsOpen)
                opisSoftver.Focus();
            else
            {
                popupCena.IsOpen = true;
                pop = popupCena;
                opisSoftver.IsEnabled = true;
            }
            fokusiran = cenaSoftver;
        }

        private void opisIzgubioFokus(object sender, RoutedEventArgs e)
        {
            if (!opisSoftver.Text.Trim().Equals(""))
            {
                popupOpis.IsOpen = false;
                pop = null;
            }
            resetujBojuOkvira(sender, e);
        }

        private void opisDobioFokus(object sender, EventArgs e)
        {
            if (popupSajt.IsOpen)
                sajtSoftver.Focus();
            else if (popupGodina.IsOpen)
                godinaSoftver.Focus();
            else if (popupCena.IsOpen)
                cenaSoftver.Focus();
            else
            {
                popupOpis.IsOpen = true;
                pop = popupOpis;
            }
            fokusiran = opisSoftver;
        }

        private void UnetOpis(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (!(t.Text.Trim().Equals("")))
            {
                finishButton.IsEnabled = true;
            }
            else
            {
                finishButton.IsEnabled = false;
            }
        }

        private void zavrsiIzgubioFokus(object sender, RoutedEventArgs e)
        {
            popupZavrsi.IsOpen = false;
            pop = null;
        }

        private void zavrsiDobioFokus(object sender, EventArgs e)
        {
            if (popupSajt.IsOpen)
                sajtSoftver.Focus();
            else if (popupGodina.IsOpen)
                godinaSoftver.Focus();
            else if (popupCena.IsOpen)
                cenaSoftver.Focus();
            else if (popupOpis.IsOpen)
                opisSoftver.Focus();
            else
            {
                popupZavrsi.IsOpen = true;
                pop = popupZavrsi;
            }
            fokusiran = finishButton;
        }

        private void drugiTabPromenaTeksta(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (!(t.Text.Trim().Equals("")))
            {
                if(cenaSoftver.IsEnabled && godinaSoftver.IsEnabled && opisSoftver.IsEnabled)
                    finishButton.IsEnabled = true;
            }
            else
            {   
                finishButton.IsEnabled = false;
            }
        }
        //poziva se kada se prozor deaktivira
        private void skloniPopUp(object sender, EventArgs e)
        {
            if(pop != null)
                pop.IsOpen = false;
        }
        //poziva se kada se prozor aktivira
        private void prikaziPopUp(object sender, EventArgs e)
        {
            if (pop != null)
                pop.IsOpen = true;
            fokusiran.Focus();
            if(fokusiran.Name == "stakRadioButton")
                popupOS.IsOpen = true;
        }   

    }
}
