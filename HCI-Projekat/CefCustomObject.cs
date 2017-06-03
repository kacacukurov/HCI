using CefSharp;
using CefSharp.Wpf;
using System.Collections.Generic;
using System.Windows;

namespace HCI_Projekat
{
    public class CefCustomObject
    {
        private static ChromiumWebBrowser _instanceBrowser = null;
        private static Window _instanceWindow = null;
        private RacunarskiCentar racunarskiCentar = null;

        public CefCustomObject(ChromiumWebBrowser originalBrowser, Window mainWindow, RacunarskiCentar racunarskiCentar)
        {
            _instanceBrowser = originalBrowser;
            _instanceWindow = mainWindow;
            this.racunarskiCentar = racunarskiCentar;
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
                    if (i + 1 == racunarskiCentar.Ucionice.Values.Count)
                    {
                        ucionice += "{\"oznaka\":\"" + u.Oznaka + "\",\"tabla\":\"" + u.PrisustvoTable + "\",\"pametnaTabla\":\"" +
                            u.PrisustvoPametneTable + "\",\"projektor\":\"" + u.PrisustvoProjektora + "\",\"brojMesta\":\"" + u.BrojRadnihMesta +
                            "\",\"os\":\"" + u.OperativniSistem + "\"}";
                    }
                    else
                    {
                        ucionice += "{\"oznaka\":\"" + u.Oznaka + "\",\"tabla\":\"" + u.PrisustvoTable + "\",\"pametnaTabla\":\"" +
                            u.PrisustvoPametneTable + "\",\"projektor\":\"" + u.PrisustvoProjektora + "\",\"brojMesta\":\"" + u.BrojRadnihMesta +
                            "\",\"os\":\"" + u.OperativniSistem + "\"},";
                    }
                    i++;
                }
            }
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
            nazivi = nazivi.Substring(0, nazivi.Length-1);
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
                if (j + 1 == neobrisaniPredmeti.Count)
                {
                    nazivi += "{\"oznaka\":\"" + p.Oznaka + "\",\"naziv\":\"" + p.Naziv + "\"}";
                }
                else
                {
                    nazivi += "{\"oznaka\":\"" + p.Oznaka + "\",\"naziv\":\"" + p.Naziv + "\"},";
                }
                    j++;
            }
            
            nazivi += "]}";

            //predmet za prvi smer
            string predmeti = "{\"predmeti\":[";
            foreach (string pr in prviSmer.Predmeti)
            {
                foreach (Predmet p in neobrisaniPredmeti)
                {
                    if(p.Oznaka == pr)
                    {
                        predmeti += "{\"oznaka\":\"" + p.Oznaka + "\",\"duzina\":\"" + p.MinDuzinaTermina + "\",\"termini\":\"" + p.PreostaliTermini +
                                "\",\"tabla\":\"" + p.NeophodnaTabla + "\",\"pametnaTabla\":\"" + p.NeophodnaPametnaTabla + "\",\"projektor\":\"" +
                                p.NeophodanProjektor + "\",\"brojMesta\":\"" + p.VelicinaGrupe + "\",\"os\":\"" + p.OperativniSistem + "\"},";
                    }

                }
            }
            predmeti = predmeti.Substring(0, predmeti.Length - 1);
            predmeti += "]}";
            if (_instanceBrowser.CanExecuteJavascriptInMainFrame)
            {
                _instanceBrowser.ExecuteScriptAsync("ucitajUcionice('" + ucionice + "');");
                _instanceBrowser.ExecuteScriptAsync("ucitajPredmete('" + predmeti + "');");
                _instanceBrowser.ExecuteScriptAsync("ucitajSmerove('" + smerovi + "');");
                _instanceBrowser.ExecuteScriptAsync("ucitajNazive('" + nazivi + "');");
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
                if (i + 1 == racunarskiCentar.PoljaKalendara.Values.Count)
                {
                    polja += "{\"id\":\"" + kal.Id + "\",\"pocetak\":\"" + kal.Pocetak + "\",\"kraj\":\"" + kal.Kraj + "\",\"naziv\":\"" + kal.NazivPolja + "\",\"dan\":\"" + kal.Dan + "\",\"ucionica\":\"" + kal.Ucionica + "\"}";
                }
                else
                    polja += "{\"id\":\"" + kal.Id + "\",\"pocetak\":\"" + kal.Pocetak + "\",\"kraj\":\"" + kal.Kraj + "\",\"naziv\":\"" + kal.NazivPolja + "\",\"dan\":\"" + kal.Dan + "\",\"ucionica\":\"" + kal.Ucionica + "\"},";
                i++;
            }
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
            if(!racunarskiCentar.PoljaKalendara.ContainsKey(id))
            {
                racunarskiCentar.PoljaKalendara.Add(id, new KalendarPolje(id, naziv, pocetak, kraj, dan, ucionica));
            }else
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
        }

        public void obrisiPolje(string id, string oznakaPredmeta)
        {
            racunarskiCentar.Predmeti[oznakaPredmeta].PreostaliTermini++;
            racunarskiCentar.PoljaKalendara.Remove(id);
        }

        public void dobaviPredmeteSmera(string smer)
        {
            //predmet za prvi smer
            string predmeti = "{\"predmeti\":[";
            Smer odabraniSmer = null;

            foreach(Smer s in racunarskiCentar.Smerovi.Values)
            {
                if (!s.Obrisan && (s.Oznaka == smer))
                    odabraniSmer = s;
            }
            if(odabraniSmer != null)
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
            if(odabraniSmer.Predmeti.Count != 0)
                predmeti = predmeti.Substring(0, predmeti.Length - 1);
            predmeti += "]}";
            if (_instanceBrowser.CanExecuteJavascriptInMainFrame)
            {
                _instanceBrowser.ExecuteScriptAsync("ucitajPredmete('" + predmeti + "');");
            }
        }
    }
}
