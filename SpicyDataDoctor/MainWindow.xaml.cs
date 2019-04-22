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

namespace SpicyDataDoctor
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Operations operations;

        public MainWindow()
        {
            operations = new Operations(this);

            InitializeComponent();

            this.DataContext = operations;
        }

        private void OnOpenConnector(object sender, RoutedEventArgs e)
        {
            Windows.Connector newConnector = new Windows.Connector(operations);
            newConnector.Closed += (object obj, System.EventArgs eventArgs) =>
            {
                this.IsEnabled = true;
                operations.CheckConnections();
            };

            newConnector.Show();
            this.IsEnabled = false;
        }

        private void OnSelectDataFolder(object sender, RoutedEventArgs e)
        {
            operations.SelectFolder();
        }

        private void OnHealthAssessment(object sender, RoutedEventArgs e)
        {
            operations.RunHealthAssessment();
        }

    }
}
