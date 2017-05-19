using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Projekat
{
    [Serializable]
    public class Predmet
    {
        private string oznaka;
        private string naziv;
        private Smer smer;
        private string opis;
        private int velicinaGrupe;
        private int minDuzinaTermina;
        private int brTermina;
        private bool neophodanProjektor;
        private bool neophodnaTabla;
        private bool neophodnaPametnaTabla;
        private string operativniSistem;
        private string softver;

        public Predmet() { }

        public Predmet(string oznaka, string naziv, Smer smer, string opis, int velicinaGrupe,
            int minDuzinaTermina, int brTermina, bool neophodanProjektor, bool neophodnaTabla, bool neophodnaPametnaTabla,
            string operativniSistem, string softver)
        {
            this.oznaka = oznaka;
            this.naziv = naziv;
            this.smer = smer;
            this.opis = opis;
            this.velicinaGrupe = velicinaGrupe;
            this.minDuzinaTermina = minDuzinaTermina;
            this.brTermina = brTermina;
            this.neophodanProjektor = neophodanProjektor;
            this.neophodnaTabla = neophodnaTabla;
            this.neophodnaPametnaTabla = neophodnaPametnaTabla;
            this.operativniSistem = operativniSistem;
            this.softver = softver;
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

        public Smer Smer
        {
            get { return smer; }
            set { this.smer = value; }
        }

        public string Opis
        {
            get { return opis; }
            set { this.opis = value; }
        }

        public int VelicinaGrupe
        {
            get { return velicinaGrupe; }
            set { this.velicinaGrupe = value; }
        }

        public int MinDuzinaTermina
        {
            get { return minDuzinaTermina; }
            set { this.minDuzinaTermina = value; }
        }

        public int BrTermina
        {
            get { return brTermina; }
            set { this.brTermina = value; }
        }

        public bool NeophodanProjektor
        {
            get { return neophodanProjektor; }
            set { this.neophodanProjektor = value; }
        }

        public bool NeophodnaTabla
        {
            get { return neophodnaTabla; }
            set { this.neophodnaTabla = value; }
        }

        public bool NeophodnaPametnaTabla
        {
            get { return neophodnaPametnaTabla; }
            set { this.neophodnaPametnaTabla = value; }
        }

        public string OperativniSistem
        {
            get { return operativniSistem; }
            set { this.operativniSistem = value; }
        }

        public string Softver
        {
            get { return softver; }
            set { this.softver = value; }
        }
    }
}
