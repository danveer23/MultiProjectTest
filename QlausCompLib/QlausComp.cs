using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TTILoggerLib;

namespace QlausCompLib
{
    public abstract class QlausComp
    {
        //All the parameters received from CLI
        Dictionary<string, string> diParameters = new Dictionary<string, string>();
        public string strQlausPath = @"C:\SIEMENS\QlausPlus\QlausExecute.exe";
        public string strPrepResFileName = "Task_list.xml";
        public string strDefaultQlausDB = "SINUMERIK";
        public string strPlinkPath = @"..\..\..\..\Tools\plink.exe";
        public string strPscpPath = @"..\..\..\..\Tools\pscp.exe";
        public string strPsexecPath = @"..\..\..\..\Tools\psexec.exe";
        public string strPuttyKey = @"..\..\..\..\Tools\Keys\manufact-ad-mc-key.ppk";
        public string str1740PuttyKey = @"..\..\..\..\Tools\Keys\manufact-df-mc-key.ppk";
        public static short nRetryCount = 5;
        public static string strQlausIP = "157.163.247.2";
        public string strQlausExecuteTimeout = "5";//5 minutes

        /// <summary>
        /// Execute the required functionality
        /// </summary>
        /// <returns></returns>
        public abstract QlausCompUtilities.ExitCodes Execute();

        /// <summary>
        /// Cleanup the used resources
        /// </summary>
        public abstract void Cleanup();

        /// <summary>
        /// Initialize all input parameters
        /// </summary>
        /// <param name="diParameters"></param>
        /// <returns></returns>
        public abstract QlausCompUtilities.ExitCodes Initialize(Dictionary<string, string> diParameters, FileLogger m_mainLogger);

        /// <summary>
        /// Get the object based on type of operation required
        /// </summary>
        /// <param name="strFunction"></param>
        /// <returns></returns>
        public static QlausComp GetComponent(string strFunction)
        {
            switch (strFunction.ToLower())
            {
                case "retrievetests":
                    return new TaskRetriever();
                case "prepareresults":
                    return new PrepareResults();
                case "importresults":
                    return new ResultImporter();
                default:
                    throw new NotImplementedException();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_mainLoggerTR"></param>
        /// <param name="strlogSource"></param>
        /// <returns></returns>
        public static bool CheckQlausAvailability(string strlogSource = "", FileLogger m_mainLogger = null, int nTimeoutInSecs = 60 * 5)
        {
            nTimeoutInSecs = nTimeoutInSecs * 1000;
            bool bQlausStatus = false;
            short nRetrieveRetry = nRetryCount;
            while (nRetrieveRetry > 0)
            {
                if (m_mainLogger != null)
                {
                    m_mainLogger.LogMsg(strlogSource, string.Format("Checking if Qlaus server is reachable. Try number {0}", (nRetryCount - nRetrieveRetry + 1)), LogType.LOG_INFO, true);
                }
                if (QlausCompUtilities.PingTarget(strQlausIP))
                {
                    bQlausStatus = true;
                    break;
                }
                if (m_mainLogger != null)
                {
                    m_mainLogger.LogMsg(strlogSource, string.Format("Qlaus server {0} not reachable.", strQlausIP), LogType.LOG_ERROR, true);
                }
                if (--nRetrieveRetry != 0)
                {
                    Thread.Sleep(nTimeoutInSecs);
                }
            }
            return bQlausStatus;
        }
    }
}
