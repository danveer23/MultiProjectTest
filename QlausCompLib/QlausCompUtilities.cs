using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using System.Net.NetworkInformation;
using TTILoggerLib;

namespace QlausCompLib
{
    public class QlausCompUtilities
    {
        /// <summary>
        /// Enums for the test run execution statuses
        /// </summary>
        public enum TestRunStatusCodes
        {
            //All test cases passed
            TESTED_OK = 1,
            //Tested with one or more error(s)
            TESTED_WITH_ERROR = 2,
            //Partially tested (Few of the test cases did not run)
            PARTIALLY_TESTED = 3,
            //Partially tested with error(s)
            PARTIALLY_TESTED_WITH_ERROR = 4,
            //Not testable OR could not determine one or more test case statuses
            COULD_NOT_TEST_OR_DETERMINE = 5,
            //Tests prepared
            PREPARED = 6,
            //Tests prepared for test automation
            PREPARED_FOR_TEST_AUTOMATION = 7
        }

        /// <summary>
        /// Enums for the test case execution statuses
        /// </summary>
        public enum TestCaseStatusCodes
        {
            //All test points passed
            ALL_PASS = 2,
            //One or more warnings
            ONE_OR_MORE_WARNINGS = 2, //For now, this is the same as tested OK. Qlaus has only 3 statuses
            //Partially tested (Few of the test cases did not run)
            ONE_OR_MORE_FAILS_ERRORS_FATALS = 3,
            //Could not determine the test case status  
            COULD_NOT_DETERMINE = 5,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum ExitCodes
        {
            SUCCESSFUL = 0,
            UNSUCCESSFUL = -1,
            INVALID_PATH = 1,
            INSUFFICIENT_ARGUMENTS = 2,
            QLAUS_QUERY_FAILED = 3,
            QLAUS_RETURNED_EMPTY_LIST = 4,
            ACCESS_DENIED = 5,
            FOLDER_NOT_FOUND = 6,
            COULD_NOT_READ_QLAUS_GENERATED_XML = 7,
            OUTPUT_FILE_GEN_FAILED = 8,
            FILE_NOT_FOUND = 9,
            COULD_NOT_GENERATE_OUTPUT_FILE = 10,
            INCORRECT_DATA = 11,
            COULD_NOT_READ_EXECUTE_LOG = 12,
            TEST_STEPS_COULD_NOT_BE_DETERMINED = 13,
            TEST_DURATION_COULD_NOT_BE_DETERMINED = 14,
            TEST_STATUS_COULD_NOT_BE_DETERMINED = 15,
            RESULT_FOLDER_ALREADY_PRESENT = 16,
            RESULT_FOLDER_CREATION_FAILED = 17,
            COULD_NOT_GENERATE_SWINFO_FILE = 18,
            COULD_NOT_COPY_SWINFO_FILE = 19,
            COULD_NOT_PARSE_SW_COMPONENTS = 20,
            COULD_NOT_USE_WIN_SHARE_FOLDER = 21,
            COULD_NOT_READ_VERSIONS_XML = 22,
            FEATURE_NOT_IMPLEMENTED = 23,
            COULD_NOT_PARSE_HW_COMPONENTS = 24,
            TEST_ENV_NOT_CONFIGURED = 25,
            RETRIEVING_HW_INDEPENDENT_VERSION_FAILED = 26,
            ONE_OR_MORE_IMPORTS_FAILED = 27,
            TARGET_NOT_PINGABLE = 28,
            PROTOCOL_FILES_DONT_EXIST_OR_NOT_PROVIDED = 29,
            LOGGER_INITIALIZATION_FAILED = 30,
            UUID_GENERATION_FAILED = 31,
            QLAUS_NOT_REACHABLE = 32,
            SW_COMPONENT_FILE_NOT_PROVIDED = 33,
            SW_COMPONENT_FILE_NOT_FOUND = 34,
            HW_COMPONENT_FILE_NOT_PROVIDED = 35,
            HW_COMPONENT_FILE_NOT_FOUND = 36,
            CONFIG_FILE_COULDNOT_BE_READ = 37,
            UNKNOWN_PARAMETER = 38,
            INCORRECT_MODE_ENTERED = 39,
        }

