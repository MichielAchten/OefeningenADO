using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AdoGemeenschap;

namespace OefeningenADO
{
    /// <summary>
    /// Interaction logic for WPFOpgave3.xaml
    /// </summary>
    public partial class WPFOpgave4 : Window
    {
        public WPFOpgave4()
        {
            InitializeComponent();
        }

        private void buttonToevoegen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var manager = new TuincentrumDbManager();
                var deLeverancier = new Leverancier();
                deLeverancier.Naam = TextBoxNaam.Text;
                deLeverancier.Adres = TextBoxAdres.Text;
                deLeverancier.PostNr = TextBoxPostNr.Text;
                deLeverancier.Woonplaats = TextBoxWoonplaats.Text;
                manager.LeverancierToevoegen(deLeverancier);
                statusLabel.Content = "Nieuwe leverancier is toegevoegd";
            }
            catch (Exception ex)
            {
                statusLabel.Content = ex.Message;
            }
        }

        private void buttonEindejaarskorting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var manager = new TuincentrumDbManager();
                statusLabel.Content = manager.EindejaarsKorting().ToString() + " plantenprijzen aangepast";
            }
            catch (Exception ex)
            {
                statusLabel.Content = ex.Message;
            }
        }

        private void buttonVervangLeverancier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var manager = new TuincentrumDbManager();
                manager.VervangLeverancier(2, 3);
                statusLabel.Content = "Leverancier 2 is verwijderd en vervangen door 3";
            }
            catch (Exception ex)
            {
                statusLabel.Content = ex.Message; 
            }
        }
    }
}
