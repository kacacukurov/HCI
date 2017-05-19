using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for DodavanjePredmeta.xaml
    /// </summary>
    public partial class DodavanjePredmeta : Window
    {
        private Predmet predmet;
        private RacunarskiCentar racunarskiCentar;

        public DodavanjePredmeta(RacunarskiCentar racunarskiCentar)
        {
            predmet = new Predmet();
            this.racunarskiCentar = racunarskiCentar;
            
            InitializeComponent();
            ObservableCollection<string> listSmerovi = new ObservableCollection<string>();
            foreach (Smer s in racunarskiCentar.Smerovi.Values)
            {
                listSmerovi.Add(s.Oznaka);
            }
            if(listSmerovi.Count != 0)
            {
                this.SmerPredmeta.ItemsSource = listSmerovi;
                this.SmerPredmeta.Text = this.SmerPredmeta.Items.GetItemAt(0).ToString();
            }

            ObservableCollection<string> listSoftveri = new ObservableCollection<string>();
            foreach (Softver s in racunarskiCentar.Softveri.Values)
            {
                listSoftveri.Add(s.Oznaka);
            }
            if (listSoftveri.Count != 0)
            {
                this.SoftverPredmeta.ItemsSource = listSoftveri;
                this.SoftverPredmeta.Text = this.SoftverPredmeta.Items.GetItemAt(0).ToString();
            }
            OznakaPredmeta.Focus();
        }

        private void nextClickPredmet(object sender, RoutedEventArgs e)
        {
            Korak2Predmet.Focus();
        }

        private void backClickPredmet(object sender, RoutedEventArgs e)
        {
            Korak1Predmet.Focus();
        }

        private void exitClickPredmet(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void finishClickPredmet(object sender, RoutedEventArgs e)
        {
            if (racunarskiCentar.Predmeti.ContainsKey(OznakaPredmeta.Text))
            {
                MessageBox.Show("Predmet sa ovom oznakom vec postoji!");
                OznakaPredmeta.Text = "";
                OznakaPredmeta.Focus();
            }else
            {
                predmet.Oznaka = OznakaPredmeta.Text;
                predmet.Naziv = NazivPredmeta.Text;
                predmet.Smer = racunarskiCentar.Smerovi[SmerPredmeta.Text];
                predmet.Opis = OpisPredmeta.Text;
                predmet.VelicinaGrupe = int.Parse(VelicinaGrupePredmet.Text);
                predmet.MinDuzinaTermina = int.Parse(DuzinaTerminaPredmet.Text);
                predmet.BrTermina = int.Parse(BrojTerminaPredmet.Text);
                predmet.NeophodanProjektor = PrisustvoProjektoraPredmet.IsChecked;
                predmet.NeophodnaTabla = PrisustvoTablePredmet.IsChecked;
                predmet.NeophodnaPametnaTabla = PrisustvoPametneTable.IsChecked;
                if ((bool)Windows.IsChecked)
                    predmet.OperativniSistem = Windows.Content.ToString();
                else if ((bool)Linux.IsChecked)
                    predmet.OperativniSistem = Linux.Content.ToString();
                else if ((bool)Svejedno.IsChecked)
                    predmet.OperativniSistem = Svejedno.Content.ToString();
                predmet.Softver = racunarskiCentar.Softveri[SoftverPredmeta.Text];
                racunarskiCentar.DodajPredmet(predmet);
            }
            
            this.Close();
        }
    }
}
