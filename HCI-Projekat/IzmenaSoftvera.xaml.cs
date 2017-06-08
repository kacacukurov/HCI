using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
        public bool potvrdaIzmena;
        private UndoRedoStack stekStanja;
        OrderedDictionary prethodnaStanjaAplikacije;
        private Notifier notifierError;

        public IzmenaSoftvera(RacunarskiCentar racunarskiCentar, ObservableCollection<Softver> softveri, List<int> indeksi, UndoRedoStack stek, OrderedDictionary prethodnaStanja)
        {
            notifierError = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: this,
                    corner: Corner.TopRight,
                    offsetX: 20,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: System.TimeSpan.FromSeconds(5),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(1));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });

            InitializeComponent();
            this.potvrdaIzmena = false;
            this.stekStanja = stek;
            this.prethodnaStanjaAplikacije = prethodnaStanja;
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
                // pamtimo stanje alikacije pre nego sto uradimo izmenu smerova
                StanjeAplikacije staroStanje = new StanjeAplikacije(DeepClone(racunarskiCentar), "Izmenjena grupa softvera", "softver");

                List<string> softveriIzmenjenogNazivaIliOpisa = new List<string>();
                foreach (int index in indeksiZaIzmenu)
                {
                    Softver softverIzmena = racunarskiCentar.Softveri[tabelaSoftvera[index].Oznaka];
                    
                    if (nazivSoftver.Text.Trim() != "")
                    {
                        if (!softverIzmena.Naziv.Equals(nazivSoftver.Text.Trim()))
                        {
                            softveriIzmenjenogNazivaIliOpisa.Add(softverIzmena.Oznaka);
                        }
                        softverIzmena.Naziv = nazivSoftver.Text.Trim();
                    }

                    if (opisSoftver.Text.Trim() != "")
                    {
                        if (!softverIzmena.Opis.Equals(opisSoftver.Text.Trim()))
                        {
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

                // na undo stek treba da upisemo staro stanje aplikacije
                // generisemo neki novi kljuc pod kojim cemo cuvati prethodno stanje na steku
                string kljuc = Guid.NewGuid().ToString();
                // proveravamo da li vec ima 10 koraka za undo operaciju, ako ima, izbacujemo prvi koji je ubacen kako bismo 
                // i dalje imali 10 mogucih koraka, ali ukljucujuci i ovaj novi
                if (prethodnaStanjaAplikacije.Count >= 2)
                    prethodnaStanjaAplikacije.RemoveAt(0);
                prethodnaStanjaAplikacije.Add(kljuc, staroStanje);
                stekStanja.GetUndo().Push(kljuc);
                // postavljamo flag na true, da bismo mogli da omogucimo klik na dugme za undo operaciju
                potvrdaIzmena = true;

                this.Close();
            }
        }
        
        private bool validacijaPodataka()
        {
            if (nazivSoftver.Text.Trim() == "" && proizvodjacSoftver.Text.Trim() == "" && opisSoftver.Text.Trim() == ""
                && godinaSoftver.Text.Trim() == "" && cenaSoftver.Text.Trim() == "" && sajtSoftver.Text.Trim() == "")
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Niste popunili nijedno polje!");
                });
                if (nazivSoftver.Text.Trim() == "")
                    nazivSoftver.Focus();

                return false;
            }

            int godina;
            if (godinaSoftver.Text.Trim() != "" && !int.TryParse(godinaSoftver.Text.Trim(), out godina))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Godina nije dobro unesena, unesite broj!");
                });
                godinaSoftver.Text = "";
                godinaSoftver.Focus();
                return false;
            }

            double cena;
            if (cenaSoftver.Text.Trim() != "" && !double.TryParse(cenaSoftver.Text.Trim(), out cena))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    notifierError.ShowError("Cena nije dobro unesena, unesite broj!");
                });
                cenaSoftver.Text = "";
                cenaSoftver.Focus();
                return false;
            }

            return true;
        }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
