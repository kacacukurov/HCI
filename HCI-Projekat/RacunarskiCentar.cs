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

        Dictionary<string, Smer> Smerovi
        {
            get { return smerovi; }
            set { this.smerovi = value; }
        }

        Dictionary<string, Softver> Softveri
        {
            get { return softveri; }
            set { this.softveri = value; }
        }

        Dictionary<string, Predmet> Predmeti
        {
            get { return predmeti; }
            set { this.predmeti = value; }
        }

        Dictionary<string, Ucionica> Ucionice
        {
            get { return ucionice; }
            set { this.ucionice = value; }
        }
    }
}
