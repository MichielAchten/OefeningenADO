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
    /// Interaction logic for WPFOpgave8.xaml
    /// </summary>
    public partial class WPFOpgave8 : Window
    {
        public WPFOpgave8()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var manager = new TuincentrumDbManager();
                comboBoxSoorten.DisplayMemberPath = "SoortNaam";
                comboBoxSoorten.SelectedValuePath = "SoortNr";
                comboBoxSoorten.ItemsSource = manager.GetSoorten();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBoxSoorten_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                listBoxPlanten.Items.Clear();
                int soortNr = Convert.ToInt32(comboBoxSoorten.SelectedValue);
                var manager = new TuincentrumDbManager();
                var allePlanten = manager.GetPlanten(soortNr);
                foreach (var eenPlant in allePlanten)
                {
                    listBoxPlanten.Items.Add(eenPlant);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
