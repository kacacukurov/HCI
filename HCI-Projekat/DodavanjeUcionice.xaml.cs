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

        public DodavanjeUcionice(RacunarskiCentar racunarskiCentar, ObservableCollection<Ucionica> ucionice, bool izmena, string oznaka)
        {
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
            foreach(Softver s in racunarskiCentar.Softveri.Values)
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
            if(!izmena)
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

        private void cutClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cut");
        }

        private void copyClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Copy");
        }

        private void pasteClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Paste");
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
            this.Close();
        }

        private void UnetaOznakaUcionice(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (!izmena)
            {
                if (racunarskiCentar.Ucionice.ContainsKey(t.Text.Trim()))
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

        private void proveraPraznogPolja(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (t.Text.Trim().Equals(string.Empty))
                t.BorderBrush = System.Windows.Media.Brushes.Red;
            else
                t.ClearValue(Border.BorderBrushProperty);
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
                this.Close();
            }
            else if (dodavanjeUcioniceIzborStarogUnosa)
            {
                // ukoliko postoji predmet (logicki neaktivan) sa istom oznakom
                // kao sto je uneta, ponovo aktiviramo taj predmet (postaje logicki aktivan)
                tabelaUcionica.Add(racunarskiCentar.Ucionice[oznakaUcionica.Text.Trim()]);
                this.Close();
            }
        }

        private bool validacijaNoveUcionice()
        {
            if (racunarskiCentar.Ucionice.ContainsKey(oznakaUcionica.Text.Trim()))
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
                        // vracamo logicki obrisanu ucionicu da bude aktivna
                        ucionica.Obrisan = false;
                        dodavanjeUcioniceIzborStarogUnosa = true;
                    }
                }    
                else
                {
                    MessageBox.Show("Učionica sa unetom oznakom već postoji!");
                    vratiNaKorak1();
                    UpdateLayout();
                    oznakaUcionica.Focus();
                    return false;
                }
            }
            if (!validacijaPodataka())
                return false;
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
                MessageBox.Show("Niste popunili sva polja!");
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
                MessageBox.Show("Broj radnih mesta nije dobro unesen, unesite broj!");
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
                    MessageBox.Show("Niste označili potreban softver/softvere!");
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
                MessageBox.Show("Morate prvo uneti softver da biste mogli da unesete učionicu!");
                return false;
            }
            return true;
        }

        private void izmenaUcionice()
        {
            if (validacijaPodataka() && validacijaIzmeneSoftvera() && validacijaIzmeneTable() && validacijaIzmenePametneTable() &&
                validacijaIzmeneProjektora())
            {
                Ucionica ucionicaIzmena = racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu];
                string staraOznaka = ucionicaIzmena.Oznaka;
                bool oznakaIzmenjena = false;

                if (!staraOznaka.Equals(oznakaUcionica.Text.Trim()))
                    oznakaIzmenjena = true;

                ucionicaIzmena.Oznaka = oznakaUcionica.Text.Trim();
                ucionicaIzmena.Opis = opisUcionica.Text.Trim();

                ucionicaIzmena.PrisustvoPametneTable = prisustvoPametneTableUcionica.IsChecked;
                ucionicaIzmena.PametnaTablaString = ucionicaIzmena.PrisustvoPametneTable ? "prisutna" : "nije prisutna";
                ucionicaIzmena.PrisustvoTable = prisustvoTableUcionica.IsChecked;
                ucionicaIzmena.TablaString = ucionicaIzmena.PrisustvoTable ? "prisutna" : "nije prisutna";
                ucionicaIzmena.PrisustvoProjektora = prisustvoProjektoraUcionica.IsChecked;
                ucionicaIzmena.ProjektorString = ucionicaIzmena.PrisustvoProjektora ? "prisutan" : "nije prisutan";

                ucionicaIzmena.BrojRadnihMesta = int.Parse(brojRadnihMestaUcionica.Text.Trim());
                if ((bool)LinuxOSUcionica.IsChecked)
                    ucionicaIzmena.OperativniSistem = "Linux";
                else if ((bool)WindowsOSUcionica.IsChecked)
                    ucionicaIzmena.OperativniSistem = "Windows";
                else
                    ucionicaIzmena.OperativniSistem = "Windows i Linux";

                ucionicaIzmena.InstaliraniSoftveri.Clear();
                StringBuilder sb = new StringBuilder();
                int brojSoftvera = 0;
                for (int i = 0; i < softverTabela.Items.Count; i++)
                {
                    Softver softver = (Softver)softverTabela.Items[i];
                    if (softver.Instaliran)
                    {
                        brojSoftvera++;
                        ucionicaIzmena.InstaliraniSoftveri.Add(softver.Oznaka);

                        if (brojSoftvera > 1)
                            sb.Append("\n");
                        sb.Append("Oznaka: " + softver.Oznaka);
                        sb.Append("\nNaziv: " + softver.Naziv);
                        sb.Append("\nOpis: " + softver.Opis + "\n");
                        softver.Instaliran = false;
                    }
                }
                ucionicaIzmena.SoftveriLista = sb.ToString();

                if(oznakaIzmenjena)
                {
                    racunarskiCentar.Ucionice.Remove(staraOznaka);
                    racunarskiCentar.Ucionice.Add(ucionicaIzmena.Oznaka, ucionicaIzmena);
                    izmenaUcioniceUPoljima(staraOznaka, oznakaUcionica.Text.Trim());
                }
                
                tabelaUcionica[indeks] = ucionicaIzmena;
                this.Close();
            }
        }

        private bool validacijaIzmeneSoftvera()
        {
            Ucionica staraUcionica = racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu];
            List<string> sviPredmetiUcionice = new List<string>();
            foreach(KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)  //trazimo sve predmete koji se odrzavaju u datoj ucionici
            {
                if (polje.Ucionica.Trim().Equals(staraUcionica.Oznaka.Trim()))
                    sviPredmetiUcionice.Add(polje.NazivPolja.Split('-')[0].Trim());
            }

            List<string> predmetiUcionice = sviPredmetiUcionice.Distinct().ToList(); //izbacimo duplikate
            
            foreach(string poz in predmetiUcionice)     // prolazim kroz sve predmete unutar ucionice
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
                        MessageBox.Show("Ne možete izmeniti softvere učionice, jer se oni potrebni predmetima koji se predaju u njoj!");
                        return false;
                    }
                }
            }
            return true;
        }

        private bool validacijaIzmeneTable()
        {
            if (prisustvoTableUcionica.IsChecked)
                return true;
            Ucionica staraUcionica = racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu];
            List<string> sviPredmetiUcionice = new List<string>();
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)  //trazimo sve predmete koji se odrzavaju u datoj ucionici
            {
                if (polje.Ucionica.Trim().Equals(staraUcionica.Oznaka.Trim()))
                    sviPredmetiUcionice.Add(polje.NazivPolja.Split('-')[0].Trim());
            }

            List<string> predmetiUcionice = sviPredmetiUcionice.Distinct().ToList(); //izbacimo duplikate
            foreach(string poz in predmetiUcionice)
            {
                if (racunarskiCentar.Predmeti[poz].NeophodnaTabla)
                {
                    MessageBox.Show("Ne možete ukloniti tablu, postoje predmeti u učionici kojima je potrebna!");
                    prisustvoTableUcionica.Focus();
                    return false;
                }
            }
            return true;
        }

        private bool validacijaIzmenePametneTable()
        {
            if (prisustvoPametneTableUcionica.IsChecked)
                return true;
            Ucionica staraUcionica = racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu];
            List<string> sviPredmetiUcionice = new List<string>();
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)  //trazimo sve predmete koji se odrzavaju u datoj ucionici
            {
                if (polje.Ucionica.Trim().Equals(staraUcionica.Oznaka.Trim()))
                    sviPredmetiUcionice.Add(polje.NazivPolja.Split('-')[0].Trim());
            }

            List<string> predmetiUcionice = sviPredmetiUcionice.Distinct().ToList(); //izbacimo duplikate
            foreach (string poz in predmetiUcionice)
            {
                if (racunarskiCentar.Predmeti[poz].NeophodnaPametnaTabla)
                {
                    MessageBox.Show("Ne možete ukloniti pametnu tablu, postoje predmeti u učionici kojima je potrebna!");
                    prisustvoPametneTableUcionica.Focus();
                    return false;
                }
            }
            return true;
        }

        private bool validacijaIzmeneProjektora()
        {
            if (prisustvoProjektoraUcionica.IsChecked)
                return true;
            Ucionica staraUcionica = racunarskiCentar.Ucionice[oznakaUcioniceZaIzmenu];
            List<string> sviPredmetiUcionice = new List<string>();
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)  //trazimo sve predmete koji se odrzavaju u datoj ucionici
            {
                if (polje.Ucionica.Trim().Equals(staraUcionica.Oznaka.Trim()))
                    sviPredmetiUcionice.Add(polje.NazivPolja.Split('-')[0].Trim());
            }

            List<string> predmetiUcionice = sviPredmetiUcionice.Distinct().ToList(); //izbacimo duplikate
            foreach (string poz in predmetiUcionice)
            {
                if (racunarskiCentar.Predmeti[poz].NeophodanProjektor)
                {
                    MessageBox.Show("Ne možete ukloniti projektor, postoje predmeti u učionici kojima je potrebna!");
                    prisustvoProjektoraUcionica.Focus();
                    return false;
                }
            }
            return true;
        }

        private void izmenaUcioniceUPoljima(string staraOznaka, string novaOznaka)
        {
            foreach (KalendarPolje polje in racunarskiCentar.PoljaKalendara.Values)
            {
                if (polje.Ucionica == staraOznaka)    //idem kroz sva polja i trazim ucionice
                    polje.Ucionica = novaOznaka;
            }
        }
    }
}
