using CefSharp;
using CefSharp.Wpf;
using System.Collections.Generic;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.Windows.Threading;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Specialized;

namespace HCI_Projekat
{
    public class CefCustomObject
    {
        private static ChromiumWebBrowser _instanceBrowser = null;
        private MainWindow _instanceWindow = null;
        private RacunarskiCentar racunarskiCentar = null;
        private Notifier not;
        private UndoRedoStack stekStanja;
        OrderedDictionary prethodnaStanjaAplikacije;

        public CefCustomObject(ChromiumWebBrowser originalBrowser, MainWindow mainWindow, RacunarskiCentar racunarskiCentar, Notifier not, UndoRedoStack stekStanja, OrderedDictionary prethodnaStanja)
        {
            _instanceBrowser = originalBrowser;
            _instanceWindow = mainWindow;
            this.racunarskiCentar = racunarskiCentar;
            this.not = not;
            this.stekStanja = stekStanja;
            this.prethodnaStanjaAplikacije = prethodnaStanja;
        }

        public RacunarskiCentar RacunarskiCentar
        {
            get { return this.racunarskiCentar; }
            set { this.racunarskiCentar = value; }
        }

        public void showDevTools()
        {
            _instanceBrowser.ShowDevTools();
        }

        public void posaljiPodatke()
        {   //ucionice
            int i = 0;
            string ucionice = "{\"ucionice\":[";
            foreach (Ucionica u in racunarskiCentar.Ucionice.Values)
            {
                if (!u.Obrisan)
                {
                    ucionice += "{\"oznaka\":\"" + u.Oznaka + "\",\"tabla\":\"" + u.PrisustvoTable + "\",\"pametnaTabla\":\"" +
                          u.PrisustvoPametneTable + "\",\"projektor\":\"" + u.PrisustvoProjektora + "\",\"brojMesta\":\"" + u.BrojRadnihMesta +
                          "\",\"os\":\"" + u.OperativniSistem + "\",\"softveri\":[";
                    int soft = 0;
                    foreach (string s in u.InstaliraniSoftveri)
                    {
                        ucionice += "{\"oznaka\":\"" + s + "\"},";
                        soft++;
                    }
                    if (soft != 0)
                    {
                        ucionice = ucionice.Substring(0, ucionice.Length - 1);
                    }
                    ucionice += "]},";

                    i++;
                }
            }
            if (i != 0)
                ucionice = ucionice.Substring(0, ucionice.Length - 1);
            ucionice += "]}";
            //smerovi
            //puni nazivi smerova i predmeta
            string nazivi = "{\"smerovi\":[";
            string smerovi = "";
            int k = 0;
            Smer prviSmer = new Smer();
            foreach (Smer s in racunarskiCentar.Smerovi.Values)
            {
                if (!s.Obrisan)
                {
                    if (k == 0)
                        prviSmer = s;
                    nazivi += "{\"oznaka\":\"" + s.Oznaka + "\",\"naziv\":\"" + s.Naziv + "\"},";
                    smerovi += s.Oznaka + '|';
                    k++;
                }

            }
            //predmeti
            nazivi = nazivi.Substring(0, nazivi.Length - 1);
            nazivi += "],\"predmeti\":[";
            List<Predmet> neobrisaniPredmeti = new List<Predmet>();
            foreach (Predmet p in racunarskiCentar.Predmeti.Values)
            {
                if (!p.Obrisan)
                    neobrisaniPredmeti.Add(p);
            }
            int j = 0;
            foreach (Predmet p in neobrisaniPredmeti)
            {
                nazivi += "{\"oznaka\":\"" + p.Oznaka + "\",\"naziv\":\"" + p.Naziv + "\"},";
                j++;
            }
            if (j != 0)
                nazivi = nazivi.Substring(0, nazivi.Length - 1);
            nazivi += "]}";

            //predmet za prvi smer
            string predmeti = "{\"predmeti\":[";
            int b = 0;
            foreach (string pr in prviSmer.Predmeti)
            {
                foreach (Predmet p in neobrisaniPredmeti)
                {
                    if (p.Oznaka == pr)
                    {
                        predmeti += "{\"oznaka\":\"" + p.Oznaka + "\",\"duzina\":\"" + p.MinDuzinaTermina + "\",\"termini\":\"" + p.PreostaliTermini +
                                "\",\"tabla\":\"" + p.NeophodnaTabla + "\",\"pametnaTabla\":\"" + p.NeophodnaPametnaTabla + "\",\"projektor\":\"" +
                                p.NeophodanProjektor + "\",\"brojMesta\":\"" + p.VelicinaGrupe + "\",\"os\":\"" + p.OperativniSistem + "\"},";
                        b++;
                    }

                }
            }
            if (b != 0)
                predmeti = predmeti.Substring(0, predmeti.Length - 1);
            predmeti += "]}";

            //svi predmeti koji postoje
            string sviPredmeti = "{\"predmeti\":[";
            int l = 0;
            foreach (Predmet p in neobrisaniPredmeti)
            {
                sviPredmeti += "{\"oznaka\":\"" + p.Oznaka + "\",\"duzina\":\"" + p.MinDuzinaTermina + "\",\"termini\":\"" + p.PreostaliTermini +
                        "\",\"tabla\":\"" + p.NeophodnaTabla + "\",\"pametnaTabla\":\"" + p.NeophodnaPametnaTabla + "\",\"projektor\":\"" +
                        p.NeophodanProjektor + "\",\"brojMesta\":\"" + p.VelicinaGrupe + "\",\"os\":\"" + p.OperativniSistem + "\", \"softveri\":[";
                int brSoft = 0;
                foreach (string s in p.Softveri)
                {
                    sviPredmeti += "{\"oznaka\":\"" + s + "\"},";
                    brSoft++;
                }
                if (brSoft != 0)
                    sviPredmeti = sviPredmeti.Substring(0, sviPredmeti.Length - 1);
                sviPredmeti += "]},";
                l++;
            }

            if (l != 0)
                sviPredmeti = sviPredmeti.Substring(0, sviPredmeti.Length - 1);
            sviPredmeti += "]}";

            if (_instanceBrowser.CanExecuteJavascriptInMainFrame)
            {
                _instanceBrowser.ExecuteScriptAsync("ucitajUcionice('" + ucionice + "');");
                _instanceBrowser.ExecuteScriptAsync("ucitajPredmete('" + predmeti + "');");
                _instanceBrowser.ExecuteScriptAsync("ucitajSmerove('" + smerovi + "');");
                _instanceBrowser.ExecuteScriptAsync("ucitajNazive('" + nazivi + "');");
                _instanceBrowser.ExecuteScriptAsync("ucitajSvePredmete('" + sviPredmeti + "');");
            }
            ucitajPolja();
        }

