using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Projekat
{
    public class StanjeAplikacije
    {
        private RacunarskiCentar racunarskiCentar;
        private string porukaOPromeni;
        private string tipPodatka;

        public StanjeAplikacije(RacunarskiCentar r, string p, string t)
        {
            this.racunarskiCentar = r;
            this.porukaOPromeni = p;
            this.tipPodatka = t;
        }

        public RacunarskiCentar RacunarskiCentar
        {
            get { return this.racunarskiCentar; }
            set { this.racunarskiCentar = value; }
        }

        public string PorukaOPromeni
        {
            get { return this.porukaOPromeni; }
            set { this.porukaOPromeni = value; }
        }

        public string TipPodataka
        {
            get { return this.tipPodatka; }
            set { this.tipPodatka = value; }
        }
    }
}
