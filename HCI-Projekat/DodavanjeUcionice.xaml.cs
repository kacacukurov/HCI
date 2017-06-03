using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public int indeks;
        public bool inicijalizacija;

        public DodavanjeUcionice(RacunarskiCentar racunarskiCentar, ObservableCollection<Ucionica> ucionice, bool izmena)
        {
            this.inicijalizacija = false;
            InitializeComponent();
            this.inicijalizacija = true;
            novaUcionica = new Ucionica();
            this.racunarskiCentar = racunarskiCentar;
            this.izmena = izmena;
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

        private void finishClick(object sender, RoutedEventArgs e)
        {
            finishButton.Focus();
            if (izmena)
            {
                izmenaUcionice();
                return;
            }
            if (validacijaNoveUcionice())
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
        }

        private bool validacijaNoveUcionice()
        {
            if (racunarskiCentar.Ucionice.ContainsKey(oznakaUcionica.Text.Trim()))
            {
                if (racunarskiCentar.Ucionice[oznakaUcionica.Text.Trim()].Obrisan)
                    racunarskiCentar.Ucionice.Remove(oznakaUcionica.Text.Trim());
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
            if (validacijaPodataka())
            {
                Ucionica ucionicaIzmena = racunarskiCentar.Ucionice[oznakaUcionica.Text.Trim()];

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

                tabelaUcionica[indeks] = ucionicaIzmena;
                this.Close();
            }
        }
    }
}
