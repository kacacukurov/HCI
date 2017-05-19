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
    /// Interaction logic for DodavanjeUcionice.xaml
    /// </summary>
    public partial class DodavanjeUcionice : Window
    {
        private Ucionica novaUcionica;
        private RacunarskiCentar racunarskiCentar;
        private ObservableCollection<Ucionica> tabelaUcionica;

        public DodavanjeUcionice(RacunarskiCentar racunarskiCentar, ObservableCollection<Ucionica> ucionice)
        {
            InitializeComponent();
            novaUcionica = new Ucionica();
            this.racunarskiCentar = racunarskiCentar;
            List<Softver> softveri = racunarskiCentar.Softveri.Values.ToList();
            softverTabela.ItemsSource = softveri;
            softverTabela.IsSynchronizedWithCurrentItem = true;
            tabelaUcionica = ucionice;
            oznakaUcionica.Focus();
        }

        private void nextClick(object sender, RoutedEventArgs e)
        {
            Korak2Ucionica.Focus();
        }

        private void backClick(object sender, RoutedEventArgs e)
        {
            Korak1Ucionica.Focus();
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void finishClick(object sender, RoutedEventArgs e)
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
                novaUcionica.OperativniSistem = "Linux i Windows";

            for(int i=0; i<softverTabela.Items.Count; i++)
            {
                DataGridRow row = (DataGridRow)softverTabela.ItemContainerGenerator.ContainerFromIndex(i);
                CheckBox box = softverTabela.Columns[3].GetCellContent(row) as CheckBox;
                if((bool)box.IsChecked)
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
}
