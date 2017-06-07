using System;
using System.Windows;

namespace HCI_Projekat
{
    /// <summary>
    /// Interaction logic for OdlukaDodavanjaSmer.xaml
    /// </summary>
    public partial class OdlukaDodavanjaSmer : Window
    {
        public bool potvrdaNovogUnosa;

        public OdlukaDodavanjaSmer()
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