        public void ucitajPolja()
        {
            //polja kalendara 
            string polja = "{\"lista\":[";
            int i = 0;
            foreach (KalendarPolje kal in racunarskiCentar.PoljaKalendara.Values)
            {
                polja += "{\"id\":\"" + kal.Id + "\",\"pocetak\":\"" + kal.Pocetak + "\",\"kraj\":\"" + kal.Kraj + "\",\"naziv\":\"" +
                        kal.NazivPolja + "\",\"dan\":\"" + kal.Dan + "\",\"ucionica\":\"" + kal.Ucionica + "\"},";
                i++;
            }
            if (i != 0)
                polja = polja.Substring(0, polja.Length - 1);
            polja += "]}";

            if (_instanceBrowser.CanExecuteJavascriptInMainFrame)
            {
                _instanceBrowser.ExecuteScriptAsync("ucitajPostojecaPolja('" + polja + "');");
            }
        }

        public void dobaviSmerovePredmeta(string predmet)
        {
            if (_instanceBrowser.CanExecuteJavascriptInMainFrame)
            {
                _instanceBrowser.ExecuteScriptAsync("ucitajSmer('" + racunarskiCentar.Predmeti[predmet].Smer + "');");
            }
        }

        public void getEvent(string id, string naziv, string pocetak, string kraj, string dan, string ucionica, bool dodat)
        {
            // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
            StanjeAplikacije staroStanje = new StanjeAplikacije(DeepClone(racunarskiCentar), "Dodat novi termin na kalendar", "kalendar");

            if (!racunarskiCentar.PoljaKalendara.ContainsKey(id))
            {
                racunarskiCentar.PoljaKalendara.Add(id, new KalendarPolje(id, naziv, pocetak, kraj, dan, ucionica));
            }
            else
            {
                KalendarPolje polje = racunarskiCentar.PoljaKalendara[id];
                polje.Pocetak = pocetak;
                polje.Kraj = kraj;
                polje.Dan = dan;
                polje.Ucionica = ucionica;
            }
            if (dodat)
            {
                Predmet p = racunarskiCentar.Predmeti[naziv.Split('-')[0]];
                p.PreostaliTermini--;
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

            MessageBox.Show("Br el na steku za undo: " + stekStanja.GetUndo().Count.ToString());
            // omogucavamo pozivanje opcije undo
            _instanceWindow.omoguciUndo();
        }

        public void obrisiPolje(string id, string oznakaPredmeta)
        {
            // pamtimo stanje alikacije pre nego sto uradimo dodavanje novog
            StanjeAplikacije staroStanje = new StanjeAplikacije(DeepClone(racunarskiCentar), "Obrisan termin sa kalendara", "kalendar");

            racunarskiCentar.Predmeti[oznakaPredmeta].PreostaliTermini++;
            racunarskiCentar.PoljaKalendara.Remove(id);

            // na undo stek treba da upisemo staro stanje aplikacije
            // generisemo neki novi kljuc pod kojim cemo cuvati prethodno stanje na steku
            string kljuc = Guid.NewGuid().ToString();
            // proveravamo da li vec ima 10 koraka za undo operaciju, ako ima, izbacujemo prvi koji je ubacen kako bismo 
            // i dalje imali 10 mogucih koraka, ali ukljucujuci i ovaj novi
            if (prethodnaStanjaAplikacije.Count >= 2)
                prethodnaStanjaAplikacije.RemoveAt(0);
            prethodnaStanjaAplikacije.Add(kljuc, staroStanje);
            stekStanja.GetUndo().Push(kljuc);

            MessageBox.Show("Br el na steku za undo: " + stekStanja.GetUndo().Count.ToString());
            // omogucavamo pozivanje opcije undo
            _instanceWindow.omoguciUndo();
        }

        public void dobaviPredmeteSmera(string smer)
        {
            //predmet za prvi smer
            string predmeti = "{\"predmeti\":[";
            Smer odabraniSmer = null;

            foreach (Smer s in racunarskiCentar.Smerovi.Values)
            {
                if (!s.Obrisan && (s.Oznaka == smer))
                    odabraniSmer = s;
            }
            if (odabraniSmer != null)
            {
                foreach (string pr in odabraniSmer.Predmeti)
                {
                    foreach (Predmet p in racunarskiCentar.Predmeti.Values)
                    {
                        if ((p.Oznaka == pr) && !p.Obrisan)
                        {
                            predmeti += "{\"oznaka\":\"" + p.Oznaka + "\",\"duzina\":\"" + p.MinDuzinaTermina + "\",\"termini\":\"" + p.PreostaliTermini +
                                    "\",\"tabla\":\"" + p.NeophodnaTabla + "\",\"pametnaTabla\":\"" + p.NeophodnaPametnaTabla + "\",\"projektor\":\"" +
                                    p.NeophodanProjektor + "\",\"brojMesta\":\"" + p.VelicinaGrupe + "\",\"os\":\"" + p.OperativniSistem + "\"},";
                        }

                    }
                }
            }
            if (odabraniSmer.Predmeti.Count != 0)
                predmeti = predmeti.Substring(0, predmeti.Length - 1);
            predmeti += "]}";
            if (_instanceBrowser.CanExecuteJavascriptInMainFrame)
            {
                _instanceBrowser.ExecuteScriptAsync("ucitajPredmete('" + predmeti + "');");
            }
        }

        public void alert(string tekst)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                not.ShowError(tekst);
            });
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
