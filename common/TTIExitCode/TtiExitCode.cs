using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTIExitCode
{
    public class TtiExitCode
    {
        /// <summary>
        /// Return/Exit codes
        /// </summary>
        /// <summary>
        /// Return/Exit codes
        /// </summary>
        public enum TTIExitCodes
        {
            //Exit codes for all target types
            /// <summary>
            /// Step successfully completed
            /// </summary>
            TERMINATED = -1,
            /// <summary>
            /// Step successfully completed
            /// </summary>
            SUCCESSFUL = 0,
            /// <summary>
            /// One or more parameters missing
            /// </summary>
            INSUFFICIENT_DATA = 1,
            /// <summary>
            /// Failed to initialize TTI components
            /// </summary>
            INITIALIZATION_FAILED = 2,
            /// <summary>
            /// HMI build path not found/reachable
            /// </summary>
            HMI_BUILDPATH_NOT_FOUND = 3,
            /// <summary>
            /// GIV build path not found/reachable
            /// </summary>
            GIV_BUILDPATH_NOT_FOUND = 4,
            /// <summary>
            /// Squish build path not found/reachable
            /// </summary>
            SQUISH_BUILDPATH_NOT_FOUND = 5,
            /// <summary>
            /// PsExec tool not found
            /// </summary>
            PSEXEC_TOOL_NOT_FOUND = 6,
            /// <summary>
            /// Supporting package/file for TTI could not be copied on to the target
            /// </summary>
            COPY_FAILED = 7,
            /// <summary>
            /// Setting the registry failed
            /// </summary>
            SETTING_REGISTRY_FAILED = 8,
            /// <summary>
            /// Existing hmislalive file cound not be deleted
            /// </summary>
            COULD_NOT_DELETE_EXISTING_HMISLALIVE_FILE = 9,
            /// <summary>
            /// Putty keyfile not found
            /// </summary>
            PUTTY_KEYFILE_NOT_FOUND = 10,
            /// <summary>
            /// Could not determine the relevant GIV
            /// </summary>
            COULDNOT_FIND_RELEVANT_GIV = 11,
            /// <summary>
            /// Test packages/addon path not available/reachable
            /// </summary>
            PACKAGES_PATH_NOT_FOUND = 12,
            /// <summary>
            /// Could not create the installation directory
            /// </summary>
            INSTALL_DIR_CREATION_ERROR = 13,
            /// <summary>
            /// Available space in the target could not be determined
            /// </summary>
            COULDNOT_CHECK_AVAILABLE_MEMORY = 14,
            /// <summary>
            /// Mounting the shared location failed
            /// </summary>
            MOUNT_FAILED = 15,
            /// <summary>
            /// Unmounting the mounted location failed
            /// </summary>
            UNMOUNT_FAILED = 16,
            /// <summary>
            /// Clearing the installation directory (cleanup) failed
            /// </summary>
            CLEAR_INSTALL_DIR_FAILED = 17,
            /// <summary>
            /// TTI configuration file not found
            /// </summary>
            CONFIG_FILE_NOT_FOUND = 18,
            /// <summary>
            /// Could not read the TTI config file
            /// </summary>
            CONFIG_FILE_COULDNOT_BE_READ = 19,
            /// <summary>
            /// Could not find the Tools directory
            /// </summary>
            TOOLS_DIR_NOT_FOUND = 20,
            /// <summary>
            /// Target type provided cound not be determined or is not implemented
            /// </summary>
            UNKNOWN_TARGET_TYPE = 21,
            /// <summary>
            /// Unknown parameter provided
            /// </summary>
            UNKNOWN_PARAMETER = 22,
            /// <summary>
            /// One or more internal issues occoured during installation
            /// </summary>
            ONE_OR_MORE_INSTALLATIONS_FAILED = 23,
            /// <summary>
            /// one or more inputs are not valid
            /// </summary>
            WRONG_INPUT_PARAMETER = 24,
            /// <summary>
            /// Renaming the archives failed
            /// </summary>
            RENAMING_ARCHIVES_FAILED = 25,
            /// <summary>
            /// Copying the GIV build on to target failed
            /// </summary>
            GIV_BUILD_COPY_FAILED = 26,
            /// <summary>
            /// Copying the HMI build on to target failed
            /// </summary>
            HMI_BUILD_COPY_FAILED = 27,
            /// <summary>
            /// Copying the Squish build on to target failed
            /// </summary>
            SQUISH_BUILD_COPY_FAILED = 28,
            /// <summary>
            /// Copying the test package(s) on to target failed
            /// </summary>
            TEST_PACKAGE_COPY_FAILED = 29,
            /// <summary>
            /// Copying the archieve(s) on to target failed
            /// </summary>
            ARCHIVES_COPY_FAILED = 30,
            /// <summary>
            /// Copying the debug package on to target failed
            /// </summary>
            DEBUG_PACKAGE_COPY_FAILED = 31,
            /// <summary>
            ///MLFB NO NOT FOUND IN CONFIG
            /// </summary>
            MLFB_NOTFOUND_INCONFIG = 32,
            /// <summary>
            /// CLONING FAILED
            /// </summary>
            CLONING_FAILED = 33,
            /// <summary>
            /// IP could not be resolved from hostname
            /// </summary>
            IP_NOT_RESOLVED = 34,
            /// <summary>
            /// Setup ini not present in the build for Win targets
            /// </summary>
            INSTALLER_SETUP_FILE_NOT_FOUND = 35,
            /// <summary>
            /// Operate did not come up after addon and/or squish installations
            /// </summary>
            OPERATE_DIDNOT_START_AFTER_ADDON_OR_SQUISH_INSTALL = 36,
            /// <summary>
            /// mmc.ini file is not updated
            /// </summary>
            MMCFILE_NOT_UPDATED = 37,

            /// <summary>
            /// Comm tool build path not available
            /// </summary>
            COMMTOOL_BUILDPATH_NOT_FOUND = 38,
            /// <summary>
            /// Comm tool build file not available
            /// </summary>
            COMMTOOL_BUILDFILE_NOT_FOUND = 39,
            /// <summary>
            /// Folder was not created
            /// </summary>
            FOLDER_CREATION_FAILED = 40,

            //Blacklisting candidates. Starts from 100
            /// <summary>
            /// Target is not pingable
            /// </summary>
            NOT_PINGABLE = 100,
            /// <summary>
            /// Target is not SSH-able
            /// </summary>
            NOT_SSHABLE = 101,
            /// <summary>
            /// Access denied
            /// </summary>
            ACCESS_DENIED = 102,
            /// <summary>
            /// Target did not reboot
            /// </summary>
            REBOOT_FAILED = 103,
            /// <summary>
            /// Not enough memory in the target
            /// </summary>
            NOT_ENOUGH_MEMORY = 104,
            /// <summary>
            /// Could not find the un-install key
            /// </summary>
            UNINSTALL_KEY_NOT_FOUND = 105,
            /// <summary>
            /// Target did not come up after installation
            /// </summary>
            DEVICE_DOWN_AFTER_INSTALLTION = 106,
            /// <summary>
            /// Could not update the connected NCU
            /// </summary>
            CONNECTED_NCU_INSTALLATION_FAILED = 107,
            /// <summary>
            /// Target did not reboot after installation
            /// </summary>
            TARGET_DIDNOT_REBOOT_AFTER_INSTALLTION = 108,
            /// <summary>
            /// Uninstallation of existing sinumerik operate failed
            /// </summary>
            UNINSTALL_FAILED = 109,
            /// <summary>
            /// Changing Setup.ini failed after copying the build 
            /// </summary>
            CHANGING_SETUP_INI_FAILED = 110,
            /// <summary>
            /// "sc stop all" failed
            /// </summary>
            SC_STOPALL_FAILED = 111,
            /// <summary>
            /// Could not set the firewall rules
            /// </summary>
            FIREWALL_ACCESS_FAILURE = 112,
            /// <summary>
            /// "sc enable hmi" failed
            /// </summary>
            ENABLE_HMI_FAILURE = 113,
            /// <summary>
            /// "sc disable hmi" failed
            /// </summary>
            DISABLE_HMI_FAILURE = 114,
            /// <summary>
            /// Could not set the auto logon
            /// </summary>
            AUTOLOGON_COULD_NOT_BE_SET = 115,
            /// <summary>
            /// usb connection issue
            /// </summary>
            USB_CONNECTION_ISSUE = 116,
            /// <summary>
            /// Installation timed out
            /// </summary>
            INSTALLATION_TIMED_OUT = 117,
            /// <summary>
            /// cannot install HMI
            /// </summary>
            CANNOT_INSTALL_HMI_IN_SECURE_TARGET = 118,
            /// <summary>
            /// Commissioning Tool Installation Failed
            /// </summary>
            INSTALL_FAILED = 119,
            //Faulty build candidates. Starts from 200
            /// <summary>
            /// HMI build file not found
            /// </summary>
            HMI_BUILDFILE_NOT_FOUND = 200,
            /// <summary>
            /// GIV build file not found
            /// </summary>
            GIV_BUILDFILE_NOT_FOUND = 201,
            /// <summary>
            /// Squish build file not found
            /// </summary>
            SQUISH_BUILDFILE_NOT_FOUND = 202,
            /// <summary>
            /// Installer file Setup_HMIsl_(Version).exe not found
            /// </summary>
            INSTALLER_FILE_NOT_FOUND = 203,
            /// <summary>
            /// Test packages/addon file not available
            /// </summary>
            PACKAGES_FILE_NOT_FOUND = 204,
            /// <summary>
            /// Test packages/addon file not available
            /// </summary>
            INSTALLATION_DIR_NOT_FOUND = 205,

            //Blacklisting and Faulty build Candidates. Starts from 300
            /// <summary>
            /// hmislalive file did not appear
            /// </summary>
            HMISLALIVE_DID_NOT_APPEAR = 300,
            /// <summary>
            /// Signature files not found
            /// </summary>
            SIGNATURE_FILE_NOT_FOUND = 301,
            /// <summary>
            ///  When Target is stuck in hung state
            /// </summary>
            TARGET_IN_HUNG_STATE_AFTER_GIV_INSTALLATION = 302,
            /// <summary>
            ///  When Target is not pingable after installation
            /// </summary>
            TARGET_NOT_PINGABLE_AFTER_GIV_INSTALLATION = 303,
            /// <summary>
            ///  When Target is not sshable after installation
            /// </summary>
            TARGET_NOT_SSHABLE_AFTER_GIV_INSTALLATION = 304,
            /// <summary>
            ///  When Target is not allowing access to user
            /// </summary>
            TARGET_ACCESS_DENIED_AFTER_GIV_INSTALLATION = 305,
            /// <summary>
            ///  When Target is stuck in hung state
            /// </summary>
            TARGET_IN_HUNG_STATE_AFTER_OPERATE_INSTALLATION = 306,
            /// <summary>
            ///  When Target is not pingable after installation
            /// </summary>
            TARGET_NOT_PINGABLE_AFTER_OPERATE_INSTALLATION = 307,
            /// <summary>
            ///  When Target is not sshable after installation
            /// </summary>
            TARGET_NOT_SSHABLE_AFTER_OPERATE_INSTALLATION = 308,
            /// <summary>
            ///  When Target is not allowing access to user
            /// </summary>
            TARGET_ACCESS_DENIED_AFTER_OPERATE_INSTALLATION = 309,
            /// <summary>
            ///  When Squish port is not open on target
            /// </summary>
            SQUISH_PORT_NOT_OPEN = 310,
            /// <summary>
            /// Uncompressing the build in the target failed
            /// </summary>
            UNCOMPRESSING_FAILED = 311,
            /// <summary>
            /// Automatic installation failed on the target
            /// </summary>
            AUTOMATIC_INSTALLATION_FAILED = 312,
            /// <summary>
            /// Could not start the installation executable on the target
            /// </summary>
            STARTING_EXE_FOR_INSTALLATION_FAILED = 313,
            /// <summary>
            /// Could not start the sinumerik operate after installation
            /// </summary>
            COULDNOT_START_OPERATE_AFTER_INSTALLTION = 314,
            /// <summary>
            /// Squish server not running in the target after installation
            /// </summary>
            SQUISH_SERVER_NOT_RUNNING = 315,
            /// <summary>
            /// Could not validate the version from the target after installation
            /// </summary>
            VERSIONXML_VALIDATION_FAILED = 316,
            /// <summary>
            /// Operate did not start after automatic installation
            /// </summary>
            OPERATE_DIDNOT_START_AFTER_AUTO_INSTALL = 317,
            /// <summary>
            /// Installation command failed on the target
            /// </summary>
            INSTALLATION_COMMAND_FAILED = 318

        }
        /// <summary>
        /// Get Exit Code equivalent String
        /// </summary>
        /// <param name="ExitCode"></param>
        /// <returns></returns>
        public static String GetEquivalentString(TTIExitCodes ExitCode)
        {
            String strEquivalentString = "Installation failed due to one or more internal issues";

            switch (ExitCode)
            {
                case TTIExitCodes.TERMINATED:
                    strEquivalentString = "Execution unsuccessful. The TTI exited abruptly";
                    break;
                case TTIExitCodes.SUCCESSFUL:
                    strEquivalentString = "Execution successful";
                    break;
                case TTIExitCodes.INSTALLATION_TIMED_OUT:
                    strEquivalentString = "Installation timed out";
                    break;
                case TTIExitCodes.INSUFFICIENT_DATA:
                    strEquivalentString = "One or more parameters missing";
                    break;
                case TTIExitCodes.INITIALIZATION_FAILED:
                    strEquivalentString = "Failed to initialize TTI components";
                    break;
                case TTIExitCodes.HMI_BUILDPATH_NOT_FOUND:
                    strEquivalentString = "HMI build path not found/reachable";
                    break;
                case TTIExitCodes.HMI_BUILDFILE_NOT_FOUND:
                    strEquivalentString = "HMI build file not found";
                    break;
                case TTIExitCodes.GIV_BUILDPATH_NOT_FOUND:
                    strEquivalentString = "GIV build path not found/reachable";
                    break;
                case TTIExitCodes.GIV_BUILDFILE_NOT_FOUND:
                    strEquivalentString = "GIV build file not found";
                    break;
                case TTIExitCodes.SQUISH_BUILDPATH_NOT_FOUND:
                    strEquivalentString = "Squish build path not found/reachable";
                    break;
                case TTIExitCodes.SQUISH_BUILDFILE_NOT_FOUND:
                    strEquivalentString = "Squish build file not found";
                    break;
                case TTIExitCodes.HMISLALIVE_DID_NOT_APPEAR:
                    strEquivalentString = "hmislalive file did not appear";
                    break;
                case TTIExitCodes.COMMTOOL_BUILDPATH_NOT_FOUND:
                    strEquivalentString = "Commissioning Tool Build Path  not found";
                    break;
                case TTIExitCodes.COMMTOOL_BUILDFILE_NOT_FOUND:
                    strEquivalentString = "Commissioning Tool Build File not found";
                    break;
                case TTIExitCodes.PSEXEC_TOOL_NOT_FOUND:
                    strEquivalentString = "PsExec tool not found";
                    break;
                case TTIExitCodes.INSTALLER_FILE_NOT_FOUND:
                    strEquivalentString = "Installer file Setup_HMIsl_(Version).exe not found";
                    break;
                case TTIExitCodes.COPY_FAILED:
                    strEquivalentString = "Supporting package/file for TTI could not be copied on to the target";
                    break;
                case TTIExitCodes.SETTING_REGISTRY_FAILED:
                    strEquivalentString = "Setting the registry failed";
                    break;
                case TTIExitCodes.FOLDER_CREATION_FAILED:
                    strEquivalentString = "Folder was not created";
                    break;
                case TTIExitCodes.COULD_NOT_DELETE_EXISTING_HMISLALIVE_FILE:
                    strEquivalentString = "Existing hmislalive file cound not be deleted";
                    break;
                case TTIExitCodes.INSTALL_FAILED:
                    strEquivalentString = "Commissioning tool installation failed";
                    break;
                case TTIExitCodes.PUTTY_KEYFILE_NOT_FOUND:
                    strEquivalentString = "Putty keyfile not found";
                    break;
                case TTIExitCodes.COULDNOT_FIND_RELEVANT_GIV:
                    strEquivalentString = "Could not determine the relevant GIV";
                    break;
                case TTIExitCodes.PACKAGES_PATH_NOT_FOUND:
                    strEquivalentString = "Test packages/addon path not available/reachable";
                    break;
                case TTIExitCodes.PACKAGES_FILE_NOT_FOUND:
                    strEquivalentString = "Test packages/addon file not found";
                    break;
                case TTIExitCodes.INSTALL_DIR_CREATION_ERROR:
                    strEquivalentString = "Could not create the installation directory";
                    break;
                case TTIExitCodes.COULDNOT_CHECK_AVAILABLE_MEMORY:
                    strEquivalentString = "Available space in the target could not be determined";
                    break;
                case TTIExitCodes.MOUNT_FAILED:
                    strEquivalentString = "Mounting the shared location failed";
                    break;
                case TTIExitCodes.UNMOUNT_FAILED:
                    strEquivalentString = "Unmounting the mounted location failed";
                    break;
                case TTIExitCodes.CLEAR_INSTALL_DIR_FAILED:
                    strEquivalentString = "Clearing the installation directory (cleanup) failed";
                    break;
                case TTIExitCodes.CONFIG_FILE_NOT_FOUND:
                    strEquivalentString = "TTI configuration file not found";
                    break;
                case TTIExitCodes.CONFIG_FILE_COULDNOT_BE_READ:
                    strEquivalentString = "Could not read the TTI config file";
                    break;
                case TTIExitCodes.TOOLS_DIR_NOT_FOUND:
                    strEquivalentString = "Could not find the Tools directory";
                    break;
                case TTIExitCodes.UNKNOWN_TARGET_TYPE:
                    strEquivalentString = "Target type provided cound not be determined or is not implemented";
                    break;
                case TTIExitCodes.UNKNOWN_PARAMETER:
                    strEquivalentString = "Unknown parameter provided";
                    break;
                case TTIExitCodes.ONE_OR_MORE_INSTALLATIONS_FAILED:
                    strEquivalentString = "One or more internal issues occoured during installation";
                    break;
                case TTIExitCodes.WRONG_INPUT_PARAMETER:
                    strEquivalentString = "one or more inputs are not valid";
                    break;
                case TTIExitCodes.RENAMING_ARCHIVES_FAILED:
                    strEquivalentString = "Renaming the archives failed";
                    break;
                case TTIExitCodes.GIV_BUILD_COPY_FAILED:
                    strEquivalentString = "Copying the GIV build on to target failed";
                    break;
                case TTIExitCodes.HMI_BUILD_COPY_FAILED:
                    strEquivalentString = "Copying the HMI build on to target failed";
                    break;
                case TTIExitCodes.SQUISH_BUILD_COPY_FAILED:
                    strEquivalentString = "Copying the Squish build on to target failed";
                    break;
                case TTIExitCodes.TEST_PACKAGE_COPY_FAILED:
                    strEquivalentString = "Copying the test package(s) on to target failed";
                    break;
                case TTIExitCodes.ARCHIVES_COPY_FAILED:
                    strEquivalentString = "Copying the archieve(s) on to target failed";
                    break;
                case TTIExitCodes.DEBUG_PACKAGE_COPY_FAILED:
                    strEquivalentString = "Copying the debug package on to target failed";
                    break;
                case TTIExitCodes.CLONING_FAILED:
                    strEquivalentString = "Cloning on the VM failed";
                    break;
                case TTIExitCodes.INSTALLATION_DIR_NOT_FOUND:
                    strEquivalentString = "Installation directory to copy build files not present in the target";
                    break;
                case TTIExitCodes.IP_NOT_RESOLVED:
                    strEquivalentString = "IP could not be resolved from HostName";
                    break;
                //Blacklist candidates
                case TTIExitCodes.NOT_PINGABLE:
                    strEquivalentString = "Target is not pingable";
                    break;
                case TTIExitCodes.NOT_SSHABLE:
                    strEquivalentString = "Target is not SSH-able";
                    break;
                case TTIExitCodes.ACCESS_DENIED:
                    strEquivalentString = "Access denied";
                    break;
                case TTIExitCodes.REBOOT_FAILED:
                    strEquivalentString = "Reboot of target failed";
                    break;
                case TTIExitCodes.TARGET_IN_HUNG_STATE_AFTER_GIV_INSTALLATION:
                    strEquivalentString = "Target stuck in hung state after GIV installation";
                    break;
                case TTIExitCodes.TARGET_NOT_PINGABLE_AFTER_GIV_INSTALLATION:
                    strEquivalentString = "Target is not pingable after GIV installation";
                    break;
                case TTIExitCodes.TARGET_NOT_SSHABLE_AFTER_GIV_INSTALLATION:
                    strEquivalentString = "Target is not SSH-able after GIV installation";
                    break;
                case TTIExitCodes.TARGET_ACCESS_DENIED_AFTER_GIV_INSTALLATION:
                    strEquivalentString = "Access denied on target after GIV installation";
                    break;
                case TTIExitCodes.TARGET_IN_HUNG_STATE_AFTER_OPERATE_INSTALLATION:
                    strEquivalentString = "Target stuck in hung state after Operate installation";
                    break;
                case TTIExitCodes.TARGET_NOT_PINGABLE_AFTER_OPERATE_INSTALLATION:
                    strEquivalentString = "Target is not pingable after Operate installation";
                    break;
                case TTIExitCodes.TARGET_NOT_SSHABLE_AFTER_OPERATE_INSTALLATION:
                    strEquivalentString = "Target is not SSH-able after Operate installation";
                    break;
                case TTIExitCodes.TARGET_ACCESS_DENIED_AFTER_OPERATE_INSTALLATION:
                    strEquivalentString = "Access denied on target after Operate installation";
                    break;
                case TTIExitCodes.NOT_ENOUGH_MEMORY:
                    strEquivalentString = "Not enough memory in the target";
                    break;
                case TTIExitCodes.UNCOMPRESSING_FAILED:
                    strEquivalentString = "Uncompressing the build in the target failed";
                    break;
                case TTIExitCodes.UNINSTALL_KEY_NOT_FOUND:
                    strEquivalentString = "Could not find the un-install key";
                    break;
                case TTIExitCodes.AUTOMATIC_INSTALLATION_FAILED:
                    strEquivalentString = "Automatic installation failed on the target";
                    break;
                case TTIExitCodes.STARTING_EXE_FOR_INSTALLATION_FAILED:
                    strEquivalentString = "Could not start the installation executable on the target";
                    break;
                case TTIExitCodes.DEVICE_DOWN_AFTER_INSTALLTION:
                    strEquivalentString = "Target did not come up after installation";
                    break;
                case TTIExitCodes.CONNECTED_NCU_INSTALLATION_FAILED:
                    strEquivalentString = "Could not update the connected NCU";
                    break;
                case TTIExitCodes.TARGET_DIDNOT_REBOOT_AFTER_INSTALLTION:
                    strEquivalentString = "Target did not reboot after installation";
                    break;
                case TTIExitCodes.COULDNOT_START_OPERATE_AFTER_INSTALLTION:
                    strEquivalentString = "Could not start the sinumerik operate after installation";
                    break;
                case TTIExitCodes.SQUISH_SERVER_NOT_RUNNING:
                    strEquivalentString = "Squish server not running in the target after installation";
                    break;
                case TTIExitCodes.SQUISH_PORT_NOT_OPEN:
                    strEquivalentString = "Squish port not open on the target";
                    break;
                case TTIExitCodes.VERSIONXML_VALIDATION_FAILED:
                    strEquivalentString = "Could not validate the version from the target after installation";
                    break;
                case TTIExitCodes.UNINSTALL_FAILED:
                    strEquivalentString = "Uninstallation of existing sinumerik operate failed";
                    break;
                case TTIExitCodes.CHANGING_SETUP_INI_FAILED:
                    strEquivalentString = "Changing Setup.ini failed after copying the build ";
                    break;
                case TTIExitCodes.OPERATE_DIDNOT_START_AFTER_AUTO_INSTALL:
                    strEquivalentString = "Operate did not start after automatic installation";
                    break;
                case TTIExitCodes.SC_STOPALL_FAILED:
                    strEquivalentString = "'sc stop all' failed";
                    break;
                case TTIExitCodes.INSTALLATION_COMMAND_FAILED:
                    strEquivalentString = "Installation command failed on the target";
                    break;
                case TTIExitCodes.FIREWALL_ACCESS_FAILURE:
                    strEquivalentString = "Could not set the firewall rules";
                    break;
                case TTIExitCodes.ENABLE_HMI_FAILURE:
                    strEquivalentString = "'sc enable hmi' failed";
                    break;
                case TTIExitCodes.DISABLE_HMI_FAILURE:
                    strEquivalentString = "'sc disable hmi' failed";
                    break;
                case TTIExitCodes.AUTOLOGON_COULD_NOT_BE_SET:
                    strEquivalentString = "Could not set the auto logon";
                    break;
                case TTIExitCodes.INSTALLER_SETUP_FILE_NOT_FOUND:
                    strEquivalentString = "Setup ini not present in the build for Win targets";
                    break;
                case TTIExitCodes.OPERATE_DIDNOT_START_AFTER_ADDON_OR_SQUISH_INSTALL:
                    strEquivalentString = "Operate did not come up after addon and/or squish installations";
                    break;
                case TTIExitCodes.MMCFILE_NOT_UPDATED:
                    strEquivalentString = "MMC File is not updated";
                    break;
                default:
                    strEquivalentString = "Unrecognised exit code from TTI";
                    break;
            }
            return strEquivalentString;
        }
    }
}
