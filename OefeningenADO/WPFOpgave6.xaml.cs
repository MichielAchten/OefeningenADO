using AdoGemeenschap;
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

namespace OefeningenADO
{
    /// <summary>
    /// Interaction logic for WPFOpgave6.xaml
    /// </summary>
    public partial class WPFOpgave6 : Window
    {
        public WPFOpgave6()
        {
            InitializeComponent();
        }

        private void buttonOpzoeken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var manager = new TuincentrumDbManager();
                var resultaat = manager.PlantGegevensOpzoeken(Convert.ToInt16(TextBoxPlantNummer.Text));
                LabelNaam.Content = resultaat.Naam;
                LabelSoort.Content = resultaat.Soort;
                LabelLeverancier.Content = resultaat.Leverancier;
                LabelKleur.Content = resultaat.Kleur;
                LabelKostprijs.Content = String.Format("{0:C}", resultaat.Kostprijs);
            }
            catch(Exception ex)
            {
                LabelNaam.Content = ex.Message;
                LabelSoort.Content = String.Empty;
                LabelLeverancier.Content = String.Empty;
                LabelKleur.Content = String.Empty;
                LabelKostprijs.Content = String.Empty;
            }
        }


    }
}
