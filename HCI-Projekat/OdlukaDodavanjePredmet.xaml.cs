using System;
using System.Windows;

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for OdlukaDodavanjePredmet.xaml
    /// </summary>
    public partial class OdlukaDodavanjePredmet : Window
    {
        public bool potvrdaNovogUnosa;

        public OdlukaDodavanjePredmet()
        {
            InitializeComponent();
            potvrdiUnos.Focus();
            potvrdaNovogUnosa = true;
        }

        public void noviUnos(object sender, EventArgs e)
        {
            potvrdaNovogUnosa = true;
            this.Close();
        }

        public void stariUnos(object sender, EventArgs e)
        {
            potvrdaNovogUnosa = false;
            this.Close();
        }
    }
}