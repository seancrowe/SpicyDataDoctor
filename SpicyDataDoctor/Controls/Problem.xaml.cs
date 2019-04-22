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

namespace SpicyDataDoctor.Controls
{
    /// <summary>
    /// Interaction logic for Problem.xaml
    /// </summary>
    public partial class Problem : UserControl
    {
        private string info;

        private Dictionary<string, Action> fixes;

        public Problem(string problemMessage, string infoMessage, Dictionary<string, Action> fixes)
        {
            InitializeComponent();

            pName.Content = problemMessage;
            info = infoMessage;

            foreach (string fix in fixes.Keys)
            {
                Label label = new Label();

                label.Content = fix;

                pFixes.Items.Add(label);
            }

            this.fixes = fixes;
        }

        private void OnMoreInfo(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show(info);
        }

        private void OnRunFix(object sender, RoutedEventArgs e)
        {
            int index = pFixes.SelectedIndex;
            string key = (pFixes.Items[index] as Label).Content.ToString();

            if (fixes.ContainsKey(key))
            {
                fixes[key].Invoke();
            }
        }
    }
}
