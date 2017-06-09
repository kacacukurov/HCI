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
    /// Interaction logic for DodavanjeUcionice.xaml
    /// </summary>
    public partial class DodavanjeUcionice : Window
    {
        private Ucionica novaUcionica;
        private RacunarskiCentar racunarskiCentar;
        private ObservableCollection<Ucionica> tabelaUcionica;
        private bool izmena;
        private bool unosPrviPut;
        private string oznakaUcioniceZaIzmenu;
        public int indeks;
        public bool inicijalizacija;
        private bool dodavanjeUcioniceIzborStarogUnosa;
        private Notifier notifierError;
        private Notifier notifierMainWindow;
        OrderedDictionary prethodnaStanjaAplikacije;
        StanjeAplikacije staroStanje;
        public bool potvrdio;
        private UndoRedoStack stekStanja;

        public DodavanjeUcionice(RacunarskiCentar racunarskiCentar, ObservableCollection<Ucionica> ucionice, bool izmena, string oznaka,
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
            this.inicijalizacija = false;
            InitializeComponent();
            this.inicijalizacija = true;
            this.dodavanjeUcioniceIzborStarogUnosa = false;
            novaUcionica = new Ucionica();
            this.racunarskiCentar = racunarskiCentar;
            this.izmena = izmena;
            this.unosPrviPut = true;
            this.oznakaUcioniceZaIzmenu = oznaka;
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
            tabelaUcionica = ucionice;
            if (!izmena)
                oznakaUcionica.Focus();
            BackStepMenuItem.IsEnabled = false;
        }

        private void prikazOdgovarajucihSoftvera(object sender, EventArgs e)
        {
            if (inicijalizacija)
            {
                if ((bool)LinuxOSUcionica.IsChecked)
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
                else if ((bool)WindowsOSUcionica.IsChecked)
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
                else if ((bool)WindowsAndLinuxOSUcionica.IsChecked)
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
            Korak2Ucionica.Focus();
        }

        public void backClick(object sender, RoutedEventArgs e)
        {
            BackStepMenuItem.IsEnabled = false;
            NextStepMenuItem.IsEnabled = true;
            Korak1Ucionica.Focus();
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

        private void UnetaOznakaUcionice(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (!izmena)
            {
                if (racunarskiCentar.Ucionice.ContainsKey(t.Text.Trim()) && !racunarskiCentar.Ucionice[t.Text.Trim()].Obrisan)
                    GreskaOznakaUcionice.Text = "Oznaka zauzeta!";
                else
                    GreskaOznakaUcionice.Text = "";
            }
            else if (!unosPrviPut && izmena)
            {
                if (racunarskiCentar.Ucionice.ContainsKey(t.Text.Trim()) && !t.Text.Trim().Equals(oznakaUcioniceZaIzmenu))
                    GreskaOznakaUcionice.Text = "Oznaka zauzeta!";
                else
                    GreskaOznakaUcionice.Text = "";
            }
            unosPrviPut = false;
        }

        private void finishClick(object sender, RoutedEventArgs e)
        {
            finishButton.Focus();
            if (izmena)
            {
                izmenaUcionice();
                return;
            }
            if (validacijaNoveUcionice() && !dodavanjeUcioniceIzborStarogUnosa)
            {
                // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                staroStanje = new StanjeAplikacije();
                staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                staroStanje.TipPodataka = "ucionica";
                staroStanje.Kolicina = 1;
                staroStanje.TipPromene = "brisanje";
                staroStanje.Oznake.Add(oznakaUcionica.Text.Trim());

                novaUcionica.Oznaka = oznakaUcionica.Text.Trim();
                novaUcionica.Opis = opisUcionica.Text.Trim();

                novaUcionica.PrisustvoPametneTable = prisustvoPametneTableUcionica.IsChecked;
                novaUcionica.PametnaTablaString = novaUcionica.PrisustvoPametneTable ? "prisutna" : "nije prisutna";
                novaUcionica.PrisustvoTable = prisustvoTableUcionica.IsChecked;
                novaUcionica.TablaString = novaUcionica.PrisustvoTable ? "prisutna" : "nije prisutna";
                novaUcionica.PrisustvoProjektora = prisustvoProjektoraUcionica.IsChecked;
                novaUcionica.ProjektorString = novaUcionica.PrisustvoProjektora ? "prisutan" : "nije prisutan";

                novaUcionica.BrojRadnihMesta = int.Parse(brojRadnihMestaUcionica.Text.Trim());
                if ((bool)LinuxOSUcionica.IsChecked)
                    novaUcionica.OperativniSistem = "Linux";
                else if ((bool)WindowsOSUcionica.IsChecked)
                    novaUcionica.OperativniSistem = "Windows";
                else
                    novaUcionica.OperativniSistem = "Windows i Linux";

                StringBuilder sb = new StringBuilder();
                int brojSoftvera = 0;
                for (int i = 0; i < softverTabela.Items.Count; i++)
                {
                    Softver softver = (Softver)softverTabela.Items[i];
                    if (softver.Instaliran)
                    {
                        brojSoftvera++;
                        novaUcionica.InstaliraniSoftveri.Add(softver.Oznaka);

                        if (brojSoftvera > 1)
                            sb.Append("\n");
                        sb.Append("Oznaka: " + softver.Oznaka);
                        sb.Append("\nNaziv: " + softver.Naziv);
                        sb.Append("\nOpis: " + softver.Opis + "\n");
                        softver.Instaliran = false;
                    }
                }
                novaUcionica.SoftveriLista = sb.ToString();

                tabelaUcionica.Add(novaUcionica);
                racunarskiCentar.DodajUcionicu(novaUcionica);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierMainWindow.ShowSuccess("Uspešno ste dodali novu učionicu!");
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
            else if (dodavanjeUcioniceIzborStarogUnosa)
            {
                // ukoliko postoji predmet (logicki neaktivan) sa istom oznakom
                // kao sto je uneta, ponovo aktiviramo taj predmet (postaje logicki aktivan)
                tabelaUcionica.Add(racunarskiCentar.Ucionice[oznakaUcionica.Text.Trim()]);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierMainWindow.ShowSuccess("Uspešno ste aktivirali postojeću učionicu!");
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

        private bool validacijaNoveUcionice()
        {
            if (!validacijaPodataka())
                return false;
            else if (racunarskiCentar.Ucionice.ContainsKey(oznakaUcionica.Text.Trim()))
            {
                if (racunarskiCentar.Ucionice[oznakaUcionica.Text.Trim()].Obrisan)
                {
                    dodavanjeUcioniceIzborStarogUnosa = false;
                    Ucionica ucionica = racunarskiCentar.Ucionice[oznakaUcionica.Text.Trim()];

                    // vec postoji ucionica sa tom oznakom, ali je logicki obrisana
                    OdlukaDodavanjaUcionica odluka = new OdlukaDodavanjaUcionica();
                    odluka.Oznaka.Text = "Oznaka: " + ucionica.Oznaka;
                    odluka.BrojRadnihMesta.Text = "Broj radnih mesta: " + ucionica.BrojRadnihMesta;
                    odluka.Projektor.Text = "Projektor: " + ucionica.ProjektorString;
                    odluka.Tabla.Text = "Tabla: " + ucionica.TablaString;
                    odluka.PametnaTabla.Text = "Pametna tabla: " + ucionica.PametnaTablaString;
                    odluka.OperativniSistem.Text = "Operativni sistem: " + ucionica.OperativniSistem;
                    odluka.ShowDialog();

                    if (odluka.potvrdaNovogUnosa)
                        // ukoliko je korisnik potvrdio da zeli da unese nove podatke, gazimo postojecu neaktivnu ucionicu
                        racunarskiCentar.Ucionice.Remove(oznakaUcionica.Text.Trim());
                    else {
                        // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                        staroStanje = new StanjeAplikacije();
                        staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                        staroStanje.TipPodataka = "ucionica";
                        staroStanje.Kolicina = 1;
                        staroStanje.TipPromene = "brisanje";
                        staroStanje.Oznake.Add(ucionica.Oznaka);

                        // vracamo logicki obrisanu ucionicu da bude aktivna
                        ucionica.Obrisan = false;
                        dodavanjeUcioniceIzborStarogUnosa = true;
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        notifierError.ShowError("Učionica sa unetom oznakom već postoji!");
                    });
                    vratiNaKorak1();
                    UpdateLayout();
                    oznakaUcionica.Focus();
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
            Korak1Ucionica.Focus();
        }

        private void vratiNaKorak2()
        {
            Keyboard.ClearFocus();
            BackStepMenuItem.IsEnabled = true;
            NextStepMenuItem.IsEnabled = false;
            Korak2Ucionica.Focus();
        }

        private bool validacijaPodataka()
        {
            if (oznakaUcionica.Text.Trim() == "" || opisUcionica.Text.Trim() == "" || brojRadnihMestaUcionica.Text.Trim() == "")
            {
                // provera praznih polja da bismo ih uokvirili u crveno
                if (oznakaUcionica.Text.Trim() == "")
                    oznakaUcionica.BorderBrush = System.Windows.Media.Brushes.Red;
                if (opisUcionica.Text.Trim() == "")
                    opisUcionica.BorderBrush = System.Windows.Media.Brushes.Red;


                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Niste popunili sva polja!");
                });
                if (oznakaUcionica.Text.Trim() == "")
                {
                    vratiNaKorak1();
                    UpdateLayout();
                    oznakaUcionica.Focus();
                }
                else if (opisUcionica.Text.Trim() == "")
                {
                    vratiNaKorak1();
                    UpdateLayout();
                    opisUcionica.Focus();
                }
                else if (brojRadnihMestaUcionica.Text.Trim() == "")
                {
                    vratiNaKorak2();
                    UpdateLayout();
                    brojRadnihMestaUcionica.Focus();
                }
                return false;
            }

            int brMesta;
            if (!int.TryParse(brojRadnihMestaUcionica.Text.Trim(), out brMesta))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Broj radnih mesta nije dobro unesen, unesite broj!");
                });
                brojRadnihMestaUcionica.Text = "";
                brojRadnihMestaUcionica.Focus();
                return false;
            }

            bool postojiSoftver = false;
            if (softverTabela.Items.Count > 0)
            {
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
                    if (tabControlUcionica.SelectedIndex != 1)
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
            }
            else {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Morate prvo uneti softver da biste mogli da unesete učionicu!");
                });
                return false;
            }
            return true;
        }

        private void izmenaUcionice()
        {
            string novaOznaka = oznakaUcionica.Text.Trim();
            if (novaOznaka != oznakaUcioniceZaIzmenu && racunarskiCentar.Ucionice.ContainsKey(novaOznaka))
            {
                notifierError.ShowError("Učionica sa unetom oznakom već postoji u bazi!");
                oznakaUcionica.Focus();
                return;
            }

            if (validacijaPodataka() && validacijaIzmeneSoftvera() && validacijaIzmeneTable() && validacijaIzmenePametneTable() &&
                validacijaIzmeneProjektora() && validacijaBrojaRadnihMesta())
            {
                // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                staroStanje = new StanjeAplikacije();
                staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                staroStanje.TipPodataka = "ucionica";
                staroStanje.Kolicina = 1;
                staroStanje.TipPromene = "izmena";
                staroStanje.Oznake.Add(oznakaUcioniceZaIzmenu);

                Ucionica ucionicaIzmena = racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu];
                string staraOznaka = ucionicaIzmena.Oznaka;
                bool oznakaIzmenjena = false;

                if (!staraOznaka.Equals(oznakaUcionica.Text.Trim()))
                    oznakaIzmenjena = true;

                ucionicaIzmena.Oznaka = oznakaUcionica.Text.Trim();
                ucionicaIzmena.Opis = opisUcionica.Text.Trim();

                if (oznakaIzmenjena)
                {
                    racunarskiCentar.Ucionice.Remove(staraOznaka);
                    racunarskiCentar.Ucionice.Add(ucionicaIzmena.Oznaka, ucionicaIzmena);
                }

                tabelaUcionica[indeks] = ucionicaIzmena;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierMainWindow.ShowSuccess("Uspešno ste izmenili učionicu!");
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

        private bool validacijaIzmeneSoftvera()
        {
            Ucionica staraUcionica = racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu];
            List<string> sviPredmetiUcionice = new List<string>();
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)  //trazimo sve predmete koji se odrzavaju u datoj ucionici
            {
                if (polje.Ucionica.Trim().Equals(staraUcionica.Oznaka.Trim()))
                    sviPredmetiUcionice.Add(polje.NazivPolja.Split('-')[0].Trim());
            }

            List<string> predmetiUcionice = sviPredmetiUcionice.Distinct().ToList(); //izbacimo duplikate
            List<string> predmetiBezSoftvera = new List<string>();
            
            foreach(string poz in sviPredmetiUcionice)     // prolazim kroz sve predmete unutar ucionice
            {
                Predmet predmet = racunarskiCentar.Predmeti[poz];
                foreach (string soft in predmet.Softveri)       //prolazim kroz sve softvere jednog predmeta
                {
                    bool postoji = false;
                    for (int i = 0; i < softverTabela.Items.Count; i++) //iteriram kroz svaki oznaceni softver
                    {
                        Softver softver = (Softver)softverTabela.Items[i];
                        if (softver.Instaliran)
                        {
                            if (soft.Trim().Equals(softver.Oznaka.Trim()))  //trazim taj softver u ucionici
                                postoji = true;
                        }
                    }
                    if (!postoji)
                    {
                        predmetiBezSoftvera.Add(poz);
                    }
                }
            }
            
            List<string> predmetiBezSoftBezDupl = predmetiBezSoftvera.Distinct().ToList();
            if (predmetiBezSoftBezDupl.Count > 0)
            {
                PotvrdaIzmene potvrda = new PotvrdaIzmene();
                potvrda.Title = "Nedostatak softvera";
                potvrda.PorukaBrisanja.Text = "Da li ste sigurni?\n\nUkoliko potvrdite izmenu, sledeci predmeti ce se ukloniti iz rasporeda"
                                               + " u ucionici zbog nedostatka softvera:\n";
                List<string> kljuceviPolja = new List<string>();
                for (int i = 0; i < predmetiBezSoftBezDupl.Count; i++)
                {
                    potvrda.PorukaBrisanja.Text += "\n" + (i+1) + ". " + predmetiBezSoftBezDupl[i];
                    foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
                    {
                        if (polje.NazivPolja.Split('-')[0].Trim().Equals(predmetiBezSoftBezDupl[i]) && polje.Ucionica.Equals(staraUcionica.Oznaka))
                            kljuceviPolja.Add(polje.Id);
                    }
                }
                potvrda.ShowDialog();
                if (potvrda.daKlik)
                {
                    foreach (string id in kljuceviPolja)
                        racunarskiCentar.PoljaKalendara.Remove(id);
                    foreach (string poz in predmetiBezSoftvera)
                        racunarskiCentar.Predmeti[poz].PreostaliTermini++;
                }
                else
                    return false;
            }
            if ((bool)LinuxOSUcionica.IsChecked)
                staraUcionica.OperativniSistem = "Linux";
            else if ((bool)WindowsOSUcionica.IsChecked)
                staraUcionica.OperativniSistem = "Windows";
            else
                staraUcionica.OperativniSistem = "Windows i Linux";

            staraUcionica.InstaliraniSoftveri.Clear();
            StringBuilder sb = new StringBuilder();
            int brojSoftvera = 0;
            for (int i = 0; i < softverTabela.Items.Count; i++)
            {
                Softver softver = (Softver)softverTabela.Items[i];
                if (softver.Instaliran)
                {
                    brojSoftvera++;
                    staraUcionica.InstaliraniSoftveri.Add(softver.Oznaka);

                    if (brojSoftvera > 1)
                        sb.Append("\n");
                    sb.Append("Oznaka: " + softver.Oznaka);
                    sb.Append("\nNaziv: " + softver.Naziv);
                    sb.Append("\nOpis: " + softver.Opis + "\n");
                    softver.Instaliran = false;
                }
            }
            staraUcionica.SoftveriLista = sb.ToString();
            return true;
        }

        private bool validacijaIzmeneTable()
        {
            Ucionica staraUcionica = racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu];
            if (prisustvoTableUcionica.IsChecked)
            {
                staraUcionica.PrisustvoTable = prisustvoTableUcionica.IsChecked;
                staraUcionica.TablaString = staraUcionica.PrisustvoTable ? "prisutna" : "nije prisutna";
                return true;
            }
                
            List<string> sviPredmetiUcionice = new List<string>();
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)  //trazimo sve predmete koji se odrzavaju u datoj ucionici
            {
                if (polje.Ucionica.Trim().Equals(staraUcionica.Oznaka.Trim()))
                    sviPredmetiUcionice.Add(polje.NazivPolja.Split('-')[0].Trim());
            }
            List<string> predmetiKojiZahtevajuTablu = new List<string>();
            foreach(string poz in sviPredmetiUcionice)
            {
                if (racunarskiCentar.Predmeti[poz].NeophodnaTabla)
                {
                    predmetiKojiZahtevajuTablu.Add(poz);
                }
            }
            List<string> predmetiKojiZahtevajuTabluBezDupl = predmetiKojiZahtevajuTablu.Distinct().ToList();
            if (predmetiKojiZahtevajuTabluBezDupl.Count > 0)
            {
                PotvrdaIzmene potvrda = new PotvrdaIzmene();
                potvrda.Title = "Nedostatak table";
                potvrda.PorukaBrisanja.Text = "Da li ste sigurni?\n\nUkoliko potvrdite izmenu, sledeci predmeti ce se ukloniti iz rasporeda"
                                               + " u ucionici zbog nedostatka table:\n";
                List<string> kljuceviPolja = new List<string>();
                for (int i = 0; i < predmetiKojiZahtevajuTabluBezDupl.Count; i++)
                {
                    potvrda.PorukaBrisanja.Text += "\n" + (i + 1) + ". " + predmetiKojiZahtevajuTabluBezDupl[i];
                    foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
                    {
                        if (polje.NazivPolja.Split('-')[0].Trim().Equals(predmetiKojiZahtevajuTabluBezDupl[i]) && polje.Ucionica.Equals(staraUcionica.Oznaka))
                            kljuceviPolja.Add(polje.Id);
                    }
                }
                potvrda.ShowDialog();
                if (potvrda.daKlik)
                {
                    foreach (string id in kljuceviPolja)
                        racunarskiCentar.PoljaKalendara.Remove(id);
                    foreach (string poz in predmetiKojiZahtevajuTablu)
                        racunarskiCentar.Predmeti[poz].PreostaliTermini++;
               }
                else
                {
                    return false;
                }
            }
            staraUcionica.PrisustvoTable = prisustvoTableUcionica.IsChecked;
            staraUcionica.TablaString = staraUcionica.PrisustvoTable ? "prisutna" : "nije prisutna";
            return true;
        }

        private bool validacijaIzmenePametneTable()
        {
            Ucionica staraUcionica = racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu];
            if (prisustvoPametneTableUcionica.IsChecked)
            {
                racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu].PrisustvoPametneTable = prisustvoPametneTableUcionica.IsChecked;
                racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu].PametnaTablaString = racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu].PrisustvoPametneTable ? "prisutna" : "nije prisutna";
                return true;
            }
            List<string> sviPredmetiUcionice = new List<string>();
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)  //trazimo sve predmete koji se odrzavaju u datoj ucionici
            {
                if (polje.Ucionica.Trim().Equals(staraUcionica.Oznaka.Trim()))
                    sviPredmetiUcionice.Add(polje.NazivPolja.Split('-')[0].Trim());
            }
            List<string> predmetiKojiZahtevajuPametnuTablu = new List<string>();
            foreach (string poz in sviPredmetiUcionice)
            {
                if (racunarskiCentar.Predmeti[poz].NeophodnaPametnaTabla)
                {
                    predmetiKojiZahtevajuPametnuTablu.Add(poz);
                }
            }

            List<string> predmetiKojiZahtevajuPametnuTabluBezDupl = predmetiKojiZahtevajuPametnuTablu.Distinct().ToList();
            if (predmetiKojiZahtevajuPametnuTabluBezDupl.Count > 0)
            {
                PotvrdaIzmene potvrda = new PotvrdaIzmene();
                potvrda.Title = "Nedostatak pametne table";
                potvrda.PorukaBrisanja.Text = "Da li ste sigurni?\n\nUkoliko potvrdite izmenu, sledeci predmeti ce se ukloniti iz rasporeda"
                                               + " u ucionici zbog nedostatka pametne table:\n";
                List<string> kljuceviPolja = new List<string>();
                for (int i = 0; i < predmetiKojiZahtevajuPametnuTabluBezDupl.Count; i++)
                {
                    potvrda.PorukaBrisanja.Text += "\n" + (i + 1) + ". " + predmetiKojiZahtevajuPametnuTabluBezDupl[i];
                    foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
                    {
                        if (polje.NazivPolja.Split('-')[0].Trim().Equals(predmetiKojiZahtevajuPametnuTabluBezDupl[i]) && polje.Ucionica.Equals(staraUcionica.Oznaka))
                            kljuceviPolja.Add(polje.Id);
                    }
                }
                potvrda.ShowDialog();
                if (potvrda.daKlik)
                {
                    foreach (string id in kljuceviPolja)
                        racunarskiCentar.PoljaKalendara.Remove(id);
                    foreach (string poz in predmetiKojiZahtevajuPametnuTablu)
                        racunarskiCentar.Predmeti[poz].PreostaliTermini++;
                }
                else
                {
                    return false;
                }
            }
            racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu].PrisustvoPametneTable = prisustvoPametneTableUcionica.IsChecked;
            racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu].PametnaTablaString = racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu].PrisustvoPametneTable ? "prisutna" : "nije prisutna";
            return true;
        }

        private bool validacijaIzmeneProjektora()
        {
            Ucionica staraUcionica = racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu];
            if (prisustvoProjektoraUcionica.IsChecked)
            {
                staraUcionica.PrisustvoProjektora = prisustvoProjektoraUcionica.IsChecked;
                staraUcionica.ProjektorString = staraUcionica.PrisustvoProjektora ? "prisutan" : "nije prisutan";
                return true;
            }
            List<string> sviPredmetiUcionice = new List<string>();
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)  //trazimo sve predmete koji se odrzavaju u datoj ucionici
            {
                if (polje.Ucionica.Trim().Equals(staraUcionica.Oznaka.Trim()))
                    sviPredmetiUcionice.Add(polje.NazivPolja.Split('-')[0].Trim());
            }

            List<string> predmetiKojiZahtevajuProjektor = new List<string>();
            foreach (string poz in sviPredmetiUcionice)
            {
                if (racunarskiCentar.Predmeti[poz].NeophodanProjektor)
                {
                    predmetiKojiZahtevajuProjektor.Add(poz);
                }
            }

            List<string> predmetiKojiZahtevajuProjektorBezDupl = predmetiKojiZahtevajuProjektor.Distinct().ToList();
            if (predmetiKojiZahtevajuProjektorBezDupl.Count > 0)
            {
                PotvrdaIzmene potvrda = new PotvrdaIzmene();
                potvrda.Title = "Nedostatak projektora";
                potvrda.PorukaBrisanja.Text = "Da li ste sigurni?\n\nUkoliko potvrdite izmenu, sledeci predmeti ce se ukloniti iz rasporeda"
                                               + " u ucionici zbog nedostatka projektora:\n";
                List<string> kljuceviPolja = new List<string>();
                for (int i = 0; i < predmetiKojiZahtevajuProjektorBezDupl.Count; i++)
                {
                    potvrda.PorukaBrisanja.Text += "\n" + (i + 1) + ". " + predmetiKojiZahtevajuProjektorBezDupl[i];
                    foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
                    {
                        if (polje.NazivPolja.Split('-')[0].Trim().Equals(predmetiKojiZahtevajuProjektorBezDupl[i]) && polje.Ucionica.Equals(staraUcionica.Oznaka))
                            kljuceviPolja.Add(polje.Id);
                    }
                }
                potvrda.ShowDialog();
                if (potvrda.daKlik)
                {
                    foreach (string id in kljuceviPolja)
                        racunarskiCentar.PoljaKalendara.Remove(id);
                    foreach (string poz in predmetiKojiZahtevajuProjektor)
                        racunarskiCentar.Predmeti[poz].PreostaliTermini++;
                  }
                else
                {
                    return false;
                }
            }
            staraUcionica.PrisustvoProjektora = prisustvoProjektoraUcionica.IsChecked;
            staraUcionica.ProjektorString = staraUcionica.PrisustvoProjektora ? "prisutan" : "nije prisutan";
            return true;
        }

        private bool validacijaBrojaRadnihMesta()
        {
            Ucionica staraUcionica = racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu];
            if (staraUcionica.BrojRadnihMesta == int.Parse(brojRadnihMestaUcionica.Text.Trim()))
                return true;
            int noviBrMesta = int.Parse(brojRadnihMestaUcionica.Text.Trim());
            List<string> sviPredmetiUcionice = new List<string>();
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)  //trazimo sve predmete koji se odrzavaju u datoj ucionici
            {
                if (polje.Ucionica.Trim().Equals(staraUcionica.Oznaka.Trim()))
                    sviPredmetiUcionice.Add(polje.NazivPolja.Split('-')[0].Trim());
            }

            List<string> predmetiKojiZahtevajuBrojMesta = new List<string>();
            foreach (string poz in sviPredmetiUcionice)
            {
                if (racunarskiCentar.Predmeti[poz].VelicinaGrupe > noviBrMesta)
                {
                    predmetiKojiZahtevajuBrojMesta.Add(poz);
                }
            }

            List<string> predmetiKojiZahtevajuBrojMestaBezDupl = predmetiKojiZahtevajuBrojMesta.Distinct().ToList();
            if (predmetiKojiZahtevajuBrojMestaBezDupl.Count > 0)
            {
                PotvrdaIzmene potvrda = new PotvrdaIzmene();
                potvrda.Title = "Nedovoljno mesta";
                potvrda.PorukaBrisanja.Text = "Da li ste sigurni?\n\nUkoliko potvrdite izmenu, sledeci predmeti ce se ukloniti iz rasporeda"
                                               + " u ucionici zbog nedostatka mesta u ucionici:\n";
                List<string> kljuceviPolja = new List<string>();
                for (int i = 0; i < predmetiKojiZahtevajuBrojMestaBezDupl.Count; i++)
                {
                    potvrda.PorukaBrisanja.Text += "\n" + (i + 1) + ". " + predmetiKojiZahtevajuBrojMestaBezDupl[i];
                    foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
                    {
                        if (polje.NazivPolja.Split('-')[0].Trim().Equals(predmetiKojiZahtevajuBrojMestaBezDupl[i]) && polje.Ucionica.Equals(staraUcionica.Oznaka))
                            kljuceviPolja.Add(polje.Id);
                    }
                }
                potvrda.ShowDialog();
                if (potvrda.daKlik)
                {
                    foreach (string id in kljuceviPolja)
                        racunarskiCentar.PoljaKalendara.Remove(id);
                    foreach (string poz in predmetiKojiZahtevajuBrojMesta)
                        racunarskiCentar.Predmeti[poz].PreostaliTermini++;
                }
                else
                    return false;
            }
            staraUcionica.BrojRadnihMesta = int.Parse(brojRadnihMestaUcionica.Text.Trim());
            return true;
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
                HelpProvider.ShowHelp("dodavanjeUcionice", this);
            }
        }
    }
}
