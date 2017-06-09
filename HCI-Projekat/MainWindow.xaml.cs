using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.Serialization;
using System.Xml;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CefSharp;
using CefSharp.Wpf;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Specialized;
using Microsoft.Win32;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UndoRedoStack stekStanja;
        OrderedDictionary prethodnaStanjaAplikacije;
        OrderedDictionary sledecaStanjaAplikacije;
        private RacunarskiCentar racunarskiCentar;
        private ObservableCollection<Predmet> predmetiKolekcija;
        ObservableCollection<Softver> softveriKolekcija;
        ObservableCollection<Smer> smeroviKolekcija;
        ObservableCollection<Ucionica> ucioniceKolekcija;
        private static string imeFajla = "racunarskiCentar.xml";
        public ChromiumWebBrowser chromeBrowser;
        CefCustomObject cef;
        private int brojAktivnihSmerova;
        private int brojAktivnihSoftvera;
        private Notifier notifierSucces;
        private Notifier notifierError;
        private Notifier notifierUndoRedo;
        private DispatcherTimer timer;

        public MainWindow()
        {
            notifierError = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.TopRight,
                    offsetX: 30,
                    offsetY: 30);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: System.TimeSpan.FromSeconds(7),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });

            notifierSucces = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.TopRight,
                    offsetX: 30,
                    offsetY: 30);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: System.TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });

            notifierUndoRedo = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: this,
                    corner: Corner.BottomRight,
                    offsetX: 20,
                    offsetY: 50);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: System.TimeSpan.FromSeconds(5),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(1));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });

            // stek za undo redo mehanizam za kolekciju stanja aplikacije
            stekStanja = new UndoRedoStack();
            InitializeComponent();
            Loaded += OnLoaded;
            KalendarTab.Focus();

            racunarskiCentar = new RacunarskiCentar();
            DeserijalizacijaPodataka();
            inicijalizujPodatke();
            InitializeChromium();
            cef = new CefCustomObject(chromeBrowser, this, racunarskiCentar, notifierError, stekStanja, prethodnaStanjaAplikacije);
            chromeBrowser.RegisterJsObject("cefCustomObject", cef);
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.timer = new DispatcherTimer(new TimeSpan(0, 0, 6), DispatcherPriority.Normal, OnTimerTick, Dispatcher);
            this.timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            var balloon = new CustomBalloon { BalloonText = "Ring Ring" };
            tb.ShowCustomBalloon(balloon, PopupAnimation.Scroll, 3000);
        }

        private void inicijalizujPodatke()
        {
            brojAktivnihSmerova = brojLogickiAktivnihSmerova();
            brojAktivnihSoftvera = brojLogickiAktivnihSoftvera();

            predmetiKolekcija = new ObservableCollection<Predmet>();
            foreach (Predmet p in racunarskiCentar.Predmeti.Values)
            {
                if (!p.Obrisan)
                    predmetiKolekcija.Add(p);
            }
            tabelaPredmeta.ItemsSource = predmetiKolekcija;
            tabelaPredmeta.IsReadOnly = true;
            tabelaPredmeta.UnselectAll();
            detaljanPrikazPredmet.Visibility = Visibility.Collapsed;

            softveriKolekcija = new ObservableCollection<Softver>();
            foreach (Softver s in racunarskiCentar.Softveri.Values)
            {
                if (!s.Obrisan)
                    softveriKolekcija.Add(s);
            }
            tabelaSoftvera.ItemsSource = softveriKolekcija;
            tabelaSoftvera.IsReadOnly = true;
            tabelaSoftvera.UnselectAll();
            detaljanPrikazSoftver.Visibility = Visibility.Hidden;
            // kolekcija kolekcija softvera za undo redo mehanizam
            prethodnaStanjaAplikacije = new OrderedDictionary();
            sledecaStanjaAplikacije = new OrderedDictionary();

            smeroviKolekcija = new ObservableCollection<Smer>();
            foreach (Smer s in racunarskiCentar.Smerovi.Values)
            {
                if (!s.Obrisan)
                    smeroviKolekcija.Add(s);
            }
            tabelaSmerova.ItemsSource = smeroviKolekcija;
            tabelaSmerova.IsReadOnly = true;
            tabelaSmerova.UnselectAll();
            detaljanPrikazSmer.Visibility = Visibility.Hidden;

            ucioniceKolekcija = new ObservableCollection<Ucionica>();
            foreach (Ucionica u in racunarskiCentar.Ucionice.Values)
            {
                if (!u.Obrisan)
                    ucioniceKolekcija.Add(u);
            }
            tabelaUcionica.ItemsSource = ucioniceKolekcija;
            tabelaUcionica.IsReadOnly = true;
            tabelaUcionica.UnselectAll();
            detaljanPrikazUcionica.Visibility = Visibility.Hidden;

        }

        private void InitializeChromium()
        {
            var path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            List<string> tokens = path.Split('\\').ToList();
            tokens.RemoveAt(tokens.Count - 1);
            path = String.Join("\\", tokens.ToArray());
            String page = string.Format(@"{0}\html\html\kalendar.html", path);

            CefSettings settings = new CefSettings();

            Cef.Initialize(settings);

            chromeBrowser = new ChromiumWebBrowser();
            chromeBrowser.Address = page;

            BrowserGrid.Children.Add(chromeBrowser);
        }
        
        private int brojLogickiAktivnihSoftvera()
        {
            int brojLogickiAktivnih = 0;
            foreach (Softver s in racunarskiCentar.Softveri.Values)
            {
                if (!s.Obrisan)
                    brojLogickiAktivnih++;
            }
            return brojLogickiAktivnih;
        }

        private int brojLogickiAktivnihSmerova()
        {
            int brojLogickiAktivnihSmerova = 0;
            foreach (Smer s in racunarskiCentar.Smerovi.Values)
            {
                if (!s.Obrisan)
                    brojLogickiAktivnihSmerova++;
            }
            return brojLogickiAktivnihSmerova;
        }

        private void dodavanjeUcioniceClick(object sender, RoutedEventArgs e)
        {
            if (racunarskiCentar.Softveri.Count > 0 && brojAktivnihSoftvera > 0)
            {
                // dodavanje nove ucionice je moguce samo ako postoji neki logicki aktivan softver
                if (tabControl.SelectedIndex != 1)
                    tabControl.SelectedIndex = 1;
                var ucionicaWindow = new DodavanjeUcionice(racunarskiCentar, ucioniceKolekcija, false, "", notifierSucces, stekStanja, prethodnaStanjaAplikacije);
                ucionicaWindow.ShowDialog();
                if (ucionicaWindow.potvrdio)
                {
                    omoguciUndo();
                    ponistiOnemoguciRedo();
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Ne možete uneti učionicu dok god ne unesete bar jedan softver!");
                });
            }
        }

        private void dodavanjePredmetaClick(object sender, RoutedEventArgs e)
        {
            if (racunarskiCentar.Smerovi.Count > 0 && racunarskiCentar.Softveri.Count > 0 && brojAktivnihSmerova > 0 && brojAktivnihSoftvera > 0)
            {
                // dodavanje novog predmeta je moguce samo ako postoji neki logicki aktivan softver i neki logicki aktivan smer
                if (tabControl.SelectedIndex != 2)
                    tabControl.SelectedIndex = 2;
                var predmetWindow = new DodavanjePredmeta(racunarskiCentar, predmetiKolekcija, false, "", notifierSucces, stekStanja, prethodnaStanjaAplikacije);
                predmetWindow.ShowDialog();
                if (predmetWindow.potvrdio)
                {
                    omoguciUndo();
                    ponistiOnemoguciRedo();
                }
            }
            else if ((racunarskiCentar.Smerovi.Count == 0 && racunarskiCentar.Softveri.Count == 0) || (brojAktivnihSoftvera == 0 && brojAktivnihSmerova == 0))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Ne možete uneti predmet dok god ne unesete bar jedan smer i bar jedan softver!");
                });
            }
            else if (racunarskiCentar.Smerovi.Count == 0 || brojAktivnihSmerova == 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Ne možete uneti predmet dok god ne unesete bar jedan smer!");
                });
            }
            else if (racunarskiCentar.Softveri.Count == 0 || brojAktivnihSoftvera == 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Ne možete uneti predmet dok god ne unesete bar jedan softver!");
                });
            }
        }

        private void dodavanjeSmeraClick(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 3)
                tabControl.SelectedIndex = 3;
            int stariBrojSmerova = racunarskiCentar.Smerovi.Count;
            var smerWindow = new DodavanjeSmera(racunarskiCentar, smeroviKolekcija, false, "", notifierSucces, stekStanja, prethodnaStanjaAplikacije);
            smerWindow.ShowDialog();
            if (smerWindow.potvrdio)
            {
                omoguciUndo();
                ponistiOnemoguciRedo();
            }

            if (racunarskiCentar.Smerovi.Count - stariBrojSmerova == 1)
                // uspesno je dodat novi smer (logicki je aktivan)
                brojAktivnihSmerova++;
            else
                // mozda je izvrseno aktiviranje nekog logicki neaktivnog smera, pa je sada postao aktivan
                brojAktivnihSmerova = brojLogickiAktivnihSmerova();
        }

        private void dodavanjeSoftveraClick(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 4)
                tabControl.SelectedIndex = 4;
            int stariBrojSoftvera = racunarskiCentar.Softveri.Count;
            var softverWindow = new DodavanjeSoftvera(racunarskiCentar, softveriKolekcija, false, "", notifierSucces, stekStanja, prethodnaStanjaAplikacije);
            softverWindow.ShowDialog();
            if (softverWindow.potvrdio)
            {
                omoguciUndo();
                ponistiOnemoguciRedo();
            }

            if (racunarskiCentar.Softveri.Count - stariBrojSoftvera == 1)
                // uspesno je dodat novi softver (logicki je aktivan)
                brojAktivnihSoftvera++;
            else
                // mozda je izvrseno aktiviranje nekog logicki neaktivnog softvera, pa je sada postao aktivan
                brojAktivnihSoftvera = brojLogickiAktivnihSoftvera();
        }

        private void pregledKalendaraClick(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 0)
                tabControl.SelectedIndex = 0;
        }

        private void pregledUcionicaClick(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 1)
                tabControl.SelectedIndex = 1;
        }

        private void pregledPredmetaClick(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 2)
                tabControl.SelectedIndex = 2;
        }

        private void pregledSmerovaClick(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 3)
                tabControl.SelectedIndex = 3;
        }

        private void pregledSoftveraClick(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 4)
                tabControl.SelectedIndex = 4;
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            mainWindow.Close();
        }

        private void undoClick(object sender, RoutedEventArgs e)
        {
            // pruzimamo vrh undo steka, kao poslednje zabelezeno stanje aplikacije (starije) drugacije od trenutnog
            string kljucStarog = stekStanja.Undo_Pop();
            if (kljucStarog != "" && prethodnaStanjaAplikacije.Contains(kljucStarog))
            {
                // ako ima elemenata na steku i postoji taj kljuc u kolekciji (nismo presli 10 koraka undo mehanizma)
                // dobavljamo staro stanje aplikacije i azuriramo trenutni prikaz
                StanjeAplikacije staroStanje = (StanjeAplikacije)(prethodnaStanjaAplikacije[kljucStarog]);

                // pamtimo trenutno stanje aplikacije
                StanjeAplikacije trenutnoStanje = new StanjeAplikacije();
                trenutnoStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                trenutnoStanje.TipPodataka = staroStanje.TipPodataka;
                trenutnoStanje.Oznake = staroStanje.Oznake;
                trenutnoStanje.Kolicina = staroStanje.Kolicina;

                switch (staroStanje.TipPromene)
                {
                    case "dodavanje":
                        {
                            trenutnoStanje.TipPromene = "brisanje";
                            break;
                        }
                    case "brisanje":
                        {
                            trenutnoStanje.TipPromene = "dodavanje";
                            break;
                        }
                    case "pomeranje":
                        {
                            // moguca akcija na kalendaru
                            trenutnoStanje.TipPromene = "pomeranje";
                            break;
                        }
                    case "izmena":
                        {
                            // moguca akcija nad tabelama entiteta
                            trenutnoStanje.TipPromene = "izmena";
                            break;
                        }
                }

                // postavljamo trenutno stanje na staro stanje koje smo preuzeli sa undo steka
                racunarskiCentar = staroStanje.RacunarskiCentar;
                prethodnaStanjaAplikacije.Remove(kljucStarog);

                cef.RacunarskiCentar = racunarskiCentar;
                cef.posaljiPodatke();
                azurirajKolekcije();    

                string kljucTrenutnog = Guid.NewGuid().ToString();
                sledecaStanjaAplikacije.Add(kljucTrenutnog, trenutnoStanje);
                stekStanja.GetRedo().Push(kljucTrenutnog);
                // omogucavamo klik na dugme za redo operaciju
                MenuItemRedo.IsEnabled = true;
                MenuItemRedoPicture.IsEnabled = true;
                MenuItemRedoPictureImg.Source = new BitmapImage(new Uri(@"/picture/redo.png", UriKind.Relative));

                // prebacujemo fokus na tabelu u kojoj se odslikala promena stanja i ispisujemo poruku o promeni
                string poruka = generisiPoruku(staroStanje.TipPodataka, staroStanje.TipPromene, staroStanje.Kolicina, staroStanje.Oznake);
                prebaciFokusUzObavestenje(staroStanje.TipPodataka, poruka, staroStanje.TipPromene);
            }
            if (stekStanja.UndoCount == 0 || !prethodnaStanjaAplikacije.Contains(stekStanja.GetUndo().Peek()))
            {
                // ako nemamo vise objekata na steku ili smo iskoristili svih 10 koraka undo operacije
                // onemogucavamo poziv ove operacije
                MenuItemUndo.IsEnabled = false;
                MenuItemUndoPicture.IsEnabled = false;
                MenuItemUndoPictureImg.Source = new BitmapImage(new Uri(@"/picture/undo1.png", UriKind.Relative));
            }
        }

        private void redoClick(object sender, RoutedEventArgs e)
        {
            // pruzimamo vrh redo steka, kao poslednje zabelezeno stanje aplikacije (novije) drugacije od trenutnog
            string kljucNovog = stekStanja.Redo_Pop();
            if (kljucNovog != "" && sledecaStanjaAplikacije.Contains(kljucNovog))
            {
                // ako ima elemenata na steku i postoji taj kljuc u kolekciji (nismo presli 10 koraka undo mehanizma)
                // dobavljamo staro stanje aplikacije i azuriramo trenutni prikaz
                StanjeAplikacije novoStanje = (StanjeAplikacije)(sledecaStanjaAplikacije[kljucNovog]);

                // pamtimo trenutno stanje aplikacije
                StanjeAplikacije trenutnoStanje = new StanjeAplikacije();
                trenutnoStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                trenutnoStanje.TipPodataka = novoStanje.TipPodataka;
                trenutnoStanje.Oznake = novoStanje.Oznake;
                trenutnoStanje.Kolicina = novoStanje.Kolicina;

                switch (novoStanje.TipPromene)
                {
                    case "dodavanje":
                        {
                            trenutnoStanje.TipPromene = "brisanje";
                            break;
                        }
                    case "brisanje":
                        {
                            trenutnoStanje.TipPromene = "dodavanje";
                            break;
                        }
                    case "pomeranje":
                        {
                            // moguca akcija na kalendaru
                            trenutnoStanje.TipPromene = "pomeranje";
                            break;
                        }
                    case "izmena":
                        {
                            // moguca akcija nad tabelama entiteta
                            trenutnoStanje.TipPromene = "izmena";
                            break;
                        }
                }

                // postavljamo trenutno stanje na novo stanje koje smo preuzeli sa redo steka
                racunarskiCentar = novoStanje.RacunarskiCentar;
                sledecaStanjaAplikacije.Remove(kljucNovog);

                cef.RacunarskiCentar = racunarskiCentar;
                cef.posaljiPodatke();
                azurirajKolekcije();

                string kljucTrenutnog = Guid.NewGuid().ToString();
                prethodnaStanjaAplikacije.Add(kljucTrenutnog, trenutnoStanje);
                stekStanja.GetUndo().Push(kljucTrenutnog);
                // omogucavamo klik na dugme za undo operaciju
                MenuItemUndo.IsEnabled = true;
                MenuItemUndoPicture.IsEnabled = true;
                MenuItemUndoPictureImg.Source = new BitmapImage(new Uri(@"/picture/undo.png", UriKind.Relative));

                // prebacujemo fokus na tabelu u kojoj se odslikala promena stanja i ispisujemo poruku o promeni
                string poruka = generisiPoruku(novoStanje.TipPodataka, novoStanje.TipPromene, novoStanje.Kolicina, novoStanje.Oznake);
                prebaciFokusUzObavestenje(novoStanje.TipPodataka, poruka, novoStanje.TipPromene);
            }
            if (stekStanja.RedoCount == 0 || !sledecaStanjaAplikacije.Contains(stekStanja.GetRedo().Peek()))
            {
                // ako nemamo vise objekata na steku ili smo iskoristili svih 10 koraka redo operacije
                // onemogucavamo poziv ove operacije
                MenuItemRedo.IsEnabled = false;
                MenuItemRedoPicture.IsEnabled = false;
                MenuItemRedoPictureImg.Source = new BitmapImage(new Uri(@"/picture/redo1.png", UriKind.Relative));
            }
        }

        private string generisiPoruku(string tipPodataka, string tipPromene, int kolicina, List<string> oznake)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbOznake = new StringBuilder();

            foreach(string o in oznake)
            {
                if (oznake.IndexOf(o) == oznake.Count - 1)
                    sbOznake.Append(o);
                else
                    sbOznake.Append(o + ", ");
            }
            
            switch(tipPromene)
            {
                case "dodavanje":
                    {
                        if(tipPodataka == "kalendar")
                        {
                            sb.Append("Izvršeno dodavanje termina na kalendar");
                        }
                        else
                        {
                            if (kolicina == 1) {
                                // poruka o dodavanju jednog predmeta/softvera/smera/ucionice
                                sb.Append("Izvršeno dodavanje ");
                                vratiStringTipaPodataka(tipPodataka, sb, kolicina);
                                sb.Append("sa oznakom " + sbOznake.ToString());
                            }
                            else
                            {
                                // poruka o dodavanju vise predmeta/softvera/smerova/ucionica
                                sb.Append("Izvršeno dodavanje ");
                                vratiStringTipaPodataka(tipPodataka, sb, kolicina);
                                sb.Append("sa oznakama " + sbOznake.ToString());
                            }
                        }
                        break;
                    }
                case "brisanje":
                    {
                        if (tipPodataka == "kalendar")
                        {
                            sb.Append("Izvršeno uklanjanje termina sa kalendara");
                        }
                        else
                        {
                            if (kolicina == 1)
                            {
                                // poruka o dodavanju jednog predmeta/softvera/smera/ucionice
                                sb.Append("Izvršeno uklanjanje ");
                                vratiStringTipaPodataka(tipPodataka, sb, kolicina);
                                sb.Append("sa oznakom " + sbOznake.ToString());
                            }
                            else
                            {
                                // poruka o dodavanju vise predmeta/softvera/smerova/ucionica
                                sb.Append("Izvršeno uklanjanje ");
                                vratiStringTipaPodataka(tipPodataka, sb, kolicina);
                                sb.Append("sa oznakama " + sbOznake.ToString());
                            }
                        }
                        break;
                    }
                case "pomeranje":
                    {
                        sb.Append("Izvršeno pomeranje termina na kalendaru");
                        break;
                    }
                case "izmena":
                    {
                        // moguca akcija nad tabelama entiteta
                        if (kolicina == 1)
                        {
                            // poruka o izmeni jednog predmeta/softvera/smera/ucionice
                            sb.Append("Izvršena izmena ");
                            vratiStringTipaPodataka(tipPodataka, sb, kolicina);
                            sb.Append("sa oznakom " + sbOznake.ToString());
                        }
                        else
                        {
                            // poruka o izmeni vise softvera/smerova
                            sb.Append("Izvršena izmena ");
                            vratiStringTipaPodataka(tipPodataka, sb, kolicina);
                            sb.Append("sa oznakama " + sbOznake.ToString());
                        }
                        break;
                    }
            }
            return sb.ToString();
        }

        private void vratiStringTipaPodataka(string tipPodatka, StringBuilder sb, int kolicina)
        {
            switch (tipPodatka)
            {
                case "predmet":
                    {
                        sb.Append("predmeta ");
                        break;
                    }
                case "ucionica":
                    {
                        if (kolicina == 1)
                            sb.Append("učionice ");
                        else
                            sb.Append("učionica ");
                        break;
                    }
                case "smer":
                    {
                        if (kolicina == 1)
                            sb.Append("smera ");
                        else
                            sb.Append("smerova ");
                        break;
                    }
                case "softver":
                    {
                        sb.Append("softvera ");
                        break;
                    }
            }
        }

        private void prebaciFokusUzObavestenje(string tipPodatka, string poruka, string tipPromene)
        {
            switch (tipPodatka)
            {
                case "ucionica":
                    {
                        tabControl.SelectedIndex = 1;
                        break;
                    }
                case "predmet":
                    {
                        tabControl.SelectedIndex = 2;
                        break;
                    }
                case "smer":
                    {
                        tabControl.SelectedIndex = 3;
                        break;
                    }
                case "softver":
                    {
                        tabControl.SelectedIndex = 4;
                        break;
                    }
                case "kalendar":
                    {
                        tabControl.SelectedIndex = 0;
                        break;
                    }
            }

            switch (tipPromene)
            {
                case "dodavanje":
                    {
                        notifierUndoRedo.ShowSuccess(poruka);
                        break;
                    }
                case "brisanje":
                    {
                        notifierUndoRedo.ShowError(poruka);
                        break;
                    }
                case "pomeranje":
                    {
                        notifierUndoRedo.ShowInformation(poruka);
                        break;
                    }
                case "izmena":
                    {
                        notifierUndoRedo.ShowInformation(poruka);
                        break;
                    }
            }
        }

        private void tabsFocus(object sender, RoutedEventArgs e)
        {
            // trenutno smo u tabu za ucionice
            if (tabControl.SelectedIndex == 1)
            {
                UcionicaTab.Focus();
                tabControl.SelectedItem = UcionicaTab;
            }
            // trenutno smo u tabu za predmete
            else if (tabControl.SelectedIndex == 2)
            {
                PredmetTab.Focus();
                tabControl.SelectedItem = PredmetTab;
            }
            // trenutno smo u tabu za smerove
            else if (tabControl.SelectedIndex == 3)
            {
                SmerTab.Focus();
                tabControl.SelectedItem = SmerTab;
            }
            // trenutno smo u tabu za softvere
            else if (tabControl.SelectedIndex == 4)
            {
                SoftverTab.Focus();
                tabControl.SelectedItem = SoftverTab;
            }
        }

        private void pretragaFokus(object sender, EventArgs e)
        {
            // trenutno smo u tabu za ucionice
            if (tabControl.SelectedIndex == 1)
            {
                UcionicaPretragaUnos.Focus();
            }
            // trenutno smo u tabu za predmete
            else if (tabControl.SelectedIndex == 2)
            {
                PredmetPretragaUnos.Focus();
            }
            // trenutno smo u tabu za smerove
            else if (tabControl.SelectedIndex == 3)
            {
                SmerPretragaUnos.Focus();
            }
            // trenutno smo u tabu za softvere
            else if (tabControl.SelectedIndex == 4)
            {
                SoftverPretragaUnos.Focus();
            }
        }

        private void otvoriKriterijumFilter(object sender, EventArgs e)
        {
            // trenutno smo u tabu za ucionice
            if (tabControl.SelectedIndex == 1)
            {
                UcionicaFilterKriterijum.IsDropDownOpen = true;
                UcionicaFilterKriterijum.Focus();
            }
            // trenutno smo u tabu za predmete
            else if (tabControl.SelectedIndex == 2)
            {
                PredmetFilterKriterijum.IsDropDownOpen = true;
                PredmetFilterKriterijum.Focus();
            }
            // trenutno smo u tabu za smerove
            else if (tabControl.SelectedIndex == 3)
            {
                SmerFilterKriterijum.IsDropDownOpen = true;
                SmerFilterKriterijum.Focus();
            }
            // trenutno smo u tabu za softvere
            else if (tabControl.SelectedIndex == 4)
            {
                SoftverFilterKriterijum.IsDropDownOpen = true;
                SoftverFilterKriterijum.Focus();
            }
        }

        public void proveriKriterijumFiltriranja(object sender, SelectionChangedEventArgs e)
        {
            // trenutno smo u tabu za ucionice
            if (tabControl.SelectedIndex == 1)
            {
                int index = UcionicaFilterKriterijum.SelectedIndex;
                if(index == 3 || index == 4)
                {
                    // kao kriterijum za filtriranje je izabrana tabla ili pametna tabla
                    UcionicaFilterUnos.Visibility = Visibility.Hidden;
                    UcionicaFilterTablaVrednost.Visibility = Visibility.Visible;
                    UcionicaFilterTablaVrednost.Text = "";
                    UcionicaFilterProjektorVrednost.Visibility = Visibility.Hidden;
                    UcionicaFilterOSVrednost.Visibility = Visibility.Hidden;
                }
                else if(index == 2)
                {
                    // kao kriterijum za filtriranje je izabran projektor
                    UcionicaFilterUnos.Visibility = Visibility.Hidden;
                    UcionicaFilterTablaVrednost.Visibility = Visibility.Hidden;
                    UcionicaFilterProjektorVrednost.Visibility = Visibility.Visible;
                    UcionicaFilterProjektorVrednost.Text = "";
                    UcionicaFilterOSVrednost.Visibility = Visibility.Hidden;
                }
                else if (index == 5)
                {
                    // kao kriterijum za filtriranje je izabran operativni sistem
                    UcionicaFilterUnos.Visibility = Visibility.Hidden;
                    UcionicaFilterTablaVrednost.Visibility = Visibility.Hidden;
                    UcionicaFilterProjektorVrednost.Visibility = Visibility.Hidden;
                    UcionicaFilterOSVrednost.Visibility = Visibility.Visible;
                    UcionicaFilterOSVrednost.Text = "";
                }
                else
                {
                    UcionicaFilterTablaVrednost.Text = "";
                    UcionicaFilterProjektorVrednost.Text = "";
                    UcionicaFilterOSVrednost.Text = "";
                    UcionicaFilterUnos.Visibility = Visibility.Visible;
                    UcionicaFilterTablaVrednost.Visibility = Visibility.Hidden;
                    UcionicaFilterProjektorVrednost.Visibility = Visibility.Hidden;
                    UcionicaFilterOSVrednost.Visibility = Visibility.Hidden;
                }
                ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaUcionica.ItemsSource);
                cv.Filter = null;
            }
            // trenutno smo u tabu za predmete
            else if (tabControl.SelectedIndex == 2)
            {
                int index = PredmetFilterKriterijum.SelectedIndex;
                if (index == 7 || index == 8)
                {
                    // kao kriterijum za filtriranje je izabrana tabla ili pametna tabla
                    PredmetFilterUnos.Visibility = Visibility.Hidden;
                    PredmetFilterTablaVrednost.Visibility = Visibility.Visible;
                    PredmetFilterTablaVrednost.Text = "";
                    PredmetFilterProjektorVrednost.Visibility = Visibility.Hidden;
                    PredmetFilterOSVrednost.Visibility = Visibility.Hidden;
                }
                else if (index == 6)
                {
                    // kao kriterijum za filtriranje je izabran projektor
                    PredmetFilterUnos.Visibility = Visibility.Hidden;
                    PredmetFilterTablaVrednost.Visibility = Visibility.Hidden;
                    PredmetFilterProjektorVrednost.Visibility = Visibility.Visible;
                    PredmetFilterProjektorVrednost.Text = "";
                    PredmetFilterOSVrednost.Visibility = Visibility.Hidden;
                }
                else if (index == 9)
                {
                    // kao kriterijum za filtriranje je izabran operativni sistem
                    PredmetFilterUnos.Visibility = Visibility.Hidden;
                    PredmetFilterTablaVrednost.Visibility = Visibility.Hidden;
                    PredmetFilterProjektorVrednost.Visibility = Visibility.Hidden;
                    PredmetFilterOSVrednost.Visibility = Visibility.Visible;
                    PredmetFilterOSVrednost.Text = "";
                }
                else
                {
                    PredmetFilterTablaVrednost.Text = "";
                    PredmetFilterProjektorVrednost.Text = "";
                    PredmetFilterOSVrednost.Text = "";
                    PredmetFilterUnos.Visibility = Visibility.Visible;
                    PredmetFilterTablaVrednost.Visibility = Visibility.Hidden;
                    PredmetFilterProjektorVrednost.Visibility = Visibility.Hidden;
                    PredmetFilterOSVrednost.Visibility = Visibility.Hidden;
                }
                ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaPredmeta.ItemsSource);
                cv.Filter = null;
            }
            // trenutno smo u tabu za smerove
            else if (tabControl.SelectedIndex == 3)
            {
                int index = SmerFilterKriterijum.SelectedIndex;
                if (index == 2)
                {
                    // kao kriterijum za filtriranje je izabran datum uvođenja
                    SmerFilterUnos.Visibility = Visibility.Hidden;
                    SmerFilterDatumVrednost.Visibility = Visibility.Visible;
                    SmerFilterDatumVrednost.Text = "";
                }
                else
                {
                    SmerFilterUnos.Visibility = Visibility.Visible;
                    SmerFilterDatumVrednost.Visibility = Visibility.Hidden;
                    SmerFilterDatumVrednost.Text = "";
                }
                ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaSmerova.ItemsSource);
                cv.Filter = null;
            }
            // trenutno smo u tabu za softvere
            else if (tabControl.SelectedIndex == 4)
            {
                int index = SoftverFilterKriterijum.SelectedIndex;
                if (index == 2)
                {
                    // kao kriterijum za filtriranje je izabran operativni sistem
                    PredmetFilterUnos.Visibility = Visibility.Hidden;
                    PredmetFilterOSVrednost.Visibility = Visibility.Visible;
                    PredmetFilterOSVrednost.Text = "";
                }
                else
                {
                    PredmetFilterOSVrednost.Text = "";
                    PredmetFilterUnos.Visibility = Visibility.Visible;
                    PredmetFilterOSVrednost.Visibility = Visibility.Hidden;
                }
                ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaSoftvera.ItemsSource);
                cv.Filter = null;
            }
        }

        private void filtrirajPoVrednosti(object sender, TextChangedEventArgs e)
        {
            // trenutno smo u tabu za ucionice
            if (tabControl.SelectedIndex == 1)
            {
                int indexKriterijuma = UcionicaFilterKriterijum.SelectedIndex;
                if (indexKriterijuma == 3 || indexKriterijuma == 4)
                {
                    // izabrano filtriranje po tabli ili pametnoj tabli
                    string vrednost = UcionicaFilterTablaVrednost.Text.Trim();

                    ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaUcionica.ItemsSource);
                    if (vrednost == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            Ucionica u = o as Ucionica;
                            if (indexKriterijuma == 3)
                            {
                                return (u.TablaString.ToUpper().StartsWith(vrednost.ToUpper()));
                            }
                            else
                            {
                                return (u.PametnaTablaString.ToUpper().StartsWith(vrednost.ToUpper()));
                            }
                        };
                    }
                }
                else if (indexKriterijuma == 2)
                {
                    // izabrano je filtriranje po projektoru
                    string vrednost = UcionicaFilterProjektorVrednost.Text.Trim();
                    
                    ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaUcionica.ItemsSource);
                    if (vrednost == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            Ucionica u = o as Ucionica;
                            return (u.ProjektorString.ToUpper().StartsWith(vrednost.ToUpper()));
                        };
                    }
                }
                else if (indexKriterijuma == 5)
                {
                    // izabrano je filtriranje po operativnom sistemu
                    string vrednost = UcionicaFilterOSVrednost.Text.Trim();

                    ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaUcionica.ItemsSource);
                    if (vrednost == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            Ucionica u = o as Ucionica;
                            if(vrednost.ToLower() == "linux" || vrednost.ToLower() == "windows" || vrednost.ToLower() == "windows i linux")
                                return (u.OperativniSistem.ToUpper().Contains(vrednost.ToUpper()));
                            else
                                return (u.OperativniSistem.ToUpper().StartsWith(vrednost.ToUpper()));
                        };
                    }
                }
            }
            // trenutno smo u tabu za predmete
            if (tabControl.SelectedIndex == 2)
            {
                int indexKriterijuma = PredmetFilterKriterijum.SelectedIndex;
                if (indexKriterijuma == 7 || indexKriterijuma == 8)
                {
                    // izabrano filtriranje po tabli ili pametnoj tabli
                    string vrednost = PredmetFilterTablaVrednost.Text.Trim();

                    ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaPredmeta.ItemsSource);
                    if (vrednost == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            Predmet p = o as Predmet;
                            if (indexKriterijuma == 3)
                            {
                                return (p.TablaString.ToUpper().StartsWith(vrednost.ToUpper()));
                            }
                            else
                            {
                                return (p.PametnaTablaString.ToUpper().StartsWith(vrednost.ToUpper()));
                            }
                        };
                    }
                }
                else if (indexKriterijuma == 6)
                {
                    // izabrano je filtriranje po projektoru
                    string vrednost = PredmetFilterProjektorVrednost.Text.Trim();

                    ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaPredmeta.ItemsSource);
                    if (vrednost == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            Predmet p = o as Predmet;
                            return (p.ProjektorString.ToUpper().StartsWith(vrednost.ToUpper()));
                        };
                    }
                }
                else if (indexKriterijuma == 9)
                {
                    // izabrano je filtriranje po operativnom sistemu
                    string vrednost = PredmetFilterOSVrednost.Text.Trim();

                    ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaPredmeta.ItemsSource);
                    if (vrednost == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            Predmet p = o as Predmet;
                            return (p.OperativniSistem.ToUpper().StartsWith(vrednost.ToUpper()));
                        };
                    }
                }
            }
            if (tabControl.SelectedIndex == 3)
            {
                // trenutno smo u tabu za smerove
                int indexKriterijuma = SmerFilterKriterijum.SelectedIndex;
                if (indexKriterijuma == 2)
                {
                    // izabrano je filtriranje po datumu uvodjenja
                    string vrednost = SmerFilterDatumVrednost.Text.Trim();

                    ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaSmerova.ItemsSource);
                    if (vrednost == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            Smer s = o as Smer;
                            return (s.Datum.Equals(DateTime.Parse(vrednost).ToString("dd/MM/yyyy")));
                        };
                    }
                }
            }
            if (tabControl.SelectedIndex == 4)
            {
                // trenutno smo u tabu za softvere
                int indexKriterijuma = SoftverFilterKriterijum.SelectedIndex;
                if (indexKriterijuma == 2)
                {
                    // izabrano je filtriranje po operativnom sistemu
                    string vrednost = UcionicaFilterOSVrednost.Text.Trim();

                    ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaSoftvera.ItemsSource);
                    if (vrednost == "")
                        cv.Filter = null;
                    else
                    {
                        cv.Filter = o =>
                        {
                            Softver s = o as Softver;
                            if (vrednost.ToLower() == "linux" || vrednost.ToLower() == "windows" || vrednost.ToLower() == "linux i windows")
                                return (s.OperativniSistem.ToUpper().Contains(vrednost.ToUpper()));
                            else
                                return (s.OperativniSistem.ToUpper().StartsWith(vrednost.ToUpper()));
                        };
                    }
                }
            }
        }

        private void ponudaOpcija(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && e.Key == Key.LeftAlt)
            {
                if (tabControl.SelectedIndex == 1)
                    UcionicaFilterKriterijum.IsDropDownOpen = true;
                else if (tabControl.SelectedIndex == 2)
                    PredmetFilterKriterijum.IsDropDownOpen = true;
                else if (tabControl.SelectedIndex == 3)
                    SmerFilterKriterijum.IsDropDownOpen = true;
                else if (tabControl.SelectedIndex == 4)
                    SoftverFilterKriterijum.IsDropDownOpen = true;
            }
            else if (e.Key == Key.Up && e.Key == Key.LeftAlt)
            {
                if (tabControl.SelectedIndex == 1)
                    UcionicaFilterKriterijum.IsDropDownOpen = false;
                else if (tabControl.SelectedIndex == 2)
                    PredmetFilterKriterijum.IsDropDownOpen = false;
                else if (tabControl.SelectedIndex == 3)
                    SmerFilterKriterijum.IsDropDownOpen = false;
                else if (tabControl.SelectedIndex == 4)
                    SoftverFilterKriterijum.IsDropDownOpen = false;
            }
        }

        private void pretraziUcionice(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            string parametar = t.Text.Trim();

            ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaUcionica.ItemsSource);
            if (parametar == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    Ucionica u = o as Ucionica;
                    return (u.Oznaka.ToUpper().Contains(parametar.ToUpper()) || u.BrojRadnihMesta.ToString().Contains(parametar)
                    || u.ProjektorString.ToUpper().Contains(parametar.ToUpper()) || u.TablaString.ToUpper().Contains(parametar.ToUpper())
                    || u.PametnaTablaString.ToUpper().Contains(parametar.ToUpper()) || u.OperativniSistem.ToUpper().Contains(parametar.ToUpper())
                    || u.OperativniSistem.ToUpper().Contains(parametar.ToUpper()));
                };
            }
        }

        private void filtrirajUcionicu(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            string filter = t.Text.Trim();
            string parametar = UcionicaFilterKriterijum.Text.Trim();

            ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaUcionica.ItemsSource);
            if (filter == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    Ucionica u = o as Ucionica;
                    if (parametar == "Oznaka")
                        return (u.Oznaka.ToUpper().StartsWith(filter.ToUpper()));
                    else if (parametar == "Broj radnih mesta")
                        return (u.BrojRadnihMesta.ToString().StartsWith(filter));
                    else
                        return (u.Opis.ToUpper().StartsWith(filter.ToUpper()));
                };
            }
        }

        private void pretraziPredmete(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            string parametar = t.Text.Trim();

            ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaPredmeta.ItemsSource);
            if (parametar == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    Predmet p = o as Predmet;
                    return (p.Naziv.ToUpper().Contains(parametar.ToUpper()) || p.Oznaka.ToUpper().Contains(parametar.ToUpper())
                    || p.VelicinaGrupe.ToString().Contains(parametar) || p.Opis.ToUpper().Contains(parametar.ToUpper())
                    || p.MinDuzinaTermina.ToString().Contains(parametar) || p.BrTermina.ToString().Contains(parametar)
                    || p.ProjektorString.ToUpper().Contains(parametar.ToUpper()) || p.TablaString.ToUpper().Contains(parametar.ToUpper())
                    || p.PametnaTablaString.ToUpper().Contains(parametar.ToUpper()) || p.OperativniSistem.ToUpper().Contains(parametar.ToUpper()));
                };
            }
        }

        private void filtrirajPredmet(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            string filter = t.Text.Trim();
            string parametar = PredmetFilterKriterijum.Text.Trim();

            ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaPredmeta.ItemsSource);
            if (filter == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    Predmet p = o as Predmet;
                    if (parametar == "Naziv")
                        return (p.Naziv.ToUpper().StartsWith(filter.ToUpper()));
                    else if (parametar == "Oznaka")
                        return (p.Oznaka.ToUpper().StartsWith(filter.ToUpper()));
                    else if (parametar == "Veličina grupe")
                        return (p.VelicinaGrupe.ToString().StartsWith(filter));
                    else if (parametar == "Opis")
                        return (p.Opis.ToUpper().StartsWith(filter.ToUpper()));
                    else if (parametar == "Minimalna dužina termina")
                        return (p.MinDuzinaTermina.ToString().StartsWith(filter));
                    else
                        return (p.BrTermina.ToString().StartsWith(filter));
                };
            }
        }

        private void pretraziSoftvere(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            string parametar = t.Text.Trim();

            ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaSoftvera.ItemsSource);
            if (parametar == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    Softver s = o as Softver;
                    return (s.Naziv.ToUpper().Contains(parametar.ToUpper()) || s.Oznaka.ToUpper().Contains(parametar.ToUpper())
                    || s.OperativniSistem.ToUpper().Contains(parametar.ToUpper()) || s.Proizvodjac.ToUpper().Contains(parametar.ToUpper())
                    || s.GodIzdavanja.ToString().Contains(parametar) || s.Cena.ToString().Contains(parametar)
                    || s.Sajt.ToUpper().Contains(parametar.ToUpper()) || s.Opis.ToUpper().Contains(parametar.ToUpper()));
                };
            }
        }

        private void filtrirajSoftver(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            string filter = t.Text.Trim();
            string parametar = SoftverFilterKriterijum.Text.Trim();

            ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaSoftvera.ItemsSource);
            if (filter == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    Softver s = o as Softver;
                    if (parametar == "Naziv")
                        return (s.Naziv.ToUpper().StartsWith(filter.ToUpper()));
                    else if (parametar == "Oznaka")
                        return (s.Oznaka.ToUpper().StartsWith(filter.ToUpper()));
                    else if (parametar == "Proizvođač")
                        return (s.Proizvodjac.ToUpper().StartsWith(filter.ToUpper()));
                    else if (parametar == "Godina izdavanja")
                        return (s.GodIzdavanja.ToString().StartsWith(filter));
                    else if (parametar == "Cena")
                        return (s.Cena.ToString().StartsWith(filter));
                    else if (parametar == "Sajt")
                        return (s.Sajt.ToUpper().StartsWith(filter.ToUpper()));
                    else
                        return (s.Opis.ToUpper().StartsWith(filter.ToUpper()));
                };
            }
        }

        private void pretraziSmerove(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            string parametar = t.Text.Trim();

            ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaSmerova.ItemsSource);
            if (parametar == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    Smer s = o as Smer;
                    return (s.Naziv.ToUpper().Contains(parametar.ToUpper()) || s.Oznaka.ToUpper().Contains(parametar.ToUpper())
                    || s.Datum.ToString().Contains(parametar) || s.Opis.ToUpper().Contains(parametar.ToUpper()));
                };
            }
        }

        private void filtrirajSmer(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            string filter = t.Text.Trim();
            string parametar = SmerFilterKriterijum.Text.Trim();

            ICollectionView cv = CollectionViewSource.GetDefaultView(tabelaSmerova.ItemsSource);
            if (filter == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    Smer s = o as Smer;
                    if (parametar == "Naziv")
                        return (s.Naziv.ToUpper().StartsWith(filter.ToUpper()));
                    else if (parametar == "Oznaka")
                        return (s.Oznaka.ToUpper().StartsWith(filter.ToUpper()));
                    else
                        return (s.Opis.ToUpper().StartsWith(filter.ToUpper()));
                };
            }
        }

        private void obrisiElement(object sender, RoutedEventArgs e)
        {
            var brisanjeProzor = new PotvrdaBrisanja();

            // trenutno smo u tabu za ucionice
            if (tabControl.SelectedIndex == 1)
            {
                if (tabelaUcionica.SelectedItems.Count > 1)
                    brisanjeProzor.PorukaBrisanja.Text = "Da li ste sigurni da želite da \nobrišete " + tabelaUcionica.SelectedItems.Count + " izabrane ucionice?";
                brisanjeProzor.ShowDialog();
                if (brisanjeProzor.daKlik)
                    obrisiUcionicuClick(sender, e);
            }
            // trenutno smo u tabu za predmete
            else if (tabControl.SelectedIndex == 2)
            {
                if (tabelaPredmeta.SelectedItems.Count > 1)
                    brisanjeProzor.PorukaBrisanja.Text = "Da li ste sigurni da želite da \nobrišete " + tabelaPredmeta.SelectedItems.Count + " izabrana predmeta?";
                brisanjeProzor.ShowDialog();
                if (brisanjeProzor.daKlik)
                    obrisiPredmetClick(sender, e);
            }
            // trenutno smo u tabu za smerove
            else if (tabControl.SelectedIndex == 3)
            {
                if (tabelaSmerova.SelectedItems.Count > 1)
                    brisanjeProzor.PorukaBrisanja.Text = "Da li ste sigurni da želite da \nobrišete " + tabelaSmerova.SelectedItems.Count + " izabrana smera?";
                brisanjeProzor.ShowDialog();
                if (brisanjeProzor.daKlik)
                    obrisiSmerClick(sender, e);
            }
            // trenutno smo u tabu za softvere
            else if (tabControl.SelectedIndex == 4)
            {
                if (tabelaSoftvera.SelectedItems.Count > 1)
                    brisanjeProzor.PorukaBrisanja.Text = "Da li ste sigurni da želite da \nobrišete " + tabelaSoftvera.SelectedItems.Count + " izabrana softvera?";
                brisanjeProzor.ShowDialog();
                if (brisanjeProzor.daKlik)
                    obrisiSoftverClick(sender, e);
            }
        }

        private void izmeniElement(object sender, RoutedEventArgs e)
        {
            // trenutno smo u tabu za ucionice
            if (tabControl.SelectedIndex == 1)
            {
                if (tabelaUcionica.SelectedItems.Count > 1)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        notifierError.ShowError("Nije moguće izmeniti više učionica odjednom!");
                    });
                }
                else
                    izmeniUcionicuClick(sender, e);
            }
            // trenutno smo u tabu za predmete
            else if (tabControl.SelectedIndex == 2)
            {
                if (tabelaPredmeta.SelectedItems.Count > 1)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        notifierError.ShowError("Nije moguće izmeniti više predmeta odjednom!");
                    });
                }
                else
                    izmeniPredmetClick(sender, e);
            }
            // trenutno smo u tabu za smerove
            else if (tabControl.SelectedIndex == 3)
            {
                if (tabelaSmerova.SelectedItems.Count > 1)
                    izmeniSmeroveClick(sender, e);
                else
                    izmeniSmerClick(sender, e);
            }
            // trenutno smo u tabu za softvere
            else if (tabControl.SelectedIndex == 4)
            {
                if (tabelaSoftvera.SelectedItems.Count > 1)
                    izmeniSoftvereClick(sender, e);
                else
                    izmeniSoftverClick(sender, e);
            }
        }

        private void izmeniSmeroveClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSmerova.SelectedIndex != -1)
            {
                List<int> indeksi = new List<int>();
                foreach (object o in tabelaSmerova.SelectedItems)
                {
                    indeksi.Add(tabelaSmerova.Items.IndexOf(o));
                }

                IzmenaSmerova izmenaSmerova = new IzmenaSmerova(racunarskiCentar, smeroviKolekcija, indeksi, stekStanja, 
                    prethodnaStanjaAplikacije, notifierSucces);
                izmenaSmerova.ShowDialog();
                if (izmenaSmerova.potvrdaIzmena)
                {
                    omoguciUndo();
                    ponistiOnemoguciRedo();
                }
                tabelaSmerova.Items.Refresh();
            }
            else
                return;
        }

        private void izmeniSoftvereClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSoftvera.SelectedIndex != -1)
            {
                List<int> indeksi = new List<int>();
                foreach (object o in tabelaSoftvera.SelectedItems)
                {
                    indeksi.Add(tabelaSoftvera.Items.IndexOf(o));
                }

                IzmenaSoftvera izmenaSoftvera = new IzmenaSoftvera(racunarskiCentar, softveriKolekcija, indeksi, stekStanja, 
                    prethodnaStanjaAplikacije, notifierSucces);
                izmenaSoftvera.ShowDialog();
                if (izmenaSoftvera.potvrdaIzmena)
                {
                    omoguciUndo();
                    ponistiOnemoguciRedo();
                }
                tabelaSoftvera.Items.Refresh();
            }
            else
                return;
        }

        private void izmeniPredmetClick(object sender, RoutedEventArgs e)
        {
            if (tabelaPredmeta.SelectedIndex != -1)
            {
                Predmet pre = (Predmet)tabelaPredmeta.SelectedItem;
                var predmetWindow = new DodavanjePredmeta(racunarskiCentar, predmetiKolekcija, true, pre.Oznaka, notifierSucces, stekStanja, prethodnaStanjaAplikacije);
                predmetWindow.NazivPredmeta.Text = pre.Naziv;
                predmetWindow.OznakaPredmeta.Focus();
                predmetWindow.OznakaPredmeta.Text = pre.Oznaka;
                predmetWindow.OpisPredmeta.Text = pre.Opis;
                predmetWindow.VelicinaGrupePredmet.Text = pre.VelicinaGrupe.ToString();
                predmetWindow.DuzinaTerminaPredmet.Text = pre.MinDuzinaTermina.ToString();
                predmetWindow.BrojTerminaPredmet.Text = pre.BrTermina.ToString();
                predmetWindow.PrisustvoProjektoraPredmet.IsChecked = pre.NeophodanProjektor;
                predmetWindow.PrisustvoPametneTable.IsChecked = pre.NeophodnaPametnaTabla;
                predmetWindow.PrisustvoTablePredmet.IsChecked = pre.NeophodnaTabla;

                if (pre.OperativniSistem.Equals("Windows"))
                    predmetWindow.Windows.IsChecked = true;
                else if (pre.OperativniSistem.Equals("Linux"))
                    predmetWindow.Linux.IsChecked = true;
                else if (pre.OperativniSistem.Equals("Svejedno"))
                    predmetWindow.Svejedno.IsChecked = true;

                for (int i = 0; i < predmetWindow.smeroviTabela.Items.Count; i++) // iteriram kroz tabelu prozora za smerove
                {
                    Smer smer = (Smer)predmetWindow.smeroviTabela.Items[i]; //uzmem softver iz tekuceg reda
                    if (pre.Smer == smer.Oznaka)  //ako postoji u listi, cekiram ga
                    {
                        predmetWindow.smeroviTabela.SelectedIndex = i;
                        smer.UPredmetu = true;
                    }
                    else
                        smer.UPredmetu = false;
                }

                for (int i = 0; i < predmetWindow.softverTabela.Items.Count; i++) // isto i za softvere
                {
                    Softver softver = (Softver)predmetWindow.softverTabela.Items[i];
                    if (pre.Softveri.IndexOf(softver.Oznaka) != -1)
                        softver.Instaliran = true;
                    else
                        softver.Instaliran = false;
                }

                predmetWindow.indeks = tabelaPredmeta.SelectedIndex;
                predmetWindow.ShowDialog();
                if (predmetWindow.potvrdio)
                {
                    omoguciUndo();
                    ponistiOnemoguciRedo();
                }
                tabelaPredmeta.Items.Refresh();
            }
            else
                return;
        }

        private void izmeniSoftverClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSoftvera.SelectedIndex != -1)
            {
                Softver red = (Softver)tabelaSoftvera.SelectedItem;
                var softverWindow = new DodavanjeSoftvera(racunarskiCentar, softveriKolekcija, true, red.Oznaka, notifierSucces, stekStanja, prethodnaStanjaAplikacije);
                softverWindow.nazivSoftver.Text = red.Naziv;
                softverWindow.oznakaSoftver.Focus();
                softverWindow.proizvodjacSoftver.Text = red.Proizvodjac;
                softverWindow.sajtSoftver.Text = red.Sajt;
                softverWindow.godinaSoftver.Text = red.GodIzdavanja.ToString();
                softverWindow.cenaSoftver.Text = red.Cena.ToString();
                softverWindow.opisSoftver.Text = red.Opis;
                softverWindow.oznakaSoftver.Text = red.Oznaka;

                if (red.OperativniSistem.Equals("Windows"))
                    softverWindow.WindowsOSSoftver.IsChecked = true;
                else if (red.OperativniSistem.Equals("Linux"))
                    softverWindow.LinuxOSSoftver.IsChecked = true;
                else if (red.OperativniSistem.Equals("Windows i Linux"))
                    softverWindow.WindowsAndLinuxOSSoftver.IsChecked = true;

                softverWindow.indeks = tabelaSoftvera.SelectedIndex;
                softverWindow.ShowDialog();
                if (softverWindow.potvrdio)
                {
                    omoguciUndo();
                    ponistiOnemoguciRedo();
                }
                tabelaSoftvera.Items.Refresh();
            }
            else
                return;
        }

        private void izmeniUcionicuClick(object sender, RoutedEventArgs e)
        {
            if (tabelaUcionica.SelectedIndex != -1)
            {
                Ucionica red = (Ucionica)tabelaUcionica.SelectedItem;
                var ucionicaWindow = new DodavanjeUcionice(racunarskiCentar, ucioniceKolekcija, true, red.Oznaka, notifierSucces, stekStanja, prethodnaStanjaAplikacije);
                ucionicaWindow.oznakaUcionica.Text = red.Oznaka;
                ucionicaWindow.brojRadnihMestaUcionica.Text = red.BrojRadnihMesta.ToString();
                ucionicaWindow.oznakaUcionica.Focus();
                ucionicaWindow.opisUcionica.Text = red.Opis;
                ucionicaWindow.prisustvoPametneTableUcionica.IsChecked = red.PrisustvoPametneTable;
                ucionicaWindow.prisustvoProjektoraUcionica.IsChecked = red.PrisustvoProjektora;
                ucionicaWindow.prisustvoTableUcionica.IsChecked = red.PrisustvoTable;

                if (red.OperativniSistem.Equals("Windows"))
                    ucionicaWindow.WindowsOSUcionica.IsChecked = true;
                else if (red.OperativniSistem.Equals("Linux"))
                    ucionicaWindow.LinuxOSUcionica.IsChecked = true;
                else if (red.OperativniSistem.Equals("Windows i Linux"))
                    ucionicaWindow.WindowsAndLinuxOSUcionica.IsChecked = true;

                for (int i = 0; i < ucionicaWindow.softverTabela.Items.Count; i++)
                {
                    Softver softver = (Softver)ucionicaWindow.softverTabela.Items[i];
                    if (red.InstaliraniSoftveri.IndexOf(softver.Oznaka) != -1)
                        softver.Instaliran = true;
                    else
                        softver.Instaliran = false;
                }

                ucionicaWindow.indeks = tabelaUcionica.SelectedIndex;
                ucionicaWindow.ShowDialog();
                if (ucionicaWindow.potvrdio)
                {
                    omoguciUndo();
                    ponistiOnemoguciRedo();
                }
                tabelaUcionica.Items.Refresh();
            }
            else
                return;
        }

        private void izmeniSmerClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSmerova.SelectedIndex != -1)
            {
                Smer row = (Smer)tabelaSmerova.SelectedItem;
                var smerWindow = new DodavanjeSmera(racunarskiCentar, smeroviKolekcija, true, row.Oznaka, notifierSucces, stekStanja, prethodnaStanjaAplikacije);
                smerWindow.NazivSmera.Text = row.Naziv;
                smerWindow.OznakaSmera.Focus();
                smerWindow.OznakaSmera.Text = row.Oznaka;
                smerWindow.OpisSmera.Text = row.Opis;
                smerWindow.DatumUvodjenja.Text = row.Datum.ToString();

                smerWindow.indeks = tabelaSmerova.SelectedIndex;
                smerWindow.ShowDialog();
                if (smerWindow.potvrdio)
                {
                    omoguciUndo();
                    ponistiOnemoguciRedo();
                }
                tabelaSmerova.Items.Refresh();
            }
            else
                return;
        }

        private void obrisiPredmetClick(object sender, RoutedEventArgs e)
        {
            if (tabelaPredmeta.SelectedIndex != -1)
            {
                // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                StanjeAplikacije staroStanje = new StanjeAplikacije();
                staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                staroStanje.TipPodataka = "predmet";
                staroStanje.Kolicina = tabelaPredmeta.SelectedItems.Count;
                staroStanje.TipPromene = "dodavanje";

                List<string> oznakePolja = new List<string>();
                List<string> predmetiUcionice = new List<string>(); //sadrzi duplikate
                PotvrdaIzmene potvrda = new PotvrdaIzmene();
                potvrda.Title = "Postoje predmeti";

                string tekstUpozorenja = potvrda.porukaUpozorenja.Text;
                if (tabelaPredmeta.SelectedItems.Count == 1)
                    potvrda.porukaUpozorenja.Text = tekstUpozorenja + " izabrani predmet!";
                else
                    potvrda.porukaUpozorenja.Text = tekstUpozorenja + " izabrane predmete!";
                
                List<Predmet> removedItems = new List<Predmet>();
                foreach (object o in tabelaPredmeta.SelectedItems)
                {
                    // u staro stanje smestamo oznaku predmeta koji brisemo, zbog generisanja poruke u undo/redo operaciji
                    staroStanje.Oznake.Add(((Predmet)o).Oznaka);

                    int index = tabelaPredmeta.Items.IndexOf(o);
                    DataGridRow selektovaniRed = (DataGridRow)tabelaPredmeta.ItemContainerGenerator.ContainerFromIndex(index);
                    TextBlock content = tabelaPredmeta.Columns[1].GetCellContent(selektovaniRed) as TextBlock;
                    string oznakaPredmeta = content.Text;

                    foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)  //za svako polje inace
                    {
                        if (polje.NazivPolja.Split('-')[0].Trim() == oznakaPredmeta)       //ako je u taj predmet
                        {
                            oznakePolja.Add(polje.Id);
                            predmetiUcionice.Add(polje.NazivPolja.Split('-')[0].Trim());    //dodam ga u polja i u predmete
                        }
                    }
                    removedItems.Add(racunarskiCentar.Predmeti[oznakaPredmeta]);
                }
                List<string> predmetiUcioniceBezDupl = predmetiUcionice.Distinct().ToList();
                if (oznakePolja.Count > 0)
                {
                    potvrda.PorukaBrisanja.Text = "\nPredmeti:\n";
                    for (int i = 0; i < predmetiUcioniceBezDupl.Count; i++)
                    {
                        potvrda.PorukaBrisanja.Text += "\n" + (i + 1) + ". " + predmetiUcioniceBezDupl[i];
                    }
                    potvrda.ShowDialog();
                    if (potvrda.daKlik)
                    {
                        foreach (string id in oznakePolja)
                            racunarskiCentar.PoljaKalendara.Remove(id);
                        foreach (string poz in predmetiUcionice)
                            racunarskiCentar.Predmeti[poz].PreostaliTermini++;

                        foreach (Predmet predmet in removedItems)
                        {
                            racunarskiCentar.Smerovi[racunarskiCentar.Predmeti[predmet.Oznaka].Smer].Predmeti.Remove(predmet.Oznaka);
                            racunarskiCentar.Predmeti[predmet.Oznaka].Obrisan = true;
                            predmetiKolekcija.Remove(predmet);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                foreach (Predmet predmet in removedItems)
                {
                    racunarskiCentar.Smerovi[racunarskiCentar.Predmeti[predmet.Oznaka].Smer].Predmeti.Remove(predmet.Oznaka);
                    racunarskiCentar.Predmeti[predmet.Oznaka].Obrisan = true;
                    predmetiKolekcija.Remove(predmet);
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierSucces.ShowSuccess("Brisanje predmeta uspešno izvršeno!");
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

                // omogucavamo pozivanje opcije undo
                omoguciUndo();
                ponistiOnemoguciRedo();
            }
            else
                return;
        }

        private void obrisiSoftverClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSoftvera.SelectedIndex != -1)
            {
                // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                StanjeAplikacije staroStanje = new StanjeAplikacije();
                staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                staroStanje.TipPodataka = "softver";
                staroStanje.Kolicina = tabelaSoftvera.SelectedItems.Count;
                staroStanje.TipPromene = "dodavanje";

                List<Softver> removedItems = new List<Softver>();
                foreach (object o in tabelaSoftvera.SelectedItems)
                {
                    // u staro stanje smestamo oznaku softvera koji brisemo, zbog generisanja poruke u undo/redo operaciji
                    staroStanje.Oznake.Add(((Softver)o).Oznaka);

                    int index = tabelaSoftvera.Items.IndexOf(o);
                    DataGridRow selektovaniRed = (DataGridRow)tabelaSoftvera.ItemContainerGenerator.ContainerFromIndex(index);
                    TextBlock content = tabelaSoftvera.Columns[1].GetCellContent(selektovaniRed) as TextBlock;
                    string oznakaSoftvera = content.Text;

                    //provera da li se nalazi u nekom predmetu, ako se nalazi, sprecava se brisanje
                    bool koristiSeUPredmetu = false;
                    foreach (Predmet p in racunarskiCentar.Predmeti.Values)
                    {
                        if (!p.Obrisan && p.Softveri.Contains(oznakaSoftvera))
                            koristiSeUPredmetu = true;
                    }
                    if (koristiSeUPredmetu)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            notifierError.ShowError("Ne možete obrisati softver " + oznakaSoftvera + ", jer ga koristi neki od predmeta!");
                        });
                        continue;
                    }

                    //provera da li se nalazi u nekoj ucionici, ako se nalazi, sprecava se brisanje
                    bool koristiSeUucionici = false;
                    foreach (Ucionica u in racunarskiCentar.Ucionice.Values)
                    {
                        if (!u.Obrisan && u.InstaliraniSoftveri.Contains(oznakaSoftvera))
                            koristiSeUucionici = true;
                    }
                    if (koristiSeUucionici)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            notifierError.ShowError("Ne možete obrisati softver " + oznakaSoftvera + ", jer se koristi u nekoj od učionica!");
                        });
                        continue;
                    }

                    removedItems.Add(racunarskiCentar.Softveri[oznakaSoftvera]);
                    racunarskiCentar.Softveri[oznakaSoftvera].Obrisan = true;
                    // za svako logicko brisanje softvera se smanjuje broj logicki aktivnih
                    brojAktivnihSoftvera--;
                }

                foreach (Softver softver in removedItems)
                    softveriKolekcija.Remove(softver);
                if(removedItems.Count != 0)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        notifierSucces.ShowSuccess("Brisanje softvera uspešno izvršeno!");
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

                    // omogucavamo pozivanje opcije undo
                    omoguciUndo();
                    ponistiOnemoguciRedo();
                }
            }
            else
                return;
        }

        private void obrisiUcionicuClick(object sender, RoutedEventArgs e)
        {
            if (tabelaUcionica.SelectedIndex != -1)
            {
                // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                StanjeAplikacije staroStanje = new StanjeAplikacije();
                staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                staroStanje.TipPodataka = "ucionica";
                staroStanje.Kolicina = tabelaUcionica.SelectedItems.Count;
                staroStanje.TipPromene = "dodavanje";

                List<Ucionica> removedItems = new List<Ucionica>();
                List<string> oznakePolja = new List<string>();
                List<string> predmetiUcionice = new List<string>(); //sadrzi duplikate
                PotvrdaIzmene potvrda = new PotvrdaIzmene();
                potvrda.Title = "Postoje predmeti";

                foreach (object o in tabelaUcionica.SelectedItems)  //za svaku ucionicu
                {
                    // u staro stanje smestamo oznaku ucionice koju brisemo, zbog generisanja poruke u undo/redo operaciji
                    staroStanje.Oznake.Add(((Ucionica)o).Oznaka);

                    int index = tabelaUcionica.Items.IndexOf(o);
                    DataGridRow selektovaniRed = (DataGridRow)tabelaUcionica.ItemContainerGenerator.ContainerFromIndex(index);
                    TextBlock content = tabelaUcionica.Columns[0].GetCellContent(selektovaniRed) as TextBlock;
                    string oznakaUcionice = content.Text;

                    foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)  //za svako polje inace
                    {
                        if (polje.Ucionica == oznakaUcionice)       //ako je u toj ucionici
                        {
                            oznakePolja.Add(polje.Id);
                            predmetiUcionice.Add(polje.NazivPolja.Split('-')[0].Trim());    //dodam ga u polja i u predmete
                        }
                    }
                    removedItems.Add(racunarskiCentar.Ucionice[oznakaUcionice]);
                }
                List<string> predmetiUcioniceBezDupl = predmetiUcionice.Distinct().ToList();

                string tekstUpozorenja = potvrda.porukaUpozorenja.Text;
                if (predmetiUcioniceBezDupl.Count == 1)
                    potvrda.porukaUpozorenja.Text = tekstUpozorenja + " predmet u izabranoj učionici!";
                else
                    potvrda.porukaUpozorenja.Text = tekstUpozorenja + " predmete u izabranoj učionici!";

                if (oznakePolja.Count > 0)
                {
                    potvrda.PorukaBrisanja.Text = "\nPredmeti:\n";
                    for(int i = 0; i < predmetiUcioniceBezDupl.Count; i++)
                    {
                        potvrda.PorukaBrisanja.Text += "\n" + (i + 1) + ". " + predmetiUcioniceBezDupl[i];
                    }
                    potvrda.ShowDialog();
                    if (potvrda.daKlik)
                    {
                        foreach (string id in oznakePolja)
                            racunarskiCentar.PoljaKalendara.Remove(id);
                        foreach (string poz in predmetiUcionice)
                            racunarskiCentar.Predmeti[poz].PreostaliTermini++;

                        foreach (Ucionica ucionica in removedItems)
                        {
                            racunarskiCentar.Ucionice[ucionica.Oznaka].Obrisan = true;
                            ucioniceKolekcija.Remove(ucionica);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                foreach (Ucionica ucionica in removedItems)
                {
                    racunarskiCentar.Ucionice[ucionica.Oznaka].Obrisan = true;
                    ucioniceKolekcija.Remove(ucionica);
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if(removedItems.Count == 1)
                        notifierSucces.ShowSuccess("Brisanje učionice uspešno izvršeno!");
                    else
                        notifierSucces.ShowSuccess("Brisanje učionica uspešno izvršeno!");
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

                // omogucavamo pozivanje opcije undo
                omoguciUndo();
                ponistiOnemoguciRedo();
            }
            else
                return;
        }

        public void obrisiSmerClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSmerova.SelectedIndex != -1)
            {
                // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
                StanjeAplikacije staroStanje = new StanjeAplikacije();
                staroStanje.RacunarskiCentar = DeepClone(racunarskiCentar);
                staroStanje.TipPodataka = "smer";
                staroStanje.Kolicina = tabelaSmerova.SelectedItems.Count;
                staroStanje.TipPromene = "dodavanje";

                List<string> oznakePolja = new List<string>();
                List<string> predmetiUcionice = new List<string>(); //sadrzi duplikate
                PotvrdaIzmene potvrda = new PotvrdaIzmene();
                potvrda.Title = "Postoje predmeti";
                potvrda.PorukaBrisanja.Text = "Da li ste sigurni?\n\nPostoje predmeti u rasporedu. \nUkoliko potvrdite brisanje, uklonicete predmete iz rasporeda.\n";

                List<Smer> removedItems = new List<Smer>();
                foreach (object o in tabelaSmerova.SelectedItems)
                {
                    // u staro stanje smestamo oznaku smera koji brisemo, zbog generisanja poruke u undo/redo operaciji
                    staroStanje.Oznake.Add(((Smer)o).Oznaka);

                    int index = tabelaSmerova.Items.IndexOf(o);
                    DataGridRow selektovaniRed = (DataGridRow)tabelaSmerova.ItemContainerGenerator.ContainerFromIndex(index);
                    TextBlock content = tabelaSmerova.Columns[1].GetCellContent(selektovaniRed) as TextBlock;
                    string oznakaSmera = content.Text;

                    foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)  //za svako polje inace
                    {
                        if (polje.NazivPolja.Split('-')[1].Trim() == oznakaSmera)       //ako je u toj ucionici
                        {
                            oznakePolja.Add(polje.Id);
                            predmetiUcionice.Add(polje.NazivPolja.Split('-')[0].Trim());    //dodam ga u polja i u predmete
                        }
                    }

                    removedItems.Add(racunarskiCentar.Smerovi[oznakaSmera]);
                    
                    // za svako logicko brisanje smera se smanjuje broj logicki aktivnih
                    brojAktivnihSmerova--;
                }
                List<string> predmetiUcioniceBezDupl = predmetiUcionice.Distinct().ToList();
                if (oznakePolja.Count > 0)
                {
                    for (int i = 0; i < predmetiUcioniceBezDupl.Count; i++)
                    {
                        potvrda.PorukaBrisanja.Text += "\n" + (i + 1) + ". " + predmetiUcioniceBezDupl[i];
                    }
                    potvrda.ShowDialog();
                    if (potvrda.daKlik)
                    {
                        foreach (string id in oznakePolja)
                            racunarskiCentar.PoljaKalendara.Remove(id);
                        foreach (string poz in predmetiUcionice)
                            racunarskiCentar.Predmeti[poz].PreostaliTermini++;
                    }
                    else
                        return;
                }
                foreach (Smer smer in removedItems)
                {//provera da li se nalazi u nekom predmetu, ako se nalazi, brise se i taj predmet kom pripada
                    if (racunarskiCentar.Smerovi[smer.Oznaka].Predmeti.Count > 0)
                    {
                        foreach (string predmetOznaka in racunarskiCentar.Smerovi[smer.Oznaka].Predmeti)
                        {
                            racunarskiCentar.Predmeti[predmetOznaka].Obrisan = true;
                            predmetiKolekcija.Remove(racunarskiCentar.Predmeti[predmetOznaka]);
                        }
                    }
                    racunarskiCentar.Smerovi[smer.Oznaka].Obrisan = true;
                    smeroviKolekcija.Remove(smer);
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (removedItems.Count == 1)
                        notifierSucces.ShowSuccess("Brisanje smera uspešno izvršeno!");
                    else
                        notifierSucces.ShowSuccess("Brisanje smerova uspešno izvršeno!");
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

                // omogucavamo pozivanje opcije undo
                omoguciUndo();
                ponistiOnemoguciRedo();
            }
            else
                return;
        }

        private void tabelaSoftveraIzgubilaFokus(object sender, EventArgs e)
        {
            MenuItemIzmeni.IsEnabled = false;
            MenuItemObrisi.IsEnabled = false;
        }

        private void tabelaSmerovaIzgubilaFokus(object sender, EventArgs e)
        {
            MenuItemIzmeni.IsEnabled = false;
            MenuItemObrisi.IsEnabled = false;
        }

        private void tabelaPredmetaIzgubilaFokus(object sender, EventArgs e)
        {
            MenuItemIzmeni.IsEnabled = false;
            MenuItemObrisi.IsEnabled = false;
        }

        private void tabelaUcionicaIzgubilaFokus(object sender, EventArgs e)
        {
            MenuItemIzmeni.IsEnabled = false;
            MenuItemObrisi.IsEnabled = false;
        }

        private void tabelaSmerovaDobilaFokus(object sender, EventArgs e)
        {
            detaljanPrikazSmer.Visibility = Visibility.Visible;
            MenuItemIzmeni.IsEnabled = true;
            MenuItemObrisi.IsEnabled = true;
        }

        private void tabelaSoftveraDobilaFokus(object sender, EventArgs e)
        {
            detaljanPrikazSoftver.Visibility = Visibility.Visible;
            MenuItemIzmeni.IsEnabled = true;
            MenuItemObrisi.IsEnabled = true;
        }

        private void tabelaPredmetaDobilaFokus(object sender, EventArgs e)
        {
            detaljanPrikazPredmet.Visibility = Visibility.Visible;
            MenuItemIzmeni.IsEnabled = true;
            MenuItemObrisi.IsEnabled = true;
        }

        private void tabelaUcionicaDobilaFokus(object sender, EventArgs e)
        {
            detaljanPrikazUcionica.Visibility = Visibility.Visible;
            MenuItemIzmeni.IsEnabled = true;
            MenuItemObrisi.IsEnabled = true;
        }

        private void SerijalizacijaPodataka(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(imeFajla, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            DataContractSerializer serializer = new DataContractSerializer(typeof(RacunarskiCentar));

            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                serializer.WriteObject(writer, racunarskiCentar);

                writer.Flush();
                writer.Close();
            }

            sw.Close();
            fs.Close();
        }

        private void DeserijalizacijaPodataka()
        {
            FileStream fs = null;
            if (File.Exists(imeFajla))
            {
                fs = new FileStream(imeFajla, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);

                DataContractSerializer serializer = new DataContractSerializer(typeof(RacunarskiCentar));

                using (XmlTextReader reader = new XmlTextReader(sr))
                {
                    racunarskiCentar = (RacunarskiCentar)serializer.ReadObject(reader);
                    reader.Close();
                }

                sr.Close();
                fs.Close();
                Console.WriteLine("Deserijalizacija uspesno izvrsena!\n");
            }
        }

        public void omoguciUndo()
        {
            MenuItemUndo.IsEnabled = true;
            MenuItemUndoPicture.IsEnabled = true;
            MenuItemUndoPictureImg.Source = new BitmapImage(new Uri(@"/picture/undo.png", UriKind.Relative));
        }

        private void ponistiOnemoguciRedo()
        {
            // redo stek praznimo, kao i kolekciju sledecih mogucih stanja jer smo sad uneli novu izmenu
            // koja prekida lanac undo-redo
            stekStanja.GetRedo().Clear();
            sledecaStanjaAplikacije.Clear();
            MenuItemRedo.IsEnabled = true;
            MenuItemRedoPicture.IsEnabled = true;
            MenuItemRedoPictureImg.Source = new BitmapImage(new Uri(@"/picture/redo1.png", UriKind.Relative));
        }

        private void tabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                cef.posaljiPodatke();
                MenuItemPretraga.IsEnabled = false;
                MenuItemIzborFiltera.IsEnabled = false;
            }
            else if (tabControl.SelectedIndex == 1 || tabControl.SelectedIndex == 2 || tabControl.SelectedIndex == 3 || tabControl.SelectedIndex == 4)
            {
                MenuItemIzborFiltera.IsEnabled = true;
                MenuItemPretraga.IsEnabled = true;
            }
        }

        public Notifier Notifier
        {
            get;
            set;
        }

        private void pokreniTutorijal(object sender, EventArgs e)
        {
            notifierError.ShowInformation("pokrenut tutorijal!");
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

        private void azurirajKolekcije()
        {
            predmetiKolekcija = new ObservableCollection<Predmet>();
            foreach (Predmet p in racunarskiCentar.Predmeti.Values)
            {
                if (!p.Obrisan)
                    predmetiKolekcija.Add(p);
            }
            tabelaPredmeta.ItemsSource = predmetiKolekcija;

            softveriKolekcija = new ObservableCollection<Softver>();
            foreach (Softver s in racunarskiCentar.Softveri.Values)
            {
                if (!s.Obrisan)
                    softveriKolekcija.Add(s);
            }
            tabelaSoftvera.ItemsSource = softveriKolekcija;

            smeroviKolekcija = new ObservableCollection<Smer>();
            foreach (Smer s in racunarskiCentar.Smerovi.Values)
            {
                if (!s.Obrisan)
                    smeroviKolekcija.Add(s);
            }
            tabelaSmerova.ItemsSource = smeroviKolekcija;

            ucioniceKolekcija = new ObservableCollection<Ucionica>();
            foreach (Ucionica u in racunarskiCentar.Ucionice.Values)
            {
                if (!u.Obrisan)
                    ucioniceKolekcija.Add(u);
            }
            tabelaUcionica.ItemsSource = ucioniceKolekcija;
        }

        private void sacuvajKalendar(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Xml file (*.xml)|*.xml";

            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                serijalizacijaPosleSave(saveFileDialog.FileName);
                notifierSucces.ShowSuccess("Uspesno ste zapamtili raspored!  ");
            }

        }

        private void serijalizacijaPosleSave(string imeKorisnikovogFajla)
        {
            FileStream fs = new FileStream(imeKorisnikovogFajla, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            DataContractSerializer serializer = new DataContractSerializer(typeof(RacunarskiCentar));

            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                serializer.WriteObject(writer, racunarskiCentar);

                writer.Flush();
                writer.Close();
            }
            sw.Close();
            fs.Close();
        }

        private void otvoriKalendar(object sender, RoutedEventArgs e)
        {
            MenuItemRedo.IsEnabled = false;
            MenuItemUndo.IsEnabled = false;
            MenuItemUndoPicture.IsEnabled = false;
            MenuItemUndoPictureImg.Source = new BitmapImage(new Uri(@"/picture/undo1.png", UriKind.Relative));
            MenuItemRedoPicture.IsEnabled = false;
            MenuItemRedoPictureImg.Source = new BitmapImage(new Uri(@"/picture/redo1.png", UriKind.Relative));
            stekStanja.GetUndo().Clear();
            stekStanja.GetRedo().Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            Nullable<bool> result =  openFileDialog.ShowDialog();
            if(result == true)
            {
                serijalizacijaPosleSave(imeFajla);
                imeFajla = openFileDialog.FileName;
                DeserijalizacijaPodataka();
                inicijalizujPodatke();
                cef.RacunarskiCentar = racunarskiCentar;
                cef.posaljiPodatke();
            }
        }

        private void otvoriHelp(object sender, RoutedEventArgs e)
        {
            IInputElement focusedControl = FocusManager.GetFocusedElement(Application.Current.Windows[0]);
            if (focusedControl is DependencyObject)
            {
                string str = HelpProvider.GetHelpKey((DependencyObject)focusedControl);
                HelpProvider.ShowHelp("mainWindow", this);
            }
        }
    }
}
