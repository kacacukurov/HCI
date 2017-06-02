using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
        private bool izmena;
        public int indeks;

        public DodavanjePredmeta(RacunarskiCentar racunarskiCentar, ObservableCollection<Predmet> predmeti, bool izmena)
        {
            predmet = new Predmet();
            this.racunarskiCentar = racunarskiCentar;
            this.izmena = izmena;
            tabelaPredmeta = predmeti;
            
            InitializeComponent();
            List<Smer> smerovi = new List<Smer>();
            foreach(Smer s in racunarskiCentar.Smerovi.Values)
            {
                if (!s.Obrisan)
                {
                    s.UPredmetu = false;
                    smerovi.Add(s);
                }
            }
            smeroviTabela.ItemsSource = smerovi;
            smeroviTabela.IsSynchronizedWithCurrentItem = true;

            List<Softver> softveri = new List<Softver>();
            foreach (Softver s in racunarskiCentar.Softveri.Values)
            {
                if (!s.Obrisan)
                {
                    s.Instaliran = false;
                    softveri.Add(s);
                }
            }
            softverTabela.ItemsSource = softveri;
            softverTabela.IsSynchronizedWithCurrentItem = true;
            if(!izmena)
                OznakaPredmeta.Focus();
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
            Korak2Predmet.Focus();
        }

        public void backClick(object sender, RoutedEventArgs e)
        {
            BackStepMenuItem.IsEnabled = false;
            NextStepMenuItem.IsEnabled = true;
            Korak1Predmet.Focus();
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void finishClick(object sender, RoutedEventArgs e)
        {
            quitButton.Focus();
            finishButton.Focus();
            if (izmena) { 
                izmenaPredmeta();
                return;
            }
            if (validacijaNovogPredmeta())
            {
                predmet.Oznaka = OznakaPredmeta.Text;
                predmet.Naziv = NazivPredmeta.Text;
                predmet.Opis = OpisPredmeta.Text;
                predmet.VelicinaGrupe = int.Parse(VelicinaGrupePredmet.Text);
                predmet.MinDuzinaTermina = int.Parse(DuzinaTerminaPredmet.Text);
                predmet.BrTermina = int.Parse(BrojTerminaPredmet.Text);
                predmet.PreostaliTermini = predmet.BrTermina;

                predmet.NeophodanProjektor = PrisustvoProjektoraPredmet.IsChecked;
                predmet.ProjektorString = predmet.NeophodanProjektor ? "neophodan" : "nije neophodan";
                predmet.NeophodnaTabla = PrisustvoTablePredmet.IsChecked;
                predmet.TablaString = predmet.NeophodnaTabla ? "neophodna" : "nije neophodna";
                predmet.NeophodnaPametnaTabla = PrisustvoPametneTable.IsChecked;
                predmet.PametnaTablaString = predmet.NeophodnaPametnaTabla ? "neophodna" : "nije neophodna";

                if ((bool)Windows.IsChecked)
                    predmet.OperativniSistem = Windows.Content.ToString();
                if ((bool)Linux.IsChecked)
                    predmet.OperativniSistem = Linux.Content.ToString();
                if ((bool)Svejedno.IsChecked)
                    predmet.OperativniSistem = Svejedno.Content.ToString();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < softverTabela.Items.Count; i++)
                {
                    Softver softver = (Softver)softverTabela.Items[i];
                    if (softver.Instaliran)
                    {    
                        predmet.Softveri.Add(softver.Oznaka);
                        sb.Append("Oznaka softvera: " + softver.Oznaka);
                        sb.Append("\nNaziv softvera: " + softver.Naziv);
                        sb.Append("\nOpis softvera: " + softver.Opis);
                        softver.Instaliran = false;
                    }
                }
                predmet.SoftveriLista = sb.ToString();

                sb.Clear();
                for (int i = 0; i < smeroviTabela.Items.Count; i++)
                {
                    Smer smer = (Smer)smeroviTabela.Items[i];
                    if (smer.UPredmetu)
                    {
                        predmet.Smer = smer.Oznaka;
                        smer.Predmeti.Add(predmet.Oznaka);
                        
                        sb.Append("\n\n");
                        sb.Append("Oznaka: " + smer.Oznaka);
                        sb.Append("\nNaziv: " + smer.Naziv);
                        smer.UPredmetu = false;
                        break;
                    }
                }
                predmet.SmerDetalji = sb.ToString();

                tabelaPredmeta.Add(predmet);
                racunarskiCentar.DodajPredmet(predmet);
                this.Close();
            }
        }

        private bool validacijaNovogPredmeta()
        {
            if (racunarskiCentar.Predmeti.ContainsKey(OznakaPredmeta.Text))
            {
                if (racunarskiCentar.Predmeti[OznakaPredmeta.Text].Obrisan)
                    racunarskiCentar.Predmeti.Remove(OznakaPredmeta.Text);
                else
                {
                    MessageBox.Show("Predmet sa ovom oznakom već postoji!");
                    OznakaPredmeta.Text = "";
                    OznakaPredmeta.Focus();
                    return false;
                }
            }else if (!validacijaPodataka()) {
                return false;
            }
            return true;
        }

        private bool validacijaPodataka()
        {
            if (OznakaPredmeta.Text == "" || NazivPredmeta.Text == "" || OpisPredmeta.Text == "" ||
                BrojTerminaPredmet.Text == "" || VelicinaGrupePredmet.Text == "" || DuzinaTerminaPredmet.Text == "")
            {
                MessageBox.Show("Niste popunili sva polja!");
                if (OznakaPredmeta.Text == "")
                    OznakaPredmeta.Focus();
                else if (NazivPredmeta.Text == "")
                    NazivPredmeta.Focus();
                else if (OpisPredmeta.Text == "")
                    OpisPredmeta.Focus();
                else if (BrojTerminaPredmet.Text == "")
                    BrojTerminaPredmet.Focus();
                else if (VelicinaGrupePredmet.Text == "")
                    VelicinaGrupePredmet.Focus();
                else if (DuzinaTerminaPredmet.Text == "")
                    DuzinaTerminaPredmet.Focus();
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
                MessageBox.Show("Niste oznacili potreban softver!");
                return false;
            }
            bool postojiSmer = false;
            for (int i = 0; i < smeroviTabela.Items.Count; i++)
            {
                Smer smer = (Smer)smeroviTabela.Items[i];
                if (smer.UPredmetu)
                    postojiSmer = true;
            }
            if (!postojiSmer)
            {
                MessageBox.Show("Niste oznacili nijedan smer!");
                return false;
            }
            return true;
        }

        private void izmenaPredmeta()
        {
            if (validacijaPodataka())
            {
                Predmet predmetIzmena = racunarskiCentar.Predmeti[OznakaPredmeta.Text];
                predmetIzmena.Naziv = NazivPredmeta.Text;
                predmetIzmena.Opis = OpisPredmeta.Text;
                predmetIzmena.VelicinaGrupe = int.Parse(VelicinaGrupePredmet.Text);
                predmetIzmena.MinDuzinaTermina = int.Parse(DuzinaTerminaPredmet.Text);
                predmetIzmena.BrTermina = int.Parse(BrojTerminaPredmet.Text);

                predmetIzmena.NeophodanProjektor = PrisustvoProjektoraPredmet.IsChecked;
                predmetIzmena.ProjektorString = predmetIzmena.NeophodanProjektor ? "neophodan" : "nije neophodan";
                predmetIzmena.NeophodnaTabla = PrisustvoTablePredmet.IsChecked;
                predmetIzmena.TablaString = predmetIzmena.NeophodnaTabla ? "neophodna" : "nije neophodna";
                predmetIzmena.NeophodnaPametnaTabla = PrisustvoPametneTable.IsChecked;
                predmetIzmena.PametnaTablaString = predmetIzmena.NeophodnaPametnaTabla ? "neophodna" : "nije neophodna";

                if ((bool)Windows.IsChecked)
                    predmetIzmena.OperativniSistem = Windows.Content.ToString();
                else if ((bool)Linux.IsChecked)
                    predmetIzmena.OperativniSistem = Linux.Content.ToString();
                else if ((bool)Svejedno.IsChecked)
                    predmetIzmena.OperativniSistem = Svejedno.Content.ToString();

                StringBuilder sb = new StringBuilder();
                predmetIzmena.Softveri.Clear();
                for (int i = 0; i < softverTabela.Items.Count; i++)
                {
                    Softver softver = (Softver)softverTabela.Items[i];
                    if (softver.Instaliran)
                    {
                        predmetIzmena.Softveri.Add(softver.Oznaka);
                        sb.Append("Oznaka softvera: " + softver.Oznaka);
                        sb.Append("\nNaziv softvera: " + softver.Naziv);
                        sb.Append("\nOpis softvera: " + softver.Opis);
                        softver.Instaliran = false;
                    }
                }
                predmetIzmena.SoftveriLista = sb.ToString();

                sb.Clear();
                bool stariSmerPronadjen = false;
                bool noviPredmetPostavljen = false;
                for (int i = 0; i < smeroviTabela.Items.Count; i++)
                {
                    Smer smer = (Smer)smeroviTabela.Items[i];
                    //iz smera za koji je bio vezan predmet uklanjamo vezu
                    if (smer.Predmeti.Contains(predmetIzmena.Oznaka))
                    {
                        smer.Predmeti.Remove(predmetIzmena.Oznaka);
                        stariSmerPronadjen = true;
                    }
                    if (smer.UPredmetu)
                    {
                        //u listu predmeta novoizabranog smera 
                        if (!smer.Predmeti.Contains(predmetIzmena.Oznaka))
                            smer.Predmeti.Add(predmetIzmena.Oznaka);
                        predmetIzmena.Smer = smer.Oznaka;

                        sb.Append("\n\n");
                        sb.Append("Oznaka: " + smer.Oznaka);
                        sb.Append("\nNaziv: " + smer.Naziv);
                        smer.UPredmetu = false;
                        noviPredmetPostavljen = true;
                    }

                    if (stariSmerPronadjen && noviPredmetPostavljen)
                        break;
                }
                predmetIzmena.SmerDetalji = sb.ToString();

                tabelaPredmeta[indeks] = predmetIzmena;
                this.Close();
            }
        }
    }
}
