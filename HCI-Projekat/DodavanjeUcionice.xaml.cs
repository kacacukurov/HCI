using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                    softveri.Add(s);
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
                novaUcionica.PrisustvoTable = prisustvoTableUcionica.IsChecked;
                novaUcionica.PrisustvoProjektora = prisustvoProjektoraUcionica.IsChecked;
                novaUcionica.BrojRadnihMesta = int.Parse(brojRadnihMestaUcionica.Text);
                if ((bool)LinuxOSUcionica.IsChecked)
                    novaUcionica.OperativniSistem = "Linux";
                else if ((bool)WindowsOSUcionica.IsChecked)
                    novaUcionica.OperativniSistem = "Windows";
                else
                    novaUcionica.OperativniSistem = "Windows i Linux";

                for (int i = 0; i < softverTabela.Items.Count; i++)
                {
                    DataGridRow row = (DataGridRow)softverTabela.ItemContainerGenerator.ContainerFromIndex(i);
                    CheckBox box = softverTabela.Columns[3].GetCellContent(row) as CheckBox;
                    if ((bool)box.IsChecked)
                    {
                        TextBlock content = softverTabela.Columns[1].GetCellContent(row) as TextBlock;
                        novaUcionica.InstaliraniSoftveri.Add(content.Text);
                    }
                }

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
                DataGridRow row = (DataGridRow)softverTabela.ItemContainerGenerator.ContainerFromIndex(i);
                CheckBox box = softverTabela.Columns[3].GetCellContent(row) as CheckBox;
                if ((bool)box.IsChecked)
                {
                    postojiSoftver = true;
                }
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
                racunarskiCentar.Ucionice[oznakaUcionica.Text].Oznaka = oznakaUcionica.Text;
                racunarskiCentar.Ucionice[oznakaUcionica.Text].Opis = opisUcionica.Text;
                racunarskiCentar.Ucionice[oznakaUcionica.Text].PrisustvoPametneTable = prisustvoPametneTableUcionica.IsChecked;
                racunarskiCentar.Ucionice[oznakaUcionica.Text].PrisustvoTable = prisustvoTableUcionica.IsChecked;
                racunarskiCentar.Ucionice[oznakaUcionica.Text].PrisustvoProjektora = prisustvoProjektoraUcionica.IsChecked;
                racunarskiCentar.Ucionice[oznakaUcionica.Text].BrojRadnihMesta = int.Parse(brojRadnihMestaUcionica.Text);
                if ((bool)LinuxOSUcionica.IsChecked)
                    racunarskiCentar.Ucionice[oznakaUcionica.Text].OperativniSistem = "Linux";
                else if ((bool)WindowsOSUcionica.IsChecked)
                    racunarskiCentar.Ucionice[oznakaUcionica.Text].OperativniSistem = "Windows";
                else
                    racunarskiCentar.Ucionice[oznakaUcionica.Text].OperativniSistem = "Windows i Linux";

                racunarskiCentar.Ucionice[oznakaUcionica.Text].InstaliraniSoftveri.Clear();
                for (int i = 0; i < softverTabela.Items.Count; i++)
                {
                    DataGridRow row = (DataGridRow)softverTabela.ItemContainerGenerator.ContainerFromIndex(i);
                    CheckBox box = softverTabela.Columns[3].GetCellContent(row) as CheckBox;
                    if ((bool)box.IsChecked)
                    {
                        TextBlock content = softverTabela.Columns[1].GetCellContent(row) as TextBlock;
                        racunarskiCentar.Ucionice[oznakaUcionica.Text].InstaliraniSoftveri.Add(content.Text);
                    }
                }
                
                tabelaUcionica[indeks] = racunarskiCentar.Ucionice[oznakaUcionica.Text];
                this.Close();
            }
        }
    }
}
