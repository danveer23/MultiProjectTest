using System;
using System.Collections.Generic;
using System.Xml;

namespace QlausCompLib
{
    public class QlausConfigManager
    {
        // For read & write properties <GlobalConfig file tags>
        private string m_strTestEngine;
        private string m_strQlausDB;
        private string m_nRetryCount;
        private string m_nQlausTimeout;

        public string TestEngine
        {
            get { return m_strTestEngine; }
            set { m_strTestEngine = value; }
        }

        public string QlausDB
        {
            get { return m_strQlausDB; }
            set { m_strQlausDB = value; }
        }

        public string RetryCount
        {
            get { return m_nRetryCount; }
            set { m_nRetryCount = value; }
        }

        public string QlausTimeout
        {
            get { return m_nQlausTimeout; }
            set { m_nQlausTimeout = value; }
        }

        // For read & write both properties <LocalConfig file tags>
        private string m_strQlausImportFile;
        private string m_strImportLocations;
        private string m_strTestenv;
        private string m_strTaskid;
        private string m_strPuuid;
        private string m_strUUIDIdentifier;
        private string m_strRetrievePath;
        private string m_strAttachments;
        private string m_strPrepareResultsPath;
        private string m_strTargetType;
        private string m_strSWXML;
        private string m_strHWXML;

        public string QlausImportFile
        {
            get { return m_strQlausImportFile; }
            set { m_strQlausImportFile = value; }
        }

        public string ImportLocations
        {
            get { return m_strImportLocations; }
            set { m_strImportLocations = value; }
        }

        public string Testenv
        {
            get { return m_strTestenv; }
            set { m_strTestenv = value; }
        }

        public string Taskid
        {
            get { return m_strTaskid; }
            set { m_strTaskid = value; }
        }

        public string Puuid
        {
            get { return m_strPuuid; }
            set { m_strPuuid = value; }
        }

        public string UUIDIdentifier
        {
            get { return m_strUUIDIdentifier; }
            set { m_strUUIDIdentifier = value; }
        }

        public string RetrievePath
        {
            get { return m_strRetrievePath; }
            set { m_strRetrievePath = value; }
        }

        public string Attachments
        {
            get { return m_strAttachments; }
            set { m_strAttachments = value; }
        }

        public string PrepareResultsPath
        {
            get { return m_strPrepareResultsPath; }
            set { m_strPrepareResultsPath = value; }
        }

        public string TargetType
        {
            get { return m_strTargetType; }
            set { m_strTargetType = value; }
        }

        public string SWXML
        {
            get { return m_strSWXML; }
            set { m_strSWXML = value; }
        }

        public string HWXML
        {
            get { return m_strHWXML; }
            set { m_strHWXML = value; }
        }

        // For Global & Local Config files
        private string m_strGlobalConfigFile;
        private string m_strLocalConfigFile;

        public QlausConfigManager(string strGlobalConfig, string strLocalConfig)
        {
            m_strGlobalConfigFile = strGlobalConfig;
            m_strLocalConfigFile = strLocalConfig;
        }

