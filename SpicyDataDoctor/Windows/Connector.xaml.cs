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

namespace SpicyDataDoctor.Windows
{
    /// <summary>
    /// Interaction logic for Connector.xaml
    /// </summary>
    public partial class Connector : Window
    {
        private Operations operations;

        public Connector(Operations operations)
        {
            InitializeComponent();

            this.operations = operations;

            this.DataContext = operations;
        }

        private void OnTestConnection(object sender, RoutedEventArgs e)
        {
            operations.TestConnection();
        }

        private void OnConnectToServer(object sender, RoutedEventArgs e)
        {
            operations.TestConnection();
            this.Close();
        }
    }
}
