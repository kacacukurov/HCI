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
        private OSType operativniSistem;
        private Softver softver;

        public Predmet() { }

        public Predmet(string oznaka, string naziv, Smer smer, string opis, int velicinaGrupe,
            int minDuzinaTermina, int brTermina, bool neophodanProjektor, bool neophodnaTabla, bool neophodnaPametnaTabla,
            OSType operativniSistem, Softver softver)
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

        Smer Smer
        {
            get { return smer; }
            set { this.smer = value; }
        }

        string Opis
        {
            get { return opis; }
            set { this.opis = value; }
        }

        int VelicinaGrupe
        {
            get { return velicinaGrupe; }
            set { this.velicinaGrupe = value; }
        }

        int MinDuzinaTermina
        {
            get { return minDuzinaTermina; }
            set { this.minDuzinaTermina = value; }
        }

        int BrTermina
        {
            get { return brTermina; }
            set { this.brTermina = value; }
        }

        bool NeophodanProjektor
        {
            get { return neophodanProjektor; }
            set { this.neophodanProjektor = value; }
        }

        bool NeophodnaTabla
        {
            get { return neophodnaTabla; }
            set { this.neophodnaTabla = value; }
        }

        bool NeophodnaPametnaTabla
        {
            get { return neophodnaPametnaTabla; }
            set { this.neophodnaPametnaTabla = value; }
        }

        OSType OperativniSistem
        {
            get { return operativniSistem; }
            set { this.operativniSistem = value; }
        }

        Softver Softver
        {
            get { return softver; }
            set { this.softver = value; }
        }
    }
}
