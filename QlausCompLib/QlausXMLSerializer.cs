using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace QlausCompLib
{
    /// <summary>
    /// 
    /// </summary>
    //[Serializable]
    [XmlRoot("QlausPlus")]
    public class QlausPlus
    {
        private TestResult[] arTestResult;

        /// <summary>
        /// 
        /// </summary>
        [XmlArrayItem(ElementName = "Testresult")]
        public TestResult[] TestresultList
        {
            get
            {
                return arTestResult;
            }

            set
            {
                arTestResult = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TestResult
    {
        private string m_strTestcaseName;
        private string m_strReferenceID;
        private string m_strTestEngine;
        private string m_strStatus;
        private string m_strDate;
        private string m_strTester;
        private string m_strDuration;
        private string m_strTaskID;
        private string m_strComment;
        private string[] Files;
        private string m_strRackName;
        private Software[] SWVersions;
        private Software[] HWIndVersions;
        private Hardware[] Hardwares;
        private TestStep[] TestSteps;
        private TestEnvironment[] TestEnvironments;

        /// <summary>
        /// 
        /// </summary>
        public string TestcaseName
        {
            get
            {
                return m_strTestcaseName;
            }

            set
            {
                m_strTestcaseName = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("Reference-ID")]
        public string ReferenceID
        {
            get
            {
                return m_strReferenceID;
            }

            set
            {
                m_strReferenceID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TestEngine
        {
            get
            {
                return m_strTestEngine;
            }

            set
            {
                m_strTestEngine = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Status
        {
            get
            {
                return m_strStatus;
            }

            set
            {
                m_strStatus = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Date
        {
            get
            {
                return m_strDate;
            }

            set
            {
                m_strDate = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Tester
        {
            get
            {
                return m_strTester;
            }

            set
            {
                m_strTester = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Duration
        {
            get
            {
                return m_strDuration;
            }

            set
            {
                m_strDuration = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TaskID
        {
            get
            {
                return m_strTaskID;
            }

            set
            {
                m_strTaskID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Comment
        {
            get
            {
                return m_strComment;
            }

            set
            {
                m_strComment = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        // The XmlArray attribute changes the XML element name
        // from the default of "Files1" to "Files".
        [XmlArray("Files")]
        [XmlArrayItem("File")]
        public string[] liFiles
        {
            get
            {
                return Files;
            }

            set
            {
                Files = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlArray("SWVersions")]
        public Software[] liSWVersions
        {
            get
            {
                return SWVersions;
            }

            set
            {
                SWVersions = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlArray("HwIndVersions")]
        public Software[] liHWIndVersions
        {
            get
            {
                return HWIndVersions;
            }

            set
            {
                HWIndVersions = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlArray("Hardwares")]
        public Hardware[] liHardwares
        {
            get
            {
                return Hardwares;
            }

            set
            {
                Hardwares = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlArray("TestSteps")]
        public TestStep[] liTestSteps
        {
            get
            {
                return TestSteps;
            }

            set
            {
                TestSteps = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlArray("TestEnvironments")]
        public TestEnvironment[] liTestEnvironments
        {
            get
            {
                return TestEnvironments;
            }

            set
            {
                TestEnvironments = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Rack
        {
            get
            {
                return m_strRackName;
            }

            set
            {
                m_strRackName = value;
            }
        }
    }
}
