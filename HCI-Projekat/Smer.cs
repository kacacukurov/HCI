using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Projekat
{
    public class Smer
    {
        private string oznaka;
        private string naziv;
        private DateTime datum;
        private string opis;

        public Smer() { }

        public Smer(string oznaka, string naziv, DateTime datum, string opis)
        {
            this.oznaka = oznaka;
            this.naziv = naziv;
            this.datum = datum;
            this.opis = opis;
        }

        string Oznaka
        {
            get { return oznaka; }
            set { this.oznaka = value; }
        }

        string Naziv
        {
            get { return naziv; }
            set { this.naziv = value; }
        }

        DateTime Datum
        {
            get { return datum; }
            set { this.datum = value; }
        }

        string Opis
        {
            get { return opis; }
            set { this.opis = value; }
        }
    }
}
