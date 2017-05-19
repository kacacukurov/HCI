using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Projekat
{
    public enum OSType
    {
        Linux, Windows, LinuxAndWindows
    }

    [Serializable]
    public class Ucionica
    {
        private string oznaka;
        private string opis;
        private int brojRadnihMesta;
        private bool prisustvoProjektora;
        private bool prisustvoTable;
        private bool prisustvoPametneTable;
        private string operativniSistem;
        private Softver instaliraniSoftver;

        public Ucionica() { }

        public Ucionica(string oznaka, string opis, int brojRadnihMesta, bool prisustvoProjektora,
            bool prisustvoTable, bool prisustvoPametneTable, string operativniSistem, Softver instaliraniSoftver)
        {
            this.oznaka = oznaka;
            this.opis = opis;
            this.brojRadnihMesta = brojRadnihMesta;
            this.prisustvoProjektora = prisustvoProjektora;
            this.prisustvoTable = prisustvoTable;
            this.prisustvoPametneTable = prisustvoPametneTable;
            this.operativniSistem = operativniSistem;
            this.instaliraniSoftver = instaliraniSoftver;
        }

        public string Oznaka
        {
            get { return oznaka; }
            set { this.oznaka = value; }
        }

        public string Opis
        {
            get { return opis; }
            set { this.opis = value; }
        }

        public int BrojRadnihMesta
        {
            get { return brojRadnihMesta; }
            set { this.brojRadnihMesta = value; }
        }

        public bool PrisustvoProjektora
        {
            get { return prisustvoProjektora; }
            set { this.prisustvoProjektora = value; }
        }

        public bool PrisustvoTable
        {
            get { return prisustvoTable; }
            set { this.prisustvoTable = value; }
        }

        public bool PrisustvoPametneTable
        {
            get { return prisustvoPametneTable; }
            set { this.PrisustvoPametneTable = value; }
        }

        public string OperativniSistem
        {
            get { return operativniSistem; }
            set { this.operativniSistem = value; }
        }

        public Softver InstaliraniSoftver
        {
            get { return instaliraniSoftver; }
            set { this.instaliraniSoftver = value; }
        }
    }
}
