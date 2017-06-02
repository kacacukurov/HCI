using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

        public DodavanjeUcionice(RacunarskiCentar racunarskiCentar, ObservableCollection<Ucionica> ucionice, bool izmena)
        {
            InitializeComponent();
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
                novaUcionica.Oznaka = oznakaUcionica.Text;
                novaUcionica.Opis = opisUcionica.Text;

                novaUcionica.PrisustvoPametneTable = prisustvoPametneTableUcionica.IsChecked;
                novaUcionica.PametnaTablaString = novaUcionica.PrisustvoPametneTable ? "prisutna" : "nije prisutna";
                novaUcionica.PrisustvoTable = prisustvoTableUcionica.IsChecked;
                novaUcionica.TablaString = novaUcionica.PrisustvoTable ? "prisutna" : "nije prisutna";
                novaUcionica.PrisustvoProjektora = prisustvoProjektoraUcionica.IsChecked;
                novaUcionica.ProjektorString = novaUcionica.PrisustvoProjektora ? "prisutan" : "nije prisutan";

                novaUcionica.BrojRadnihMesta = int.Parse(brojRadnihMestaUcionica.Text);
                if ((bool)LinuxOSUcionica.IsChecked)
                    novaUcionica.OperativniSistem = "Linux";
                else if ((bool)WindowsOSUcionica.IsChecked)
                    novaUcionica.OperativniSistem = "Windows";
                else
                    novaUcionica.OperativniSistem = "Windows i Linux";

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < softverTabela.Items.Count; i++)
                {
                    Softver softver = (Softver)softverTabela.Items[i];
                    if (softver.Instaliran)
                    {
                        novaUcionica.InstaliraniSoftveri.Add(softver.Oznaka);
                        sb.Append("\n" + softver.Oznaka);
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
            if (racunarskiCentar.Ucionice.ContainsKey(oznakaUcionica.Text))
            {
                if (racunarskiCentar.Ucionice[oznakaUcionica.Text].Obrisan)
                    racunarskiCentar.Ucionice.Remove(oznakaUcionica.Text);
                else
                {
                    MessageBox.Show("Učionica sa ovom oznakom već postoji!");
                    oznakaUcionica.Text = "";
                    oznakaUcionica.Focus();
                    return false;
                }
            }
            if (!validacijaPodataka())
                return false;
            return true;
        }

        private bool validacijaPodataka()
        {
            if (oznakaUcionica.Text == "" || opisUcionica.Text == "" || brojRadnihMestaUcionica.Text == "")
            {
                MessageBox.Show("Niste popunili sva polja!");
                if (oznakaUcionica.Text == "")
                    oznakaUcionica.Focus();
                else if (opisUcionica.Text == "")
                    opisUcionica.Focus();
                else if (brojRadnihMestaUcionica.Text == "")
                    brojRadnihMestaUcionica.Focus();
                return false;
            }
            int brMesta;
            if (!int.TryParse(brojRadnihMestaUcionica.Text, out brMesta))
            {
                MessageBox.Show("Broj radnih mesta nije dobro unesen, unesite broj!");
                brojRadnihMestaUcionica.Text = "";
                brojRadnihMestaUcionica.Focus();
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
                MessageBox.Show("Niste označili potreban softver!");
                return false;
            }
            return true;
        }

        private void izmenaUcionice()
        {
            if (validacijaPodataka())
            {
                Ucionica ucionicaIzmena = racunarskiCentar.Ucionice[oznakaUcionica.Text];
                ucionicaIzmena.Oznaka = oznakaUcionica.Text;
                ucionicaIzmena.Opis = opisUcionica.Text;

                ucionicaIzmena.PrisustvoPametneTable = prisustvoPametneTableUcionica.IsChecked;
                ucionicaIzmena.PametnaTablaString = ucionicaIzmena.PrisustvoPametneTable ? "prisutna" : "nije prisutna";
                ucionicaIzmena.PrisustvoTable = prisustvoTableUcionica.IsChecked;
                ucionicaIzmena.TablaString = ucionicaIzmena.PrisustvoTable ? "prisutna" : "nije prisutna";
                ucionicaIzmena.PrisustvoProjektora = prisustvoProjektoraUcionica.IsChecked;
                ucionicaIzmena.ProjektorString = ucionicaIzmena.PrisustvoProjektora ? "prisutan" : "nije prisutan";

                ucionicaIzmena.BrojRadnihMesta = int.Parse(brojRadnihMestaUcionica.Text);
                if ((bool)LinuxOSUcionica.IsChecked)
                    ucionicaIzmena.OperativniSistem = "Linux";
                else if ((bool)WindowsOSUcionica.IsChecked)
                    ucionicaIzmena.OperativniSistem = "Windows";
                else
                    ucionicaIzmena.OperativniSistem = "Windows i Linux";

                ucionicaIzmena.InstaliraniSoftveri.Clear();
                for (int i = 0; i < softverTabela.Items.Count; i++)
                {
                    Softver softver = (Softver)softverTabela.Items[i];
                    if (softver.Instaliran)
                    {
                        ucionicaIzmena.InstaliraniSoftveri.Add(softver.Oznaka);
                        softver.Instaliran = false;
                    }
                }

                tabelaUcionica[indeks] = ucionicaIzmena;
                this.Close();
            }
        }
    }
}
