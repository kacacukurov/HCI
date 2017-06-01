using System;

namespace HCI_Projekat
{
    [Serializable]
    public class KalendarPolje
    {
        private string id;
        private string nazivPolja;
        private string pocetak;
        private string dan;
        private string kraj;
        private string ucionica;

        public KalendarPolje() { }

        public KalendarPolje(string id, string naziv, string pocetak, string kraj, string dan, string ucionica)
        {
            this.id = id;
            this.nazivPolja = naziv;
            this.pocetak = pocetak;
            this.kraj = kraj;
            this.dan = dan;
            this.ucionica = ucionica;
        }

        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public string NazivPolja
        {
            get { return this.nazivPolja; }
            set { this.nazivPolja = value; }
        }

        public string Pocetak
        {
            get { return this.pocetak; }
            set { this.pocetak = value; }
        }

        public string Kraj
        {
            get { return this.kraj; }
            set { this.kraj = value; }
        }

        public string Dan
        {
            get { return this.dan; }
            set { this.dan = value; }
        }

        public string Ucionica
        {
            get { return this.ucionica; }
            set { this.ucionica = value; }
        }
    }
}
