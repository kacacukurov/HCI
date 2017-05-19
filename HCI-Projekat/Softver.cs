using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Projekat
{
    public class Softver
    {
        private string oznaka;
        private string naziv;
        private OSType operativniSistem;
        private string proizvodjac;
        private string sajt;
        private int godIzdavanja;
        private double cena;
        private string opis;

        public Softver() { }

        public Softver(string oznaka, string naziv, OSType operativniSistem, string proizvodjac,
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

        OSType OperativniSistem
        {
            get { return operativniSistem; }
            set { this.operativniSistem = value; }
        }

        string Proizvodjac
        {
            get { return proizvodjac; }
            set { this.proizvodjac = value; }
        }

        string Sajt
        {
            get { return sajt; }
            set { this.sajt = value; }
        }

        int GodIzdavanja
        {
            get { return godIzdavanja; }
            set { this.godIzdavanja = value; }
        }

        double Cena
        {
            get { return cena; }
            set { this.cena = value; }
        }

        string Opis
        {
            get { return opis; }
            set { this.opis = value; }
        }
    }
}
