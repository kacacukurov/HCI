using System;
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

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RacunarskiCentar racunarskiCentar;
        private static string imeFajla = "racunarskiCentar.xml";

        public MainWindow()
        {
            InitializeComponent();
            KalendarTab.Focus();

            racunarskiCentar = new RacunarskiCentar();
            DeserijalizacijaPodataka();
            Console.WriteLine(racunarskiCentar.Softveri.Count);

            tabelaPredmeta.ItemsSource = racunarskiCentar.Predmeti.Values.ToList();
            tabelaPredmeta.IsSynchronizedWithCurrentItem = true;
        }

        private void dodavanjeUcioniceClick(object sender, RoutedEventArgs e)
        {
            var ucionicaWindow = new DodavanjeUcionice(racunarskiCentar);
            ucionicaWindow.ShowDialog();
        }

        private void dodavanjePredmetaClick(object sender, RoutedEventArgs e)
        {
            var predmetWindow = new DodavanjePredmeta(racunarskiCentar);
            predmetWindow.ShowDialog();
            tabelaPredmeta.ItemsSource = racunarskiCentar.Predmeti.Values.ToList();
        }

        private void dodavanjeSmeraClick(object sender, RoutedEventArgs e)
        {
            var smerWindow = new DodavanjeSmera(racunarskiCentar);
            smerWindow.ShowDialog();
        }

        private void dodavanjeSoftveraClick(object sender, RoutedEventArgs e)
        {
            var softverWindow = new DodavanjeSoftvera(racunarskiCentar);
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

        private void IzmeniPredmet (object sender, RoutedEventArgs e)
        {
            if (tabelaPredmeta.SelectedIndex != -1)
                MessageBox.Show(tabelaPredmeta.SelectedIndex.ToString());
        }
    }
}
