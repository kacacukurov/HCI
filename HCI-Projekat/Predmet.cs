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
        private List<string> smerovi; 
        private string opis;
        private int velicinaGrupe;
        private int minDuzinaTermina;
        private int brTermina;
        private bool neophodanProjektor;
        private bool neophodnaTabla;
        private bool neophodnaPametnaTabla;
        private string operativniSistem;
        private List<string> softveri;  
        private bool obrisan;
        private string softveriLista;
        private string smeroviLista;

        public Predmet() {
            this.softveri = new List<string>();
            this.smerovi = new List<string>();
        }

        public Predmet(string oznaka, string naziv, List<string> smerovi, string opis, int velicinaGrupe,
            int minDuzinaTermina, int brTermina, bool neophodanProjektor, bool neophodnaTabla, bool neophodnaPametnaTabla,
            string operativniSistem, List<string> softveri)
        {
            this.oznaka = oznaka;
            this.naziv = naziv;
            this.smerovi = smerovi;
            this.opis = opis;
            this.velicinaGrupe = velicinaGrupe;
            this.minDuzinaTermina = minDuzinaTermina;
            this.brTermina = brTermina;
            this.neophodanProjektor = neophodanProjektor;
            this.neophodnaTabla = neophodnaTabla;
            this.neophodnaPametnaTabla = neophodnaPametnaTabla;
            this.operativniSistem = operativniSistem;
            this.softveri = softveri;
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

        public List<string> Smerovi
        {
            get { return smerovi; }
            set { this.smerovi = value; }
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

        public List<string> Softveri
        {
            get { return softveri; }
            set { this.softveri = value; }
        }

        public bool Obrisan
        {
            get { return obrisan; }
            set { this.obrisan = value; }
        }

        public string SoftveriLista
        {
            get { return softveriLista; }
            set { this.softveriLista = value; }
        }

        public string SmeroviLista
        {
            get { return smeroviLista; }
            set { this.smeroviLista = value; }
        }
    }
}
