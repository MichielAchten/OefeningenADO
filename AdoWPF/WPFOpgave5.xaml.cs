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
    /// Interaction logic for WPFOpgave5.xaml
    /// </summary>
    public partial class WPFOpgave5 : Window
    {
        public WPFOpgave5()
        {
            InitializeComponent();
        }

        private void buttonGemiddelde_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var manager = new TuincentrumDbManager();
                labelResultaat.Content = String.Format("Gemiddelde prijs : {0:C}",
                    manager.GemiddeldePrijsVanEenSoort(TextBoxSoort.Text));
            }
            catch (Exception ex)
            {
                labelResultaat.Content = ex.Message;
            }
        }
    }
}