        /// <summary>
        /// 
        /// </summary>
        public enum HW_Type
        {
            /// <summary>
            /// For target types that are not recognised
            /// </summary>
            NOT_RECOGNISED = -1,
            /// <summary>
            /// Sinutrain in VM
            /// </summary>
            VM_SINUTRAIN = 0,
            /// <summary>
            /// NCU
            /// </summary>
            NCU = 1,
            /// <summary>
            /// PCU
            /// </summary>
            PCU = 2,
            /// <summary>
            /// Sinumerik Operate
            /// </summary>
            LOCAL_SINUMERIK_OPERATE = 3,
            /// <summary>
            /// Sinumerik Operate
            /// </summary>
            VM_SINUMERIK_OPERATE = 4,
            /// <summary>
            /// Sinutrain in local system
            /// </summary>
            LOCAL_SINUTRAIN = 5,
            /// <summary>
            /// EVO 
            /// </summary>
            EVO = 6,
            /// <summary>
            /// IPC 
            /// </summary>
            IPC = 7,
            /// <summary>
            /// PPU3 
            /// </summary>
            PPU3 = 8,
            /// <summary>
            /// Evo - VC
            /// </summary>
            EVC = 9,
            /// <summary>
            /// PPU1740 
            /// </summary>
            PPU1740 = 10,
            /// <summary>
            /// PPU4
            /// </summary>
            PPU4 = 11,
            /// <summary>
            /// OOPC
            /// </summary>
            OOPC = 12
        }

        /// <summary>
        /// Generates unique identifier for the task
        /// </summary>
        /// <returns></returns>
        public static string GenerateUUID(string strIdentifierWithUUID = "QlausComp", string strParentUUID = "")
        {
            string strTaskUUID = string.Empty;
            string strChild = string.Empty;
            if (!string.IsNullOrEmpty(strParentUUID))
            {
                strTaskUUID = "Parent:" + strParentUUID + "\n";
                strChild = "Child:";
            }
            try
            {
                Guid objUUID = Guid.NewGuid();
                strTaskUUID += strChild + objUUID.ToString() + "-" + strIdentifierWithUUID + "-" + DateTime.Now.ToString("yyyy-MM-dd");
                return strTaskUUID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("UUID could not be created. Error:{0}", ex.Message));
                return string.Empty;
            }
        }

        /// <summary>
        /// Pings the IPaddress/Hostname provided and returns true if successful and false otherwise
        /// </summary>
        /// <param name="strTarget">IP address or hostname</param>
        /// <returns>true or false</returns>
        public static bool PingTarget(string strTarget)
        {
            Ping pinger = null;
            bool bPoolStatus = false;
            try
            {
                //Check if devices provided are pingable
                pinger = new Ping();
                PingReply pingReply = pinger.Send(strTarget);
                if (pingReply.Status == IPStatus.Success)
                {
                    bPoolStatus = true;
                }
            }
            catch { }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }
            return bPoolStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strCommand"></param>
        /// <param name="strOutput"></param>
        /// <returns></returns>
        public static int ExecuteCommand(string strCommand, ref string strOutput, int nTimeoutMinutes)
        {
            int RetVal = -1;
            try
            {
                string error = string.Empty;
                var processStartInfo = new ProcessStartInfo("cmd.exe", "/C" + strCommand);
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
                processStartInfo.UseShellExecute = false;

                Process process = Process.Start(processStartInfo);
                process.WaitForExit(nTimeoutMinutes * 60 * 1000);
                if (process.HasExited)
                {
                    using (StreamReader streamReader = process.StandardError)
                    {
                        strOutput = streamReader.ReadToEnd();
                    }
                    using (StreamReader streamReader = process.StandardOutput)
                    {
                        strOutput += streamReader.ReadToEnd();
                    }
                    RetVal = 0;
                }
                process.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RetVal = -1;
            }

            return RetVal;
        }

