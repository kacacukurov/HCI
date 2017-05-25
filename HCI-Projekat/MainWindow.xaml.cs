using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

            //  predmetiKolekcija = new ObservableCollection<Predmet>(racunarskiCentar.Predmeti.Values);
            predmetiKolekcija = new ObservableCollection<Predmet>();
            foreach (Predmet p in racunarskiCentar.Predmeti.Values)
            {
                if (!p.Obrisan)
                    predmetiKolekcija.Add(p);
            }
            tabelaPredmeta.ItemsSource = predmetiKolekcija;
            tabelaPredmeta.IsSynchronizedWithCurrentItem = true;
            tabelaPredmeta.IsReadOnly = true;

            //softveriKolekcija = new ObservableCollection<Softver>(racunarskiCentar.Softveri.Values);
            softveriKolekcija = new ObservableCollection<Softver>();
            foreach(Softver s in racunarskiCentar.Softveri.Values)
            {
                if (!s.Obrisan)
                    softveriKolekcija.Add(s);
            }
            tabelaSoftvera.ItemsSource = softveriKolekcija;
            tabelaSoftvera.IsSynchronizedWithCurrentItem = true;
            tabelaSoftvera.IsReadOnly = true;

            //smeroviKolekcija = new ObservableCollection<Smer>(racunarskiCentar.Smerovi.Values);
            smeroviKolekcija = new ObservableCollection<Smer>();
            foreach(Smer s in racunarskiCentar.Smerovi.Values)
            {
                if (!s.Obrisan)
                    smeroviKolekcija.Add(s);
            }
            tabelaSmerova.ItemsSource = smeroviKolekcija;
            tabelaSmerova.IsSynchronizedWithCurrentItem = true;
            tabelaSoftvera.IsReadOnly = true;

            //ucioniceKolekcija = new ObservableCollection<Ucionica>(racunarskiCentar.Ucionice.Values);
            ucioniceKolekcija = new ObservableCollection<Ucionica>();
            foreach(Ucionica u in racunarskiCentar.Ucionice.Values)
            {
                if (!u.Obrisan)
                    ucioniceKolekcija.Add(u);
            }
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

        private void undoClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Undo");
        }

        private void redoClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Redo");
        }

        private void detaljanPrikazPodataka(object sender, EventArgs e)
        {
            // trenutno smo u tabu za ucionice
            if (tabControl.SelectedIndex == 1)
               detaljanPrikazUcionice();
            // trenutno smo u tabu za predmete
            else if (tabControl.SelectedIndex == 2)
                detaljanPrikazPredmeta();
            // trenutno smo u tabu za smerove
            else if (tabControl.SelectedIndex == 3)
                detaljanPrikazSmera();
            // trenutno smo u tabu za softvere
            else if (tabControl.SelectedIndex == 4)
                detaljanPrikazSoftvera();
        }

        private void detaljanPrikazUcionice()
        {
            if(tabelaUcionica.SelectedIndex != -1)
            {
                DataGridRow selektovaniRed = (DataGridRow)tabelaUcionica.ItemContainerGenerator.ContainerFromIndex(tabelaUcionica.SelectedIndex);
                TextBlock content = tabelaUcionica.Columns[0].GetCellContent(selektovaniRed) as TextBlock;
                string oznakaUcionice = content.Text;

                Ucionica ucionica = racunarskiCentar.Ucionice[oznakaUcionice];
                StringBuilder sb = new StringBuilder();
                sb.Append("Oznaka: " + ucionica.Oznaka);
                sb.Append("\nOpis: " + ucionica.Opis);
                sb.Append("\nBroj radnih mesta: " + ucionica.BrojRadnihMesta);
                sb.Append("\nOperativni sistem: " + ucionica.OperativniSistem);
                sb.Append("\nTabla: " + (ucionica.PrisustvoTable ? "postoji" : "ne postoji"));
                sb.Append("\nPametna tabla: " + (ucionica.PrisustvoPametneTable ? "postoji" : "ne postoji"));
                sb.Append("\nProjektor: " + (ucionica.PrisustvoProjektora ? "postoji" : "ne postoji"));
                sb.Append("\nInstalirani softveri: ");
                foreach(string softver in ucionica.InstaliraniSoftveri)
                {
                    sb.Append("\n\t" + softver);
                }
                UcionicaDetaljanPrikazBox.Text = sb.ToString();
            }
        }

        private void detaljanPrikazPredmeta()
        {
            if (tabelaPredmeta.SelectedIndex != -1)
            {
                DataGridRow selektovaniRed = (DataGridRow)tabelaPredmeta.ItemContainerGenerator.ContainerFromIndex(tabelaPredmeta.SelectedIndex);
                TextBlock content = tabelaPredmeta.Columns[1].GetCellContent(selektovaniRed) as TextBlock;
                string oznakaPredmeta = content.Text;

                Predmet predmet = racunarskiCentar.Predmeti[oznakaPredmeta];
                StringBuilder sb = new StringBuilder();
                sb.Append("Oznaka: " + predmet.Oznaka);
                sb.Append("\nNaziv: " + predmet.Naziv);
                sb.Append("\nOpis: " + predmet.Opis);
                sb.Append("\nBroj termina: " + predmet.BrTermina);
                sb.Append("\nMinimalna dužina termina: " + predmet.MinDuzinaTermina);
                sb.Append("\nOperativni sistem: " + predmet.OperativniSistem);
                sb.Append("\nSmer: " + predmet.Smer.Oznaka);
                sb.Append("\nProjektor: " + (predmet.NeophodanProjektor ? "neophodan" : "nije neophodan"));
                sb.Append("\nTabla: " + (predmet.NeophodnaTabla ? "neophodna" : "nije neophodna"));
                sb.Append("\nPametna tabla: " + (predmet.NeophodnaPametnaTabla ? "neophodna" : "nije neophodna"));
                sb.Append("\nSoftveri:");
                foreach(string softver in predmet.Softveri)
                {
                    sb.Append("\n\t" + softver);
                }
                PredmetDetaljanPrikazBox.Text = sb.ToString();
            }
        }

        private void detaljanPrikazSmera()
        {
            if (tabelaSmerova.SelectedIndex != -1)
            {
                DataGridRow selektovaniRed = (DataGridRow)tabelaSmerova.ItemContainerGenerator.ContainerFromIndex(tabelaSmerova.SelectedIndex);
                TextBlock content = tabelaSmerova.Columns[1].GetCellContent(selektovaniRed) as TextBlock;
                string oznakaSmera = content.Text;

                Smer smer = racunarskiCentar.Smerovi[oznakaSmera];
                StringBuilder sb = new StringBuilder();
                sb.Append("Oznaka: " + smer.Oznaka);
                sb.Append("\nNaziv: " + smer.Naziv);
                sb.Append("\nDatum uvođenja: " + smer.Datum);
                sb.Append("\nOpis: " + smer.Opis);
                SmerDetaljanPrikazBox.Text = sb.ToString();
            }
        }

        private void detaljanPrikazSoftvera()
        {
            if (tabelaSoftvera.SelectedIndex != -1)
            {
                DataGridRow selektovaniRed = (DataGridRow)tabelaSoftvera.ItemContainerGenerator.ContainerFromIndex(tabelaSoftvera.SelectedIndex);
                TextBlock content = tabelaSoftvera.Columns[1].GetCellContent(selektovaniRed) as TextBlock;
                string oznakaSoftver = content.Text;

                Softver softver = racunarskiCentar.Softveri[oznakaSoftver];
                StringBuilder sb = new StringBuilder();
                sb.Append("Oznaka: " + softver.Oznaka);
                sb.Append("\nNaziv: " + softver.Naziv);
                sb.Append("\nOpis: " + softver.Opis);
                sb.Append("\nGodina izdavanja: " + softver.GodIzdavanja);
                sb.Append("\nProizvođač: " + softver.Proizvodjac);
                sb.Append("\nSajt: " + softver.Sajt);
                sb.Append("\nCena: " + softver.Cena);
                sb.Append("\nOperativni sistem: " + softver.OperativniSistem);
                SoftverDetaljanPrikazBox.Text = sb.ToString();
            }
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
            var brisanjeProzor = new PotvrdaBrisanja();
            brisanjeProzor.ShowDialog();
            if (brisanjeProzor.daKlik)
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
                
                racunarskiCentar.Predmeti[oznakaPredmeta].Obrisan = true;
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
                //provera da li se nalazi u nekom predmetu, ako se nalazi, sprecava se brisanje
                bool koristiSeUPredmetu = false;
                foreach(Predmet p in racunarskiCentar.Predmeti.Values)
                {
                    if (!p.Obrisan)
                    {
                        foreach (string s in p.Softveri)
                        {
                            if (s == oznakaSoftvera)
                                koristiSeUPredmetu = true;
                        }
                    }
                }
                if (koristiSeUPredmetu)
                {
                    MessageBox.Show("Ne možete obrisati softver, koristi se u nekom predmetu!");
                    return;
                }
                //provera da li se nalazi u nekoj ucionici, ako se nalazi, sprecava se brisanje
                bool koristiSeUucionici = false;
                foreach (Ucionica u in racunarskiCentar.Ucionice.Values)
                {
                    if (!u.Obrisan)
                    {
                        foreach (string s in u.InstaliraniSoftveri)
                        {
                            if (s == oznakaSoftvera)
                                koristiSeUucionici = true;
                        }
                    }
                }
                if (koristiSeUucionici)
                {
                    MessageBox.Show("Ne možete obrisati softver, koristi se u nekoj učionici!");
                    return;
                }
                softveriKolekcija.Remove(racunarskiCentar.Softveri[oznakaSoftvera]);
                
                racunarskiCentar.Softveri[oznakaSoftvera].Obrisan = true;
            }
            else
                return;
        }

        private void obrisiUcionicuClick(object sender, RoutedEventArgs e)
        {
            if (tabelaUcionica.SelectedIndex != -1)
            {
                DataGridRow selektovaniRed = (DataGridRow)tabelaUcionica.ItemContainerGenerator.ContainerFromIndex(tabelaUcionica.SelectedIndex);
                TextBlock content = tabelaUcionica.Columns[0].GetCellContent(selektovaniRed) as TextBlock;
                string oznakaUcionice = content.Text;

                ucioniceKolekcija.Remove(racunarskiCentar.Ucionice[oznakaUcionice]);
                
                racunarskiCentar.Ucionice[oznakaUcionice].Obrisan = true;
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
                //provera da li se nalazi u nekom predmetu, ako se nalazi, sprecava se brisanje
                bool koristiSeUPredmetu = false;
                foreach (Predmet p in racunarskiCentar.Predmeti.Values)
                {
                    if (!p.Obrisan)
                    {
                        if (p.Smer.Oznaka == oznakaSmera)
                            koristiSeUPredmetu = true;
                    }
                }
                if (koristiSeUPredmetu)
                {
                    MessageBox.Show("Ne možete obrisati smer, sadrži neke predmete!");
                    return;
                }
                smeroviKolekcija.Remove(racunarskiCentar.Smerovi[oznakaSmera]);
                
                racunarskiCentar.Smerovi[oznakaSmera].Obrisan = true;
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
