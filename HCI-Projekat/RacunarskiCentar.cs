using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Projekat
{
    public class RacunarskiCentar
    {
        private Dictionary<string, Smer> smerovi;
        private Dictionary<string, Softver> softveri;
        private Dictionary<string, Ucionica> ucionice;
        private Dictionary<string, Predmet> predmeti;

        public RacunarskiCentar()
        {
            this.smerovi = new Dictionary<string, Smer>();
            this.softveri = new Dictionary<string, Softver>();
            this.ucionice = new Dictionary<string, Ucionica>();
            this.predmeti = new Dictionary<string, Predmet>();
        }

        public RacunarskiCentar(Dictionary<string, Smer> smerovi, Dictionary<string, Softver> softveri,
            Dictionary<string, Ucionica> ucionice, Dictionary<string, Predmet> predmeti)
        {
            this.smerovi = smerovi;
            this.softveri = softveri;
            this.predmeti = predmeti;
            this.ucionice = ucionice;
        }

        public Dictionary<string, Smer> Smerovi
        {
            get { return smerovi; }
            set { this.smerovi = value; }
        }

        public Dictionary<string, Softver> Softveri
        {
            get { return softveri; }
            set { this.softveri = value; }
        }

        public Dictionary<string, Predmet> Predmeti
        {
            get { return predmeti; }
            set { this.predmeti = value; }
        }

        Dictionary<string, Ucionica> Ucionice
        {
            get { return ucionice; }
            set { this.ucionice = value; }
        }

        public void DodajSmer(Smer noviSmer)
        {
            this.smerovi.Add(noviSmer.Oznaka, noviSmer);
            Console.WriteLine("Smerova: " + smerovi.Count);
        }

        public void DodajPredmet(Predmet predmet)
        {
            this.predmeti.Add(predmet.Oznaka, predmet);
            Console.WriteLine("Predmeta: " + predmeti.Count);
        }

        public bool DodajSoftver(Softver noviSoftver)
        {
            if (softveri.ContainsKey(noviSoftver.Oznaka))
                return false;
            this.softveri.Add(noviSoftver.Oznaka, noviSoftver);
            return true;
        }

        public bool DodajUcionicu(Ucionica novaUcionica)
        {
            if (ucionice.ContainsKey(novaUcionica.Oznaka))
                return false;
            this.ucionice.Add(novaUcionica.Oznaka, novaUcionica);
            return true;
        }
    }
}
