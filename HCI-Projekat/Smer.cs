using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Projekat
{
    [Serializable]
    public class Smer
    {
        private string oznaka;
        private string naziv;
        private DateTime datum;
        private string opis;
        private bool obrisan;

        public Smer() { }

        public Smer(string oznaka, string naziv, DateTime datum, string opis)
        {
            this.oznaka = oznaka;
            this.naziv = naziv;
            this.datum = datum;
            this.opis = opis;
            this.obrisan = false;
        }

        public string Oznaka
        {
            get { return oznaka; }
            set { this.oznaka = value; }
        }

        public string Naziv
        {
            get { return naziv; }
            set { this.naziv = value; }
        }

        public DateTime Datum
        {
            get { return datum; }
            set { this.datum = value; }
        }

        public string Opis
        {
            get { return opis; }
            set { this.opis = value; }
        }

        public bool Obrisan
        {
            get { return obrisan; }
            set { this.obrisan = value; }
        }
    }
}
