using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QlausCompLib
{
    /// <summary>
    /// Class that encpasulates TestTarget details
    /// </summary>
    [Serializable]
    public class TestTargetData
    {
        private String m_strTestTargetAddress;
        private String m_strTargetIdent;
        private String m_strCurrentVersion;
        private String m_strRequiredVersion;
        private QlausCompUtilities.HW_Type m_strTestTargetType;
        private String m_strWindowsPath;
        private String m_strTemplateName;
        private String m_strGIVVersion;
        private String m_strGIVPath;

        /// <summary>
        /// In case of PCU/IPC
        /// </summary>
        public string ConnectedNCU { get; set; }

        /// <summary>
        /// In case of PCU/IPC
        /// </summary>
        public string ConnectedNCUIdent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WindowsPath
        {
            get { return m_strWindowsPath; }
            set { m_strWindowsPath = value; }
        }

        /// <summary>
        /// IP of Test Target
        /// </summary>
        public string TestTargetIPAddress
        {
            get { return m_strTestTargetAddress; }
            set { m_strTestTargetAddress = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public QlausCompUtilities.HW_Type TestTargetType
        {
            get { return m_strTestTargetType; }
            set { m_strTestTargetType = value; }
        }

        public string GIVPath
        {
            get { return m_strGIVPath; }
            set { m_strGIVPath = value; }
        }
        /// <summary>
        /// Ident of TestTarget as in TestOperate.xml
        /// </summary>
        public string TargetIdent
        {
            get { return m_strTargetIdent; }
            set { m_strTargetIdent = value; }
        }

        /// <summary>
        /// Current build version available in the operate
        /// </summary>
        public string CurrentVersion
        {
            get { return m_strCurrentVersion; }
            set { m_strCurrentVersion = value; }
        }

        /// <summary>
        /// Required EVC Template Name
        /// </summary>
        public string EVCTemplateName
        {
            get { return m_strTemplateName; }
            set { m_strTemplateName = value; }
        }
        /// <summary>
        /// Required EVC Template Name
        /// </summary>
        public string GIVVersion
        {
            get { return m_strGIVVersion; }
            set { m_strGIVVersion = value; }
        }

        /// <summary>
        /// Required build version to be installed in the operate
        /// </summary>
        public string RequiredVersion
        {
            get { return m_strRequiredVersion; }
            set { m_strRequiredVersion = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public TestTargetData()
        {
            ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strTestTargetIPAddress"></param>
        /// <param name="strRequiredVersion"></param>
        /// <param name="strCurrentVersion"></param>
        /// <param name="strTargetIdent"></param>
        /// <param name="strWindowspath"></param>
        /// <param name="TestTargetType"></param>
        public TestTargetData(String strTestTargetIPAddress, String strRequiredVersion, String strTargetIdent = "", String strWindowspath = "", QlausCompUtilities.HW_Type TestTargetType = QlausCompUtilities.HW_Type.NCU)
        {
            m_strTestTargetAddress = strTestTargetIPAddress;
            m_strRequiredVersion = strRequiredVersion;
            //m_strCurrentVersion = strCurrentVersion;
            m_strTargetIdent = strTargetIdent;
            m_strTestTargetType = TestTargetType;
            m_strWindowsPath = strWindowspath;

        }
    }
}
