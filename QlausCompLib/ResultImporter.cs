using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TTILoggerLib;
using static QlausCompLib.QlausCompUtilities;

namespace QlausCompLib
{
    class ResultImporter : QlausComp, IDisposable
    {
        #region Variables used throughout the class
        public string strImportPackagesPath { get; private set; }
        public string strQlausDB { get; private set; }
        List<string> lPackages = new List<string>();
        FileLogger m_fileLogger = null;
        FileLogger m_mainLogger = null;
        string strlogSource = "ImportResults";
        string strTargetLoggerName = string.Empty;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public ResultImporter()
        {
            ;  
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
            ExitCodes retExitCode = ExitCodes.ONE_OR_MORE_IMPORTS_FAILED;
            short nImportRetry = nRetryCount;
            while (nImportRetry > 0)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Trying to import. Try number {0}", (nRetryCount - nImportRetry + 1)), LogType.LOG_INFO, true);
                retExitCode = ImportResultToQlaus();
                if (retExitCode == ExitCodes.SUCCESSFUL)
                {
                    break;
                }
                if (--nImportRetry != 0)
                {
                    Thread.Sleep(20*1000);
                }
            }
            return retExitCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diParameters"></param>
        /// <returns></returns>
        public override ExitCodes Initialize(Dictionary<string, string> diParameters, FileLogger m_mainLoggerIR)
        {
            m_mainLogger = m_mainLoggerIR;
            if (!CheckQlausAvailability(m_mainLogger: m_mainLoggerIR, strlogSource: strlogSource))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("Qlaus server not reachable."), LogType.LOG_ERROR, true);
                return ExitCodes.QLAUS_NOT_REACHABLE;
            }
            if (!diParameters.ContainsKey("qlausdb") || string.IsNullOrEmpty(diParameters["qlausdb"]))
            {
                strQlausDB = strDefaultQlausDB;//Default database
            }
            else
            {
                strQlausDB = diParameters["qlausdb"];
            }
            if (!diParameters.ContainsKey("importlocations") || string.IsNullOrEmpty(diParameters["importlocations"]))
            {
                m_mainLogger.LogMsg(strlogSource, string.Format("One or more prepare results package paths must be provided"), LogType.LOG_ERROR, true);
                return ExitCodes.INSUFFICIENT_ARGUMENTS;
            }
            strImportPackagesPath = diParameters["importlocations"];
            lPackages = strImportPackagesPath.Split(',').ToList();
            foreach (string strPackage in lPackages)
            {                
                if (!File.Exists(Path.Combine(strPackage, strPrepResFileName)))
                {
                    m_mainLogger.LogMsg(strlogSource, string.Format("Result Package {0} does not contain the prepare result XML {1}."
                                                                               , strPackage, strPrepResFileName), LogType.LOG_ERROR, true);
                    return ExitCodes.INCORRECT_DATA;
                }
            }
            m_mainLogger.LogMsg(strlogSource, string.Format("Initialization successfull."), LogType.LOG_ERROR, true);
            strTargetLoggerName = "QlausComp_" + diParameters["mode"] + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv";
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
        private ExitCodes ImportResultToQlaus()
        {
            string strOutput = string.Empty;
            string strArgsForQlaus = string.Empty;
            string strImportFile = string.Empty;
            bool bImportFailed = false;
            Tuple<int, string> tuCommandRet;
            foreach (string strPackage in lPackages)
            {
                m_fileLogger.LogMsg(strlogSource, string.Format("Trying to import the reports for the package {0}..", strPackage), LogType.LOG_INFO, true);
                strImportFile = Path.Combine(strPackage, strPrepResFileName);
                strArgsForQlaus = strQlausPath + " /d " + strQlausDB + " /i " + strImportFile;
                tuCommandRet = ExecuteCommand(ref strOutput, strFileName: "cmd.exe", strArguments: "/C "+strArgsForQlaus, strWorkingDir:strPackage, strTimeOut: strQlausExecuteTimeout);
                m_fileLogger.LogMsg(strlogSource, string.Format("Trying to execute the command {0} {1}", "cmd.exe", strArgsForQlaus), LogType.LOG_INFO);
                if (tuCommandRet.Item1 != 0 || strOutput.Contains("could not import"))
                {
                    m_fileLogger.LogMsg(strlogSource, string.Format("Importing reports for the package {0} failed", strPackage), LogType.LOG_INFO, true);
                    bImportFailed = true;
                }
                else
                {
                    m_fileLogger.LogMsg(strlogSource, string.Format("Successfully imported reports for the package {0}", strPackage), LogType.LOG_INFO, true);
                }
            }
            if (bImportFailed)
            {
                return ExitCodes.ONE_OR_MORE_IMPORTS_FAILED;
            }
            m_fileLogger.LogMsg(strlogSource, string.Format("Import reports successful."), LogType.LOG_INFO, true);
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
