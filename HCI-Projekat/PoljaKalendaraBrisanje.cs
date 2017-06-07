using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Projekat
{
    public class PoljaKalendaraBrisanje
    {
        private string oznaka;
        private string ucionica;
        private string dan;
        private string pocetak;
        private string kraj;
        private bool obrisan;

        public PoljaKalendaraBrisanje() { }

        public PoljaKalendaraBrisanje(string oznaka, string ucionica, string dan, string pocetak, string kraj, bool obrisan)
        {
            this.oznaka = oznaka;
            this.ucionica = ucionica;
            this.dan = dan;
            this.pocetak = pocetak;
            this.kraj = kraj;
            this.obrisan = obrisan;
        }

        public string Oznaka
        {
            get { return oznaka; }
            set { oznaka = value; }
        }

        public string Ucionica
        {
            get { return ucionica; }
            set { ucionica = value; }
        }

        public string Dan
        {
            get { return dan; }
            set { dan = value; }
        }

        public string Pocetak
        {
            get { return pocetak; }
            set { pocetak = value; }
        }

        public string Kraj
        {
            get { return kraj; }
            set { kraj = value; }
        }

        public bool Obrisan
        {
            get { return obrisan; }
            set { obrisan = value; }
        }
    }
}
