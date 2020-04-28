using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using TTILoggerLib;
using static QlausCompLib.QlausCompUtilities;


namespace QlausCompLib
{
    class TaskRetriever : QlausComp, IDisposable
    {
        #region Variables used throughout the class
        /// <summary>
        /// 
        /// </summary>
        public string strOutputPath { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string strTaskID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string strQlausDB { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string strTestEnv { get; set; }
        //Filtered list of test runs based on test environment configured
        List<string> lTestRuns = new List<string>();
        Dictionary<string, List<string>> dTestRunsWithTasks = new Dictionary<string, List<string>>();
        string strOutputFileName = "Testruns2Execute.txt";
        string strOutputFileTRandTasks = "TestrunsAndTasks.txt";
        ExitCodes exitStatus;
        FileLogger m_fileLogger = null;
        FileLogger m_mainLogger = null;
        string strTargetLoggerName = string.Empty;
        string strlogSource = "RetrieveTests";
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public TaskRetriever()
        {
            ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strTestEnv"></param>
        /// <param name="strTaskID"></param>
        /// <param name="strOutputPath"></param>
        /// <param name="strQlausDB"></param>
        /// <returns></returns>
        public override ExitCodes Initialize(Dictionary<string, string> diParameters, FileLogger m_mainLoggerTR)
        {
            m_mainLogger = m_mainLoggerTR;
            if (!CheckQlausAvailability(m_mainLogger:m_mainLoggerTR, strlogSource:strlogSource))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Qlaus server not reachable."), LogType.LOG_ERROR, true);
                return ExitCodes.QLAUS_NOT_REACHABLE;
            }
            if (!diParameters.ContainsKey("retrievepath") || string.IsNullOrEmpty(diParameters["retrievepath"]))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Output path for retieving tests is not provided"), LogType.LOG_ERROR, true);
                return ExitCodes.INSUFFICIENT_ARGUMENTS;
            }
            strOutputPath = diParameters["retrievepath"];
            if (!Directory.Exists(strOutputPath))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Output path {0} provided does not exist.", strOutputPath), LogType.LOG_ERROR, true);
                return ExitCodes.FOLDER_NOT_FOUND;
            }
            if (!diParameters.ContainsKey("taskid") || string.IsNullOrEmpty(diParameters["taskid"]))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Qlaus task ID not provided"), LogType.LOG_ERROR, true);
                return ExitCodes.INSUFFICIENT_ARGUMENTS;
            }
            strTaskID = diParameters["taskid"];
            if (Regex.Match(strTaskID, @"^\d+$").Success != true)
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Task ID specfied {0} is invalid", strTaskID), LogType.LOG_ERROR, true);
                return ExitCodes.INCORRECT_DATA;
            }
            if (!diParameters.ContainsKey("qlausdb") || string.IsNullOrEmpty(diParameters["qlausdb"]))
            {
                strQlausDB = strDefaultQlausDB;
            }
            else
            {
                strQlausDB = diParameters["qlausdb"];
            }
            if (!diParameters.ContainsKey("testenv") || string.IsNullOrEmpty(diParameters["testenv"]))
            {
                strTestEnv = "";
            }
            else
            {
                strTestEnv = diParameters["testenv"];
            }
            strTargetLoggerName = "QlausComp_" + diParameters["mode"] + "_" + diParameters["taskid"] + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv";
            if (!InitializeLogger())
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Could not initialize the logger {0}", strTargetLoggerName), LogType.LOG_ERROR, true);
                return ExitCodes.LOGGER_INITIALIZATION_FAILED;
            }
            return ExitCodes.SUCCESSFUL;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool InitializeLogger()
        {
            try
            {
                m_fileLogger = new FileLogger(strTargetLoggerName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override ExitCodes Execute()
        {
            bool bExecuteQuery = false;
            short nRetrieveRetry = nRetryCount;
            while (nRetrieveRetry > 0)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Trying to retrieve tasks. Try number {0}", (nRetryCount - nRetrieveRetry + 1)), LogType.LOG_INFO, true);
                if (PrepareAndExecuteQuery())
                {
                    bExecuteQuery = true;
                    break;
                }
                if (--nRetrieveRetry != 0)
                {
                    Thread.Sleep(5 * 1000);
                }
            }
            if (!bExecuteQuery)
            {
                return ExitCodes.QLAUS_QUERY_FAILED;
            }
            exitStatus = GetListOfTestRuns();
            if (exitStatus != ExitCodes.SUCCESSFUL)
            {
                return exitStatus;
            }
            if (!GenerateOutputFile())
            {
                return ExitCodes.OUTPUT_FILE_GEN_FAILED;
            }
            m_fileLogger.LogMsg(strlogSource, string.Format("Retrieving tasks successful."), LogType.LOG_INFO, true);
            return ExitCodes.SUCCESSFUL;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Cleanup()
        {
            ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool PrepareAndExecuteQuery()
        {
            string strArgs = " /d " + strQlausDB + " /a " + strTaskID + " /listpath " + strOutputPath;
            string CmdOutput = string.Empty;
            Tuple <int, string> tuCommandResp = ExecuteCommand(ref CmdOutput, strFileName: strQlausPath, strArguments:strArgs, strTimeOut: strQlausExecuteTimeout);
            m_fileLogger.LogMsg(strlogSource, string.Format("Trying to execute command to retrieve tasks.."), LogType.LOG_INFO, true);
            m_fileLogger.LogMsg(strlogSource, string.Format("Executing the command {0} {1}", strQlausPath, strArgs), LogType.LOG_INFO);
            if (tuCommandResp.Item1 != 0)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Retreiving tasks from Qlaus failed. Error: {0}", tuCommandResp.Item2), LogType.LOG_ERROR, true);
                return false;
            }
            m_fileLogger.LogMsg(strlogSource, string.Format("Retrieved tasks successfully."), LogType.LOG_INFO, true);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> FilterByInputEnv()
        {
            return lTestRuns;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool GenerateOutputFile()
        {
            m_fileLogger.LogMsg(strlogSource, string.Format("Trying to genetare output file."), LogType.LOG_INFO, true);
            string strOutputFile = Path.Combine(strOutputPath, strOutputFileName);
            string strOutputFileTR = Path.Combine(strOutputPath, strOutputFileTRandTasks);
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(strOutputFile))
                {
                    foreach (string strTestTun in lTestRuns)
                    {
                        streamWriter.Write(strTestTun + "\n");
                    }
                }
                using (StreamWriter streamWriter = new StreamWriter(strOutputFileTR))
                {
                    foreach (var dTestRundetail in dTestRunsWithTasks)
                    {
                        streamWriter.Write(dTestRundetail.Key + ";" + string.Join(",", dTestRundetail.Value) + "\n");
                    }
                }
            }
            catch (Exception ex)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Could not generate output file {0}. Error: {1}", strOutputFile, ex.Message), LogType.LOG_ERROR, true);
                return false;
            }
            m_fileLogger.LogMsg(strlogSource, string.Format("Generated output file {0} successfully.", strOutputFile), LogType.LOG_INFO, true);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ExitCodes GetListOfTestRuns()
        {
            List<string> strFilePaths = new List<string>();
            m_fileLogger.LogMsg(strlogSource, string.Format("Getting list of test runs.."), LogType.LOG_INFO, true);
            try
            {
                strFilePaths = Directory.GetFiles(strOutputPath, "*.xml", SearchOption.TopDirectoryOnly).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Access to the path {0} is denied", strOutputPath), LogType.LOG_ERROR, true);
                return ExitCodes.ACCESS_DENIED;
            }
            catch (Exception ex)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Folder {0} not found. Error: {1}", strOutputPath, ex.Message), LogType.LOG_ERROR, true);
                return ExitCodes.FOLDER_NOT_FOUND;
            }
            if (strFilePaths.Count == 0)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("No xmls genetared after executing the query for the task {0}", strTaskID), LogType.LOG_ERROR, true);
                return ExitCodes.QLAUS_RETURNED_EMPTY_LIST;
            }
            // Read xml files created by QlausPlus to get the child tasks
            m_fileLogger.LogMsg(strlogSource, string.Format("Reading the xmls genetared.."), LogType.LOG_INFO, true);
            try
            {
                foreach (string strFileName in strFilePaths)
                {
                    using (StreamReader srQlausChildTasks = new StreamReader(strFileName, true))
                    {
                        XmlDocument qlausParentXML = new XmlDocument();
                        qlausParentXML.Load(srQlausChildTasks);
                        XmlNodeList noOfTaskItemNodes = qlausParentXML.SelectNodes("/Tasks/Task");
                        if (noOfTaskItemNodes.Count > 0)
                        {
                            foreach (XmlElement node in noOfTaskItemNodes)
                            {
                                string strTestName = node["Name"].InnerText;
                                string strTaskID = node["ID"].InnerText;
                                XmlNodeList noOfTestrunFiles = qlausParentXML.SelectNodes("/Tasks/Task[Name='" + strTestName + "']");
                                if (noOfTestrunFiles.Count > 0)
                                {
                                    List<string> lTaskRefIDs = new List<string>();
                                    List<string> lTestRunsNames = new List<string>();
                                    foreach (XmlElement testRun in noOfTestrunFiles)
                                    {
                                        XmlNodeList lOfRefIDs = testRun.GetElementsByTagName("Reference-ID");
                                        for (int i = 0; i < lOfRefIDs.Count; i++)
                                        {
                                            lTestRuns.Add(@"\hmi_swtest\testcase_repository" + lOfRefIDs[i].InnerText.ToString());
                                            lTestRunsNames.Add(@"\hmi_swtest\testcase_repository" + lOfRefIDs[i].InnerText.ToString());
                                        }
                                    }
                                    dTestRunsWithTasks.Add(strTestName + ";" + strTaskID, lTestRunsNames);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Reading the xmls failed. Error: {0}", ex.Message), LogType.LOG_ERROR, true);
                return ExitCodes.COULD_NOT_READ_QLAUS_GENERATED_XML;
            }
            if (lTestRuns.Count == 0)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("No test runs found in the xmls generated by qlaus."), LogType.LOG_ERROR, true);
                return ExitCodes.QLAUS_RETURNED_EMPTY_LIST;
            }
            m_fileLogger.LogMsg(strlogSource, string.Format("Test run list generated."), LogType.LOG_INFO, true);
            return ExitCodes.SUCCESSFUL;
        }

        /// <summary>
        /// Dispose the disposable elements
        /// </summary>
        public void Dispose()
        {
            if (m_fileLogger != null)
            {
                m_fileLogger.Dispose();
            }
        }
    }
}
