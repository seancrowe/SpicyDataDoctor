using SpicyDataDoctor.Controls;
using SpicyDataDoctor.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace SpicyDataDoctor
{

    public class Operations : INotifyPropertyChanged
    {
        private ImageSource serverIcon;
        public ImageSource ServerIcon
        {
            get { return serverIcon; }
            set
            {
                serverIcon = value;
                OnPropertyChanged("ServerIcon");
            }
        }

        private ImageSource dataIcon;
        public ImageSource DataIcon
        {
            get { return dataIcon; }
            set
            {
                dataIcon = value;
                OnPropertyChanged("DataIcon");
            }
        }

        private string serverUrl;
        public string ServerUrl
        {
            get { return serverUrl; }
            set
            {
                serverUrl = value;
                OnPropertyChanged("ServerUrl");
            }
        }

        private bool examEnabled;
        public bool ExamEnabled
        {
            get { return examEnabled; }
            set
            {
                examEnabled = value;
                OnPropertyChanged("ExamEnabled");
            }
        }

        private bool healthAssessmentEnabled;
        public bool HealthAssessmentEnabled
        {
            get { return healthAssessmentEnabled; }
            set
            {
                healthAssessmentEnabled = value;
                OnPropertyChanged("HealthAssessmentEnabled");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public Logger logger;
        public string applicationPath;
        public MainWindow mainWindow;

        public ChiliService.mainSoapClient soapClient;

        public Operations(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            applicationPath = System.AppDomain.CurrentDomain.BaseDirectory;

            SetupMethod();

            logger = new Logger(applicationPath + "/Logs");
        }

        private void SetupMethod()
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri("/SpicyDataDoctor;component/Images/icons8-help-32.png", UriKind.Relative));

            ServerIcon = bitmapImage;
            DataIcon = bitmapImage;

            ExamEnabled = false;
            HealthAssessmentEnabled = true;

            if (File.Exists(applicationPath + "/lib/sav1.txt"))
            {
                StreamReader streamReader = new StreamReader(applicationPath + "/lib/sav1.txt");

                ServerUrl = streamReader.ReadToEnd();

                streamReader.Close();
            }
        }

        public string dataFolder;

        public void SelectFolder()
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderBrowserDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == true)
            {
                CheckDataFolder(folderBrowserDialog.SelectedPath);
            }
        }

        public void CheckDataFolder(string folder)
        {
            if (Directory.Exists(folder) == true)
            {
                bool correctFolders = Directory.Exists(folder + "/config") && File.Exists(folder + "/Environments/Admin/Resources/Users/data.xml") && File.Exists(folder + "/config/environments.xml");

                if (correctFolders == false)
                {
                    System.Windows.MessageBox.Show("Error: Data folder doesn't meet the right requirements");
                    logger.WriteLog("Error: Data folder doesn't meet the right requirements");

                    DataIcon = new BitmapImage(new Uri("/SpicyDataDoctor;component/Images/icons8-cancel-32.png", UriKind.Relative));

                }
                else
                {
                    dataFolder = folder;
                    App.dataFolder = dataFolder;
                    DataIcon = new BitmapImage(new Uri("/SpicyDataDoctor;component/Images/icons8-ok-32.png", UriKind.Relative));
                    CheckConnections();
                }
                
            }
            else
            {
                System.Windows.MessageBox.Show("Error: Folder doesn't exist");
                logger.WriteLog("Error: Folder doesn't exist");
            }
        }

        public bool urlGood = false;

        public void TestConnection()
        {
            string url = ServerUrl;

            if (url != null)
            {
                if (url.Contains("?wsdl") == false)
                {
                    url += "?wsdl";
                }

                using (WebClient webClient = new WebClient())
                {
                    try
                    {
                        StreamReader streamReader = new StreamReader(webClient.OpenRead(url));
                        string xml = streamReader.ReadToEnd();

                        XmlDocument xmlDocument = new XmlDocument();

                        try
                        {
                            xmlDocument.LoadXml(xml);
                            
                            

                        }
                        catch (Exception e)
                        {
                            
                        }

                        foreach (XmlNode xmlNode in xmlDocument.ChildNodes)
                        {

                            if (xmlNode.Attributes != null && xmlNode.Attributes["targetNamespace"] != null)
                            {
                                if (xmlNode.Attributes["targetNamespace"].Value == "http://www.chili-publisher.com/")
                                {
                                    urlGood = true;
                                }
                            }
                        }

                        streamReader.Close();
                    }
                    catch (WebException e)
                    {

                    }
                }
            }

            if (urlGood == true)
            {
                ServerIcon = new BitmapImage(new Uri("/SpicyDataDoctor;component/Images/icons8-ok-32.png", UriKind.Relative));
                Logger writeSav = new Logger(applicationPath + "/lib");
                writeSav.WriteFile(url, "sav1");

                soapClient = Connector.SetupConnection(url);
            }
            else
            {
                System.Windows.MessageBox.Show("Error: CHILI URL is bad");
                ServerIcon = new BitmapImage(new Uri("/SpicyDataDoctor;component/Images/icons8-cancel-32.png", UriKind.Relative));
            }
        }

        public void CheckConnections()
        {
            if (urlGood == true && (dataFolder != "" && dataFolder != null))
            {
                ExamEnabled = true;
                CreateKey();
                GetEnvironments();
            }
        }

        public void CreateKey()
        {
            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load(dataFolder + "/Environments/Admin/Resources/Users/data.xml");

            XmlNode adminUser = xmlDocument.SelectSingleNode("//item[@name='ChiliAdmin']");

            string adminId = adminUser.Attributes["id"].Value;

            Directory.CreateDirectory(dataFolder + "/config/apiKeys");

            if (File.Exists(dataFolder + "/config/apiKeys/spicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDR==.xml"))
            {
                File.Delete(dataFolder + "/config/apiKeys/spicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDR==.xml");
            }

            File.Copy(applicationPath + "/lib/key.xml", dataFolder + "/config/apiKeys/spicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDR==.xml");

            xmlDocument.Load(dataFolder + "/config/apiKeys/spicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDR==.xml");

            XmlNode xmlNode = xmlDocument.SelectSingleNode("//key");

            xmlNode.Attributes["userID"].Value = adminId;

            DateTime date = DateTime.Now;

            date = date.AddHours(12);

            string dateStr = date.Year + "-" + date.Month.ToString("D2") + "-" + date.Day.ToString("D2") + "T" + date.Hour.ToString("D2") + ":00:00";

            xmlNode.Attributes["expires"].Value = dateStr;

            xmlDocument.Save(dataFolder + "/config/apiKeys/spicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDR==.xml");
        }

        public void GetEnvironments()
        {
            XmlDocument xmlDocument = new XmlDocument();

            if (Directory.Exists(dataFolder + "/Environments"))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dataFolder + "/Environments");

                DirectoryInfo[] environmentDirectories = directoryInfo.GetDirectories();

                mainWindow.ddEnvironment.Items.Clear();

                foreach (DirectoryInfo envDirectory in environmentDirectories)
                {
                    if (VerifyEnvironment(envDirectory))
                    {
                            Label label = new Label()
                            {
                                Content = envDirectory.Name,
                                //VerticalContentAlignment = VerticalAlignment.Center,
                                FontSize = 14
                            };

                        //if (rc == true)
                        //{
                        //    label.Foreground = new SolidColorBrush(Colors.Red);
                        //}

                    

                        mainWindow.ddEnvironment.Items.Add(label);
                    }
                }


            }
            else
            {
                System.Windows.MessageBox.Show("Error: environments.xml cannot be found");
                logger.WriteLog("Error: environments.xml cannot be found");
            }

            if (mainWindow.ddEnvironment.Items.Count > 0)
            {
                mainWindow.ddEnvironment.SelectedIndex = 0;
            }
            else
            {
                HealthAssessmentEnabled = false;
            }

        
        }

        public bool VerifyEnvironment(DirectoryInfo environmentDirectory)
        {
            string envPath = environmentDirectory.FullName;

            return Directory.Exists(envPath + "/Resources") && File.Exists(envPath + "/Resources/Documents/data.xml") && File.Exists(envPath + "/Resources/Assets/data.xml");

        }

        public void SetApiEnvironment(string environmentName)
        {
            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load(dataFolder + "/config/apiKeys/spicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDR==.xml");

            XmlNode xmlNode = xmlDocument.SelectSingleNode("//key");

            xmlNode.Attributes["userEnvironmentName"].Value = environmentName;
        }

        //userEnvironmentName

        public void RunHealthAssessment()
        {
            int index = mainWindow.ddEnvironment.SelectedIndex;
            string environmentName = (mainWindow.ddEnvironment.Items[index] as Label).Content.ToString();

            SetApiEnvironment(environmentName);

            HealthAssessment health = new HealthAssessment(dataFolder + "/Environments/" + environmentName);

            //mainWindow.ddHealthAssessmentButton.Visibility = System.Windows.Visibility.Collapsed;
            //mainWindow.ddHealthAssessmentProgressBar.Visibility = System.Windows.Visibility.Visible;

            mainWindow.ddHealthAssessmentProgressBar.Value = 10;
            var assets = health.Assets;

            mainWindow.ddHealthAssessmentProgressBar.Value = 20;
            var documents = health.Documents;

            mainWindow.ddHealthAssessmentProgressBar.Value = 80;
            List<Asset> unusedAssets = health.GetUnusedAssets();

            mainWindow.ddHealthAssessmentProgressBar.Value = 100;


            mainWindow.ddProblemsPanel.Visibility = System.Windows.Visibility.Visible;

            mainWindow.ddProblemListBox.Items.Clear();

            if (unusedAssets.Count > 0)
            {
                mainWindow.ddProblemListBox.Items.Clear();

                Problem problem = new Problem($"Unused Assets ({unusedAssets.Count})", "These are assets that not found in any documents and thus safe to delete without ruining any existing documents.", new Dictionary<string, Action>
                {
                    {"Move all unused assets to a folder UnusedAsssetsFolder", () =>
                    {
                        mainWindow.IsEnabled = false;
                        Thread.Sleep(100);

                        foreach (Asset asset in unusedAssets)
                        {
                            soapClient.ResourceItemMove("spicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDR==", "Assets", asset.id, asset.name, "UnusedAsssetsFolder");
                        }

                        mainWindow.IsEnabled = true;
                        System.Windows.MessageBox.Show("Action Completed");

                    }},
                    {"Delete all unused assets", () =>
                    {
                        mainWindow.IsEnabled = false;
                        Thread.Sleep(100);

                        foreach (Asset asset in unusedAssets)
                        {
                            soapClient.ResourceItemDelete("spicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDOCTORspicyDATAdoctorSPICYdataDR==", "Assets", asset.id);
                        }

                        mainWindow.IsEnabled = true;
                        System.Windows.MessageBox.Show("Action Completed");
                    }}
                });

                mainWindow.ddProblemListBox.Items.Add(problem);
            }
        }
    }


}
