using System;
using DV.Resources;

namespace Activity_Logger_05 {
	/// <summary>
	/// Summary description for ExtendedLogger.
	/// </summary>
	public class ExtendedLogger : Logger {
		/********************************************************************************
		* ExtendedLogger class
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* This class is based on the original Logger class to provide simplified logging.
		*********************************************************************************/
	
		#region Constructor/s
		/********************************************************************************
		* Constructors
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		*
		*********************************************************************************/
		
		/// <summary>
		/// Creates an ExtendedLogger. This constructor is used for all logging in this project.
		/// </summary>
		public ExtendedLogger(string strApplicationName, string strErrorFolder, string strMessageFolder , string strLevel, string strPeriod)
			: base ( strApplicationName,  strErrorFolder,  strMessageFolder, GetLevel(strLevel), GetLogType(strPeriod)) {

		}
		#endregion

		#region public methods

		/********************************************************************************
		* LogNormal
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Logs 'Normal Level' messages
		* Parametres: strLogMessage - User defined log message.
		*
		*********************************************************************************/
		public void LogNormal (string strLogMessage) {
			this.LogMessage(strLogMessage, LogLevels.Normal);
		}

		/********************************************************************************
		* LogNormal
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Logs 'Verbose Level' messages
		* Parametres: strLogMessage - User defined log message.
		*
		*********************************************************************************/
		public void LogVerbose (string strLogMessage) {
			this.LogMessage(strLogMessage, LogLevels.Verbose);
		}

		/********************************************************************************
		* LogNormal
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Logs 'Debug Level' messages
		* Parametres: strLogMessage - User defined log message.
		*
		*********************************************************************************/
		public void LogDebug (string strLogMessage) {
			this.LogMessage(strLogMessage, LogLevels.Debug);
		}

		/********************************************************************************
		* EnterFunction
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Logs 'Debug Level' messages. Use specifically for enter function logging.
		* Parametres: strLogMessage - User defined log message.
		*
		*********************************************************************************/
		public void EnterFunction (string strLogMessage) {
			this.LogMessage("Entering function '" + strLogMessage.Trim() + "'...", LogLevels.Debug);
		}
		/********************************************************************************
		* LeaveFunction
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Logs 'Debug Level' messages.  Use specifically for end function logging.
		* Parametres: strLogMessage - User defined log message.
		*
		*********************************************************************************/

		public void LeaveFunction (string strLogMessage) {
			this.LogMessage("Leaving function '" + strLogMessage.Trim() + "'", LogLevels.Debug);
		}

		/********************************************************************************
		* LogFunctionError
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Logs function error messages.
		* Parametres: strLogMessage - User defined log message.
		*
		*********************************************************************************/
		public void LogFunctionError (string strClassName, string strFunctionName, string strCurrentOpp, string strErrorMessage) {
			this.LogError("|" + this.ApplicationName.Trim() + "." + strClassName.Trim() + "." + strFunctionName.Trim() + "(): Error '" + strErrorMessage.Trim() + "' occurred whilst '" + strCurrentOpp.Trim() + "'.");
		}

	
		/********************************************************************************
		* LogSQL
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Logs 'Verbose Level' messages.  Used specifically for SQL logging.
		* Parametres: strLogMessage - User defined log message.
		*
		*********************************************************************************/
		public void LogSQL (string strLogMessage) {
			this.LogMessage(strLogMessage, LogLevels.Verbose);
		}

		#endregion

	}
}
