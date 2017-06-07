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
    /// Interaction logic for IzmenaSoftvera.xaml
    /// </summary>
    public partial class IzmenaSoftvera : Window
    {
        private RacunarskiCentar racunarskiCentar;
        private ObservableCollection<Softver> tabelaSoftvera;
        private List<int> indeksiZaIzmenu;

        public IzmenaSoftvera(RacunarskiCentar racunarskiCentar, ObservableCollection<Softver> softveri, List<int> indeksi)
        {
            InitializeComponent();
            this.racunarskiCentar = racunarskiCentar;
            this.indeksiZaIzmenu = indeksi;
            tabelaSoftvera = softveri;
            nazivSoftver.Focus();
        }

        private void undoClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Undo");
        }

        private void redoClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Redo");
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void proveriUnetuGodinu(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (t.Text.Trim().Equals(string.Empty))
                GreskaGodinaSoftver.Text = "";
            else {
                try
                {
                    int godina = Int32.Parse(t.Text.Trim());
                    if (godina <= 0)
                        GreskaGodinaSoftver.Text = "Unesite ceo pozitivan broj!";
                    else
                        GreskaGodinaSoftver.Text = "";
                }
                catch
                {
                    GreskaGodinaSoftver.Text = "Unesite ceo pozitivan broj!";
                }
            }
        }

        private void proveriUnetuCenu(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            if (t.Text.Trim().Equals(string.Empty))
                GreskaCenaSoftver.Text = "";
            else {
                try
                {
                    double cena = Double.Parse(t.Text.Trim());
                    if (cena <= 0)
                        GreskaCenaSoftver.Text = "Unesite realan pozitivan broj!";
                    else
                        GreskaCenaSoftver.Text = "";
                }
                catch
                {
                    GreskaCenaSoftver.Text = "Unesite realan pozitivan broj!";
                }
            }
        }

        private void finishClick(object sender, RoutedEventArgs e)
        {
            if (validacijaPodataka())
            {
                List<string> softveriIzmenjenogNazivaIliOpisa = new List<string>();
                foreach (int index in indeksiZaIzmenu)
                {
                    Softver softverIzmena = racunarskiCentar.Softveri[tabelaSoftvera[index].Oznaka];
                    bool promenioSeNaziv = false;
                    bool promenioSeOpis = false;
                    
                    if (nazivSoftver.Text.Trim() != "")
                    {
                        if (!softverIzmena.Naziv.Equals(nazivSoftver.Text.Trim()))
                        {
                            promenioSeNaziv = true;
                            softveriIzmenjenogNazivaIliOpisa.Add(softverIzmena.Oznaka);
                        }
                        softverIzmena.Naziv = nazivSoftver.Text.Trim();
                    }

                    if (opisSoftver.Text.Trim() != "")
                    {
                        if (!softverIzmena.Opis.Equals(opisSoftver.Text.Trim()))
                        {
                            promenioSeOpis = true;
                            softveriIzmenjenogNazivaIliOpisa.Add(softverIzmena.Oznaka);
                        }
                        softverIzmena.Opis = opisSoftver.Text.Trim();
                    }

                    if(godinaSoftver.Text.Trim() != "")
                        softverIzmena.GodIzdavanja = int.Parse(godinaSoftver.Text.Trim());
                    if(cenaSoftver.Text.Trim() != "")
                        softverIzmena.Cena = double.Parse(cenaSoftver.Text.Trim());
                    if(proizvodjacSoftver.Text.Trim() != "")
                        softverIzmena.Proizvodjac = proizvodjacSoftver.Text.Trim();
                    if(sajtSoftver.Text.Trim() != "")
                        softverIzmena.Sajt = sajtSoftver.Text.Trim();

                    tabelaSoftvera[index] = softverIzmena;
                }

                if (softveriIzmenjenogNazivaIliOpisa.Count > 0)
                {
                    // azurira se i ispis softvera za ucionicu/predmet
                    StringBuilder sb = new StringBuilder();
                    
                    List<string> obradjeneUcionice = new List<string>();
                    List<string> obradjeniPredmeti = new List<string>();
                    foreach(string oznaka in softveriIzmenjenogNazivaIliOpisa)
                    {
                        // azuriranje ispisa u ucionici ako postoji taj softver
                        foreach(Ucionica u in racunarskiCentar.Ucionice.Values)
                        {
                            if(!obradjeneUcionice.Contains(u.Oznaka) && u.InstaliraniSoftveri.Contains(oznaka))
                            {
                                foreach (string s in u.InstaliraniSoftveri)
                                {
                                    Softver softver = racunarskiCentar.Softveri[s];
                                    if (u.InstaliraniSoftveri.IndexOf(softver.Oznaka) != 0)
                                        sb.Append("\n");
                                    sb.Append("Oznaka: " + softver.Oznaka);
                                    sb.Append("\nNaziv: " + softver.Naziv);
                                    sb.Append("\nOpis: " + softver.Opis + "\n");
                                }
                                u.SoftveriLista = sb.ToString();
                                sb.Clear();
                                obradjeneUcionice.Add(u.Oznaka);
                            }
                        }

                        // azuriranje ispisa u predmetu ako postoji taj softver
                        foreach (Predmet p in racunarskiCentar.Predmeti.Values)
                        {
                            if (!obradjeniPredmeti.Contains(p.Oznaka) && p.Softveri.Contains(oznaka))
                            {
                                foreach (string s in p.Softveri)
                                {
                                    Softver softver = racunarskiCentar.Softveri[s];
                                    if (p.Softveri.IndexOf(softver.Oznaka) != 0)
                                        sb.Append("\n");
                                    sb.Append("Oznaka: " + softver.Oznaka);
                                    sb.Append("\nNaziv: " + softver.Naziv);
                                    sb.Append("\nOpis: " + softver.Opis + "\n");
                                }
                                p.SoftveriLista = sb.ToString();
                                sb.Clear();
                                obradjeniPredmeti.Add(p.Oznaka);
                            }
                        }
                    }
                }
                this.Close();
            }
        }
        
        private bool validacijaPodataka()
        {
            if (nazivSoftver.Text.Trim() == "" && proizvodjacSoftver.Text.Trim() == "" && opisSoftver.Text.Trim() == ""
                && godinaSoftver.Text.Trim() == "" && cenaSoftver.Text.Trim() == "" && sajtSoftver.Text.Trim() == "")
            {
                MessageBox.Show("Niste popunili nijedno polje!");
                if (nazivSoftver.Text.Trim() == "")
                    nazivSoftver.Focus();

                return false;
            }

            int godina;
            if (godinaSoftver.Text.Trim() != "" && !int.TryParse(godinaSoftver.Text.Trim(), out godina))
            {
                MessageBox.Show("Godina nije dobro unesena, unesite broj!");
                godinaSoftver.Text = "";
                godinaSoftver.Focus();
                return false;
            }

            double cena;
            if (cenaSoftver.Text.Trim() != "" && !double.TryParse(cenaSoftver.Text.Trim(), out cena))
            {
                MessageBox.Show("Cena nije dobro unesena, unesite broj!");
                cenaSoftver.Text = "";
                cenaSoftver.Focus();
                return false;
            }

            return true;
        }
    }
}
