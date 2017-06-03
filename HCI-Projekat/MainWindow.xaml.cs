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

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RacunarskiCentar racunarskiCentar;
        private ObservableCollection<Predmet> predmetiKolekcija;
        ObservableCollection<Softver> softveriKolekcija;
        ObservableCollection<Smer> smeroviKolekcija;
        ObservableCollection<Ucionica> ucioniceKolekcija;
        private static string imeFajla = "racunarskiCentar.xml";
        public ChromiumWebBrowser chromeBrowser;
        CefCustomObject cef;

        public MainWindow()
        {
            InitializeComponent();
            KalendarTab.Focus();

            racunarskiCentar = new RacunarskiCentar();
            DeserijalizacijaPodataka();

            predmetiKolekcija = new ObservableCollection<Predmet>();
            foreach (Predmet p in racunarskiCentar.Predmeti.Values)
            {
                if (!p.Obrisan)
                    predmetiKolekcija.Add(p);
            }
            tabelaPredmeta.ItemsSource = predmetiKolekcija;
            tabelaPredmeta.IsSynchronizedWithCurrentItem = true;
            tabelaPredmeta.IsReadOnly = true;
            tabelaPredmeta.UnselectAll();
            detaljanPrikazPredmet.Visibility = Visibility.Collapsed;

            softveriKolekcija = new ObservableCollection<Softver>();
            foreach(Softver s in racunarskiCentar.Softveri.Values)
            {
                if (!s.Obrisan)
                    softveriKolekcija.Add(s);
            }
            tabelaSoftvera.ItemsSource = softveriKolekcija;
            tabelaSoftvera.IsSynchronizedWithCurrentItem = true;
            tabelaSoftvera.IsReadOnly = true;
            tabelaSoftvera.UnselectAll();
            detaljanPrikazSoftver.Visibility = Visibility.Hidden;
            
            smeroviKolekcija = new ObservableCollection<Smer>();
            foreach(Smer s in racunarskiCentar.Smerovi.Values)
            {
                if (!s.Obrisan)
                    smeroviKolekcija.Add(s);
            }
            tabelaSmerova.ItemsSource = smeroviKolekcija;
            tabelaSmerova.IsSynchronizedWithCurrentItem = true;
            tabelaSmerova.IsReadOnly = true;
            tabelaSmerova.UnselectAll();
            detaljanPrikazSmer.Visibility = Visibility.Hidden;

            ucioniceKolekcija = new ObservableCollection<Ucionica>();
            foreach(Ucionica u in racunarskiCentar.Ucionice.Values)
            {
                if (!u.Obrisan)
                    ucioniceKolekcija.Add(u);
            }
            tabelaUcionica.ItemsSource = ucioniceKolekcija;
            tabelaUcionica.IsSynchronizedWithCurrentItem = true;
            tabelaUcionica.IsReadOnly = true;
            tabelaUcionica.UnselectAll();
            detaljanPrikazUcionica.Visibility = Visibility.Hidden;

            InitializeChromium();
            cef = new CefCustomObject(chromeBrowser, this, racunarskiCentar);
            chromeBrowser.RegisterJsObject("cefCustomObject", cef);
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

        private void dodavanjeUcioniceClick(object sender, RoutedEventArgs e)
        {
            if (racunarskiCentar.Softveri.Count > 0)
            {
                if (tabControl.SelectedIndex != 1)
                    tabControl.SelectedIndex = 1;
                var ucionicaWindow = new DodavanjeUcionice(racunarskiCentar, ucioniceKolekcija, false);
                ucionicaWindow.ShowDialog();
            }
            else
                MessageBox.Show("Ne možete uneti učionicu dok god ne unesete bar jedan softver!");
        }

        private void dodavanjePredmetaClick(object sender, RoutedEventArgs e)
        {
            if (racunarskiCentar.Smerovi.Count > 0 && racunarskiCentar.Softveri.Count > 0)
            {
                if (tabControl.SelectedIndex != 2)
                    tabControl.SelectedIndex = 2;
                var predmetWindow = new DodavanjePredmeta(racunarskiCentar, predmetiKolekcija, false);
                predmetWindow.ShowDialog();
            }
            else if (racunarskiCentar.Smerovi.Count == 0 && racunarskiCentar.Softveri.Count == 0)
                MessageBox.Show("Ne možete uneti predmet dok god ne unesete bar jedan smer i bar jedan softver!");
            else if (racunarskiCentar.Smerovi.Count == 0)
                MessageBox.Show("Ne možete uneti predmet dok god ne unesete bar jedan smer!");
            else if (racunarskiCentar.Softveri.Count == 0)
                MessageBox.Show("Ne možete uneti predmet dok god ne unesete bar jedan softver!");
        }

        private void dodavanjeSmeraClick(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 3)
                tabControl.SelectedIndex = 3;
            var smerWindow = new DodavanjeSmera(racunarskiCentar, smeroviKolekcija, false);
            smerWindow.ShowDialog();
        }

        private void dodavanjeSoftveraClick(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != 4)
                tabControl.SelectedIndex = 4;
            var softverWindow = new DodavanjeSoftvera(racunarskiCentar, softveriKolekcija, false);
            softverWindow.ShowDialog();
        }

        private void pregledKalendaraClick(object sender, RoutedEventArgs e)
        {
            if(tabControl.SelectedIndex != 0)
                tabControl.SelectedIndex = 0;
        }

        private void pregledUcionicaClick(object sender, RoutedEventArgs e)
        {
            if(tabControl.SelectedIndex != 1)
                tabControl.SelectedIndex = 1;
        }

        private void pregledPredmetaClick(object sender, RoutedEventArgs e)
        {
            if(tabControl.SelectedIndex != 2)
                tabControl.SelectedIndex = 2;
        }

        private void pregledSmerovaClick(object sender, RoutedEventArgs e)
        {
            if(tabControl.SelectedIndex != 3)
                tabControl.SelectedIndex = 3;
        }

        private void pregledSoftveraClick(object sender, RoutedEventArgs e)
        {
            if(tabControl.SelectedIndex != 4)
                tabControl.SelectedIndex = 4;
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Snimi");
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            mainWindow.Close();
        }

        private void undoClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Undo");
        }

        private void redoClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Redo");
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

        private void pretraziSmer(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
        }

        private void pretraziSoftver(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
        }

        private void pretraziPredmet(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
        }

        private void pretraziUcionicu(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
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
                    else if (parametar == "Projektor")
                        return (u.ProjektorString.ToUpper().StartsWith(filter.ToUpper()));
                    else if (parametar == "Tabla")
                        return (u.TablaString.ToUpper().StartsWith(filter.ToUpper()));
                    else if (parametar == "Pametna tabla")
                        return (u.PametnaTablaString.ToUpper().StartsWith(filter.ToUpper()));
                    else if (parametar == "Operativni sistem")
                        return (u.OperativniSistem.ToUpper().StartsWith(filter.ToUpper()));
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
                    else if (parametar == "Broj termina")
                        return (p.BrTermina.ToString().StartsWith(filter));
                    else if (parametar == "Projektor")
                        return (p.ProjektorString.ToUpper().StartsWith(filter.ToUpper()));
                    else if (parametar == "Tabla")
                        return (p.TablaString.ToUpper().StartsWith(filter.ToUpper()));
                    else if (parametar == "Pametna tabla")
                        return (p.PametnaTablaString.ToUpper().StartsWith(filter.ToUpper()));
                    else
                        return (p.OperativniSistem.ToUpper().StartsWith(filter.ToUpper()));
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
                    else if (parametar == "Operativni sistem")
                        return (s.OperativniSistem.ToUpper().StartsWith(filter.ToUpper()));
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
                    else if (parametar == "Datum uvođenja")
                    {
                        MessageBox.Show(s.Datum.ToString());
                        return (s.Datum.ToString().StartsWith(filter));
                    }
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
                if(brisanjeProzor.daKlik)
                    obrisiUcionicuClick(sender, e);
            }
            // trenutno smo u tabu za predmete
            else if (tabControl.SelectedIndex == 2)
            {
                if (tabelaPredmeta.SelectedItems.Count > 1)
                    brisanjeProzor.PorukaBrisanja.Text = "Da li ste sigurni da želite da \nobrišete " + tabelaPredmeta.SelectedItems.Count + " izabrana predmeta?";
                brisanjeProzor.ShowDialog();
                if(brisanjeProzor.daKlik)
                    obrisiPredmetClick(sender, e);
            }
            // trenutno smo u tabu za smerove
            else if (tabControl.SelectedIndex == 3)
            {
                if (tabelaSmerova.SelectedItems.Count > 1)
                    brisanjeProzor.PorukaBrisanja.Text = "Da li ste sigurni da želite da \nobrišete " + tabelaSmerova.SelectedItems.Count + " izabrana smera?";
                brisanjeProzor.ShowDialog();
                if(brisanjeProzor.daKlik)
                    obrisiSmerClick(sender, e);
            }
            // trenutno smo u tabu za softvere
            else if (tabControl.SelectedIndex == 4)
            {
                if (tabelaSoftvera.SelectedItems.Count > 1)
                    brisanjeProzor.PorukaBrisanja.Text = "Da li ste sigurni da želite da \nobrišete " + tabelaSoftvera.SelectedItems.Count + " izabrana softvera?";
                brisanjeProzor.ShowDialog();
                if(brisanjeProzor.daKlik)
                    obrisiSoftverClick(sender, e);
            }
        }

        private void izmeniElement(object sender, RoutedEventArgs e)
        {
            // trenutno smo u tabu za ucionice
            if (tabControl.SelectedIndex == 1)
            {
                if (tabelaUcionica.SelectedItems.Count > 1)
                    izmeniUcioniceClick(sender, e);
                else
                    izmeniUcionicuClick(sender, e);
            }
            // trenutno smo u tabu za predmete
            else if (tabControl.SelectedIndex == 2)
            {
                if (tabelaPredmeta.SelectedItems.Count > 1)
                    izmeniPredmeteClick(sender, e);
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

        private void izmeniUcioniceClick(object sender, RoutedEventArgs e)
        {
            var izmenaUcionica = new IzmenaUcionica();
            if (izmenaUcionica.potvrdaIzmena)
            {

            }
        }

        private void izmeniPredmeteClick(object sender, RoutedEventArgs e)
        {
            var izmenaPredmeta = new IzmenaPredmeta();
            if (izmenaPredmeta.potvrdaIzmena)
            {

            }
        }

        private void izmeniSmeroveClick(object sender, RoutedEventArgs e)
        {
            var izmenaSmerova = new IzmenaSmerova();
            if(izmenaSmerova.potvrdaIzmena)
            {

            }
        }

        private void izmeniSoftvereClick(object sender, RoutedEventArgs e)
        {
            var izmenaSoftvera = new IzmenaSoftvera();
            if (izmenaSoftvera.potvrdaIzmena)
            {

            }
        }

        private void izmeniPredmetClick(object sender, RoutedEventArgs e)
        {
            if (tabelaPredmeta.SelectedIndex != -1)
            {
                var predmetWindow = new DodavanjePredmeta(racunarskiCentar, predmetiKolekcija, true);
                Predmet pre = (Predmet)tabelaPredmeta.SelectedItem;
                predmetWindow.NazivPredmeta.Text = pre.Naziv;
                predmetWindow.NazivPredmeta.Focus();
                predmetWindow.OznakaPredmeta.Text = pre.Oznaka;
                predmetWindow.OznakaPredmeta.IsEnabled = false;
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
                
                for(int i = 0; i < predmetWindow.softverTabela.Items.Count; i++) // isto i za softvere
                {
                    Softver softver = (Softver)predmetWindow.softverTabela.Items[i];
                    if (pre.Softveri.IndexOf(softver.Oznaka) != -1)
                        softver.Instaliran = true;
                    else
                        softver.Instaliran = false;
                }

                predmetWindow.indeks = tabelaPredmeta.SelectedIndex;
                predmetWindow.ShowDialog();
                tabelaPredmeta.Items.Refresh();
            }
            else
                return;
        }

        private void izmeniSoftverClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSoftvera.SelectedIndex != -1)
            {
                var softverWindow = new DodavanjeSoftvera(racunarskiCentar, softveriKolekcija, true);
                Softver red = (Softver)tabelaSoftvera.SelectedItem;
                softverWindow.nazivSoftver.Text = red.Naziv;
                softverWindow.nazivSoftver.Focus();
                softverWindow.proizvodjacSoftver.Text = red.Proizvodjac;
                softverWindow.sajtSoftver.Text = red.Sajt;
                softverWindow.godinaSoftver.Text = red.GodIzdavanja.ToString();
                softverWindow.cenaSoftver.Text = red.Cena.ToString();
                softverWindow.opisSoftver.Text = red.Opis;
                softverWindow.oznakaSoftver.Text = red.Oznaka;
                softverWindow.oznakaSoftver.IsEnabled = false;

                if (red.OperativniSistem.Equals("Windows"))
                    softverWindow.WindowsOSSoftver.IsChecked = true;
                else if (red.OperativniSistem.Equals("Linux"))
                    softverWindow.LinuxOSSoftver.IsChecked = true;
                else if (red.OperativniSistem.Equals("Windows i Linux"))
                    softverWindow.WindowsAndLinuxOSSoftver.IsChecked = true;

                softverWindow.indeks = tabelaSoftvera.SelectedIndex;
                softverWindow.ShowDialog();
                tabelaSoftvera.Items.Refresh();
            }
            else
                return;
        }

        private void izmeniUcionicuClick(object sender, RoutedEventArgs e)
        {
            if (tabelaUcionica.SelectedIndex != -1)
            {
                var ucionicaWindow = new DodavanjeUcionice(racunarskiCentar, ucioniceKolekcija, true);
                Ucionica red = (Ucionica)tabelaUcionica.SelectedItem;
                ucionicaWindow.oznakaUcionica.Text = red.Oznaka;
                ucionicaWindow.oznakaUcionica.IsEnabled = false;
                ucionicaWindow.brojRadnihMestaUcionica.Text = red.BrojRadnihMesta.ToString();
                ucionicaWindow.brojRadnihMestaUcionica.Focus();
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
                tabelaUcionica.Items.Refresh();
            }
            else
                return;
        }

        private void izmeniSmerClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSmerova.SelectedIndex != -1)
            {
                var smerWindow = new DodavanjeSmera(racunarskiCentar, smeroviKolekcija, true);
                Smer row = (Smer)tabelaSmerova.SelectedItem;
                smerWindow.NazivSmera.Text = row.Naziv;
                smerWindow.NazivSmera.Focus();
                smerWindow.OznakaSmera.Text = row.Oznaka;
                smerWindow.OznakaSmera.IsEnabled = false;
                smerWindow.OpisSmera.Text = row.Opis;
                smerWindow.DatumUvodjenja.Text = row.Datum.ToString();

                smerWindow.indeks = tabelaSmerova.SelectedIndex;
                smerWindow.ShowDialog();
                tabelaSmerova.Items.Refresh();
            }
            else
                return;
        }

        private void obrisiPredmetClick(object sender, RoutedEventArgs e)
        {
            if (tabelaPredmeta.SelectedIndex != -1)
            {
                List<Predmet> removedItems = new List<Predmet>();
                foreach (object o in tabelaPredmeta.SelectedItems)
                {
                    int index = tabelaPredmeta.Items.IndexOf(o);
                    DataGridRow selektovaniRed = (DataGridRow)tabelaPredmeta.ItemContainerGenerator.ContainerFromIndex(index);
                    TextBlock content = tabelaPredmeta.Columns[1].GetCellContent(selektovaniRed) as TextBlock;
                    string oznakaPredmeta = content.Text;

                    racunarskiCentar.Smerovi[racunarskiCentar.Predmeti[oznakaPredmeta].Smer].Predmeti.Remove(oznakaPredmeta);
                    removedItems.Add(racunarskiCentar.Predmeti[oznakaPredmeta]);
                    racunarskiCentar.Predmeti[oznakaPredmeta].Obrisan = true;
                }

                foreach (Predmet predmet in removedItems)
                    predmetiKolekcija.Remove(predmet);
            }
            else
                return;
        }

        private void obrisiSoftverClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSoftvera.SelectedIndex != -1)
            {
                List<Softver> removedItems = new List<Softver>();
                foreach (object o in tabelaSoftvera.SelectedItems)
                {
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
                        MessageBox.Show("Ne možete obrisati softver " + oznakaSoftvera + ", jer ga koristi neki od predmeta!");
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
                        MessageBox.Show("Ne možete obrisati softver " + oznakaSoftvera + ", jer se koristi u nekoj od učionica!");
                        continue;
                    }

                    removedItems.Add(racunarskiCentar.Softveri[oznakaSoftvera]);
                    racunarskiCentar.Softveri[oznakaSoftvera].Obrisan = true;
                }

                foreach (Softver softver in removedItems)
                    softveriKolekcija.Remove(softver);
            }
            else
                return;
        }

        private void obrisiUcionicuClick(object sender, RoutedEventArgs e)
        {
            if (tabelaUcionica.SelectedIndex != -1)
            {
                List<Ucionica> removedItems = new List<Ucionica>();
                foreach (object o in tabelaUcionica.SelectedItems)
                {
                    int index = tabelaUcionica.Items.IndexOf(o);
                    DataGridRow selektovaniRed = (DataGridRow)tabelaUcionica.ItemContainerGenerator.ContainerFromIndex(index);
                    TextBlock content = tabelaUcionica.Columns[0].GetCellContent(selektovaniRed) as TextBlock;
                    string oznakaUcionice = content.Text;

                    removedItems.Add(racunarskiCentar.Ucionice[oznakaUcionice]);
                    racunarskiCentar.Ucionice[oznakaUcionice].Obrisan = true;
                }

                foreach (Ucionica ucionica in removedItems)
                    ucioniceKolekcija.Remove(ucionica);
            }
            else
                return;
        }

        public void obrisiSmerClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSmerova.SelectedIndex != -1) {
                List<Smer> removedItems = new List<Smer>();
                foreach (object o in tabelaSmerova.SelectedItems) {
                    int index = tabelaSmerova.Items.IndexOf(o);
                    DataGridRow selektovaniRed = (DataGridRow)tabelaSmerova.ItemContainerGenerator.ContainerFromIndex(index);
                    TextBlock content = tabelaSmerova.Columns[1].GetCellContent(selektovaniRed) as TextBlock;
                    string oznakaSmera = content.Text;

                    //provera da li se nalazi u nekom predmetu, ako se nalazi, brise se i taj predmet kom pripada
                    if (racunarskiCentar.Smerovi[oznakaSmera].Predmeti.Count > 0)
                    {
                        foreach(string predmetOznaka in racunarskiCentar.Smerovi[oznakaSmera].Predmeti)
                        {
                            racunarskiCentar.Predmeti[predmetOznaka].Obrisan = true;
                            predmetiKolekcija.Remove(racunarskiCentar.Predmeti[predmetOznaka]);
                        }
                    }

                    removedItems.Add(racunarskiCentar.Smerovi[oznakaSmera]);
                    racunarskiCentar.Smerovi[oznakaSmera].Obrisan = true;
                }

                foreach (Smer smer in removedItems)
                    smeroviKolekcija.Remove(smer);
            }
            else
                return;
        }

        private void tabelaSoftveraIzgubilaFokus(object sender, EventArgs e)
        {
            detaljanPrikazSoftver.Visibility = Visibility.Hidden;
            MenuItemIzmeni.IsEnabled = false;
            MenuItemObrisi.IsEnabled = false;
            MenuItemIzborFiltera.IsEnabled = false;
            MenuItemPretraga.IsEnabled = false;
        }

        private void tabelaSmerovaIzgubilaFokus(object sender, EventArgs e)
        {
            detaljanPrikazSmer.Visibility = Visibility.Hidden;
            MenuItemIzmeni.IsEnabled = false;
            MenuItemObrisi.IsEnabled = false;
            MenuItemIzborFiltera.IsEnabled = false;
            MenuItemPretraga.IsEnabled = false;
        }

        private void tabelaPredmetaIzgubilaFokus(object sender, EventArgs e)
        {
            detaljanPrikazPredmet.Visibility = Visibility.Hidden;
            MenuItemIzmeni.IsEnabled = false;
            MenuItemObrisi.IsEnabled = false;
            MenuItemIzborFiltera.IsEnabled = false;
            MenuItemPretraga.IsEnabled = false;
        }

        private void tabelaUcionicaIzgubilaFokus(object sender, EventArgs e)
        {
            detaljanPrikazUcionica.Visibility = Visibility.Hidden;
            MenuItemIzmeni.IsEnabled = false;
            MenuItemObrisi.IsEnabled = false;
            MenuItemIzborFiltera.IsEnabled = false;
            MenuItemPretraga.IsEnabled = false;
        }

        private void tabelaSmerovaDobilaFokus(object sender, EventArgs e)
        {
            detaljanPrikazSmer.Visibility = Visibility.Visible;
            MenuItemIzmeni.IsEnabled = true;
            MenuItemObrisi.IsEnabled = true;
            MenuItemIzborFiltera.IsEnabled = true;
            MenuItemPretraga.IsEnabled = true;
        }

        private void tabelaSoftveraDobilaFokus(object sender, EventArgs e)
        {
            detaljanPrikazSoftver.Visibility = Visibility.Visible;
            MenuItemIzmeni.IsEnabled = true;
            MenuItemObrisi.IsEnabled = true;
            MenuItemIzborFiltera.IsEnabled = true;
            MenuItemPretraga.IsEnabled = true;
        }

        private void tabelaPredmetaDobilaFokus(object sender, EventArgs e)
        {
            detaljanPrikazPredmet.Visibility = Visibility.Visible;
            MenuItemIzmeni.IsEnabled = true;
            MenuItemObrisi.IsEnabled = true;
            MenuItemIzborFiltera.IsEnabled = true;
            MenuItemPretraga.IsEnabled = true;
        }

        private void tabelaUcionicaDobilaFokus(object sender, EventArgs e)
        {
            detaljanPrikazUcionica.Visibility = Visibility.Visible;
            MenuItemIzmeni.IsEnabled = true;
            MenuItemObrisi.IsEnabled = true;
            MenuItemIzborFiltera.IsEnabled = true;
            MenuItemPretraga.IsEnabled = true;
        }

        private void SerijalizacijaPodataka(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(imeFajla, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            DataContractSerializer serializer = new DataContractSerializer(typeof(RacunarskiCentar));

            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                // add formatting so the XML is easy to read in the log
                writer.Formatting = Formatting.Indented;

                serializer.WriteObject(writer, racunarskiCentar);

                writer.Flush();
                writer.Close();
            }

            sw.Close();
            fs.Close();
            Console.WriteLine("Serijalizacija uspesno izvrsena!\n");
            foreach(KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
            {
                Console.WriteLine(polje.Id + "|" + polje.Pocetak + "|" + polje.Kraj + "|" + polje.Dan + "|" + polje.NazivPolja);
            }
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
        
        private void tabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                cef.posaljiPodatke();
                MenuItemPretraga.IsEnabled = false;
                MenuItemPretraga.IsEnabled = false;
            }
            else if(tabControl.SelectedIndex == 1 || tabControl.SelectedIndex == 2 || tabControl.SelectedIndex == 3 || tabControl.SelectedIndex == 4)
            {
                MenuItemIzborFiltera.IsEnabled = true;
                MenuItemPretraga.IsEnabled = true;
            }
        }
    }
}
