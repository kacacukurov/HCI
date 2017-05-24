﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        public DodavanjePredmeta(RacunarskiCentar racunarskiCentar, ObservableCollection<Predmet> predmeti)
        {
            predmet = new Predmet();
            this.racunarskiCentar = racunarskiCentar;
            tabelaPredmeta = predmeti;
            
            InitializeComponent();
            ObservableCollection<string> listSmerovi = new ObservableCollection<string>();
            foreach (Smer s in racunarskiCentar.Smerovi.Values)
            {
                if(!s.Obrisan)
                    listSmerovi.Add(s.Oznaka);
            }
            if(listSmerovi.Count != 0)
            {
                this.SmerPredmeta.ItemsSource = listSmerovi;
                this.SmerPredmeta.Text = this.SmerPredmeta.Items.GetItemAt(0).ToString();
            }

            List<Softver> softveri = new List<Softver>();
            foreach (Softver s in racunarskiCentar.Softveri.Values)
            {
                if (!s.Obrisan)
                    softveri.Add(s);
            }
            softverTabela.ItemsSource = softveri;
            softverTabela.IsSynchronizedWithCurrentItem = true;
            OznakaPredmeta.Focus();
        }

        public void nextStep(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            nextClickPredmet(sender, e);
        }

        public void backStep(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            backClickPredmet(sender, e);
        }

        public void nextClickPredmet(object sender, RoutedEventArgs e)
        {
            Korak2Predmet.Focus();
        }

        public void backClickPredmet(object sender, RoutedEventArgs e)
        {
            Korak1Predmet.Focus();
        }

        private void exitClickPredmet(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void finishClickPredmet(object sender, RoutedEventArgs e)
        {
            if (validacijaNovogPredmeta())
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
                for (int i = 0; i < softverTabela.Items.Count; i++)
                {
                    DataGridRow row = (DataGridRow)softverTabela.ItemContainerGenerator.ContainerFromIndex(i);
                    CheckBox box = softverTabela.Columns[3].GetCellContent(row) as CheckBox;
                    if ((bool)box.IsChecked)
                    {
                        TextBlock content = softverTabela.Columns[1].GetCellContent(row) as TextBlock;
                        predmet.Softveri.Add(content.Text);
                    }
                }

                tabelaPredmeta.Add(predmet);
                racunarskiCentar.DodajPredmet(predmet);
                this.Close();
            }
        }

        private bool validacijaNovogPredmeta()
        {
            if (racunarskiCentar.Predmeti.ContainsKey(OznakaPredmeta.Text))
            {
                MessageBox.Show("Predmet sa ovom oznakom vec postoji!");
                OznakaPredmeta.Text = "";
                OznakaPredmeta.Focus();
                return false;
            }else if(OznakaPredmeta.Text == "" || NazivPredmeta.Text == "" || SmerPredmeta.Text == "" || OpisPredmeta.Text == "")
            {
                MessageBox.Show("Niste popunili sva polja!");
                if (OznakaPredmeta.Text == "")
                    OznakaPredmeta.Focus();
                else if (NazivPredmeta.Text == "")
                    NazivPredmeta.Focus();
                else if (SmerPredmeta.Text == "")
                    SmerPredmeta.Focus();
                else if (OpisPredmeta.Text == "")
                    OpisPredmeta.Focus();
                return false;
            }
            return true;
        }
    }
}
