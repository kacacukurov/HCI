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

    public class Ucionica
    {
        private string oznaka;
        private string opis;
        private int brojRadnihMesta;
        private bool prisustvoProjektora;
        private bool prisustvoTable;
        private bool prisustvoPametneTable;
        private OSType operativniSistem;
        private Softver instaliraniSoftver;

        public Ucionica() { }

        public Ucionica(string oznaka, string opis, int brojRadnihMesta, bool prisustvoProjektora,
            bool prisustvoTable, bool prisustvoPametneTable, OSType operativniSistem, Softver instaliraniSoftver)
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

        string Oznaka
        {
            get { return oznaka; }
            set { this.oznaka = value; }
        }

        string Opis
        {
            get { return opis; }
            set { this.opis = value; }
        }

        int BrojRadnihMesta
        {
            get { return brojRadnihMesta; }
            set { this.brojRadnihMesta = value; }
        }

        bool PrisustvoProjektora
        {
            get { return prisustvoProjektora; }
            set { this.prisustvoProjektora = value; }
        }

        bool PrisustvoTable
        {
            get { return prisustvoTable; }
            set { this.prisustvoTable = value; }
        }

        bool PrisustvoPametneTable
        {
            get { return prisustvoPametneTable; }
            set { this.PrisustvoPametneTable = value; }
        }

        OSType OperativniSistem
        {
            get { return operativniSistem; }
            set { this.operativniSistem = value; }
        }

        Softver InstaliraniSoftver
        {
            get { return instaliraniSoftver; }
            set { this.instaliraniSoftver = value; }
        }
    }
}
