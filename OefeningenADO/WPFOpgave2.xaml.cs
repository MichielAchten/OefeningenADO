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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using AdoGemeenschap;

namespace OefeningenADO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WPFOpgave2 : Window
    {
        public WPFOpgave2()
        {
            InitializeComponent();
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            try
            {
                //var manager = new TuincentrumDbManager();
                //using (var conTuincentrum = manager.GetConnection())
                using (var conTuincentrum = new TuincentrumDbManager().GetConnection())
                {
                    conTuincentrum.Open();
                    labelStatus.Content = "Tuincentrum geopend";
                }
            }
            catch (Exception ex)
            {
                labelStatus.Content = ex.Message;
            }
        }
    }
}
