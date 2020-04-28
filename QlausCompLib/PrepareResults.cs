using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using TTILoggerLib;
using static QlausCompLib.QlausCompUtilities;

namespace QlausCompLib
{
    class PrepareResults : QlausComp, IDisposable
    {
        #region Variables used throughout the class
        public string strDestPath { get; set; }
        public string strExecuteTestsLog { get; set; }
        public Hardware hwInfo { get; set; }
        public string strIdentifierWithUUID { get; set; }
        public string strTargetAddress { get; set; }
        public string strTestRun { get; set; }
        public string strTaskID { get; set; }
        public string strUUID { get; set; }
        public string strProtocolFiles { get; set; }
        public string strTargetIdent { get; set; }
        public string strParentUUID { get; set; }
        public HW_Type hwType { get; set; }
        public string strTargetType { get; set; }//To be added as parameter
        //Variables
        private string strPrepResFolderName = "Result";
        private string strPrepResOutput = string.Empty;
        private List<string> lFilesToBeImported = new List<string>();
        private string strTestEngine = "Squish";
        private string strIPidentifier = "Testtarget is:";
        private string strSummaryIdentifier = "Total squish summary";
        private string strTargetIdentIdentifier = "Testtarget Ident is:";
        private string strNumFailsIdent = "Fails:";
        private string strNumErrorsIdent = "Errors:";
        private string strNumFatalsIdent = "Fatals:";
        private string strNumWarningsIdent = "Warnings:";
        private string strNumPassesIdent = "Passes:";
        private string strNumTestsIdent = "Tests:";
        private string strNumTCNameIdent = "Testcase:";
        private string strNumTCDescIdent = "MA:";
        private string strNumTCDurationIdent = "Total-Duration:";
        private string strSummaryDurationIdent = "time needed:";
        private string strHoursIdent = "hours";
        private string strMinutesIdent = "minutes";
        private string strSecondsIdent = "seconds";
        private string strTCRepoIdent = "testcase_repository";
        private string strtestRunIdent = "Configurationfile";
        private Dictionary<string, string> diParameters = new Dictionary<string, string>();
        private string strTCSeparator = "---------------------------------------------------------------";
        private bool bValidLog = false;
        private bool bSummary = false;
        private bool bDuration = false;
        private bool bTargetAddress = false;
        FileLogger m_fileLogger = null;
        FileLogger m_mainLogger = null;
        string strTargetLoggerName = string.Empty;
        string strSWCompXML = string.Empty;
        string strHWCompXML = string.Empty;
        string strlogSource = "PrepareResults";
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public PrepareResults()
        {
            ;
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
        /// Initializes and checks all the required data
        /// </summary>
        /// <returns></returns>
        public override ExitCodes Initialize(Dictionary<string, string> diInputParameters, FileLogger m_mainLoggerPR)
        {
            m_mainLogger = m_mainLoggerPR;
            diParameters = diInputParameters;
            ExitCodes exitInitialize;
            if ((exitInitialize = CheckTargetDetails()) != ExitCodes.SUCCESSFUL || (exitInitialize = CheckHWandSWCompXMLDetails()) != ExitCodes.SUCCESSFUL)
            {
                return exitInitialize;
            }
            if (diParameters.ContainsKey("uuid-identifier") && !string.IsNullOrEmpty(diParameters["uuid-identifier"]))
            {
                strIdentifierWithUUID = diParameters["uuid-identifier"];
            }
            else
            {
                strIdentifierWithUUID = "QlausComponent";
            }
            if (!diParameters.ContainsKey("executetestslog") || string.IsNullOrEmpty(diParameters["executetestslog"]))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Execute tests log file not provided."), LogType.LOG_ERROR, true);
                return ExitCodes.INSUFFICIENT_ARGUMENTS;
            }
            strExecuteTestsLog = diParameters["executetestslog"];
            if (!File.Exists(strExecuteTestsLog))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Execute tests log {0} does not exist", strExecuteTestsLog), LogType.LOG_ERROR, true);
                return ExitCodes.FILE_NOT_FOUND;
            }
            if (diParameters.ContainsKey("puuid") && !string.IsNullOrEmpty(diParameters["puuid"]))
            {
                strParentUUID = diParameters["puuid"];
            }
            else
            {
                strParentUUID = "";
            }
            if (!diParameters.ContainsKey("protocolfiles") || string.IsNullOrEmpty(diParameters["protocolfiles"]))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("One or more protocol fiels have to be specfied"), LogType.LOG_ERROR, true);
                return ExitCodes.INSUFFICIENT_ARGUMENTS;
            }
            strProtocolFiles = diParameters["protocolfiles"];
            lFilesToBeImported = strProtocolFiles.Split(',').ToList();
            lFilesToBeImported = GetListOfAvailableProtocolFiles();
            if (lFilesToBeImported.Count == 0)
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("No files to be imported"), LogType.LOG_ERROR, true);
                return ExitCodes.PROTOCOL_FILES_DONT_EXIST_OR_NOT_PROVIDED;
            }
            if (!diParameters.ContainsKey("taskid") || string.IsNullOrEmpty(diParameters["taskid"]))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("No Task ID specified"), LogType.LOG_ERROR, true);
                return ExitCodes.INSUFFICIENT_ARGUMENTS;
            }
            strTaskID = diParameters["taskid"];
            if (Regex.Match(strTaskID, @"^\d+$").Success != true)
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Task ID specfied {0} is invalid", strTaskID), LogType.LOG_ERROR, true);
                return ExitCodes.INCORRECT_DATA;
            }
            exitInitialize = PrepareResultFolder();
            if (exitInitialize != ExitCodes.SUCCESSFUL)
            {
                return exitInitialize;
            }
            m_mainLogger.LogMsg(strlogSource, string.Format("Initialization successfull."), LogType.LOG_ERROR, true);
            strTargetLoggerName = "QlausComp_" + diParameters["mode"] + "_" + diParameters["taskid"] + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv";
            if (!InitializeLogger())
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Could not initialize the logger {0}", strTargetLoggerName), LogType.LOG_ERROR, true);
                return ExitCodes.LOGGER_INITIALIZATION_FAILED;
            }
            return ExitCodes.SUCCESSFUL;
        }

        /// <summary>
        /// validates the target related information
        /// </summary>
        /// <returns></returns>
        public ExitCodes CheckTargetDetails()
        {
            if (!diParameters.ContainsKey("targettype") || string.IsNullOrEmpty(diParameters["targettype"]))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Target type not specified"), LogType.LOG_ERROR, true);
                Console.WriteLine("Target type not specified");
                return ExitCodes.INSUFFICIENT_ARGUMENTS;
            }
            strTargetType = diParameters["targettype"];
            hwType = GetTargetType(strTargetType);
            return ExitCodes.SUCCESSFUL;
        }

        /// <summary>
        /// validates the HW and SW component related information
        /// </summary>
        /// <returns></returns>
        public ExitCodes CheckHWandSWCompXMLDetails()
        {
            if (!diParameters.ContainsKey("swxml") || string.IsNullOrEmpty(diParameters["swxml"]))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Software component XML file not provided"), LogType.LOG_ERROR, true);
                return ExitCodes.SW_COMPONENT_FILE_NOT_PROVIDED;
            }
            strSWCompXML = diParameters["swxml"];
            if (!File.Exists(strSWCompXML))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Software component XML file {0} not found", strSWCompXML), LogType.LOG_ERROR, true);
                return ExitCodes.SW_COMPONENT_FILE_NOT_FOUND;
            }
            try
            {
                XmlDocument xmlSWCompDoc = new XmlDocument();
                xmlSWCompDoc.Load(strSWCompXML);
            }
            catch
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Software component XML file {0} not valid", strSWCompXML), LogType.LOG_ERROR, true);
                return ExitCodes.INCORRECT_DATA;
            }
            if (!diParameters.ContainsKey("hwxml") || string.IsNullOrEmpty(diParameters["hwxml"]))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Hardware component XML file not provided"), LogType.LOG_WARNING, true);
                //return ExitCodes.HW_COMPONENT_FILE_NOT_PROVIDED; For now, qlaus import can be done without HW components
            }
            else
            {
                strHWCompXML = diParameters["hwxml"];
                if (!File.Exists(strHWCompXML))
                {
                    m_mainLogger.LogMsg(strlogSource, string.Format("Hardware component XML file {0} not found", strHWCompXML), LogType.LOG_ERROR, true);
                    return ExitCodes.HW_COMPONENT_FILE_NOT_FOUND;
                }
                try
                {
                    XmlDocument xmlSWCompDoc = new XmlDocument();
                    xmlSWCompDoc.Load(strHWCompXML);
                }
                catch
                {
                    m_mainLogger.LogMsg(strlogSource, string.Format("Hardware component XML file {0} not valid", strHWCompXML), LogType.LOG_ERROR, true);
                    return ExitCodes.INCORRECT_DATA;
                }
            }
            return ExitCodes.SUCCESSFUL;
        }

        /// <summary>
        /// Checks the path and/or creates the result folder
        /// </summary>
        /// <returns></returns>
        public ExitCodes PrepareResultFolder()
        {
            m_mainLogger.LogMsg(strlogSource, string.Format("Preparing the result folder..."), LogType.LOG_INFO, true);
            if (!diParameters.ContainsKey("prepareresultspath") || string.IsNullOrEmpty(diParameters["prepareresultspath"]))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Destination path for prepare rsult is not provided. Results will be prepared in the current directory")
                                                                                                                                                , LogType.LOG_INFO, true);
                strDestPath = Environment.CurrentDirectory;
            }
            else
            {
                strDestPath = diParameters["prepareresultspath"];
                if (!Directory.Exists(strDestPath))
                {

                    m_mainLogger.LogMsg(strlogSource, string.Format("Result path {0} for prepare result does not exist.", strDestPath), LogType.LOG_ERROR, true);
                    return ExitCodes.FOLDER_NOT_FOUND;
                }
            }
            strPrepResOutput = Path.Combine(strDestPath, strPrepResFolderName);
            //As per requirements if the result folder exists with contents, the component should exit with a valid message
            if (Directory.Exists(strPrepResOutput))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Result folder {0} exists.", strPrepResOutput), LogType.LOG_INFO, true);
                string[] lContents = Directory.GetFiles(strPrepResOutput);
                if (lContents.Count() > 0)
                {
                    m_mainLogger.LogMsg(strlogSource, string.Format("The output result directory {0} is already present with previous results. Please delete it and try again."
                                                                                                                                , strPrepResOutput), LogType.LOG_ERROR, true);
                    return ExitCodes.RESULT_FOLDER_ALREADY_PRESENT;
                }
            }
            else
            {
                try
                {
                    m_mainLogger.LogMsg(strlogSource, string.Format("Creating the prepare result folder"), LogType.LOG_INFO, true);
                    Directory.CreateDirectory(strPrepResOutput);
                    m_mainLogger.LogMsg(strlogSource, string.Format("Created the prepare result folder"), LogType.LOG_INFO, true);
                }
                catch (Exception ex)
                {
                    m_mainLogger.LogMsg(strlogSource, string.Format("Could not create the prepare result directory {0}. Error: {1}", strPrepResOutput, ex.Message), LogType.LOG_ERROR, true);
                    return ExitCodes.RESULT_FOLDER_CREATION_FAILED;
                }
            }
            m_mainLogger.LogMsg(strlogSource, string.Format("Prepared the result folder successfully."), LogType.LOG_INFO, true);
            return ExitCodes.SUCCESSFUL;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Cleanup()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Preparation of XML
        /// </summary>
        /// <returns></returns>
        public override ExitCodes Execute()
        {
            return CreateQlausImoprtDataXML();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<string> GetListOfAvailableProtocolFiles()
        {
            List<string> lAvailableProtocolFiles = new List<string>();
            foreach (string strProtocolFile in lFilesToBeImported)
            {
                if (!File.Exists(strProtocolFile))
                {
                    Console.WriteLine(string.Format("Protocol file {0} not found so it will not be imported", strProtocolFile));
                }
                else
                {
                    lAvailableProtocolFiles.Add(strProtocolFile);
                }
            }
            return lAvailableProtocolFiles;
        }

        /// <summary>
        /// Retrieves total test duration from Log file
        /// </summary>
        /// <param name="strDuration"></param>
        /// <returns></returns>
        public string GetTotalTestDuration(string strDuration)
        {
            string strTotalTestDuration = string.Empty;
            string strTestDuration = string.Empty;
            string strTotalHours = string.Empty;
            string strTotalMinutes = string.Empty;
            string strTotalSeconds = string.Empty;
            int nTotalTestDuration = 0;
            foreach (string strLine in strDuration.Split('$'))
            {
                if (strLine.Contains(strSummaryDurationIdent))
                {
                    strTestDuration = ParseIdentifier(strLine, strSummaryDurationIdent);
                }
            }
            if (string.IsNullOrEmpty(strTestDuration))
            {
                return string.Empty;
            }
            //Getting te hours minutes and seconds information from the duration
            string[] lDuration = strTestDuration.Split(',');
            if (lDuration.Length == 0)
            {
                return string.Empty;
            }
            foreach (string strTime in lDuration)
            {
                if (strTime.Contains(strHoursIdent))
                {
                    strTotalHours = ParseIdentifier(strTime, strHoursIdent);
                }
                else if (strTime.Contains(strMinutesIdent))
                {
                    strTotalMinutes = ParseIdentifier(strTime, strMinutesIdent);
                }
                else if (strTime.Contains(strSecondsIdent))
                {
                    strTotalSeconds = ParseIdentifier(strTime, strSecondsIdent);
                }
            }
            if (string.IsNullOrEmpty(strTotalHours) || string.IsNullOrEmpty(strTotalMinutes) || string.IsNullOrEmpty(strTotalSeconds))
            {
                return string.Empty;
            }
            //Calculating Total Test Duration
            try
            {
                nTotalTestDuration = Convert.ToInt32(strTotalHours) * 60 * 60 + Convert.ToInt32(strTotalMinutes) * 60 + Convert.ToInt32(strTotalSeconds);
                strTotalTestDuration = Convert.ToString(nTotalTestDuration);
                m_fileLogger.LogMsg(strlogSource, string.Format("Total test duraion: {0}", strTotalTestDuration), LogType.LOG_INFO);
            }
            catch
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Could not convert the duration obtained to integers. Hours {0}, minutes {1}, seconds {2}"
                                                                                    , strTotalHours, strTotalMinutes, strTotalSeconds), LogType.LOG_INFO, true);
                return string.Empty;
            }
            return strTotalTestDuration;
        }

        /// <summary>
        /// Get the test case details
        /// </summary>
        /// <returns></returns>
        public List<TestStep> GetTestSteps()
        {
            string strTCLog = string.Empty;
            List<TestStep> lTestTeps = new List<TestStep>();
            List<string> ltestCaselog = new List<string>();
            if (File.Exists(strExecuteTestsLog))
            {
                using (StreamReader srLog = new StreamReader(strExecuteTestsLog))
                {
                    strTCLog = srLog.ReadToEnd();
                    ltestCaselog = Regex.Split(strTCLog, strTCSeparator).ToList();
                    foreach (string strTestCase in ltestCaselog)
                    {
                        if (strTestCase.Contains(strNumTestsIdent) && strTestCase.Contains(strNumPassesIdent)
                           && strTestCase.Contains(strNumFailsIdent) && strTestCase.Contains(strNumFatalsIdent)
                           && strTestCase.Contains(strNumWarningsIdent) && strTestCase.Contains(strNumErrorsIdent))
                        {
                            try
                            {
                                lTestTeps.Add(GetTestCaseDetails(strTestCase));
                            }
                            catch { }
                        }
                    }
                }
            }
            return lTestTeps;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strTestCase"></param>
        /// <returns></returns>
        private TestStep GetTestCaseDetails(string strTestCase)
        {
            TestStep testCase = new TestStep();
            testCase.Status = ((int)GetTestStatus(strTestCase, '\n')).ToString();
            foreach (string strLine in strTestCase.Split('\n'))
            {
                if (strLine.Contains(strNumTCNameIdent))
                {
                    string strTestCaseFullName = ParseIdentifier(strLine, strNumTCNameIdent);
                    if (!string.IsNullOrEmpty(strTestCaseFullName))
                    {
                        string[] lNameSplit = strTestCaseFullName.Split('\\');
                        if (lNameSplit.Length > 2)
                        {
                            lNameSplit = lNameSplit.Reverse().Take(2).ToArray();
                            testCase.Name = lNameSplit[0] + " " + lNameSplit[1];
                        }
                    }
                }
                if (strLine.Contains(strNumTCDescIdent))
                {
                    testCase.Description = ParseIdentifier(strLine, strNumTCDescIdent);
                }
                if (strLine.Contains(strNumTCDurationIdent))
                {
                    testCase.Timestamp = ParseIdentifier(strLine, strNumTCDurationIdent);
                }
            }
            return testCase;
        }

        

        /// <summary>
        /// Get test details from execute tests log file
        /// </summary>
        private ExitCodes GetTestDetails(TestResult objTestResult)
        {
            string strLine = string.Empty;
            string strSummary = string.Empty;
            string strDuration = string.Empty;
            try
            {
                using (StreamReader logStreamer = new StreamReader(strExecuteTestsLog))
                {
                    //Read the log and gather all regured info
                    m_fileLogger.LogMsg(strlogSource, string.Format("Validating execute tests log file.."), LogType.LOG_INFO, true);
                    while ((strLine = logStreamer.ReadLine()) != null)
                    {
                        //Validate id the log file provided is for the test run specified
                        if (strLine.Contains(strtestRunIdent))
                        {
                            strTestRun = ParseIdentifier(strLine, strtestRunIdent);
                            if (!string.IsNullOrEmpty(strTestRun))
                            {
                                strTestRun = strTestRun.Split()[0];
                                if (strTestRun.EndsWith(".xml"))
                                {
                                    bValidLog = true;
                                }
                            }
                        }
                        if (strLine.Contains(strIPidentifier))
                        {
                            strTargetAddress = ParseIdentifier(strLine, strIPidentifier);
                            if (!string.IsNullOrEmpty(strTargetAddress))
                            {
                                bTargetAddress = true;
                            }
                        }
                        if (strLine.Contains(strTargetIdentIdentifier))
                        {
                            strTargetIdent = ParseIdentifier(strLine, strTargetIdentIdentifier);
                        }
                        if (strLine.Contains(strSummaryIdentifier))
                        {
                            bSummary = true;
                        }
                        if (bSummary)
                        {
                            strSummary += strLine + "$";
                        }
                        if (strLine.Contains(strSummaryDurationIdent))
                        {
                            bDuration = true;
                        }
                        if (bDuration)
                        {
                            strDuration += strLine + "$";
                        }
                    }
                    if (!CheckValidLog())
                    {
                        return ExitCodes.INCORRECT_DATA;
                    }
                    m_fileLogger.LogMsg(strlogSource, string.Format("Log file {0} is a valid execute tests log file.", strExecuteTestsLog), LogType.LOG_INFO, true);
                }
                m_fileLogger.LogMsg(strlogSource, string.Format("Trying to get the test steps"), LogType.LOG_INFO, true);
                objTestResult.liTestSteps = GetTestSteps().ToArray();
                if (objTestResult.liTestSteps.Length == 0)
                {
                    m_fileLogger.LogMsg(strlogSource, string.Format("Could not parse the test steps from the log file {0}", strExecuteTestsLog), LogType.LOG_ERROR, true);
                    return ExitCodes.TEST_STEPS_COULD_NOT_BE_DETERMINED;
                }
                m_fileLogger.LogMsg(strlogSource, string.Format("Got the test steps"), LogType.LOG_INFO, true);
                m_fileLogger.LogMsg(strlogSource, string.Format("Trying to get the test duration"), LogType.LOG_INFO, true);
                objTestResult.Duration = GetTotalTestDuration(strDuration);
                if (string.IsNullOrEmpty(objTestResult.Duration))
                {
                    m_fileLogger.LogMsg(strlogSource, string.Format("test duration could not be determined"), LogType.LOG_ERROR, true);
                    return ExitCodes.TEST_DURATION_COULD_NOT_BE_DETERMINED;
                }
                m_fileLogger.LogMsg(strlogSource, string.Format("Got the test duration"), LogType.LOG_INFO, true);
                m_fileLogger.LogMsg(strlogSource, string.Format("Got the test status"), LogType.LOG_INFO, true);
                objTestResult.Status = ((int)GetTestRunStatus(objTestResult.liTestSteps)).ToString();
                m_fileLogger.LogMsg(strlogSource, string.Format("Got the test status"), LogType.LOG_INFO, true);
            }
            catch (Exception ex)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Error in parsing the execute tests log file. Error {0}", ex.Message), LogType.LOG_ERROR, true);
                return ExitCodes.COULD_NOT_READ_EXECUTE_LOG;
            }
            return ExitCodes.SUCCESSFUL;
        }

        /// <summary>
        /// Validates if all the details are present in the execute tests log
        /// </summary>
        /// <returns></returns>
        public bool CheckValidLog()
        {
            if (!bTargetAddress)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Provided execute tests log {0} Does not contain target IP/host details"
                                                                                            , strExecuteTestsLog), LogType.LOG_ERROR, true);
                return false;
            }
            if (!bValidLog)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Provided execute tests log {0} is not related to the test run specified {1}"
                                                                                    , strExecuteTestsLog, strTestRun), LogType.LOG_ERROR, true);
                return false;
            }
            if (!bSummary)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Test summary information not present in the execution log file"), LogType.LOG_ERROR, true);
                Console.WriteLine();
                return false;
            }
            if (!bDuration)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Test duration information not present in the execution log file"), LogType.LOG_ERROR, true);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the test execution status code
        /// </summary>
        /// <param name="strSumamry"></param>
        /// <returns></returns>
        public TestCaseStatusCodes GetTestStatus(string strSumamry, char cSplitChar)
        {
            string strStatus = string.Empty;
            string strErrors = string.Empty;
            string strFails = string.Empty;
            string strWarnings = string.Empty;
            string strFatals = string.Empty;
            foreach (string strLine in strSumamry.Split(cSplitChar))
            {
                if (strLine.Contains(strNumErrorsIdent))
                {
                    strErrors = ParseIdentifier(strLine, strNumErrorsIdent);
                }
                if (strLine.Contains(strNumFailsIdent))
                {
                    strFails = ParseIdentifier(strLine, strNumFailsIdent);
                }
                if (strLine.Contains(strNumFatalsIdent))
                {
                    strFatals = ParseIdentifier(strLine, strNumFatalsIdent);
                }
                if (strLine.Contains(strNumWarningsIdent))
                {
                    strWarnings = ParseIdentifier(strLine, strNumWarningsIdent);
                }
            }
            if (string.IsNullOrEmpty(strFails) || string.IsNullOrEmpty(strFatals) || string.IsNullOrEmpty(strErrors) || string.IsNullOrEmpty(strWarnings))
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Could not parse one or more information from the execute tests log file."), LogType.LOG_INFO, true);
                return TestCaseStatusCodes.COULD_NOT_DETERMINE;//Could not determine the status
            }
            if (strFails != "0" || strFatals != "0" || strErrors != "0")
            {
                //m_fileLogger.LogMsg(strlogSource, string.Format("One or more Fails/Fatals/Errors exist."), LogType.LOG_INFO, true);
                return TestCaseStatusCodes.ONE_OR_MORE_FAILS_ERRORS_FATALS;
            }
            else
            {
                if (strWarnings != "0")
                {
                    //m_fileLogger.LogMsg(strlogSource, string.Format("One or more warnings exist."), LogType.LOG_INFO, true);
                    return TestCaseStatusCodes.ONE_OR_MORE_WARNINGS;
                }
                else
                {
                    //m_fileLogger.LogMsg(strlogSource, string.Format("No warnings or errors."), LogType.LOG_INFO, true);
                    return TestCaseStatusCodes.ALL_PASS;//No fails/fatals/errors or warnings
                }
            }
        }

        /// <summary>
        /// Get the test run execution status. Currently, we can only check for the statuses 1,2,5 from the below ones
        ///   •1 : tested O.K.
        ///   •2 : tested with error
        ///   •3 : partially tested
        ///   •4 : partially with error
        ///   •5 : not testable
        ///   •6 : prepared
        ///   •7 : prepared for test automation
        /// </summary>
        /// <returns></returns>
        private TestRunStatusCodes GetTestRunStatus(TestStep[] liTestSteps)
        {
            //Checking if all test case statuses are available
            foreach (TestStep testStep in liTestSteps)
            {
                if (testStep.Status == ((int)TestCaseStatusCodes.COULD_NOT_DETERMINE).ToString())
                {
                    //Could not determine the status of one or more test cases. No suitable status available so using 'not testable'
                    return TestRunStatusCodes.COULD_NOT_TEST_OR_DETERMINE;
                }
            }

            //Now, all the test results are available. Check if there are any errors in the test cases
            foreach (TestStep testStep in liTestSteps)
            {
                if (testStep.Status == ((int)TestCaseStatusCodes.ONE_OR_MORE_FAILS_ERRORS_FATALS).ToString()) 
                {
                    return TestRunStatusCodes.TESTED_WITH_ERROR; //tested with error
                }
            }
            return TestRunStatusCodes.TESTED_OK;//Tested OK
        }

        /// <summary>
        /// Fetch all required data and create the Qlaus import XML
        /// </summary>
        /// <returns></returns>
        private ExitCodes CreateQlausImoprtDataXML()
        {
            // Creates an instance of the XmlSerializer class;
            // specifies the type of object to serialize.
            QlausPlus QlausImportData = new QlausPlus();
            string strOutputFile = Path.Combine(strPrepResOutput, strPrepResFileName);
            //Data to be imported to QlausPlus
            List<TestResult> lTestResults = new List<TestResult>();
            TestResult objTestResult = new TestResult();
            string strReportFile = string.Empty;
            TextWriter textWriter = null;

            XmlSerializer serializer = new XmlSerializer(typeof(QlausPlus));
            try
            {
                textWriter = new StreamWriter(strOutputFile);
            }
            catch (UnauthorizedAccessException)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Access denied to access the path {0}", strOutputFile), LogType.LOG_ERROR, true);
                return ExitCodes.ACCESS_DENIED;
            }
            catch (Exception ex)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Could not create the output file {0}. Error: {1}", strOutputFile, ex.Message), LogType.LOG_ERROR, true);
                return ExitCodes.COULD_NOT_GENERATE_OUTPUT_FILE;
            }
            objTestResult.liFiles = lFilesToBeImported.ToArray();
            try
            {
                objTestResult.Tester = UserPrincipal.Current.EmailAddress;
                m_fileLogger.LogMsg(strlogSource, string.Format("Tester {0}", objTestResult.Tester), LogType.LOG_WARNING, true);
            }
            catch
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Could not find the tester information from the host"), LogType.LOG_WARNING, true);
            }
            objTestResult.TestEngine = strTestEngine;
            m_fileLogger.LogMsg(strlogSource, string.Format("Getting the test details.."), LogType.LOG_INFO, true);
            ExitCodes ReadExecuteLogStatus = GetTestDetails(objTestResult);
            if (ReadExecuteLogStatus != ExitCodes.SUCCESSFUL)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Getting the test details failed."), LogType.LOG_ERROR, true);
                return ReadExecuteLogStatus;
            }
            m_fileLogger.LogMsg(strlogSource, string.Format("Got the test details.."), LogType.LOG_INFO, true);
            objTestResult.TestcaseName = Path.GetFileName(strTestRun);
            if (strTestRun.Contains(strTCRepoIdent))
            {
                objTestResult.ReferenceID = strTestRun.Substring(strTestRun.IndexOf(strTCRepoIdent) + strTCRepoIdent.Length);
            }
            else
            {
                objTestResult.ReferenceID = strTestRun;
            }
            objTestResult.Rack = strTargetIdent;
            m_fileLogger.LogMsg(strlogSource, string.Format("Generating UUID for the task.."), LogType.LOG_INFO, true);
            objTestResult.Comment = GenerateUUID(strIdentifierWithUUID, strParentUUID);
            if (objTestResult.Comment == string.Empty)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Generating UUID for the task failed."), LogType.LOG_ERROR, true);
                return ExitCodes.UUID_GENERATION_FAILED;
            }
            m_fileLogger.LogMsg(strlogSource, string.Format("Generated UUID for the task. UUID: {0}", objTestResult.Comment), LogType.LOG_INFO, true);
            try
            {
                objTestResult.Date = File.GetCreationTime(strExecuteTestsLog).ToString("dd.MM.yyyy HH:mm:ss");
            }
            catch (Exception)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Could not determine the creation date of the execute tests log file"), LogType.LOG_WARNING, true);
            }
            objTestResult.TaskID = strTaskID;
            // Retreive SW components
            m_fileLogger.LogMsg(strlogSource, string.Format("Retrieving the SW component info.."), LogType.LOG_INFO, true);
            Tuple<ExitCodes, List<Software>> tuRetData = RetrieveSWCompInfo();
            if (tuRetData.Item1 != ExitCodes.SUCCESSFUL)
            {
                return tuRetData.Item1;
            }
            m_fileLogger.LogMsg(strlogSource, string.Format("Retrieving the SW component info successful."), LogType.LOG_INFO, true);
            objTestResult.liSWVersions = tuRetData.Item2.ToArray();
            // Retreive HW components
            if (!string.IsNullOrEmpty(strHWCompXML))
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Retrieving the HW component info.."), LogType.LOG_INFO, true);
                Tuple<ExitCodes, List<Hardware>> tuHWRetData = RetrieveHWCompInfo();
                if (tuHWRetData.Item1 == ExitCodes.SUCCESSFUL)
                {
                    m_fileLogger.LogMsg(strlogSource, string.Format("Retrieving the HW component info successful."), LogType.LOG_INFO, true);
                    objTestResult.liHardwares = tuHWRetData.Item2.ToArray();
                }
                else
                {
                    //Currently HW info is not mandatory. Import can be done even if HW components could not be fetched
                    m_fileLogger.LogMsg(strlogSource, string.Format("Could not retrieve the HW component info."), LogType.LOG_WARNING, true);
                }
            }
            m_fileLogger.LogMsg(strlogSource, string.Format("Getting the test environment details.."), LogType.LOG_INFO, true);
            objTestResult.liTestEnvironments = GetTestEnvironment(hwType).ToArray();
            if (objTestResult.liTestEnvironments.Length == 0)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Test environment not configured for target type: {0}", strTargetType), LogType.LOG_ERROR, true);
                return ExitCodes.TEST_ENV_NOT_CONFIGURED;
            }
            m_fileLogger.LogMsg(strlogSource, string.Format("Fetching test environment details completed."), LogType.LOG_INFO, true);
            m_fileLogger.LogMsg(strlogSource, string.Format("Getting the hardware independent version details.."), LogType.LOG_INFO, true);
            Tuple<bool, List<Software>> tuHWindRetData = RetrieveHWIndVersionsInfo();
            if (tuHWindRetData.Item1)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Fetching the hardware independent version details completed."), LogType.LOG_INFO, true);
                objTestResult.liHWIndVersions = tuHWindRetData.Item2.ToArray();
            }
            else
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Fetching the hardware independent version details failed."), LogType.LOG_WARNING, true);
                //return ExitCodes.RETRIEVING_HW_INDEPENDENT_VERSION_FAILED; 
            }
            lTestResults.Add(objTestResult);
            // Inserts the item into the array.
            QlausImportData.TestresultList = lTestResults.ToArray();
            // Serializes the object of QlausPlus into XML structure
            try
            {
                serializer.Serialize(textWriter, QlausImportData);
            }
            catch (Exception ex)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Could not write into the output file {0}. Error: {1}", strOutputFile, ex.Message), LogType.LOG_ERROR, true);
                return ExitCodes.COULD_NOT_GENERATE_OUTPUT_FILE;
            }
            textWriter.Close();
            m_fileLogger.LogMsg(strlogSource, string.Format("Prepare results successful."), LogType.LOG_INFO, true);
            return ExitCodes.SUCCESSFUL;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Tuple<ExitCodes, List<Software>> RetrieveSWCompInfo()
        {
            string strOutput = string.Empty;
            List<Software> lSWCompDetails = new List<Software>();
            Tuple<bool, List<Software>> tuReadSWVersions;
            if (hwType.Equals(HW_Type.NCU) || hwType.Equals(HW_Type.PPU3) || hwType.Equals(HW_Type.PPU4)
                || hwType.Equals(HW_Type.EVO) || hwType.Equals(HW_Type.PPU1740) || hwType.Equals(HW_Type.EVC)
                || hwType.Equals(HW_Type.OOPC) || hwType.Equals(HW_Type.PCU) || hwType.Equals(HW_Type.IPC))
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Trying to parse the sw info file.."), LogType.LOG_INFO, true);
                tuReadSWVersions = ParseSWInfoFile(strSWCompXML);
                if (!tuReadSWVersions.Item1)
                {
                    return Tuple.Create(ExitCodes.COULD_NOT_PARSE_SW_COMPONENTS, lSWCompDetails);
                }
                m_fileLogger.LogMsg(strlogSource, string.Format("Parsing the sw info file completed."), LogType.LOG_INFO, true);
                return Tuple.Create(ExitCodes.SUCCESSFUL, tuReadSWVersions.Item2);
            }
            else
            {
                return Tuple.Create(ExitCodes.FEATURE_NOT_IMPLEMENTED, lSWCompDetails);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSWInfoFile"></param>
        /// <returns></returns>
        public Tuple<bool, List<Software>> ParseSWInfoFile(string strSWInfoFile)
        {
            List<Software> lSWCompDetails = new List<Software>();
            using (StreamReader strSWCompXMLFile = new StreamReader(strSWInfoFile, true))
            {
                XmlDocument swInfoXML = new XmlDocument();
                XmlNodeList liComponentNodes;
                XmlNode RequiredHostTag = null;
                try
                {
                    swInfoXML.Load(strSWCompXMLFile);
                    XmlNodeList HostNodes = swInfoXML.SelectNodes("/Hosts/Host");
                    if (HostNodes == null || HostNodes.Count == 0)
                    {
                        m_fileLogger.LogMsg(strlogSource, string.Format("Could not find any Hosts/Host tag in software components from SW xml file"), LogType.LOG_ERROR, true);
                        return Tuple.Create(false, lSWCompDetails);
                    }
                    if (HostNodes.Count > 1)
                    {
                        foreach (XmlNode HostNode in HostNodes)
                        {
                            XmlNode DescriptionNode = HostNode.SelectSingleNode("Description");
                            if (DescriptionNode != null)
                            {
                                if (DescriptionNode.InnerText.Contains("SINUMERIK"))
                                {
                                    RequiredHostTag = HostNode;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        RequiredHostTag = HostNodes[0];
                    }
                    if (RequiredHostTag == null)
                    {
                        m_fileLogger.LogMsg(strlogSource, string.Format("Could not find any software components from SW xml file"), LogType.LOG_ERROR, true);
                        return Tuple.Create(false, lSWCompDetails);
                    }
                    liComponentNodes = RequiredHostTag.SelectNodes("Software/Component");
                    if (liComponentNodes == null)
                    {
                        m_fileLogger.LogMsg(strlogSource, string.Format("No software component found /Hosts/Host/Software/Component in generated Software XML {0}"
                                                                            , strSWInfoFile), LogType.LOG_ERROR, true);
                        return Tuple.Create(false, new List<Software>());
                    }
                    foreach (XmlNode compNode in liComponentNodes)
                    {
                        XmlNode compChildNode = compNode.SelectSingleNode("Name");
                        if (compChildNode != null)
                        {
                            if (compChildNode.InnerText.Contains("SINUMERIK"))
                            {
                                Software MainComponent = new Software();
                                MainComponent.Name = compChildNode.InnerText;
                                if (compNode.SelectSingleNode("Version") != null)
                                {
                                    MainComponent.Version = compNode.SelectSingleNode("Version").InnerText;
                                }
                                if (compNode.SelectSingleNode("InternalVersion") != null)
                                {
                                    MainComponent.Version = compNode.SelectSingleNode("InternalVersion").InnerText;
                                }
                                XmlNodeList swCompNodeList = compNode.SelectNodes("Component");
                                foreach (XmlNode SWChildComp in swCompNodeList)
                                {
                                    Software SWComponent = new Software();
                                    foreach (XmlNode SWChildCompItem in SWChildComp.ChildNodes)
                                    {
                                        if (SWChildCompItem.Name.Equals("Name"))
                                        {
                                            SWComponent.Name = SWChildCompItem.InnerText;
                                        }
                                        else if (SWChildCompItem.Name.Equals("Version"))
                                        {
                                            SWComponent.Version = SWChildCompItem.InnerText;
                                        }
                                        else if (SWChildCompItem.Name.Equals("InternalVersion"))
                                        {
                                            SWComponent.Release = SWChildCompItem.InnerText;
                                        }
                                    }
                                    if (SWComponent != null)
                                    {
                                        lSWCompDetails.Add(SWComponent);
                                    }
                                }
                                lSWCompDetails.Add(MainComponent);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_fileLogger.LogMsg(strlogSource, string.Format("Could not load or parse SW Component XML file. Error: {0}", ex.Message)
                                                                                                                   , LogType.LOG_ERROR, true);
                    return Tuple.Create(false, new List<Software>());
                }
            }
            if (lSWCompDetails.Count == 0)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Could not find any software components from SW xml file"), LogType.LOG_ERROR, true);
                return Tuple.Create(false, lSWCompDetails);
            }
            return Tuple.Create(true, lSWCompDetails);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strHWInfoFile"></param>
        /// <returns></returns>
        public Tuple<bool, List<Hardware>> ParseHWInfoFile(string strHWInfoFile)
        {
            List<Hardware> lHWCompDetails = new List<Hardware>();
            using (StreamReader strSWCompXMLFile = new StreamReader(strHWInfoFile, true))
            {
                XmlDocument hwInfoXML = new XmlDocument();
                XmlNodeList liComponentNodes;
                XmlNode RequiredHostTag = null;
                try
                {
                    hwInfoXML.Load(strSWCompXMLFile);
                    XmlNodeList HostNodes = hwInfoXML.SelectNodes("/Hosts/Host");
                    if (HostNodes == null || HostNodes.Count == 0)
                    {
                        m_fileLogger.LogMsg(strlogSource, string.Format("Could not find any Hosts/Host tag in Hardware components from HW xml file"), LogType.LOG_ERROR, true);
                        return Tuple.Create(false, lHWCompDetails);
                    }
                    if (HostNodes.Count > 1)
                    {
                        foreach (XmlNode HostNode in HostNodes)
                        {
                            XmlNode DescriptionNode = HostNode.SelectSingleNode("Description");
                            if (DescriptionNode != null)
                            {
                                if (DescriptionNode.InnerText.Contains("SINUMERIK"))
                                {
                                    RequiredHostTag = HostNode;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        RequiredHostTag = HostNodes[0];
                    }
                    if (RequiredHostTag == null)
                    {
                        m_fileLogger.LogMsg(strlogSource, string.Format("Could not find any Hardware components from HW xml file"), LogType.LOG_ERROR, true);
                        return Tuple.Create(false, lHWCompDetails);
                    }
                    liComponentNodes = RequiredHostTag.SelectNodes("Hardware");
                    if (liComponentNodes == null)
                    {
                        m_fileLogger.LogMsg(strlogSource, string.Format("No Hardware component found /Hosts/Host/Hardware in generated Hardware XML {0}"
                                                                            , strHWInfoFile), LogType.LOG_ERROR, true);
                        return Tuple.Create(false, new List<Hardware>());
                    }
                    foreach (XmlNode compNode in liComponentNodes)
                    {
                        XmlNode compChildNode = compNode.SelectSingleNode("Name");
                        if (compChildNode != null)
                        {
                            if (compChildNode.InnerText.Contains("SINUMERIK"))
                            {
                                XmlNodeList hwCompNodeList = compNode.SelectNodes("Component");
                                foreach (XmlNode HWChildComp in hwCompNodeList)
                                {
                                    Hardware HWComponent = new Hardware();
                                    foreach (XmlNode HWChildCompItem in HWChildComp.ChildNodes)
                                    {
                                        if (HWChildCompItem.Name.Equals("Name"))
                                        {
                                            HWComponent.Name = HWChildCompItem.InnerText;
                                        }
                                        else if (HWChildCompItem.Name.Equals("MLFB"))
                                        {
                                            HWComponent.MLFB = HWChildCompItem.InnerText;
                                        }
                                        else if (HWChildCompItem.Name.Equals("SerialNo"))
                                        {
                                            HWComponent.Serialnumber = HWChildCompItem.InnerText;
                                        }
                                    }
                                    if (HWComponent != null)
                                    {
                                        lHWCompDetails.Add(HWComponent);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_fileLogger.LogMsg(strlogSource, string.Format("Could not load or parse HW Component XML file. Error: {0}", ex.Message)
                                                                                                                   , LogType.LOG_ERROR, true);
                    return Tuple.Create(false, new List<Hardware>());
                }
            }
            if (lHWCompDetails.Count == 0)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Could not find any Hardware components from HW xml file"), LogType.LOG_ERROR, true);
                return Tuple.Create(false, lHWCompDetails);
            }
            return Tuple.Create(true, lHWCompDetails);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Tuple<ExitCodes, List<Hardware>> RetrieveHWCompInfo()
        {
            string strOutput = string.Empty;
            List<Hardware> lHWCompDetails = new List<Hardware>();
            Tuple<bool, List<Hardware>> tuReadHWVersions;
            if (hwType.Equals(HW_Type.NCU) || hwType.Equals(HW_Type.PPU3) || hwType.Equals(HW_Type.PPU4)
                 || hwType.Equals(HW_Type.EVO) || hwType.Equals(HW_Type.PPU1740) || hwType.Equals(HW_Type.EVC)
                 || hwType.Equals(HW_Type.OOPC) || hwType.Equals(HW_Type.PCU) || hwType.Equals(HW_Type.IPC))
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Trying to parse the hw info file.."), LogType.LOG_INFO, true);
                tuReadHWVersions = ParseHWInfoFile(strHWCompXML);
                if (!tuReadHWVersions.Item1)
                {
                    return Tuple.Create(ExitCodes.COULD_NOT_PARSE_HW_COMPONENTS, lHWCompDetails);
                }
                m_fileLogger.LogMsg(strlogSource, string.Format("Parsing the hw info file completed."), LogType.LOG_INFO, true);
                return Tuple.Create(ExitCodes.SUCCESSFUL, tuReadHWVersions.Item2);
            }
            //else if (hwType.Equals(HW_Type.PCU) || hwType.Equals(HW_Type.IPC))
            //{
            //    m_fileLogger.LogMsg(strlogSource, string.Format("Trying to parse the hw info file.."), LogType.LOG_INFO, true);
            //    tuReadHWVersions = ParseWinHWInfoFile(strHWCompXML);
            //    if (!tuReadHWVersions.Item1)
            //    {
            //        return Tuple.Create(ExitCodes.COULD_NOT_PARSE_HW_COMPONENTS, lHWCompDetails);
            //    }
            //    m_fileLogger.LogMsg(strlogSource, string.Format("Parsing the hw info file completed."), LogType.LOG_INFO, true);
            //    return Tuple.Create(ExitCodes.SUCCESSFUL, tuReadHWVersions.Item2);
            //}
            else
            {
                return Tuple.Create(ExitCodes.FEATURE_NOT_IMPLEMENTED, lHWCompDetails);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Tuple<bool, List<Software>> RetrieveHWIndVersionsInfo()
        {
            List<Software> lHWIndCompDetails = new List<Software>();
            string strOutput = string.Empty;
            string strOSName = string.Empty;
            string strOSVersion = string.Empty;
            Tuple<int, string> tuRetVal;
            string strInfoFile = strDestPath + "\\info.txt";
            string strGetMSWindowsVersion = "systeminfo > " + strInfoFile;
            tuRetVal = ExecuteCommand(ref strOutput, strFileName: "cmd.exe", strArguments: "/C " + strGetMSWindowsVersion);
            m_fileLogger.LogMsg(strlogSource, string.Format("Trying to execute the command {0} {1}", "cmd.exe", strGetMSWindowsVersion), LogType.LOG_INFO, true);
            if (tuRetVal.Item1 != 0 || strOutput.Contains("failed") || strOutput.Contains("Error"))
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Command to retrieve Windows information failed."), LogType.LOG_ERROR, true);
                return Tuple.Create(false, lHWIndCompDetails);
            }
            else
            {
                StreamReader rdrFile = File.OpenText(strInfoFile);
                string line;
                while ((line = rdrFile.ReadLine()) != null)
                {
                    if (line.Split(':')[0].Equals("OS Name"))
                    {
                        strOSName = line.Split(':')[1].Trim();
                    }
                    if (line.Split(':')[0].Equals("OS Version"))
                    {
                        strOSVersion = line.Split(':')[1].Trim();
                    }
                }
            }
            Software SWComp = new Software();
            SWComp.Name = strOSName;
            SWComp.Version = strOSVersion;
            if (SWComp.Name.Equals("Microsoft Windows 10 Enterprise"))
            {
                SWComp.Name = "Win10 Enterprise";
                lHWIndCompDetails.Add(SWComp);
            }
            else if (SWComp.Name.Equals("Microsoft Windows 7 Enterprise"))
            {
                SWComp.Name = "Win7 Enterprise";
                lHWIndCompDetails.Add(SWComp);
            }
            // Squish Details            
            try
            {
                Software SWSquishComp = new Software();
                SWSquishComp.Name = "SQUISH";
                StreamReader rdrFile = File.OpenText(strExecuteTestsLog);
                string line;
                while ((line = rdrFile.ReadLine()) != null)
                {
                    if (line.Contains("Squish Test Runner"))
                    {
                        int nBeginIndex = line.IndexOf("Runner") + 7;
                        line = line.Substring(nBeginIndex);
                        SWSquishComp.Version = line;
                        break;
                    }
                }
                lHWIndCompDetails.Add(SWSquishComp);
            }
            catch (Exception ex)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Could not parse log file: {0} Exception : {1}", strExecuteTestsLog, ex.Message), LogType.LOG_ERROR, true);
                return Tuple.Create(false, lHWIndCompDetails);
            }
            return Tuple.Create(true, lHWIndCompDetails);
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
