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
    /// Interaction logic for WPFOpgave9.xaml
    /// </summary>
    public partial class WPFOpgave9 : Window
    {
        public WPFOpgave9()
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

        private List<Plant> ListBoxPlantenLijst = new List<Plant>();
        private string GeselecteerdeSoortNaam;

        private void comboBoxSoorten_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WijzigingenOplslaan();
            GeselecteerdeSoortNaam = ((Soort)comboBoxSoorten.SelectedItem).SoortNaam;
            try
            {
                var manager = new TuincentrumDbManager();
                ListBoxPlantenLijst = manager.GetPlanten(Convert.ToInt32(comboBoxSoorten.SelectedValue));
                listBoxPlanten.ItemsSource = ListBoxPlantenLijst;
                listBoxPlanten.DisplayMemberPath = "PlantNaam";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void WijzigingenOplslaan()
        {
            List<Plant> gewijzigdePlanten = new List<Plant>();
            foreach (Plant p in ListBoxPlantenLijst)
            {
                if (p.Changed == true)
                {
                    gewijzigdePlanten.Add(p);
                    p.Changed = false;
                }
            }
            if ((gewijzigdePlanten.Count > 0) && (MessageBox.Show("Gewijzigde planten van soort '" + 
                GeselecteerdeSoortNaam + "' opslaan?", "Opslaan", MessageBoxButton.YesNo,
                MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes))
            {
                var manager = new TuincentrumDbManager();
                try
                {
                    manager.GewijzigdePlantenOpslaan(gewijzigdePlanten);
                    MessageBox.Show("Planten opgeslagen", "Opslaan", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Er is een fout opgetreden: " + ex.Message,
                        "Opslaan", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void buttonOpslaan_Click(object sender, RoutedEventArgs e)
        {
            if (!PlantHasErrors())
            {
                WijzigingenOplslaan();
            }
        }

        private bool PlantHasErrors()
        {
            bool foutGevonden = false;
            if (Validation.GetHasError(textBoxKleur))
            {
                foutGevonden = true;
            }
            if (Validation.GetHasError(textBoxPrijs))
            {
                foutGevonden = true;
            }
            return foutGevonden;
        }

        private void listboxPlanten_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PlantHasErrors()) 
            {
                e.Handled = true;
            }
        }

        private void listBoxPlanten_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (PlantHasErrors())
            {
                e.Handled = true;
            }
        }

        private void comboBoxSoorten_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PlantHasErrors())
            {
                e.Handled = true;
            }
        }

        private void comboBoxSoorten_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (PlantHasErrors())
            {
                e.Handled = true;
            }
        }
    }
}