        public bool ReadConfig()
        {
            XmlDocument xmlGlobalConfig = new XmlDocument();
            XmlDocument xmlLocalConfig = new XmlDocument();
            try
            {
                xmlGlobalConfig.Load(m_strGlobalConfigFile);   
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught while reading Qlaus Global config file. Error message : {0}", e.Message);
                return false;
            }

            // Reading all the nodes in Global Config file
            try
            {                
                XmlNode GlobalConfigNode = xmlGlobalConfig.DocumentElement;
                XmlNodeList GlobalConfigNodeList = GlobalConfigNode.ChildNodes;

                foreach (XmlNode GlobalConfigFileNode in GlobalConfigNodeList)
                {
                    if(GlobalConfigFileNode.Name.Equals("testengine"))
                    {
                        TestEngine = GlobalConfigFileNode.InnerText;
                    }

                    if (GlobalConfigFileNode.Name.Equals("qlausdb"))
                    {
                        QlausDB = GlobalConfigFileNode.InnerText;
                    }

                    if (GlobalConfigFileNode.Name.Equals("retrycount"))
                    {
                        RetryCount = GlobalConfigFileNode.InnerText;
                    }

                    if (GlobalConfigFileNode.Name.Equals("qlaustimeout"))
                    {
                        QlausTimeout = GlobalConfigFileNode.InnerText;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught while reading QlausComp Global config file. Error message : {0}", e.Message);
                return false;
            }

            try
            {
                xmlLocalConfig.Load(m_strLocalConfigFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught while reading Qlaus Local config file. Error message : {0}", e.Message);
                return false;
            }

            // Reading all the nodes in Local Config file
            try
            {                
                XmlNode LocalConfigNode = xmlLocalConfig.DocumentElement;
                XmlNodeList LocalConfigNodeList = LocalConfigNode.ChildNodes;

                foreach (XmlNode LocalConfigFileNode in LocalConfigNodeList)
                {
                    if (LocalConfigFileNode.Name.Equals("qlausimportfile"))
                    {
                        QlausImportFile = LocalConfigFileNode.InnerText;
                    }

                    if (LocalConfigFileNode.Name.Equals("importlocations"))
                    {
                        ImportLocations = LocalConfigFileNode.InnerText;
                    }

                    if (LocalConfigFileNode.Name.Equals("testenv"))
                    {
                        Testenv = LocalConfigFileNode.InnerText;
                    }

                    if (LocalConfigFileNode.Name.Equals("taskid"))
                    {
                        Taskid = LocalConfigFileNode.InnerText;
                    }

                    if (LocalConfigFileNode.Name.Equals("puuid"))
                    {
                        Puuid = LocalConfigFileNode.InnerText;
                    }

                    if (LocalConfigFileNode.Name.Equals("uuididentifier"))
                    {
                        UUIDIdentifier = LocalConfigFileNode.InnerText;
                    }

                    if (LocalConfigFileNode.Name.Equals("retrievepath"))
                    {
                        RetrievePath = LocalConfigFileNode.InnerText;
                    }

                    if (LocalConfigFileNode.Name.Equals("attachments"))
                    {
                        Attachments = LocalConfigFileNode.InnerText;
                    }

                    if (LocalConfigFileNode.Name.Equals("prepareresultspath"))
                    {
                        PrepareResultsPath = LocalConfigFileNode.InnerText;
                    }

                    if (LocalConfigFileNode.Name.Equals("targettype"))
                    {
                        TargetType = LocalConfigFileNode.InnerText;
                    }

                    if (LocalConfigFileNode.Name.Equals("swxml"))
                    {
                        SWXML = LocalConfigFileNode.InnerText;
                    }

                    if (LocalConfigFileNode.Name.Equals("hwxml"))
                    {
                        HWXML = LocalConfigFileNode.InnerText;
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught while reading QlausComp Local config file. Error message : {0}", e.Message);
                return false;
            }

            return true;
        }

        public Dictionary<string,string> GetConfigParams()
        {
            Dictionary<string, string> dConfigParams = new Dictionary<string, string>();
            dConfigParams.Add("testengine", TestEngine);
            dConfigParams.Add("qlausdb", QlausDB);
            dConfigParams.Add("retrycount", RetryCount);
            dConfigParams.Add("qlaustimeout", QlausTimeout);
            dConfigParams.Add("qlausimportfile", QlausImportFile);
            dConfigParams.Add("importlocations", ImportLocations);
            dConfigParams.Add("testenv", Testenv);
            dConfigParams.Add("taskid", Taskid);
            dConfigParams.Add("puuid", Puuid);
            dConfigParams.Add("uuididentifier", UUIDIdentifier);
            dConfigParams.Add("retrievepath", RetrievePath);
            dConfigParams.Add("attachments", Attachments);
            dConfigParams.Add("prepareresultspath", PrepareResultsPath);
            dConfigParams.Add("targettype", TargetType);
            dConfigParams.Add("swxml", SWXML);
            dConfigParams.Add("hwxml", HWXML);
            return dConfigParams; 
        }

    }
}
