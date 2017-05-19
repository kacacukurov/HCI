﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.Xml;
using System.Collections.ObjectModel;

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

        public MainWindow()
        {
            InitializeComponent();
            KalendarTab.Focus();

            racunarskiCentar = new RacunarskiCentar();
            DeserijalizacijaPodataka();

            predmetiKolekcija = new ObservableCollection<Predmet>(racunarskiCentar.Predmeti.Values);
            tabelaPredmeta.ItemsSource = predmetiKolekcija;
            tabelaPredmeta.IsSynchronizedWithCurrentItem = true;
            tabelaPredmeta.IsReadOnly = true;

            softveriKolekcija = new ObservableCollection<Softver>(racunarskiCentar.Softveri.Values);
            tabelaSoftvera.ItemsSource = softveriKolekcija;
            tabelaSoftvera.IsSynchronizedWithCurrentItem = true;
            tabelaSoftvera.IsReadOnly = true;

            smeroviKolekcija = new ObservableCollection<Smer>(racunarskiCentar.Smerovi.Values);
            tabelaSmerova.ItemsSource = smeroviKolekcija;
            tabelaSmerova.IsSynchronizedWithCurrentItem = true;
            tabelaSoftvera.IsReadOnly = true;

            ucioniceKolekcija = new ObservableCollection<Ucionica>(racunarskiCentar.Ucionice.Values);
            tabelaUcionica.ItemsSource = ucioniceKolekcija;
            tabelaUcionica.IsSynchronizedWithCurrentItem = true;
            tabelaUcionica.IsReadOnly = true;
        }

        private void dodavanjeUcioniceClick(object sender, RoutedEventArgs e)
        {
            var ucionicaWindow = new DodavanjeUcionice(racunarskiCentar, ucioniceKolekcija);
            ucionicaWindow.ShowDialog();
        }

        private void dodavanjePredmetaClick(object sender, RoutedEventArgs e)
        {
            var predmetWindow = new DodavanjePredmeta(racunarskiCentar, predmetiKolekcija);
            predmetWindow.ShowDialog();
        }

        private void dodavanjeSmeraClick(object sender, RoutedEventArgs e)
        {
            var smerWindow = new DodavanjeSmera(racunarskiCentar, smeroviKolekcija);
            smerWindow.ShowDialog();
        }

        private void dodavanjeSoftveraClick(object sender, RoutedEventArgs e)
        {
            var softverWindow = new DodavanjeSoftvera(racunarskiCentar, softveriKolekcija);
            softverWindow.ShowDialog();
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Snimi");
        }

        private void exitClick(object sender, RoutedEventArgs e)
        {
            mainWindow.Close();
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

        private void tabsFocus(object sender, RoutedEventArgs e)
        {
            // trenutno smo u tabu za ucionice
            if (tabControl.SelectedIndex == 1)
                UcionicaTab.Focus();
            // trenutno smo u tabu za predmete
            else if (tabControl.SelectedIndex == 2)
                PredmetTab.Focus();
            // trenutno smo u tabu za smerove
            else if (tabControl.SelectedIndex == 3)
                SmerTab.Focus();
            // trenutno smo u tabu za softvere
            else if (tabControl.SelectedIndex == 4)
                SoftverTab.Focus();
        }

        private void obrisiElement(object sender, RoutedEventArgs e)
        {
            // trenutno smo u tabu za ucionice
            if (tabControl.SelectedIndex == 1)
                obrisiUcionicuClick(sender, e);
            // trenutno smo u tabu za predmete
            else if (tabControl.SelectedIndex == 2)
                obrisiPredmetClick(sender, e);
            // trenutno smo u tabu za smerove
            else if (tabControl.SelectedIndex == 3)
                obrisiSmerClick(sender, e);
            // trenutno smo u tabu za softvere
            else if (tabControl.SelectedIndex == 4)
                obrisiSoftverClick(sender, e);
        }

        private void izmeniElement(object sender, RoutedEventArgs e)
        {
            // trenutno smo u tabu za ucionice
            if (tabControl.SelectedIndex == 1)
                izmeniUcionicuClick(sender, e);
            // trenutno smo u tabu za predmete
            else if (tabControl.SelectedIndex == 2)
                izmeniPredmetClick(sender, e);
            // trenutno smo u tabu za smerove
            else if (tabControl.SelectedIndex == 3)
                izmeniSmerClick(sender, e);
            // trenutno smo u tabu za softvere
            else if (tabControl.SelectedIndex == 4)
                izmeniSoftverClick(sender, e);
        }

        private void izmeniPredmetClick(object sender, RoutedEventArgs e)
        {
            if (tabelaPredmeta.SelectedIndex != -1)
                MessageBox.Show(tabelaPredmeta.SelectedIndex.ToString());
            else
                return;
        }

        private void izmeniSoftverClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSoftvera.SelectedIndex != -1)
                MessageBox.Show(tabelaSoftvera.SelectedIndex.ToString());
            else
                return;
        }

        private void izmeniUcionicuClick(object sender, RoutedEventArgs e)
        {
            if (tabelaUcionica.SelectedIndex != -1)
                MessageBox.Show(tabelaUcionica.SelectedIndex.ToString());
            else
                return;
        }

        private void izmeniSmerClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSmerova.SelectedIndex != -1)
                MessageBox.Show(tabelaSmerova.SelectedIndex.ToString());
            else
                return;
        }

        private void obrisiPredmetClick(object sender, RoutedEventArgs e)
        {
            if (tabelaPredmeta.SelectedIndex != -1)
            {
                DataGridRow selektovaniRed = (DataGridRow)tabelaPredmeta.ItemContainerGenerator.ContainerFromIndex(tabelaPredmeta.SelectedIndex);
                TextBlock content = tabelaPredmeta.Columns[1].GetCellContent(selektovaniRed) as TextBlock;
                string oznakaPredmeta = content.Text;

                predmetiKolekcija.Remove(racunarskiCentar.Predmeti[oznakaPredmeta]);
                //promeniti sa fizickog na logicko brisanje
                racunarskiCentar.Predmeti.Remove(oznakaPredmeta);
            }
            else
                return;
        }

        private void obrisiSoftverClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSoftvera.SelectedIndex != -1)
            {
                DataGridRow selektovaniRed = (DataGridRow)tabelaSoftvera.ItemContainerGenerator.ContainerFromIndex(tabelaSoftvera.SelectedIndex);
                TextBlock content = tabelaSoftvera.Columns[1].GetCellContent(selektovaniRed) as TextBlock;
                string oznakaSoftvera = content.Text;

                softveriKolekcija.Remove(racunarskiCentar.Softveri[oznakaSoftvera]);
                // promeniti sa fizickog na logicko brisanje
                racunarskiCentar.Softveri.Remove(oznakaSoftvera);

                //u predmetima ukloniti softver iz liste softvera ako ga sadrzi
                //u ucionicama ukloniti softver iz liste softvera ako ga sadrzi
            }
            else
                return;
        }

        private void obrisiUcionicuClick(object sender, RoutedEventArgs e)
        {
            if (tabelaUcionica.SelectedIndex != -1)
            {
                DataGridRow selektovaniRed = (DataGridRow)tabelaUcionica.ItemContainerGenerator.ContainerFromIndex(tabelaUcionica.SelectedIndex);
                TextBlock content = tabelaSmerova.Columns[0].GetCellContent(tabelaUcionica) as TextBlock;
                string oznakaUcionice = content.Text;

                ucioniceKolekcija.Remove(racunarskiCentar.Ucionice[oznakaUcionice]);
                //promeniti sa fizickog na logicko brisanje
                racunarskiCentar.Ucionice.Remove(oznakaUcionice);
            }
            else
                return;
        }

        public void obrisiSmerClick(object sender, RoutedEventArgs e)
        {
            if (tabelaSmerova.SelectedIndex != -1) {
                DataGridRow selektovaniRed = (DataGridRow)tabelaSmerova.ItemContainerGenerator.ContainerFromIndex(tabelaSmerova.SelectedIndex);
                TextBlock content = tabelaSmerova.Columns[1].GetCellContent(selektovaniRed) as TextBlock;
                string oznakaSmera = content.Text;

                smeroviKolekcija.Remove(racunarskiCentar.Smerovi[oznakaSmera]);
                //promeniti sa fizickog na logicko brisanje
                racunarskiCentar.Smerovi.Remove(oznakaSmera);
            }
            else
                return;
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
    }
}
