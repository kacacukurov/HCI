using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Projekat
{
    [Serializable]
    public class Softver
    {
        private string oznaka;
        private string naziv;
        private string operativniSistem;
        private string proizvodjac;
        private string sajt;
        private int godIzdavanja;
        private double cena;
        private string opis;
        private bool obrisan;

        public Softver() { }

        public Softver(string oznaka, string naziv, string operativniSistem, string proizvodjac,
            string sajt, int godIzdavanja, double cena, string opis)
        {
            this.oznaka = oznaka;
            this.naziv = naziv;
            this.operativniSistem = operativniSistem;
            this.proizvodjac = proizvodjac;
            this.sajt = sajt;
            this.godIzdavanja = godIzdavanja;
            this.cena = cena;
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

        public string OperativniSistem
        {
            get { return operativniSistem; }
            set { this.operativniSistem = value; }
        }

        public string Proizvodjac
        {
            get { return proizvodjac; }
            set { this.proizvodjac = value; }
        }

        public string Sajt
        {
            get { return sajt; }
            set { this.sajt = value; }
        }

        public int GodIzdavanja
        {
            get { return godIzdavanja; }
            set { this.godIzdavanja = value; }
        }

        public double Cena
        {
            get { return cena; }
            set { this.cena = value; }
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
