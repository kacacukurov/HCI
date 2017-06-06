﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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

        public DodavanjePredmeta(RacunarskiCentar racunarskiCentar, ObservableCollection<Predmet> predmeti, bool izmena, string oznaka)
        {
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
            this.Close();
        }

        private void resetujBojuOkvira(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.ClearValue(Border.BorderBrushProperty);
        }

        private void UnetaOznakaPredmeta(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (!izmena)
            {
                if (racunarskiCentar.Predmeti.ContainsKey(t.Text.Trim()))
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
                this.Close();
            }
            else if (dodavanjePredmetaIzborStarogUnosa)
            {
                // ukoliko postoji predmet (logicki neaktivan) sa istom oznakom
                // kao sto je uneta, ponovo aktiviramo taj predmet (postaje logicki aktivan)
                tabelaPredmeta.Add(racunarskiCentar.Predmeti[OznakaPredmeta.Text.Trim()]);
                this.Close();
            }
        }

        private bool validacijaNovogPredmeta()
        {
            if (!validacijaPodataka())
            {
                return false;
            }
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
                        // vracamo logicki obrisan predmet da bude aktivan
                        predmet.Obrisan = false;
                        dodavanjePredmetaIzborStarogUnosa = true;
                        // u smeru kom pripada ovaj predmet belezimo promenu
                        racunarskiCentar.Smerovi[predmet.Smer].Predmeti.Add(predmet.Oznaka);
                    }
                }
                else
                {
                    MessageBox.Show("Predmet sa unetom oznakom već postoji!");
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

                MessageBox.Show("Niste popunili sva polja!");
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
                MessageBox.Show("Niste označili smer na kom se održava predmet!");
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
                MessageBox.Show("Niste popunili sva polja!");
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
                MessageBox.Show("Niste označili potreban softver/softvere!");
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
            if (validacijaPodataka() && validacijeIzmeneBrojaTermina() && validacijaIzmeneDuzineTermina() && validacijaIzmeneSoftvera()
                && validacijaIzmeneTable() && validacijaIzmenePametneTable() && validacijaIzmeneProjektora())
            {
                Predmet predmetIzmena = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu];
                string staraOznaka = predmetIzmena.Oznaka;
                bool oznakaPromenjena = false;

                if (!staraOznaka.Equals(OznakaPredmeta.Text.Trim()))
                    oznakaPromenjena = true;

                predmetIzmena.Oznaka = OznakaPredmeta.Text.Trim();
                predmetIzmena.Naziv = NazivPredmeta.Text.Trim();
                predmetIzmena.Opis = OpisPredmeta.Text.Trim();
                predmetIzmena.VelicinaGrupe = int.Parse(VelicinaGrupePredmet.Text.Trim());
                predmetIzmena.MinDuzinaTermina = int.Parse(DuzinaTerminaPredmet.Text.Trim());
                predmetIzmena.BrTermina = int.Parse(BrojTerminaPredmet.Text.Trim());

                predmetIzmena.NeophodanProjektor = PrisustvoProjektoraPredmet.IsChecked;
                predmetIzmena.ProjektorString = predmetIzmena.NeophodanProjektor ? "neophodan" : "nije neophodan";
                predmetIzmena.NeophodnaTabla = PrisustvoTablePredmet.IsChecked;
                predmetIzmena.TablaString = predmetIzmena.NeophodnaTabla ? "neophodna" : "nije neophodna";
                predmetIzmena.NeophodnaPametnaTabla = PrisustvoPametneTable.IsChecked;
                predmetIzmena.PametnaTablaString = predmetIzmena.NeophodnaPametnaTabla ? "neophodna" : "nije neophodna";

                if ((bool)Windows.IsChecked)
                    predmetIzmena.OperativniSistem = Windows.Content.ToString();
                else if ((bool)Linux.IsChecked)
                    predmetIzmena.OperativniSistem = Linux.Content.ToString();
                else if ((bool)Svejedno.IsChecked)
                    predmetIzmena.OperativniSistem = Svejedno.Content.ToString();

                StringBuilder sb = new StringBuilder();
                int brojSoftvera = 0;
                predmetIzmena.Softveri.Clear();
                for (int i = 0; i < softverTabela.Items.Count; i++)
                {
                    Softver softver = (Softver)softverTabela.Items[i];
                    if (softver.Instaliran)
                    {
                        brojSoftvera++;
                        predmetIzmena.Softveri.Add(softver.Oznaka);

                        if (brojSoftvera > 1)
                            sb.Append("\n");
                        sb.Append("Oznaka: " + softver.Oznaka);
                        sb.Append("\nNaziv: " + softver.Naziv);
                        sb.Append("\nOpis: " + softver.Opis + "\n");
                        softver.Instaliran = false;
                    }
                }
                predmetIzmena.SoftveriLista = sb.ToString();

                bool stariSmerPronadjen = false;
                bool noviPredmetPostavljen = false;
                for (int i = 0; i < smeroviTabela.Items.Count; i++)
                {
                    Smer smer = (Smer)smeroviTabela.Items[i];
                    //iz smera za koji je bio vezan predmet uklanjamo vezu
                    if (smer.Predmeti.Contains(staraOznaka))
                    {
                        smer.Predmeti.Remove(staraOznaka);
                        stariSmerPronadjen = true;
                    }
                    if (smer.UPredmetu)
                    {
                        //u listu predmeta novoizabranog smera dodajemo i ovaj smer
                        if (!smer.Predmeti.Contains(predmetIzmena.Oznaka))
                            smer.Predmeti.Add(predmetIzmena.Oznaka);
                        predmetIzmena.Smer = smer.Oznaka;

                        predmetIzmena.SmerDetalji = "Oznaka: " + smer.Oznaka + "\nNaziv: " + smer.Naziv;
                        smer.UPredmetu = false;
                        noviPredmetPostavljen = true;
                    }

                    if (stariSmerPronadjen && noviPredmetPostavljen)
                        break;
                }

                if (oznakaPromenjena)
                {
                    racunarskiCentar.Predmeti.Remove(staraOznaka);
                    racunarskiCentar.Predmeti.Add(predmetIzmena.Oznaka, predmetIzmena);
                }

                tabelaPredmeta[indeks] = predmetIzmena;
                this.Close();
            }
        }

        private bool validacijeIzmeneBrojaTermina()
        {
            int stariBrojTermina = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu].BrTermina;
            int preostaliTermini = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu].PreostaliTermini;
            int noviBrojTermina = int.Parse(BrojTerminaPredmet.Text.Trim());
            if (noviBrojTermina > stariBrojTermina)
                racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu].PreostaliTermini += noviBrojTermina - stariBrojTermina;
            else
            {
                int razlika = stariBrojTermina - noviBrojTermina;
                if (razlika > racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu].PreostaliTermini)
                {
                    MessageBox.Show("Ne možete da smanjujete broj termina, jer su oni iskorišteni u kalendaru!");
                    return false;
                }
                else
                    racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu].PreostaliTermini -= razlika;
            }
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
                    MessageBox.Show("Ne možete promeniti dužinu trajanja jednog termina, jer je predmet već raspoređen u kalendaru!");
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

            List<string> ucionicePredmeta = sveUcionicePredmeta.Distinct().ToList(); //izbacimo duplikate

            int brojPotrebnihSoftvera = 0;
            int brojNadjenihSoftvera = 0;
            for (int i = 0; i < softverTabela.Items.Count; i++) //iteriram kroz svaki oznaceni softver
            {
                Softver softver = (Softver)softverTabela.Items[i];
                if (softver.Instaliran)
                {
                    foreach (string s in ucionicePredmeta)   //za svaki idem kroz ucionice u kojima se predaje predmet
                    {
                        Ucionica u = racunarskiCentar.Ucionice[s];
                        foreach (string soft in u.InstaliraniSoftveri)
                        {
                            if (soft.Trim().Equals(softver.Oznaka.Trim()))  //trazim taj softver u ucionici
                                brojNadjenihSoftvera++;
                        }
                    }
                    brojPotrebnihSoftvera++;
                }
            }
            if (brojNadjenihSoftvera < brojPotrebnihSoftvera * ucionicePredmeta.Count)
            {
                MessageBox.Show("Ne možete izmeniti softvere predmeta, jer se oni ne nalaze u učionici u kojoj se predmet predaje!");
                return false;
            }
            return true;
        }

        private bool validacijaIzmeneTable()
        {
            if (!PrisustvoTablePredmet.IsChecked)
                return true;
            Predmet predmetStari = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu];
            List<string> sveUcionicePredmeta = new List<string>(); //tu se nalaze sve ucionice u kojima se odrzava predmet koji se menja
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
            {
                if (polje.NazivPolja.Split('-')[0].Trim() == predmetStari.Oznaka)    //idem kroz sva polja i trazim ucionice
                    sveUcionicePredmeta.Add(polje.Ucionica);
            }

            List<string> ucionicePredmeta = sveUcionicePredmeta.Distinct().ToList(); //izbacimo duplikate
            foreach (string uoz in ucionicePredmeta)
            {
                bool postoji = true;
                if (!racunarskiCentar.Ucionice[uoz].PrisustvoTable)
                    postoji = false;
                if (!postoji)
                {
                    MessageBox.Show("Ne možete dodati tablu predmetu, jer se predaje u učionici u kojoj nema table!");
                    PrisustvoTablePredmet.Focus();
                    return false;
                }
            }
            return true;
        }

        private bool validacijaIzmenePametneTable()
        {
            if (!PrisustvoPametneTable.IsChecked)
                return true;

            Predmet predmetStari = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu];
            List<string> sveUcionicePredmeta = new List<string>(); //tu se nalaze sve ucionice u kojima se odrzava predmet koji se menja
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
            {
                if (polje.NazivPolja.Split('-')[0].Trim() == predmetStari.Oznaka)    //idem kroz sva polja i trazim ucionice
                    sveUcionicePredmeta.Add(polje.Ucionica);
            }

            List<string> ucionicePredmeta = sveUcionicePredmeta.Distinct().ToList(); //izbacimo duplikate
            foreach (string uoz in ucionicePredmeta)
            {
                bool postoji = true;
                if (!racunarskiCentar.Ucionice[uoz].PrisustvoPametneTable)
                    postoji = false;
                if (!postoji)
                {
                    MessageBox.Show("Ne možete dodati pametnu tablu predmetu, jer se predaje u učionici u kojoj nema pametne table!");
                    PrisustvoPametneTable.Focus();
                    return false;
                }
            }
            return true;
        }

        private bool validacijaIzmeneProjektora()
        {
            if (!PrisustvoProjektoraPredmet.IsChecked)
                return true;

            Predmet predmetStari = racunarskiCentar.Predmeti[oznakaPredmetaZaIzmenu];
            List<string> sveUcionicePredmeta = new List<string>(); //tu se nalaze sve ucionice u kojima se odrzava predmet koji se menja
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
            {
                if (polje.NazivPolja.Split('-')[0].Trim() == predmetStari.Oznaka)    //idem kroz sva polja i trazim ucionice
                    sveUcionicePredmeta.Add(polje.Ucionica);
            }

            List<string> ucionicePredmeta = sveUcionicePredmeta.Distinct().ToList(); //izbacimo duplikate
            foreach (string uoz in ucionicePredmeta)
            {
                bool postoji = true;
                if (!racunarskiCentar.Ucionice[uoz].PrisustvoProjektora)
                    postoji = false;
                if (!postoji)
                {
                    MessageBox.Show("Ne možete dodati projektor predmetu, jer se predaje u učionici u kojoj nema projektora!");
                    PrisustvoProjektoraPredmet.Focus();
                    return false;
                }
            }
            return true;
        }
    }
}