        /// <summary>
        /// Creates a new process and executes the given command
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="strArguments"></param>
        /// <param name="strWorkingDir"></param>
        /// <param name="strValidation"></param>
        /// <param name="strTimeOut"></param>
        /// <returns></returns>
        public static Tuple<int, string> ExecuteCommand(ref string CmdOutput, string strFileName, string strArguments, string strWorkingDir = "", string strValidation = "", string strTimeOut = "")
        {
            CmdOutput = string.Empty;
            int nProcessId;
            const int nSleepTime = 1000;
            string strOutput = string.Empty;
            string strHMIOutput = string.Empty;
            Process Proc = new Process();
            int elapsedTime = 0;
            int nTimeOut = 0;
            try
            {
                nTimeOut = Int32.Parse(strTimeOut) * 60000;
            }
            catch
            {
                nTimeOut = 60000;//1 minute
            }
            ProcessStartInfo ProcInfo = new ProcessStartInfo();
            ProcInfo.UseShellExecute = false;
            ProcInfo.RedirectStandardInput = true;
            ProcInfo.RedirectStandardOutput = true;
            ProcInfo.RedirectStandardError = true;
            //ProcInfo.CreateNoWindow = true;
            if (!string.IsNullOrEmpty(strWorkingDir))
            {
                ProcInfo.WorkingDirectory = strWorkingDir;
            }
            ProcInfo.FileName = strFileName;
            ProcInfo.Arguments = strArguments;
            Proc.StartInfo = ProcInfo;
            Proc.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    strOutput += e.Data;
                }
            });
            Thread.Sleep(nSleepTime);
            try
            {
                //If timeout is there , exit the process after the specified time
                if (string.IsNullOrEmpty(strTimeOut))
                {
                    Proc.Start();
                    Proc.BeginOutputReadLine();
                    Proc.WaitForExit();
                }
                else
                {
                    Proc.EnableRaisingEvents = true;
                    //Proc.Exited += new EventHandler(proc_Exited);
                    Proc.Start();
                    Proc.BeginOutputReadLine();
                    nProcessId = Proc.Id;

                    while (true)
                    {
                        if (Proc.HasExited)
                        {
                            break;
                        }
                        elapsedTime += nSleepTime;
                        if (elapsedTime > nTimeOut)
                        {
                            KillProcessAndChildren(nProcessId);
                            CmdOutput = strOutput;
                            return Tuple.Create(-1, "Execution timed out");
                        }
                        Thread.Sleep(nSleepTime);
                    }
                }
            }
            catch (Exception ex)
            {
                CmdOutput = strOutput;
                return Tuple.Create(-1, "Exception caught while executing the command. Error:" + ex.Message);
            }
            CmdOutput = strOutput;
            if (strValidation != "")
            {
                if (!strOutput.Contains(strValidation))
                {
                    return Tuple.Create(-1, "Could not find the string : " + strValidation
                                        + ".After executing comamnd : " + strFileName + " " + strArguments);
                }
                else
                {
                    return Tuple.Create(0, "Found the string : " + strValidation
                                        + ".After executing comamnd : " + strFileName + " " + strArguments);
                }
            }
            else
            {
                // For cases where there is no validation
                return Tuple.Create(Proc.ExitCode, "Executed the comamnd " + strFileName + " " + strArguments);
            }
        }

        /// <summary>
        /// Kills all process and children
        /// </summary>
        /// <param name="nProcessId"></param>
        public static void KillProcessAndChildren(int nProcessId)
        {
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher("Select * From Win32_process Where ParentProcessID=" + nProcessId);
            ManagementObjectCollection oMOC = oSearcher.Get();
            foreach (ManagementObject oMO in oMOC)
            {
                KillProcessAndChildren(Convert.ToInt32(oMO["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(nProcessId);
                proc.Kill();
            }
            catch
            {
                //COMMENT:- Process already exited
            }
        }

        /// <summary>
        /// Get Operate type in enum HW_Type from String HW_Type
        /// </summary>
        /// <param name="strTargetType"></param>
        /// <returns></returns>
        public static HW_Type GetTargetType(String strTargetType)
        {
            HW_Type OperateType = HW_Type.LOCAL_SINUTRAIN;

            if (strTargetType.ToLower() == "sinutrain")
            {
                OperateType = HW_Type.LOCAL_SINUTRAIN;
            }
            else if (strTargetType.ToLower() == "vm")
            {
                OperateType = HW_Type.VM_SINUTRAIN;
            }
            else if (strTargetType.ToLower() == "ppu3")
            {
                OperateType = HW_Type.PPU3;
            }
            else if (strTargetType.ToLower() == "ppu4")
            {
                OperateType = HW_Type.PPU4;
            }
            else if (strTargetType.ToLower() == "evo")
            {
                OperateType = HW_Type.EVO;
            }
            else if (strTargetType.ToLower() == "evc")
            {
                OperateType = HW_Type.EVC;
            }
            else if (strTargetType.ToLower() == "ncu")
            {
                OperateType = HW_Type.NCU;
            }
            else if (strTargetType.ToLower() == "pcu")
            {
                OperateType = HW_Type.PCU;
            }
            else if (strTargetType.ToLower() == "opc")
            {
                OperateType = HW_Type.LOCAL_SINUMERIK_OPERATE;
            }
            else if (strTargetType.ToLower() == "sinutrain")
            {
                OperateType = HW_Type.VM_SINUMERIK_OPERATE;
            }
            else if (strTargetType.ToLower() == "ppu1740")
            {
                OperateType = HW_Type.PPU1740;
            }
            return OperateType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static List<TestEnvironment> GetTestEnvironment(HW_Type targetType)
        {
            List<TestEnvironment> lTestEnvironments = new List<TestEnvironment>();
            if (targetType == HW_Type.NCU)
            {
                TestEnvironment teNCU = new TestEnvironment();
                teNCU.Group = "SINUMERIK_840D_SL";
                teNCU.Name = "NCU";
                lTestEnvironments.Add(teNCU);
            }
            else if (targetType == HW_Type.EVO)
            {
                TestEnvironment teEVO = new TestEnvironment();
                teEVO.Group = "SINUMERIK_840evo";
                teEVO.Name = "NCU1750";
                lTestEnvironments.Add(teEVO);
            }
            else if (targetType == HW_Type.PPU3 || targetType == HW_Type.PPU4)
            {
                TestEnvironment tePPU = new TestEnvironment();
                tePPU.Group = "SINUMERIK_828D";
                tePPU.Name = "828D";
                lTestEnvironments.Add(tePPU);
            }
            else if (targetType == HW_Type.PPU1740)
            {
                TestEnvironment tePPUEVO = new TestEnvironment();
                tePPUEVO.Group = "SINUMERIK_840evo";
                tePPUEVO.Name = "PPU1740";
                lTestEnvironments.Add(tePPUEVO);
            }
            else if (targetType == HW_Type.EVC)
            {
                TestEnvironment teEVC = new TestEnvironment();
                teEVC.Group = "SINUMERIK_840evo";
                teEVC.Name = "VC";
                lTestEnvironments.Add(teEVC);
            }
            else if (targetType == HW_Type.PCU)
            {
                TestEnvironment tePCU = new TestEnvironment();
                tePCU.Group = "SINUMERIK_PCU";
                tePCU.Name = "PCU";
                lTestEnvironments.Add(tePCU);
            }
            else if (targetType == HW_Type.IPC)
            {
                TestEnvironment teIPC = new TestEnvironment();
                teIPC.Group = "SINUMERIK_PCU";
                teIPC.Name = "IPC";
                lTestEnvironments.Add(teIPC);
            }
            return lTestEnvironments;
        }

        /// <summary>
        /// Get the intended string from the log
        /// </summary>
        /// <param name="strLine"></param>
        /// <param name="strIdentifier"></param>
        /// <returns></returns>
        public static string ParseIdentifier(string strLine, string strIdentifier)
        {
            string strResult = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(strLine))
                {
                    return string.Empty;
                }
                strLine = strLine.Trim();
                if (strLine.StartsWith(strIdentifier))
                {
                    strResult = strLine.Substring(strIdentifier.Length).Trim();
                }
                else
                {
                    strResult = strLine.Substring(0, strLine.Length - strIdentifier.Length).Trim();
                }
                return strResult;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Delete contents of a folder
        /// </summary>
        /// <param name="strFolder"></param>
        /// <returns></returns>
        public static bool DeleteContents(string strFolder)
        {
            if (!Directory.Exists(strFolder))
            {
                return true;//Return true if directory already cleared
            }
            try
            {
                DirectoryInfo diInfo = new DirectoryInfo(strFolder);
                foreach (FileInfo file in diInfo.GetFiles())
                {
                    File.SetAttributes(file.FullName, FileAttributes.Normal); //For access denied error
                    file.Delete();
                }
                foreach (DirectoryInfo dir in diInfo.GetDirectories())
                {
                    DeleteContents(dir.FullName);
                    dir.Delete();
                }
            }
            catch
            {
                //Console.WriteLine("Could not delete the contents of {0}. Error: {1}", strFolder, ex.Message);
                return false;
            }
            return true;
        }

    }
}
