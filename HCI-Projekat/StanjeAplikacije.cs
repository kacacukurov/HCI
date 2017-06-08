using System;
using System.Collections.Generic;

namespace HCI_Projekat
{
    public class StanjeAplikacije
    {
        private RacunarskiCentar racunarskiCentar;
        private string tipPodatka;
        private string tipPromene;
        private int kolicina;
        private List<string> oznake;

        public StanjeAplikacije()
        {
            this.oznake = new List<string>();
        }

        public StanjeAplikacije(RacunarskiCentar racunarskiCentar, string tipPodatka, string tipPromene, int kolicina, List<string> oznake)
        {
            this.racunarskiCentar = racunarskiCentar;
            this.tipPodatka = tipPodatka;
            this.tipPromene = tipPromene;
            this.kolicina = kolicina;
            this.oznake = oznake;
        }

        public RacunarskiCentar RacunarskiCentar
        {
            get { return this.racunarskiCentar; }
            set { this.racunarskiCentar = value; }
        }

        public string TipPodataka
        {
            get { return this.tipPodatka; }
            set { this.tipPodatka = value; }
        }

        public string TipPromene
        {
            get { return this.tipPromene; }
            set { this.tipPromene = value; }
        }

        public int Kolicina
        {
            get { return this.kolicina; }
            set { this.kolicina = value; }
        }

        public List<string> Oznake
        {
            get { return this.oznake; }
            set { this.oznake = value; }
        }
    }
}
