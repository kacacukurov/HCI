using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
    /// Interaction logic for DodavanjePredmeta.xaml
    /// </summary>
    public partial class DodavanjePredmeta : Window
    {
        private Predmet predmet;
        private RacunarskiCentar racunarskiCentar;
        private ObservableCollection<Predmet> tabelaPredmeta;
        private bool izmena;
        private bool unosPrviPut;
        public int indeks;
        public bool inicijalizacija;
        private bool dodavanjePredmetaIzborStarogUnosa;
        private string oznakaPredmetaZaIzmenu;
        private Notifier notifierError;
        private Notifier notifierMainWindow;
        OrderedDictionary prethodnaStanjaAplikacije;
        StanjeAplikacije staroStanje;
        public bool potvrdio;
        private UndoRedoStack stekStanja;

        public DodavanjePredmeta(RacunarskiCentar racunarskiCentar, ObservableCollection<Predmet> predmeti, bool izmena, string oznaka,
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
            this.notifierMainWindow = notifierMainWindow;

            this.prethodnaStanjaAplikacije = prethodnaStanja;
            this.staroStanje = null;
            this.potvrdio = false;
            this.stekStanja = stack;
            predmet = new Predmet();
            this.racunarskiCentar = racunarskiCentar;
            this.izmena = izmena;
            this.unosPrviPut = true;
            this.oznakaPredmetaZaIzmenu = oznaka;
            tabelaPredmeta = predmeti;
            this.inicijalizacija = false;
            this.dodavanjePredmetaIzborStarogUnosa = false;

            InitializeComponent();
            this.inicijalizacija = true;

            List<Smer> smerovi = new List<Smer>();
            foreach (Smer s in racunarskiCentar.Smerovi.Values)
            {
                if (!s.Obrisan)
                {
                    s.UPredmetu = false;
                    smerovi.Add(s);
                }
            }
            smeroviTabela.ItemsSource = smerovi;
            smeroviTabela.IsSynchronizedWithCurrentItem = true;

            List<Softver> softveri = new List<Softver>();
            foreach (Softver s in racunarskiCentar.Softveri.Values)
            {
                if (!s.Obrisan)
                {
                    s.Instaliran = false;
                    softveri.Add(s);
                }
            }
            softverTabela.ItemsSource = softveri;
            softverTabela.IsSynchronizedWithCurrentItem = true;

            if (!izmena)
                OznakaPredmeta.Focus();
            BackStepMenuItem.IsEnabled = false;
        }

        private void izabranSmerPripadnostiPredmeta(object sender, EventArgs e)
        {
            for (int i = 0; i < smeroviTabela.Items.Count; i++)
            {
                if (smeroviTabela.SelectedIndex != i)
                {
                    DataGridRow selektovaniRed = (DataGridRow)smeroviTabela.ItemContainerGenerator.ContainerFromIndex(i);
                    CheckBox content = smeroviTabela.Columns[2].GetCellContent(selektovaniRed) as CheckBox;
                    if ((bool)content.IsChecked)
                        content.IsChecked = false;
                }
            }
        }

        private void prikazOdgovarajucihSoftvera(object sender, EventArgs e)
        {
            if (inicijalizacija)
            {
                if ((bool)Linux.IsChecked)
                {
                    // ukoliko postoje vec prethodno izabrani softveri, proverava se da li medju njima ima neki
                    // kom je OS Windows --> ukoliko ima, izbacuje se

                    // samo odcekiramo softvere koji imaju OS Windows iz tabele softvera u prozoru za dodavanje

                    for (int i = 0; i < softverTabela.Items.Count; i++)
                    {
                        Softver softver = (Softver)softverTabela.Items[i];
                        if (softver.Instaliran && softver.OperativniSistem.Equals("Windows"))
                        {
                            softver.Instaliran = false;
                        }
                    }
                    softverTabela.Items.Refresh();

                    // filtriranje i prikazivanje softvera za linux i cross platform
                    ICollectionView cv = CollectionViewSource.GetDefaultView(softverTabela.ItemsSource);

                    cv.Filter = o =>
                    {
                        Softver s = o as Softver;
                        return (s.OperativniSistem.ToUpper().Equals("LINUX") || s.OperativniSistem.ToUpper().Contains("LINUX"));
                    };

                }
                else if ((bool)Windows.IsChecked)
                {
                    // ukoliko postoje vec prethodno izabrani softveri, proverava se da li medju njima ima neki
                    // kom je OS Linux --> ukoliko ima, izbacuje se

                    // samo odcekiramo softvere koji imaju OS Linux iz tabele softvera u prozoru za dodavanje

                    for (int i = 0; i < softverTabela.Items.Count; i++)
                    {
                        Softver softver = (Softver)softverTabela.Items[i];
                        if (softver.Instaliran && softver.OperativniSistem.Equals("Linux"))
                        {
                            softver.Instaliran = false;
                        }
                    }
                    softverTabela.Items.Refresh();

                    // filtriranje i prikazivanje softvera za windows i cross platform
                    ICollectionView cv = CollectionViewSource.GetDefaultView(softverTabela.ItemsSource);

                    cv.Filter = o =>
                    {
                        Softver s = o as Softver;
                        return (s.OperativniSistem.ToUpper().Equals("WINDOWS") || s.OperativniSistem.ToUpper().Contains("WINDOWS"));
                    };
                }
                else if ((bool)Svejedno.IsChecked)
                {
                    // prikaz svih softvera koji postoje (linux, windows, cross platform)
                    ICollectionView cv = CollectionViewSource.GetDefaultView(softverTabela.ItemsSource);

                    cv.Filter = o =>
                    {
                        Softver s = o as Softver;
                        return (s.OperativniSistem.ToUpper().Contains("LINUX") || s.OperativniSistem.ToUpper().Contains("WINDOWS"));
                    };
                }
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
            Korak2Predmet.Focus();
        }

        public void backClick(object sender, RoutedEventArgs e)
        {
            BackStepMenuItem.IsEnabled = false;
            NextStepMenuItem.IsEnabled = true;
            Korak1Predmet.Focus();
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            PotvrdaOdustajanja potvrda = new PotvrdaOdustajanja();
            potvrda.ShowDialog();
            if(potvrda.daKlik)
                this.Close();
        }

        private void resetujBojuOkvira(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.ClearValue(Border.BorderBrushProperty);
        }

        private void softverUnosProvera(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            string vrednost = t.Text.Trim();

            ICollectionView cv = CollectionViewSource.GetDefaultView(softverTabela.ItemsSource);
            if (vrednost == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    Softver s = o as Softver;
                    return (s.Naziv.ToUpper().Contains(vrednost.ToUpper()) || s.Oznaka.ToUpper().Contains(vrednost.ToUpper())
                    || s.OperativniSistem.ToUpper().Contains(vrednost.ToUpper()));
                };
            }
        }

        private void UnetaOznakaPredmeta(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (!izmena)
            {
                if (racunarskiCentar.Predmeti.ContainsKey(t.Text.Trim()) && !racunarskiCentar.Predmeti[t.Text.Trim()].Obrisan)
                    GreskaOznakaPredmeta.Text = "Oznaka zauzeta!";
                else
                    GreskaOznakaPredmeta.Text = "";
            }
            else if (!unosPrviPut && izmena)
            {
                if (racunarskiCentar.Predmeti.ContainsKey(t.Text.Trim()) && !t.Text.Trim().Equals(oznakaPredmetaZaIzmenu))
                    GreskaOznakaPredmeta.Text = "Oznaka zauzeta!";
                else
                    GreskaOznakaPredmeta.Text = "";
            }
            unosPrviPut = false;
        }

        private void finishClick(object sender, RoutedEventArgs e)
        {
            finishButton.Focus();
            if (izmena)
            {
                izmenaPredmeta();
                return;
            }
            if (validacijaNovogPredmeta() && !dodavanjePredmetaIzborStarogUnosa)
            {
                // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                staroStanje = new StanjeAplikacije();
                staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                staroStanje.TipPodataka = "predmet";
                staroStanje.Kolicina = 1;
                staroStanje.TipPromene = "brisanje";
                staroStanje.Oznake.Add(OznakaPredmeta.Text.Trim());

                // ako su uneti podaci ispravni
                // u slucaju da postoji predmet (logicki obrisan) sa istom sifrom kao sto je uneta
                // i pritom je odlucio da zeli ipak da gazi stari unos
                predmet.Oznaka = OznakaPredmeta.Text.Trim();
                predmet.Naziv = NazivPredmeta.Text.Trim();
                predmet.Opis = OpisPredmeta.Text.Trim();
                predmet.VelicinaGrupe = int.Parse(VelicinaGrupePredmet.Text.Trim());
                predmet.MinDuzinaTermina = int.Parse(DuzinaTerminaPredmet.Text.Trim());
                predmet.BrTermina = int.Parse(BrojTerminaPredmet.Text.Trim());
                predmet.PreostaliTermini = predmet.BrTermina;

                predmet.NeophodanProjektor = PrisustvoProjektoraPredmet.IsChecked;
                predmet.ProjektorString = predmet.NeophodanProjektor ? "neophodan" : "nije neophodan";
                predmet.NeophodnaTabla = PrisustvoTablePredmet.IsChecked;
                predmet.TablaString = predmet.NeophodnaTabla ? "neophodna" : "nije neophodna";
                predmet.NeophodnaPametnaTabla = PrisustvoPametneTable.IsChecked;
                predmet.PametnaTablaString = predmet.NeophodnaPametnaTabla ? "neophodna" : "nije neophodna";

                if ((bool)Windows.IsChecked)
                    predmet.OperativniSistem = Windows.Content.ToString();
                if ((bool)Linux.IsChecked)
                    predmet.OperativniSistem = Linux.Content.ToString();
                if ((bool)Svejedno.IsChecked)
                    predmet.OperativniSistem = Svejedno.Content.ToString();

                StringBuilder sb = new StringBuilder();
                int brojSoftvera = 0;
                for (int i = 0; i < softverTabela.Items.Count; i++)
                {
                    Softver softver = (Softver)softverTabela.Items[i];
                    if (softver.Instaliran)
                    {
                        brojSoftvera++;
                        predmet.Softveri.Add(softver.Oznaka);

                        if (brojSoftvera > 1)
                            sb.Append("\n");
                        sb.Append("Oznaka: " + softver.Oznaka);
                        sb.Append("\nNaziv: " + softver.Naziv);
                        sb.Append("\nOpis: " + softver.Opis + "\n");
                        softver.Instaliran = false;
                    }
                }
                predmet.SoftveriLista = sb.ToString();

                for (int i = 0; i < smeroviTabela.Items.Count; i++)
                {
                    Smer smer = (Smer)smeroviTabela.Items[i];
                    if (smer.UPredmetu)
                    {
                        predmet.Smer = smer.Oznaka;
                        smer.Predmeti.Add(predmet.Oznaka);

                        predmet.SmerDetalji = "Oznaka: " + smer.Oznaka + "\nNaziv: " + smer.Naziv;
                        smer.UPredmetu = false;
                        break;
                    }
                }

                tabelaPredmeta.Add(predmet);
                racunarskiCentar.DodajPredmet(predmet);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierMainWindow.ShowSuccess("Uspešno ste dodali novi predmet!");
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
            else if (dodavanjePredmetaIzborStarogUnosa)
            {
                // ukoliko postoji predmet (logicki neaktivan) sa istom oznakom
                // kao sto je uneta, ponovo aktiviramo taj predmet (postaje logicki aktivan)
                tabelaPredmeta.Add(racunarskiCentar.Predmeti[OznakaPredmeta.Text.Trim()]);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierMainWindow.ShowSuccess("Uspešno ste aktivirali predmet!");
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

        private bool validacijaNovogPredmeta()
        {
            if (!validacijaPodataka())
                return false;
            else if (racunarskiCentar.Predmeti.ContainsKey(OznakaPredmeta.Text.Trim()))
            {
                if (racunarskiCentar.Predmeti[OznakaPredmeta.Text.Trim()].Obrisan)
                {
                    dodavanjePredmetaIzborStarogUnosa = false;
                    Predmet predmet = racunarskiCentar.Predmeti[OznakaPredmeta.Text.Trim()];

                    // vec postoji predmet sa tom oznakom, ali je logicki obrisan
                    OdlukaDodavanjePredmet odluka = new OdlukaDodavanjePredmet();
                    odluka.Oznaka.Text = "Oznaka: " + predmet.Oznaka;
                    odluka.Naziv.Text = "Naziv: " + predmet.Naziv;
                    odluka.Smer.Text = "Smer (oznaka): " + predmet.Smer;
                    odluka.VelicinaGrupe.Text = "Veličina grupe: " + predmet.VelicinaGrupe.ToString();
                    odluka.MinDuzinaTermina.Text = "Minimalna dužina termina: " + predmet.MinDuzinaTermina.ToString();
                    odluka.BrojTermina.Text = "Broj termina: " + predmet.BrTermina.ToString();
                    odluka.Projektor.Text = "Projektor: " + predmet.ProjektorString;
                    odluka.Tabla.Text = "Tabla: " + predmet.TablaString;
                    odluka.PametnaTabla.Text = "Pametna tabla: " + predmet.PametnaTablaString;
                    odluka.OperativniSistem.Text = "Operativni sistem: " + predmet.OperativniSistem;
                    odluka.ShowDialog();

                    if (odluka.potvrdaNovogUnosa)
                        // ukoliko je korisnik potvrdio da zeli da unese nove podatke, gazimo postojeci neaktivan predmet
                        racunarskiCentar.Predmeti.Remove(OznakaPredmeta.Text.Trim());
                    else {
                        // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                        staroStanje = new StanjeAplikacije();
                        staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                        staroStanje.TipPodataka = "predmet";
                        staroStanje.Kolicina = 1;
                        staroStanje.TipPromene = "brisanje";
                        staroStanje.Oznake.Add(predmet.Oznaka);

                        // vracamo logicki obrisan predmet da bude aktivan
                        predmet.Obrisan = false;
                        dodavanjePredmetaIzborStarogUnosa = true;
                        // u smeru kom pripada ovaj predmet belezimo promenu
                        racunarskiCentar.Smerovi[predmet.Smer].Predmeti.Add(predmet.Oznaka);
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        notifierError.ShowError("Predmet sa unetom oznakom već postoji!");
                    });
                    vratiNaKorak1();
                    UpdateLayout();
                    OznakaPredmeta.Focus();
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
            Korak1Predmet.Focus();
        }

        private void vratiNaKorak2()
        {
            Keyboard.ClearFocus();
            BackStepMenuItem.IsEnabled = true;
            NextStepMenuItem.IsEnabled = false;
            Korak2Predmet.Focus();
        }

        private bool validacijaPodataka()
        {
            if (OznakaPredmeta.Text.Trim() == "" || NazivPredmeta.Text.Trim() == "" || OpisPredmeta.Text.Trim() == "")
            {
                //podesavanje crvenog okvira za polja koja nisu popunjena
                if (OznakaPredmeta.Text.Trim() == "")
                    OznakaPredmeta.BorderBrush = System.Windows.Media.Brushes.Red;
                if (NazivPredmeta.Text.Trim() == "")
                    NazivPredmeta.BorderBrush = System.Windows.Media.Brushes.Red;
                if (OpisPredmeta.Text.Trim() == "")
                    OpisPredmeta.BorderBrush = System.Windows.Media.Brushes.Red;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Niste popunili sva polja!");
                });
                if (OznakaPredmeta.Text.Trim() == "")
                {
                    vratiNaKorak1();
                    UpdateLayout();
                    OznakaPredmeta.Focus();
                }
                else if (NazivPredmeta.Text.Trim() == "")
                {
                    vratiNaKorak1();
                    UpdateLayout();
                    NazivPredmeta.Focus();
                }
                else if (OpisPredmeta.Text.Trim() == "")
                {
                    vratiNaKorak1();
                    UpdateLayout();
                    OpisPredmeta.Focus();
                }

                return false;
            }

            bool postojiSmer = false;
            for (int i = 0; i < smeroviTabela.Items.Count; i++)
            {
                Smer smer = (Smer)smeroviTabela.Items[i];
                if (smer.UPredmetu)
                    postojiSmer = true;
            }
            if (!postojiSmer)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Niste označili smer na kom se održava predmet!");
                });
                if (tabControlPredmet.SelectedIndex != 0)
                {
                    vratiNaKorak1();
                    UpdateLayout();
                }
                smeroviTabela.Focus();
                DataGridCellInfo firstRowCell = new DataGridCellInfo(smeroviTabela.Items[0], smeroviTabela.Columns[2]);
                smeroviTabela.CurrentCell = firstRowCell;
                smeroviTabela.ScrollIntoView(smeroviTabela.Items[0]);
                smeroviTabela.BeginEdit();
                return false;
            }

            if (BrojTerminaPredmet.Text.Trim() == "" || VelicinaGrupePredmet.Text.Trim() == "" || DuzinaTerminaPredmet.Text.Trim() == "")
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Niste popunili sva polja!");
                });
                if (VelicinaGrupePredmet.Text.Trim() == "")
                {
                    vratiNaKorak1();
                    UpdateLayout();
                    VelicinaGrupePredmet.Focus();
                }
                else if (DuzinaTerminaPredmet.Text.Trim() == "")
                {
                    vratiNaKorak1();
                    UpdateLayout();
                    DuzinaTerminaPredmet.Focus();
                }
                else if (BrojTerminaPredmet.Text.Trim() == "")
                {
                    vratiNaKorak1();
                    UpdateLayout();
                    BrojTerminaPredmet.Focus();
                }

                return false;
            }

            bool postojiSoftver = false;
            for (int i = 0; i < softverTabela.Items.Count; i++)
            {
                Softver softver = (Softver)softverTabela.Items[i];
                if (softver.Instaliran)
                    postojiSoftver = true;
            }
            if (!postojiSoftver)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Niste označili potreban softver/softvere!");
                });
                if (tabControlPredmet.SelectedIndex != 1)
                {
                    vratiNaKorak2();
                    UpdateLayout();
                }
                softverTabela.Focus();
                DataGridCellInfo firstRowCell = new DataGridCellInfo(softverTabela.Items[0], softverTabela.Columns[3]);
                softverTabela.CurrentCell = firstRowCell;
                softverTabela.ScrollIntoView(softverTabela.Items[0]);
                softverTabela.BeginEdit();
                return false;
            }

            return true;
        }

        private void izmenaPredmeta()
        {
            string novaOznaka = OznakaPredmeta.Text.Trim();
            if (novaOznaka != oznakaPredmetaZaIzmenu && racunarskiCentar.Predmeti.ContainsKey(novaOznaka))
            {
                notifierError.ShowError("Predmet sa unetom oznakom već postoji u bazi!");
                OznakaPredmeta.Focus();
                return;
            }

            if (validacijaPodataka() && validacijeIzmeneBrojaTermina() && validacijaIzmeneDuzineTermina() && validacijaIzmeneSoftvera()
                && validacijaIzmeneTable() && validacijaIzmenePametneTable() && validacijaIzmeneProjektora() && validacijaIzmeneVelicineGrupe())
            {
                // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                staroStanje = new StanjeAplikacije();
                staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                staroStanje.TipPodataka = "predmet";
                staroStanje.Kolicina = 1;
                staroStanje.TipPromene = "izmena";
                staroStanje.Oznake.Add(oznakaPredmetaZaIzmenu);

                Predmet predmetIzmena = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu];
                string staraOznaka = predmetIzmena.Oznaka;
                bool oznakaPromenjena = false;

                if (!staraOznaka.Equals(OznakaPredmeta.Text.Trim()))
                    oznakaPromenjena = true;

                predmetIzmena.Oznaka = OznakaPredmeta.Text.Trim();
                predmetIzmena.Naziv = NazivPredmeta.Text.Trim();
                predmetIzmena.Opis = OpisPredmeta.Text.Trim();
                predmetIzmena.MinDuzinaTermina = int.Parse(DuzinaTerminaPredmet.Text.Trim());

                bool stariSmerPronadjen = false;
                bool noviPredmetPostavljen = false;
                string oznakaStarogSmera = "";
                string oznakaNovogSmera = "";
                for (int i = 0; i < smeroviTabela.Items.Count; i++)
                {
                    Smer smer = (Smer)smeroviTabela.Items[i];
                    //iz smera za koji je bio vezan predmet uklanjamo vezu
                    if (smer.Predmeti.Contains(staraOznaka))
                    {
                        smer.Predmeti.Remove(staraOznaka);
                        oznakaStarogSmera = smer.Oznaka;
                        stariSmerPronadjen = true;
                    }
                    if (smer.UPredmetu)
                    {
                        //u listu predmeta novoizabranog smera dodajemo i ovaj smer
                        if (!smer.Predmeti.Contains(predmetIzmena.Oznaka))
                            smer.Predmeti.Add(predmetIzmena.Oznaka);
                        predmetIzmena.Smer = smer.Oznaka;
                        oznakaNovogSmera = smer.Oznaka;
                        predmetIzmena.SmerDetalji = "Oznaka: " + smer.Oznaka + "\nNaziv: " + smer.Naziv;
                        smer.UPredmetu = false;
                        noviPredmetPostavljen = true;
                    }

                    if (stariSmerPronadjen && noviPredmetPostavljen)
                        break;
                }
                promeniSmerPredmeta(oznakaStarogSmera, oznakaNovogSmera);

                if (oznakaPromenjena)
                {
                    racunarskiCentar.Predmeti.Remove(staraOznaka);
                    racunarskiCentar.Predmeti.Add(predmetIzmena.Oznaka, predmetIzmena);
                }

                tabelaPredmeta[indeks] = predmetIzmena;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierMainWindow.ShowSuccess("Uspešno ste izmenili predmet!");
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

        private bool validacijeIzmeneBrojaTermina()
        {
            Predmet stariPredmet = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu];
            int stariBrojTermina = stariPredmet.BrTermina;
            int preostaliTermini = stariPredmet.PreostaliTermini;
            int noviBrojTermina = int.Parse(BrojTerminaPredmet.Text.Trim());
            if (noviBrojTermina > stariBrojTermina)
            {
                stariPredmet.PreostaliTermini += noviBrojTermina - stariBrojTermina;
                stariPredmet.BrTermina = noviBrojTermina;
                return true;
            }
               
            int razlika = stariBrojTermina - noviBrojTermina;
            if (razlika > stariPredmet.PreostaliTermini)
            {
                ObservableCollection<PoljaKalendaraBrisanje> poljaKalendara = new ObservableCollection<PoljaKalendaraBrisanje>(); //nadjem sva polja kalendara da bih mogla ponuditi korisniku da obrise neka
                foreach(KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
                {
                    if (polje.NazivPolja.Split('-')[0].Trim().Equals(oznakaPredmetaZaIzmenu))
                    {
                        poljaKalendara.Add(new PoljaKalendaraBrisanje(polje.Id, polje.Ucionica, polje.Dan, 
                                            polje.Pocetak.Split(' ')[1], polje.Kraj.Split(' ')[1], false));
                    }
                }
                int visak = razlika - stariPredmet.PreostaliTermini;
                int stariBrojPreostalih = stariPredmet.PreostaliTermini;
                BrisanjePoljaKalendara prozorBrisanje = new BrisanjePoljaKalendara();
                prozorBrisanje.porukaBrisanje.Text += "\nPotrebno stavki za brisanje: " + visak;
                prozorBrisanje.PredmetUkalendaru = poljaKalendara;
                prozorBrisanje.poljaKalendara.ItemsSource = poljaKalendara;
                prozorBrisanje.ShowDialog();
                if (prozorBrisanje.daKlik)
                {
                    int brojObrisanih = 0;
                    for (int i = 0; i < prozorBrisanje.poljaKalendara.Items.Count; i++)
                    {
                        PoljaKalendaraBrisanje zaBrisanje = (PoljaKalendaraBrisanje)prozorBrisanje.poljaKalendara.Items[i];
                        if (zaBrisanje.Obrisan)
                            brojObrisanih++;
                    }
                    if(brojObrisanih < visak)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            notifierError.ShowError("Niste odabrali dovoljno stavki za brisanje!");
                        });
                        return false;
                    }
                    else
                    {
                        stariPredmet.PreostaliTermini = 0;
                        for (int i = 0; i < prozorBrisanje.poljaKalendara.Items.Count; i++)
                        {
                            PoljaKalendaraBrisanje zaBrisanje = (PoljaKalendaraBrisanje)prozorBrisanje.poljaKalendara.Items[i];
                            if (zaBrisanje.Obrisan)
                                racunarskiCentar.PoljaKalendara.Remove(zaBrisanje.Oznaka);
                        }
                    }
                    if (brojObrisanih > visak)
                        stariPredmet.PreostaliTermini = brojObrisanih - visak;
                    stariPredmet.BrTermina -= noviBrojTermina;
                    return true;
                }
                else
                    return false;
            }
            else
                racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu].PreostaliTermini -= razlika;

            stariPredmet.BrTermina = noviBrojTermina;
            return true;
        }

        private bool validacijaIzmeneDuzineTermina()
        {
            int staraDuzinaTermina = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu].MinDuzinaTermina;
            int novaDuzina = int.Parse(DuzinaTerminaPredmet.Text.Trim());
            if (staraDuzinaTermina != novaDuzina)
            {
                if (racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu].PreostaliTermini !=
                    racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu].BrTermina)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        notifierError.ShowError("Ne možete promeniti dužinu trajanja jednog termina, jer je predmet već raspoređen u kalendaru!");
                    });
                    return false;
                }
            }
            return true;
        }

        private bool validacijaIzmeneSoftvera()
        {
            Predmet predmetStari = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu];
            List<string> sveUcionicePredmeta = new List<string>(); //tu se nalaze sve ucionice u kojima se odrzava predmet koji se menja
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
            {
                if (polje.NazivPolja.Split('-')[0].Trim() == predmetStari.Oznaka)    //idem kroz sva polja i trazim ucionice
                    sveUcionicePredmeta.Add(polje.Ucionica);
            }

            List<string> ucionicePredmetaBezDupl = sveUcionicePredmeta.Distinct().ToList(); //izbacimo duplikate

            int brojPotrebnihSoftvera = 0;
            int brojNadjenihSoftvera = 0;
            for (int i = 0; i < softverTabela.Items.Count; i++) //trazim koliko je softvera oznaceno
            {
                Softver softver = (Softver)softverTabela.Items[i];
                if (softver.Instaliran)
                    brojPotrebnihSoftvera++;
            }
            List<string> ucioniceBezSoftvera = new List<string>();

            foreach(string ucionica in ucionicePredmetaBezDupl)
            {
            brojNadjenihSoftvera = 0;
                foreach(string softver in racunarskiCentar.Ucionice[ucionica].InstaliraniSoftveri)  // trazim svaki softver ucionice u predmetu
                {
                    for (int i = 0; i < softverTabela.Items.Count; i++)
                    {
                        Softver soft = (Softver)softverTabela.Items[i];
                        if (soft.Instaliran && soft.Oznaka == softver)
                        brojNadjenihSoftvera++;
                    }
                }
            if (brojNadjenihSoftvera != brojPotrebnihSoftvera)
                ucioniceBezSoftvera.Add(ucionica);
            }

            if(ucioniceBezSoftvera.Count > 0)
            {
                PotvrdaIzmene potvrda = new PotvrdaIzmene();
                potvrda.Title = "Nedostatak softvera";
                potvrda.PorukaBrisanja.Text = "Da li ste sigurni?\n\nUkoliko potvrdite izmenu, predmet ce se ukloniti iz sledecih"
                                               + " ucionica zbog nedostatka softvera:\n";
                List<string> kljuceviPolja = new List<string>();
                for (int i = 0; i < ucioniceBezSoftvera.Count; i++)
                {
                    potvrda.PorukaBrisanja.Text += "\n" + (i + 1) + ". " + ucioniceBezSoftvera[i];
                    foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
                    {
                        if (polje.NazivPolja.Split('-')[0].Trim().Equals(predmetStari.Oznaka) && polje.Ucionica.Equals(ucioniceBezSoftvera[i]))
                            kljuceviPolja.Add(polje.Id);
                    }
                }
                potvrda.ShowDialog();
                if (potvrda.daKlik)
                {
                    foreach (string id in kljuceviPolja)
                    {
                        racunarskiCentar.PoljaKalendara.Remove(id);
                        predmetStari.PreostaliTermini++;
                    }
                }
                else
                    return false;
            }
            if ((bool)Windows.IsChecked)
                predmetStari.OperativniSistem = Windows.Content.ToString();
            else if ((bool)Linux.IsChecked)
                predmetStari.OperativniSistem = Linux.Content.ToString();
            else if ((bool)Svejedno.IsChecked)
                predmetStari.OperativniSistem = Svejedno.Content.ToString();

            StringBuilder sb = new StringBuilder();
            int brojSoftvera = 0;
            predmetStari.Softveri.Clear();
            for (int i = 0; i < softverTabela.Items.Count; i++)
            {
                Softver softver = (Softver)softverTabela.Items[i];
                if (softver.Instaliran)
                {
                    brojSoftvera++;
                    predmetStari.Softveri.Add(softver.Oznaka);

                    if (brojSoftvera > 1)
                        sb.Append("\n");
                    sb.Append("Oznaka: " + softver.Oznaka);
                    sb.Append("\nNaziv: " + softver.Naziv);
                    sb.Append("\nOpis: " + softver.Opis + "\n");
                    softver.Instaliran = false;
                }
            }
            predmetStari.SoftveriLista = sb.ToString();
            return true;
        }

        private bool validacijaIzmeneTable()
        {
            Predmet predmetStari = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu];
            if (!PrisustvoTablePredmet.IsChecked)
            {
                predmetStari.NeophodnaTabla = PrisustvoTablePredmet.IsChecked;
                predmetStari.TablaString = predmetStari.NeophodnaTabla ? "neophodna" : "nije neophodna";
                return true;
            }
            PotvrdaIzmene potvrda = new PotvrdaIzmene();
            potvrda.Title = "Nedostatak table";
            potvrda.PorukaBrisanja.Text = "Da li ste sigurni?\n\nUkoliko potvrdite izmenu, predmet ce se ukloniti iz sledecih ucionica"
                                           + " zbog nedostatka table:\n";

            List<string> sveUcionicePredmeta = new List<string>(); //tu se nalaze sve ucionice u kojima se odrzava predmet koji se menja
            List<string> kljuceviPolja = new List<string>();
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
            {
                if (polje.NazivPolja.Split('-')[0].Trim() == predmetStari.Oznaka)    //idem kroz sva polja i trazim ucionice predmeta
                    sveUcionicePredmeta.Add(polje.Ucionica);
            }
            
            List<string> ucionicePredmeta = sveUcionicePredmeta.Distinct().ToList(); //izbacimo duplikate
            int brojac = 1;
            for(int i = 0; i < ucionicePredmeta.Count; i++)
            {
                foreach(KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
                {
                    if((polje.Ucionica == ucionicePredmeta[i]) && (!racunarskiCentar.Ucionice[ucionicePredmeta[i]].PrisustvoTable)
                        && polje.NazivPolja.Split('-')[0].Trim() == predmetStari.Oznaka)
                    {
                        if (!potvrda.PorukaBrisanja.Text.Contains(ucionicePredmeta[i]))
                        {
                            potvrda.PorukaBrisanja.Text += "\n" + brojac + ". " + ucionicePredmeta[i];
                            brojac++;
                        }
                        kljuceviPolja.Add(polje.Id);
                    }
                }
            }
            kljuceviPolja = kljuceviPolja.Distinct().ToList();
            if (kljuceviPolja.Count > 0)
            {
                potvrda.ShowDialog();
                if (potvrda.daKlik)
                {
                    foreach (string id in kljuceviPolja)
                    {
                        racunarskiCentar.PoljaKalendara.Remove(id);
                        predmetStari.PreostaliTermini++;
                    }                        
                }
                else
                {
                    return false;
                }
            }
            predmetStari.NeophodnaTabla = PrisustvoTablePredmet.IsChecked;
            predmetStari.TablaString = predmetStari.NeophodnaTabla ? "neophodna" : "nije neophodna";
            return true;
        }

        private bool validacijaIzmenePametneTable()
        {
            Predmet predmetStari = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu];
            if (!PrisustvoPametneTable.IsChecked)
            {
                predmetStari.NeophodnaPametnaTabla = PrisustvoPametneTable.IsChecked;
                predmetStari.PametnaTablaString = predmetStari.NeophodnaPametnaTabla ? "neophodna" : "nije neophodna";
                return true;
            }
            PotvrdaIzmene potvrda = new PotvrdaIzmene();
            potvrda.Title = "Nedostatak pametne table";
            potvrda.PorukaBrisanja.Text = "Da li ste sigurni?\n\nUkoliko potvrdite izmenu, predmet ce se ukloniti iz sledecih ucionica"
                                           + " zbog nedostatka pametne table:\n";
            
            List<string> kljuceviPolja = new List<string>();

            List<string> sveUcionicePredmeta = new List<string>(); //tu se nalaze sve ucionice u kojima se odrzava predmet koji se menja
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
            {
                if (polje.NazivPolja.Split('-')[0].Trim() == predmetStari.Oznaka)    //idem kroz sva polja i trazim ucionice
                    sveUcionicePredmeta.Add(polje.Ucionica);
            }
            List<string> ucionicePredmeta = sveUcionicePredmeta.Distinct().ToList(); //izbacimo duplikate
            int brojac = 1;
            for (int i = 0; i < ucionicePredmeta.Count; i++)
            {
                foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
                {
                    if ((polje.Ucionica == ucionicePredmeta[i]) && (!racunarskiCentar.Ucionice[ucionicePredmeta[i]].PrisustvoPametneTable)
                        && polje.NazivPolja.Split('-')[0].Trim() == predmetStari.Oznaka)
                    {
                        if (!potvrda.PorukaBrisanja.Text.Contains(ucionicePredmeta[i]))
                        {
                            potvrda.PorukaBrisanja.Text += "\n" + brojac + ". " + ucionicePredmeta[i];
                            brojac++;
                        }
                        kljuceviPolja.Add(polje.Id);
                    }
                }
            }
            kljuceviPolja = kljuceviPolja.Distinct().ToList();
            if (kljuceviPolja.Count > 0)
            {
                potvrda.ShowDialog();
                if (potvrda.daKlik)
                {
                    foreach (string id in kljuceviPolja)
                    {
                        racunarskiCentar.PoljaKalendara.Remove(id);
                        predmetStari.PreostaliTermini++;
                    }
                }
                else
                    return false;
            }
            predmetStari.NeophodnaPametnaTabla = PrisustvoPametneTable.IsChecked;
            predmetStari.PametnaTablaString = predmetStari.NeophodnaPametnaTabla ? "neophodna" : "nije neophodna";
            return true;
        }

        private bool validacijaIzmeneProjektora()
        {
            Predmet predmetStari = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu];
            if (!PrisustvoProjektoraPredmet.IsChecked)
            {
                predmetStari.NeophodanProjektor = PrisustvoProjektoraPredmet.IsChecked;
                predmetStari.ProjektorString = predmetStari.NeophodanProjektor ? "neophodan" : "nije neophodan";
                return true;
            }
            PotvrdaIzmene potvrda = new PotvrdaIzmene();
            potvrda.Title = "Nedostatak pametne table";
            potvrda.PorukaBrisanja.Text = "Da li ste sigurni?\n\nUkoliko potvrdite izmenu, predmet ce se ukloniti iz sledecih ucionica"
                                           + " zbog nedostatka pametne table:\n";

            List<string> kljuceviPolja = new List<string>();

            List<string> sveUcionicePredmeta = new List<string>(); //tu se nalaze sve ucionice u kojima se odrzava predmet koji se menja
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
            {
                if (polje.NazivPolja.Split('-')[0].Trim() == predmetStari.Oznaka)    //idem kroz sva polja i trazim ucionice
                    sveUcionicePredmeta.Add(polje.Ucionica);
            }

            List<string> ucionicePredmeta = sveUcionicePredmeta.Distinct().ToList(); //izbacimo duplikate
            int brojac = 1;
            for (int i = 0; i < ucionicePredmeta.Count; i++)
            {
                foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
                {
                    if ((polje.Ucionica == ucionicePredmeta[i]) && (!racunarskiCentar.Ucionice[ucionicePredmeta[i]].PrisustvoProjektora)
                        && polje.NazivPolja.Split('-')[0].Trim() == predmetStari.Oznaka)
                    {
                        if (!potvrda.PorukaBrisanja.Text.Contains(ucionicePredmeta[i]))
                        {
                            potvrda.PorukaBrisanja.Text += "\n" + brojac + ". " + ucionicePredmeta[i];
                            brojac++;
                        }
                        kljuceviPolja.Add(polje.Id);
                    }
                }
            }
            kljuceviPolja = kljuceviPolja.Distinct().ToList();
            if (kljuceviPolja.Count > 0)
            {
                potvrda.ShowDialog();
                if (potvrda.daKlik)
                {
                    foreach (string id in kljuceviPolja)
                    {
                        racunarskiCentar.PoljaKalendara.Remove(id);
                        predmetStari.PreostaliTermini++;
                    }
                }
                else
                {
                    return false;
                }
            }
            predmetStari.NeophodanProjektor = PrisustvoProjektoraPredmet.IsChecked;
            predmetStari.ProjektorString = predmetStari.NeophodanProjektor ? "neophodan" : "nije neophodan";
            return true;
        }

        private bool validacijaIzmeneVelicineGrupe()
        {
            Predmet predmetStari = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu];
            if(predmetStari.VelicinaGrupe == int.Parse(VelicinaGrupePredmet.Text.Trim()))
                return true;

            PotvrdaIzmene potvrda = new PotvrdaIzmene();
            potvrda.Title = "Preveliki broj ljudi";
            potvrda.PorukaBrisanja.Text = "Da li ste sigurni?\n\nUkoliko potvrdite izmenu, predmet ce se ukloniti iz sledecih ucionica"
                                           + " zbog prevelike grupe:\n";

            List<string> kljuceviPolja = new List<string>();

            List<string> sveUcionicePredmeta = new List<string>(); //tu se nalaze sve ucionice u kojima se odrzava predmet koji se menja
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
            {
                if (polje.NazivPolja.Split('-')[0].Trim() == predmetStari.Oznaka)    //idem kroz sva polja i trazim ucionice
                    sveUcionicePredmeta.Add(polje.Ucionica);
            }

            List<string> ucionicePredmeta = sveUcionicePredmeta.Distinct().ToList(); //izbacimo duplikate
            int brojac = 1;
            for (int i = 0; i < ucionicePredmeta.Count; i++)
            {
                foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
                {
                    if ((polje.Ucionica == ucionicePredmeta[i]) && (racunarskiCentar.Ucionice[ucionicePredmeta[i]].BrojRadnihMesta <
                        int.Parse(VelicinaGrupePredmet.Text.Trim())) && polje.NazivPolja.Split('-')[0].Trim() == predmetStari.Oznaka)
                    {
                        if (!potvrda.PorukaBrisanja.Text.Contains(ucionicePredmeta[i]))
                        {
                            potvrda.PorukaBrisanja.Text += "\n" + brojac + ". " + ucionicePredmeta[i];
                            brojac++;
                        }
                        kljuceviPolja.Add(polje.Id);
                    }
                }
            }
            kljuceviPolja = kljuceviPolja.Distinct().ToList();
            if (kljuceviPolja.Count > 0)
            {
                potvrda.ShowDialog();
                if (potvrda.daKlik)
                {
                    foreach (string id in kljuceviPolja)
                    {
                        racunarskiCentar.PoljaKalendara.Remove(id);
                        predmetStari.PreostaliTermini++;
                    }
                }
                else
                {
                    return false;
                }
            }
            predmetStari.VelicinaGrupe = int.Parse(VelicinaGrupePredmet.Text.Trim());

            return true;
        }

        private void promeniSmerPredmeta(string oznakaStarogSmera, string oznakaNovogSmera)
        {
            foreach(KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
            {
                if((polje.NazivPolja.Split('-')[0] == oznakaPredmetaZaIzmenu) && (polje.NazivPolja.Split('-')[1].Equals(oznakaStarogSmera.Trim())))
                {
                    polje.NazivPolja = polje.NazivPolja.Split('-')[0] + '-' + oznakaNovogSmera;
                }
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
                HelpProvider.ShowHelp("dodavanjePredmeta", this);
            }
        }
    }
}
