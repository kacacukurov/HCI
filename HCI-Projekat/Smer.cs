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
        private string datum;
        private string opis;

        private bool obrisan;
        private bool uPredmetu;
        private List<string> predmeti;

        public Smer()
        {
            this.uPredmetu = false;
            this.predmeti = new List<string>();
        }

        public Smer(string oznaka, string naziv, string datum, string opis)
        {
            this.oznaka = oznaka;
            this.naziv = naziv;
            this.datum = datum;
            this.opis = opis;
            this.obrisan = false;
            this.uPredmetu = false;
            this.predmeti = new List<string>();
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

        public string Datum
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

        public bool UPredmetu
        {
            get { return uPredmetu; }
            set { this.uPredmetu = value; }
        }

        public List<string> Predmeti
        {
            get { return predmeti; }
            set { this.predmeti = value; }
        }
    }
}
