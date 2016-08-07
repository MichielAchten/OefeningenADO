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
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace OefeningenADO
{
    /// <summary>
    /// Interaction logic for WPFOpgave10.xaml
    /// </summary>
    public partial class WPFOpgave10 : Window
    {
        public ObservableCollection<Leverancier> leveranciersOb = new ObservableCollection<Leverancier>();
        public List<Leverancier> OudeLeveranciers = new List<Leverancier>();
        public List<Leverancier> NieuweLeveranciers = new List<Leverancier>();
        public List<Leverancier> GewijzigdeLeveranciers = new List<Leverancier>();

        public WPFOpgave10()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource leverancierViewSource =
                ((System.Windows.Data.CollectionViewSource)(this.FindResource("leverancierViewSource")));
            var manager = new TuincentrumDbManager();
            leveranciersOb = manager.GetLeveranciers();
            leverancierViewSource.Source = leveranciersOb;

            leveranciersOb.CollectionChanged += this.OnCollectionChanged;

            comboBoxPostnummers.Items.Add("alles");
            var post = manager.GetPostnummers();
            foreach (var p in post)
            {
                comboBoxPostnummers.Items.Add(p);
            }
            comboBoxPostnummers.SelectedIndex = 0;
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Leverancier oudeLev in e.OldItems)
                {
                    OudeLeveranciers.Add(oudeLev);
                }
            }
            if (e.NewItems != null)
            {
                foreach (Leverancier nieuweLev in e.NewItems)
                {
                    NieuweLeveranciers.Add(nieuweLev);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Wilt u alles wegschrijven naar de database ?", "Opslaan",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                leverancierDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                var manager = new TuincentrumDbManager();
                if (OudeLeveranciers.Count > 0)
                {
                    manager.SchrijfVerwijderingen(OudeLeveranciers);
                }
                if (NieuweLeveranciers.Count > 0)
                {
                    manager.SchrijfToevoegingen(NieuweLeveranciers);
                }
                foreach (Leverancier l in leveranciersOb)
                {
                    if (l.Changed == true)
                    {
                        GewijzigdeLeveranciers.Add(l);
                        l.Changed = false;
                    }
                }
                if (GewijzigdeLeveranciers.Count > 0)
                {
                    manager.SchrijfWijzigingen(GewijzigdeLeveranciers);
                }
                OudeLeveranciers.Clear();
                NieuweLeveranciers.Clear();
                GewijzigdeLeveranciers.Clear();
                System.Windows.Data.CollectionViewSource leverancierViewSource =
                    ((System.Windows.Data.CollectionViewSource)(this.FindResource("leverancierViewSource")));
                leveranciersOb = manager.GetLeveranciers();
                leverancierViewSource.Source = leveranciersOb;
            }
        }

        private void comboBoxPostnummers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxPostnummers.SelectedIndex != 0)
            {
                leverancierDataGrid.Items.Filter = new Predicate<object>(PostnummerFilter);
            }
            else
            {
                leverancierDataGrid.Items.Filter = null;
            }
        }
        public bool PostnummerFilter(object lev)
        {
            Leverancier l = lev as Leverancier;
            bool result = (l.PostNr == comboBoxPostnummers.SelectedValue.ToString());
            return result;
        }
    }
}
