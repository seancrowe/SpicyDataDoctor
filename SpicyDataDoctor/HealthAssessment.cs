using SpicyDataDoctor.Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;

namespace SpicyDataDoctor
{
    public class HealthAssessment
    {
        public string environmentPath;

        private List<string> cacheUsedAssetIds = new List<string>();

        private List<Asset> cachedAssets;
        public ReadOnlyCollection<Asset> Assets
        {
            get
            {
                if (cachedAssets == null)
                {
                    cachedAssets = GetAssets(environmentPath + "/Resources/Assets");
                }

                return cachedAssets.AsReadOnly();
            }
        }

        private List<Document> cachedDocuments;
        public ReadOnlyCollection<Document> Documents
        {
            get
            {
                if (cachedDocuments == null)
                {
                    cachedDocuments = GetDocuments(environmentPath + "/Resources/Documents");
                }

                return cachedDocuments.AsReadOnly();
            }
        }


        public HealthAssessment(string environmentPath)
        {
            this.environmentPath = environmentPath;
        }

        //public List<string> missingAssetIdList = new List<string>();
        //public Dictionary<string, string> assetIdDict = new Dictionary<string, string>();
        //public List<string> duplicateAssetIdList = new List<string>();

        public List<Asset> GetAssets(string assetPath)
        {
            List<Asset> returnList = new List<Asset>();

            if (Directory.Exists(assetPath))
            {
                List<string> assetIds = new List<string>();

                DirectoryInfo directoryInfo = new DirectoryInfo(assetPath);
                FileInfo[] files = directoryInfo.GetFiles();

                XmlDocument xmlDocument = new XmlDocument();

                foreach (FileInfo file in files)
                {
                    if (file.Name.Contains("data") && file.Name.Contains("xml"))
                    {
                        xmlDocument.Load(file.FullName);

                        foreach (XmlNode headNode in xmlDocument.ChildNodes)
                        {

                            if (headNode.Name == "Assets")
                            {
                                foreach (XmlNode assetNode in headNode.FirstChild.ChildNodes)
                                {
                                    string id  = assetNode.Attributes["id"].Value;
                                    string name = assetNode.Attributes["name"].Value;
                                    string path = assetPath + "\\" + assetNode.Attributes["relativePath"].Value;

                                    returnList.Add(new Asset(id, name, path));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Can't find assets for " + environmentPath);
            }

            return returnList;
        }


        public List<Document> GetDocuments(string documentPath)
        {
            List<Document> returnList = new List<Document>();

            if (Directory.Exists(documentPath))
            {
                List<string> assetIds = new List<string>();

                DirectoryInfo directoryInfo = new DirectoryInfo(documentPath);
                FileInfo[] files = directoryInfo.GetFiles();

                XmlDocument xmlDocument = new XmlDocument();


                foreach (FileInfo file in files)
                {
                    if (file.Name.Contains("data") && file.Name.Contains("xml"))
                    {
                        xmlDocument.Load(file.FullName);

                        foreach (XmlNode headNode in xmlDocument.ChildNodes)
                        {

                            if (headNode.Name == "Documents")
                            {
                                foreach (XmlNode documentNode in headNode.FirstChild.ChildNodes)
                                {

                                    string id = documentNode.Attributes["id"].Value;
                                    string name = documentNode.Attributes["name"].Value;
                                    string path = documentPath + "\\" + documentNode.Attributes["relativePath"].Value;

                                    returnList.Add(new Document(id, name, path));
                                }
                            }
                        }
                    }
                }
            }

            return returnList;
        }


        public List<string> GetDocumentsUsedAssetIds(IEnumerable<Document> documents)
        {
            List<string> usedAssets = new List<string>();

            if (documents.Count<Document>() > 0)
            {
                foreach (Document document in documents)
                {
                    usedAssets.AddRange(GetDocumentUsedAssetIds(document));
                }
            }

            return usedAssets;
        }

        private XmlDocument xmlDocument = new XmlDocument();

        public List<string> GetDocumentUsedAssetIds(Document document)
        {
            List<string> usedAssets = new List<string>();


            string path = document.path;

            try
            {
                xmlDocument.Load(path);

                foreach (XmlNode imageFrameNode in xmlDocument.SelectNodes("/document/pages//item[@type='image']"))
                {
                    if (imageFrameNode.Attributes["externalID"] != null && imageFrameNode.Attributes["externalID"].Value != "")
                    {
                        string id = imageFrameNode.Attributes["externalID"].Value;

                        if (usedAssets.Contains(id) == false)
                        {
                            usedAssets.Add(id);
                        }
                    }
                }

                foreach (XmlNode imageVarNode in xmlDocument.SelectNodes("/document/variables/item[@dataType='image']"))
                {
                    if (imageVarNode.Attributes["imgXML"] != null && imageVarNode.Attributes["imgXML"].Value != "")
                    {
                        string imgXML = imageVarNode.Attributes["imgXML"].Value;

                        xmlDocument.LoadXml(imgXML);

                        if (xmlDocument.FirstChild.Attributes["id"] != null)
                        {
                            string id = xmlDocument.FirstChild.Attributes["id"].Value;

                            if (usedAssets.Contains(id) == false)
                            {
                                usedAssets.Add(id);
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error Parsing " + path);
            }

            return usedAssets;
        }

        public List<string> GetDuplicateAssetIds()
        {
            List<string> duplicateIds = new List<string>();

            List<string> assetIds = new List<string>();

            foreach (Asset asset in Assets)
            {
                if (assetIds.Contains(asset.id) == false)
                {
                    assetIds.Add(asset.id);
                }
                else
                {
                    if (duplicateIds.Contains(asset.id) == false)
                    {
                        duplicateIds.Add(asset.id);
                    }
                }
            }

            return duplicateIds;
        }

        public List<Asset> GetUnusedAssets()
        {
            List<Asset> unusedAssets = new List<Asset>();

            List<string> usedAssetIds = cacheUsedAssetIds;

            if (usedAssetIds.Count == 0)
            {
                usedAssetIds = GetDocumentsUsedAssetIds(Documents);
            }

            foreach (Asset asset in Assets)
            {
                if (usedAssetIds.Contains(asset.id) == false)
                {
                    unusedAssets.Add(asset);
                }
            }

            return unusedAssets;
        }

        public List<Asset> GetAssetsWithMissingSourceFile()
        {
            List<Asset> missingAssets = new List<Asset>();

            foreach (Asset asset in Assets)
            {
                if (File.Exists(asset.path) == false)
                {
                    missingAssets.Add(asset);
                }
            }

            return missingAssets;
        }

        public List<Document> GetDocumentsWithDeletedAssets()
        {
            List<Document> documentWithDeletedAssets = new List<Document>();

            foreach (Document document in Documents)
            {
                List<string> usedIds = GetDocumentUsedAssetIds(document);

                bool assetDeleted = false;

                if (usedIds.Count > 0)
                {
                    List<Asset> assetList = Assets.ToList();

                    foreach (string id in usedIds)
                    {
                        if (assetList.FindIndex(a => a.id == id) == -1)
                        {
                            assetDeleted = true;
                        }
                    }

                }

                if (assetDeleted == true)
                {
                    documentWithDeletedAssets.Add(document);
                }
            }

            return documentWithDeletedAssets;
        }

        public List<Document> GetDocumentsWithMissingSourceFile()
        {
            List<Document> missingDocuments = new List<Document>();

            foreach (Document document in Documents)
            {
                if (File.Exists(document.path) == false)
                {
                    missingDocuments.Add(document);
                }
            }

            return missingDocuments;
        }

    }
}
