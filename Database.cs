using System;
using System.Collections;
using System.Data.Odbc;
using System.Windows.Forms;

using DV.Resources;
using LateBoundNodeLink;

namespace Activity_Logger_05
{
	/// <summary>
	/// Summary description for Database.
	/// </summary>
	public class Database
	{
	/********************************************************************************
	* Database class
	* --------------------------------------------------------------------------------
	* Author: Alun Groome
	* 
	* This class provides the following functionality:
	*	-Retrieve XML settings
	*	-Standard SQL functions
	*	-Retrieving companies and technical people from the database.	
	*********************************************************************************/
		#region Constant variables

		private const string mconPROJECT_NAME = "Activity_Logger_05";
		private const string mconCLASS_NAME = "Database";
		public const string gconDATE_FORMAT = "dd/MM/yyyy";
		public const string gconAL_REG_PATH = @"Software\Digital Vision\Activity Logger\";
		public const string gconXML_FILE_PATH = "XMLFilePath";

		#endregion
		
		#region Variable Declaration

		private ExtendedLogger mobjLogger;
		private Company mobjCompany;

		private ArrayList marrCompanies = null;
		private ArrayList marrSelectedCompanies = null;
		private ArrayList marrTechnicalPeople = null;
		private ArrayList marrTechnicalSupportPeople = null;
		private OdbcCommand mobjDBCommand = null;
		private OdbcConnection mobjDBConnection;

		private ALDataReader mobjDBDataReader = null;
		private ArrayList marrLocations = null;

		private LBNLCore mobjLBNLCore;
		private int mintClientType;
		
		
		#endregion

		#region Properties
			
		public LBNLCore LBNLCore
		{
			set{mobjLBNLCore = value;}
			get{return mobjLBNLCore;}
		}

		public ExtendedLogger Logger {
			set{mobjLogger = value;}
			get{return mobjLogger;}
		}

		public ArrayList Companies
		{
			set{marrCompanies = value;}
			get
			{
				if (marrCompanies == null)
				{
					marrCompanies = GetCompanys();
				}
				return marrCompanies;
			}
		}

		//This property returns only companies that have associated projects.
		public ArrayList SelectedCompanies {
			set{marrSelectedCompanies = value;}
			get {
				if (marrSelectedCompanies == null) {
					marrSelectedCompanies = GetSelectedCompanys();
				}
				return marrSelectedCompanies;
			}
		}

		public ArrayList Locations {
			set{marrLocations = value;}
			get {
				if (marrLocations == null) {
					marrLocations = GetLocationValues();
				}
				return marrLocations;
			}
		}

		public ArrayList TechnicalPeople
		{
			set{marrTechnicalPeople = value;}
			get
			{
				if (marrTechnicalPeople == null)
				{
					marrTechnicalPeople = GetTechnicalPeople();
				}
				return marrTechnicalPeople;
			}
		}

		public ArrayList TechnicalSupportPeople {
			set{marrTechnicalSupportPeople = value;}
			get {
				if (marrTechnicalSupportPeople == null) {
					marrTechnicalSupportPeople = GetTechnicalSupportPeople();
				}
				return marrTechnicalSupportPeople;
			}
		}
		
		//change to internal
		public ALDataReader DataReader 
		{
			get { return mobjDBDataReader ; }
		}

		#endregion

		#region Constructor/s
		/********************************************************************************
		* Constructors
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		*********************************************************************************/
		/// <summary>
		/// Creates a Database object. Main application functionality is derived from this
		/// class.
		/// 
		/// Parameters: intClientType - 0 = Thick Client, 1 = Thin Client
		/// </summary>
		public Database(int intClientType)
		{
			LBNLFactory obj;
			const string conPROC_NAME = "Database";
			string strCurrentOpp = "prior to TRY";

			mintClientType = intClientType;

			try {
				LoadSettings();
				this.Logger.EnterFunction(conPROC_NAME);

				this.Logger.LogVerbose("Creating the core NodeLink object");

				obj = new LBNLFactory();
				mobjLBNLCore = obj.CreateLBNLCore((LBNLCore.LBSourceTypes) 6, "AL TimePeriods");
			}
			catch (Exception objException) {
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				this.Logger.LeaveFunction(conPROC_NAME);
			}
			//Instantiate DatabaseInterface object
			
		}

		#endregion

		#region Private methods

		/********************************************************************************
		* CreateCase (Person objPerson, DateTime dtmWeekStart)  
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: This method creates a NodeLink case based on the following parameters.
		*
		* Parameters: objPerson - Technical member
		*			  dtmWeekStart  - W/C date
		*********************************************************************************/

		//Used to be used in TimePeriodCreator
		public void CreateCase (Person objPerson, DateTime dtmWeekStart){

			const string conPROC_NAME = "CreateCase";
			string strCurrentOpp = "prior to TRY";

			LBCase objLBCase;

			try {
				this.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "Creating new Case in NodeLink on user: " + objPerson.Name;
				this.Logger.LogVerbose(strCurrentOpp);

				objLBCase = mobjLBNLCore.CreateNewCase("Activity Logger", objPerson.Name);
				this.CreateUserActivityObjects(objPerson, dtmWeekStart, objLBCase.ID);

			}
			catch (Exception objException) {
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				this.Logger.LeaveFunction(conPROC_NAME);
			}


		}

		/********************************************************************************
		* LoadSettings
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Configures:
		*	-Database
		*	-Logger
		*	-XML
		*
		*********************************************************************************/
		private void LoadSettings () 
		{
			const string conPROC_NAME = "LoadSettings";
			string strCurrentOpp = "prior to TRY";

			try
			{
				//XML settings (Load config file)
				ConfigFile objCf;

				string strConnectionSring;
				string strLogMessagesPath;
				string strErrorMessagesPath;
				string strLogLevel;
				string strLogPeriod;
				string strXMLPath;

				//get XML file path
				strXMLPath = Registry.GetValue(Registry.HKeys.Local_Machine, gconAL_REG_PATH, gconXML_FILE_PATH);

				objCf = new ConfigFile(strXMLPath);

				//Logger settings
				strLogMessagesPath = objCf.GetValue("ACTIVITY_LOGGER_05", "Logging", "MessagesPath");
				strErrorMessagesPath = objCf.GetValue("ACTIVITY_LOGGER_05", "Logging", "ErrorMessagesPath");
				strLogLevel = objCf.GetValue("ACTIVITY_LOGGER_05","Logging","MessageLevel");
				strLogPeriod = objCf.GetValue("ACTIVITY_LOGGER_05","Logging","LogPeriod");

				mobjLogger = new ExtendedLogger(mconPROJECT_NAME, strErrorMessagesPath, strLogMessagesPath, strLogLevel, strLogPeriod);

				mobjLogger.LogMessage("Begin Logger");
			
				//Database settings
				mobjLogger.LogMessage("Set up database connection string");

				strConnectionSring = objCf.GetValue("ACTIVITY_LOGGER_05", "Database", "ConnectionString");
				mobjDBConnection = new System.Data.Odbc.OdbcConnection(strConnectionSring);
				
				mobjLogger.LogMessage("Open database connection");
				mobjDBConnection.Open();

				objCf = null;
			}
			catch (Exception objException) 
			{
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}
		#endregion

		#region SQL Functions

		//take out statics. And use DatabaseInterface object.
		internal static string ConvertToSQLSafe(int val) {
			return " " + val.ToString().Trim() + " ";
		}

		internal static string ConvertToSQLSafe(string str) {
			return " '" + str.Replace("'", "''") + "' ";
		}

		internal static string ConvertToSQLSafe(DateTime dtm) {
			return " convert(datetime, '" + dtm.ToString(gconDATE_FORMAT) + "', 103) ";
		}

		internal static string ConvertToSQLSafe(bool bln) {
			if (bln)
				return " 1 ";
			else
				return " 0 ";
		}


		internal void ExecuteSQLCommand(string strSQL) 
		{
						
			const string conPROC_NAME = "ExecuteSQLCommand";

			string strCurrentOpp = "prior to TRY";

			try 
			{
				mobjLogger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "creating ODBC command from connection";
				mobjLogger.LogVerbose(strCurrentOpp);
				mobjDBCommand = mobjDBConnection.CreateCommand();
				

				strCurrentOpp = "setting command text and executing SQL : '" + strSQL + "'";
				mobjLogger.LogSQL(strCurrentOpp);
				mobjDBCommand.CommandText = strSQL;
				mobjDBCommand.ExecuteNonQuery();
					
				ClearCommand();

			} 
			catch (Exception objException) 
			{
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;

			} 
			finally 
			{
				mobjLogger.LeaveFunction(conPROC_NAME);

			}
		}

		internal void ExecuteSQLQuery(string strSQL) 
		{
								
			const string conPROC_NAME = "ExecuteSQLQuery";

			string strCurrentOpp = "prior to TRY";

			try 
			{
				mobjLogger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "creating ODBC command from connection";
				mobjLogger.LogVerbose(strCurrentOpp);
				mobjDBCommand = mobjDBConnection.CreateCommand();
				
				strCurrentOpp = "setting command text and executing SQL : '" + strSQL + "'";
				mobjLogger.LogSQL(strCurrentOpp);
				mobjDBCommand.CommandText = strSQL;
				
				mobjDBDataReader = new ALDataReader(mobjDBCommand.ExecuteReader());

			} 
			catch (Exception objException) 
			{
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;

			} 
			finally 
			{
				mobjLogger.LeaveFunction(conPROC_NAME);

			}
		}

		internal string ExecuteSQLScaler(string strSQL) 
		{
												
			const string conPROC_NAME = "ExecuteSQLScaler";

			string strResult;

			string strCurrentOpp = "prior to TRY";

			try 
			{
				mobjLogger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "creating ODBC command from connection";
				mobjLogger.LogVerbose(strCurrentOpp);
				mobjDBCommand = mobjDBConnection.CreateCommand();

				strCurrentOpp = "setting command text and executing SQL : '" + strSQL + "'";
				mobjLogger.LogSQL(strCurrentOpp);
				mobjDBCommand.CommandText = strSQL;

				strResult = mobjDBCommand.ExecuteScalar().ToString();

				ClearCommand();

				return strResult;

			} 
			catch (Exception objException) 
			{
					this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;

			} 
			finally 
			{
				mobjLogger.LeaveFunction(conPROC_NAME);
			}
		}


		internal void ClearCommand() 
		{
			
			const string conPROC_NAME = "ClearCommand";

			string strCurrentOpp = "prior to TRY";

			try 
			{
				this.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "clearing comand and setting data reader to 'null'";
				mobjLogger.LogVerbose(strCurrentOpp);

				if (mobjDBCommand != null) 
				{
					mobjDBCommand.Dispose();
					mobjDBCommand = null;
				
				}
				mobjDBDataReader = null;

			} 
			catch (Exception objException) 
			{
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;

			} 
			finally 
			{
				this.Logger.LeaveFunction(conPROC_NAME);
	
			}
		}

		#endregion

		#region Public methods

		/********************************************************************************
		* FollowLinkOnNode (Person objPerson, DateTime dtmWeekStart)  
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: This method follows a link on a node to enable backdated cases to be 
		* displayed on the Case Search form. Once displayed on the case search form, users
		* can then open timeperiods from previous dates.
		*
		* Parameters: objPerson - Technical member
		*			  dtmWeekStart  - W/C date
		*********************************************************************************/

		//Used to be used in PreviousTimePeriods
		public void FollowLinkOnNode (Person objPerson, DateTime dtmWeekStart) {

			const string conPROC_NAME = "FollowLinkOnNode";
			string strCurrentOpp = "prior to TRY";
			
			ArrayList arrDays = new ArrayList();
			ArrayList arrSubCases;
			
			LBSubCase objLBSubCase;
			LBCase objLBCase;
			LBNode objLBNode;
			
			string strDate;
			bool blCaseExists;

			try {
				this.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "getting start day of week";
				this.Logger.LogVerbose(strCurrentOpp);

				strDate = dtmWeekStart.ToString();
				strDate = strDate.Remove(strDate.Length -9, 9);

				while (dtmWeekStart.DayOfWeek != DayOfWeek.Monday)
					dtmWeekStart = dtmWeekStart.AddDays(-1);

				blCaseExists = this.CheckCaseExistance(objPerson, dtmWeekStart);

				if(!blCaseExists) {

					this.Logger.LogVerbose("if this message is logged, !blCaseExists evaluates to FALSE");
					strCurrentOpp = "Removing 9 characters on temporary date string 'strDate'";
					this.Logger.LogVerbose(strCurrentOpp);

					strDate = dtmWeekStart.ToString();
					strDate = strDate.Remove(strDate.Length -9, 9);

					if(mintClientType == 0) {
						MessageBox.Show ("A case does not exist for user: " + objPerson.Name+ ", W/C Date: " + strDate, mconPROJECT_NAME,
							MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					else if(mintClientType == 1) {
						throw new Exception("A Case does not exist for user: " + objPerson.Name);
					}

				}
				else {
					
					arrDays = this.GetDays(objPerson.ID, dtmWeekStart);

					if(arrDays.Count != 0) {

						strCurrentOpp = "Obtaining Node Link case based on case link ID: " + ((Activity_Logger_05.Day)arrDays[0]).NFSubExternalLinkValue;
						this.Logger.LogVerbose(strCurrentOpp);

						objLBCase = mobjLBNLCore.GetCase(((Activity_Logger_05.Day) arrDays[0]).NFSubExternalLinkValue);

						this.Logger.LogVerbose("LateBound Case object created, with ID of : " + objLBCase.ID);
					}
					else {
						throw new Exception("No 'Day' information exists in the database"); 
					}

					arrSubCases = objLBCase.GetSubCases("Week Activity"); 

					if(arrSubCases.Count !=0 || arrSubCases != null) {

						strCurrentOpp = "Obtaining Node Link sub case based on case ID: " + objLBCase.ID;
						this.Logger.LogVerbose(strCurrentOpp);

						objLBSubCase = (LBSubCase) arrSubCases[arrSubCases.Count -1];

						this.Logger.LogVerbose("LateBound SubCase object created, with ID of : " + objLBSubCase.ID);
					}
					else
						throw new Exception("No subcases found on case ID: " + objLBCase.ID);

					strCurrentOpp = "Obtaining Node Link node based on subcase ID: " + objLBSubCase.ID;
					this.Logger.LogVerbose(strCurrentOpp);

					objLBNode = objLBSubCase.GetLatestNode();

					this.Logger.LogVerbose("LateBound Node object created, with ID of : " + objLBNode.ID);
					
					if(objLBNode.Description == "Complete") {
						objLBNode.FollowLink("Re-Open");

						strDate = dtmWeekStart.ToString();
						strDate = strDate.Remove(strDate.Length -9, 9);

						if(mintClientType == 0) {
							MessageBox.Show ("Activity Log for " +objPerson.Name + ", W/C Date: " + strDate+" can now be reopened from the 'Open Activity Logs' form.", mconPROJECT_NAME,
								MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
				
					}
					else {

						if(mintClientType == 0) {
							MessageBox.Show ("Cannot re-open case as the current node is not in its 'Complete' state.", mconPROJECT_NAME,
								MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
						else if (mintClientType ==1) {

							throw new Exception("Cannot re-open case as the current node is not in its 'Complete' state.");
						}
					}

				}
			}
			catch (Exception objException) {
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* CreateTimePeriods (Person objPerson, DateTime dtmWeekStart)
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Creates empty time periods based on the following parameters.
		*
		* Parameters: objPerson - Technical member
		*			  dtmWeekStart  - W/C date
		*********************************************************************************/

		//Used to be used in TimePeriodCreator
		public void CreateTimePeriods (Person objPerson, DateTime dtmWeekStart) {

			const string conPROC_NAME = "CreateTimePeriods";
			string strCurrentOpp = "prior to TRY";

			ArrayList arrTechnicalPeople = new ArrayList();

			bool blCaseExists;
			string strUserNames = "";

			try {
				this.Logger.EnterFunction(conPROC_NAME);

				//Removes the 'All' object from the technical group arraylist

				this.TechnicalPeople.RemoveAt(0);
		
				//Handles the 'ALL' technical people option
				if(objPerson.ID == -1) {
					strCurrentOpp = "getting technical people";
					this.Logger.LogVerbose(strCurrentOpp);
					arrTechnicalPeople = this.GetTechnicalPeople();

					strCurrentOpp = "getting start day of week";
					this.Logger.LogVerbose(strCurrentOpp);

					while (dtmWeekStart.DayOfWeek != DayOfWeek.Monday)
						dtmWeekStart = dtmWeekStart.AddDays(-1);

					for(int i=0; i < arrTechnicalPeople.Count; i++) {
						strCurrentOpp = "Creating NodeLink case object with owner name as: " + ((Person)arrTechnicalPeople[i]).Name;
						this.Logger.LogVerbose(strCurrentOpp);

						blCaseExists = this.CheckCaseExistance((Person)arrTechnicalPeople[i], dtmWeekStart);

						if(!blCaseExists) {

							CreateCase((Person)arrTechnicalPeople[i], dtmWeekStart);
						}
						else {

							strCurrentOpp = "Building username string ";
							this.Logger.LogVerbose(strCurrentOpp);

							strUserNames = strUserNames.Insert(strUserNames.Length, ((Person)arrTechnicalPeople[i]).Name + ", ");
						}
					}

					if(strUserNames != ""){

						strCurrentOpp = "Removing single character from username string ";
						this.Logger.LogVerbose(strCurrentOpp);

						strUserNames = strUserNames.Remove(strUserNames.Length -2, 1);
	
						if(mintClientType == 0) {
							MessageBox.Show ("Cases already exist, therefore no new cases will be created for the following users ( " + strUserNames +") all other user cases were successfully created.", mconPROJECT_NAME,
								MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
					}
					else {
						if(mintClientType == 0) {
							MessageBox.Show ("Timeperiods successfully created for ALL users", mconPROJECT_NAME,
								MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
					}
				}
				else {

					strCurrentOpp = "getting start day of week";
					this.Logger.LogVerbose(strCurrentOpp);

					while (dtmWeekStart.DayOfWeek != DayOfWeek.Monday)
						dtmWeekStart = dtmWeekStart.AddDays(-1);

				
					strCurrentOpp = "Creating NodeLink case object with owner name as: " + objPerson.Name;
					this.Logger.LogVerbose(strCurrentOpp);

					blCaseExists = this.CheckCaseExistance(objPerson, dtmWeekStart);

					if(!blCaseExists) {

						CreateCase(objPerson, dtmWeekStart);

						if(mintClientType == 0) {
							MessageBox.Show ("Timeperiods successfully created for user: "+ objPerson.Name, mconPROJECT_NAME,
								MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
					}

					else {

						if(mintClientType == 0) {
							MessageBox.Show ("A case already exists on user " + objPerson.Name+ ", therefore no new case has been created. ", mconPROJECT_NAME,
								MessageBoxButtons.OK, MessageBoxIcon.Information);
						}

					}	
				}
			}
			catch (Exception objException) {
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				this.Logger.LeaveFunction(conPROC_NAME);
			}


		}

		/********************************************************************************
		* GetObjectSelectedIndex(ArrayList arrObjects, int intObjectID) 
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Retrieves the arraylist index at a given object ID.  It is necessary to 
		* use this methoed to ensure selected indexs on combo boxes marry up to the correct
		* position in the arraylist.
		*
		* Parameters: arrObjects - Arraylist of objects to loop through, e.g. Project objects
		*			  intObjectID - Given object ID, e.g. Current Project object ID
		*********************************************************************************/
		/// <summary>
		/// Gets the index of the object from the supplied object.ID.
		/// </summary>
		public int GetObjectSelectedIndex(ArrayList arrObjects, int intObjectID) 
		{
			const string conPROC_NAME = "GetObjectSelectedIndex";
			
			int intResult = 0;
			string strCurrentOpp = "prior to TRY";

			try 
			{
				this.Logger.EnterFunction(conPROC_NAME);
				strCurrentOpp = "looping through supplied array, looking for object (id : " + intObjectID.ToString().Trim() + ")";

				this.Logger.LogVerbose(strCurrentOpp);

				for (int i = 0 ; i < arrObjects.Count ; i++) 
					if (((AL05Base) arrObjects[i]).ID == intObjectID)
						intResult = i;
				
				return intResult;

			} 
			catch (Exception objException) 
			{
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				this.Logger.LeaveFunction(conPROC_NAME);
			}

		}

		/********************************************************************************
		* GetNextID(string strTableName) 
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Retrieves the next table ID from the passed in table name
		* Parameters: strTableName - Database table name
		*********************************************************************************/
		public int GetNextID(string strTableName)
		{
			const string conPROC_NAME = "GetNextID";
			
			string strCurrentOpp = "prior to TRY";

			try {
				this.Logger.EnterFunction(conPROC_NAME);

				return Int32.Parse(ExecuteSQLScaler("docsadm.sp_dv_GetNextID '" + strTableName.Trim() + "'"));
			}
			catch (Exception objException) {
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* ValidateDeleteWorkPackage() 
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: If a workpackage has been assigned time by somebody in the activity 
		* logger system, the workpackage cannot be deleted.  Hence this function.
		*********************************************************************************/
		public bool ValidateDeleteWorkPackage(int intWPID) {
			const string conPROC_NAME = "ValidateDeleteWorkPackage";
			string strCurrentOpp = "prior to TRY";

			try {
				this.Logger.EnterFunction(conPROC_NAME);

				string strSQL =
					" SELECT" +
					"    COUNT(tp_id)" +
					" FROM" +
					"    v_dv_GetTimePeriods" +
					" WHERE" +
					"   wp_system_id = " + ConvertToSQLSafe(intWPID);

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogSQL(strCurrentOpp);

				ExecuteSQLQuery(strSQL);

				if (DataReader.Read()) {
					if (DataReader.GetInt32(0) > 0) {
						return false;
					}
					else {
						return true;
					}
				}
				else{
					return false;
				}

			}
			catch (Exception objException) {
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				mobjDBDataReader.Close();
				ClearCommand();
				
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* IsComplete(Person, Date) 
		* --------------------------------------------------------------------------------
		* Author: James Pyett
		* 
		* Synopsis: Checks weather a user has completed the core activity logs for the week.
		*********************************************************************************/
		public bool IsComplete(Person objPerson, string strDate)
		{
			const string conPROC_NAME = "IsComplete";
			string strCurrentOpp = "prior to TRY";

			try 
			{
				this.Logger.EnterFunction(conPROC_NAME);

				string strSQL = "SELECT COUNT(t_dv_ALMain.ID) AS Count FROM t_dv_ALTimePeriod " +
					"INNER JOIN t_dv_ALMain ON t_dv_ALMain.ID = t_dv_ALTimePeriod.MAIN_AL_LINK " +
					"WHERE t_dv_ALMain.PEOPLE_LINK=" + objPerson.ID + " AND t_dv_ALMain.AL_DATE='" + DateTime.Parse(strDate).ToString("yyyy-MM-dd") + "' " +
					"AND NOT ((COMPANY_LINK IS NOT NULL AND PROJECT_LINK IS NOT NULL AND WORKPACKAGE_LINK IS NOT NULL AND PRESALES=0) " +
					"OR (COMPANY_LINK IS NOT NULL AND PRESALES = 1) " +
					"OR (COMPANY_LINK IS NULL AND OFF_SICK = 1) " +
					"OR (COMPANY_LINK IS NULL AND ON_HOLIDAY = 1))";

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogSQL(strCurrentOpp);

				ExecuteSQLQuery(strSQL);

				if (DataReader.Read())
				{
					if (DataReader.GetInt32("Count") > 0)
					{
						return false;
					}
					else
					{
						return true;
					}
				}
				else
				{
					return false;
				}


			}
			catch (Exception objException) 
			{
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				mobjDBDataReader.Close();
				ClearCommand();
				
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}
		
		/********************************************************************************
		* GetCase(ArrayList arrNLCases) 
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Returns an array of Activity Logger Case Objects based Case Id's stored
		* in the database table t_dv_almain.
		* Parameters: intSubCaseLink - Case ID
		*********************************************************************************/
		public ArrayList GetCases(ArrayList arrNLCases)
		{

			const string conPROC_NAME = "GetSingleCaseInformation";
			string strCurrentOpp = "prior to TRY";

			//ArrayList arrLBSubCases;

			Case objCase = null;
			Person objPerson = null;

			ArrayList arrCases = new ArrayList();

			string strNumbers = "";

			try {
				this.Logger.EnterFunction(conPROC_NAME);

				for(int i =0; i<arrNLCases.Count; i ++) {
					strNumbers = strNumbers.Insert(strNumbers.Length, ((LBCase)arrNLCases[i]).ID.ToString().Trim()+",");
				
				}
			
				if(arrNLCases.Count > 0) {
					strNumbers = strNumbers.Remove(strNumbers.Length -1, 1);
					strNumbers.Trim();
				}

				string strSQL = 
					" SET DATEFIRST 1 " +
					" SELECT DISTINCT " +
					"   @@DATEFIRST AS 'FirstDay', DATEPART(dw, d_al_date) AS 'Date', " +
					"   d_al_date,   d_nf_sub_link,     p_system_id,     p_user_id,     d_late " +
					" FROM " +
					"   docsadm.v_dv_GetCases " + 
					" WHERE " +    
					"   d_nf_sub_link IN ( " + strNumbers + ") " + 
					" AND " + 
					"   DATEPART(dw, d_al_date) = 1 " +
					" ORDER BY " +  
					"   d_al_date ";

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogSQL(strCurrentOpp);

				if(arrNLCases.Count >0 ) {

					ExecuteSQLQuery(strSQL);

					while (DataReader.Read()) {		
						strCurrentOpp = "Building Activity Logger Case object with case ID of: " + DataReader.GetInt32("d_nf_sub_link");
						this.Logger.LogVerbose(strCurrentOpp);

						objPerson = new Person(DataReader.GetInt32("p_system_id"), DataReader.GetString("p_user_id"), this);
						this.Logger.LogVerbose("Person object created with ID: " + objPerson.ID);

						objCase = new Case(objPerson, DataReader.GetDate("d_al_date"), DataReader.GetBoolean("d_late"),DataReader.GetInt32("d_nf_sub_link"), mobjLBNLCore );
						this.Logger.LogVerbose("Case object created based on Person ID: "+ objCase.Person.ID);

						arrCases.Add(objCase);
					} 

					mobjDBDataReader.Close();
					ClearCommand();
				}

				return arrCases;

			} 
			catch (Exception objException) 
			{
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* ProjectLookUP()
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Build an arraylist of the last 5 project codes in the system 
		* (of each type).
		*
		*********************************************************************************/
		public ArrayList ProjectLookUP() {
			const string conPROC_NAME = "ProjectLookUP";
			string strCurrentOpp = "prior to TRY";

			ArrayList arrProjects = new ArrayList();

					 string strSQLRD = 
						" SELECT " +
						"    contract_id, contract_name, live_project " +
					    " FROM " +
						"    v_dv_DVTProjectLookup ";
					
					 string strSQLDVT = 
						" SELECT " +
						"    contract_id, contract_name, live_project " +
						" FROM " +
						"    v_dv_RDProjectLookup ";
	
			try {
				this.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "About to execute SQL query: " + strSQLRD;
				this.Logger.LogSQL(strCurrentOpp);

				ExecuteSQLQuery(strSQLRD);

				while (DataReader.Read()) {	

					arrProjects.Add(new Project(
						new Company(-1,"",this),
						DataReader.GetString("contract_id"),
						DataReader.GetString("contract_name"),
						DataReader.GetBoolean("live_project")));
				}

				
				strCurrentOpp = "About to execute SQL query: " + strSQLDVT;
				this.Logger.LogSQL(strCurrentOpp);

				mobjDBDataReader.Close();
				ClearCommand();

				//Add blank project to seperate both sets of data(i.e. both project types)
				arrProjects.Add(new Project(new Company(-999,"",this),"","",false));

				ExecuteSQLQuery(strSQLDVT);

				while (DataReader.Read()) {	

					arrProjects.Add(new Project(
						new Company(-1,"",this),
						DataReader.GetString("contract_id"),
						DataReader.GetString("contract_name"),
						DataReader.GetBoolean("live_project")));
				}
			
				mobjDBDataReader.Close();
				ClearCommand();

				return arrProjects;
			}
			catch (Exception objException) {
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}


		/********************************************************************************
		* GetTimePeriodStamps(int intUserID, DateTime dtDateTime)
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Build an arraylist of Time Period stamps from the database, based on
		* the current day, and the current user.
		*
		* Parameters: intUserID - PeopleID
		*			  dtDateTime - Current date the user is editing.	
		*********************************************************************************/
		public ArrayList GetTimePeriodStamps(int intUserID, DateTime dtDateTime) {
			const string conPROC_NAME = "GetTimePeriodStamp";
			string strCurrentOpp = "prior to TRY";

			ArrayList arrTimePeriodStamps = new ArrayList();

			string strSQL = 
				" SELECT DISTINCT " +   
				"    tp_time_period, " + 
				"    pl_system_id, " +  
				"    d_al_date " +  
				" FROM " +    
				"    v_dv_GetTimePeriods " + 
				" WHERE " +   
				"    pl_system_id = " + ConvertToSQLSafe(intUserID) + 
				" AND " +    
				"    d_al_date = " + ConvertToSQLSafe(dtDateTime) +   
				" ORDER BY " +     
				"    d_al_date ";
	
			try {
				this.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogSQL(strCurrentOpp);

				ExecuteSQLQuery(strSQL);

				while (DataReader.Read()) {	

					arrTimePeriodStamps.Add(DataReader.GetString("tp_time_period"));
				}
			
				mobjDBDataReader.Close();
				ClearCommand();

				return arrTimePeriodStamps;
			}
			catch (Exception objException) {
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}


		/********************************************************************************
		* GetDays(int intID, DateTime dtDateTime) 
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Build an arraylist of seven days based on the date selected and user ID
		* Parameters: intUserID - PeopleID
		*			  dtDateTime - Week Commencing date.	
		*********************************************************************************/
		public ArrayList GetDays(int intUserID, DateTime dtDateTime) 
		{
			const string conPROC_NAME = "GetTimePeriods";
			string strCurrentOpp = "prior to TRY";

			ArrayList arrDays = new ArrayList();
			DateTime dtTopDateRange = dtDateTime.AddDays(7.0);
			Company objCompany;
			Project objProject = null;
			WorkPackage objWorkPackage = null;
			Person objPerson = null;
			Company objCompany2 = null;
			Project objProject2 = null;
			Day objDay = null;
			Location objLocation = null;

			int intDayID;
			int intLastDayID;

			string strSQL = 
				" SELECT DISTINCT" +
				"    tp_id, " +
				"    tp_time_period, " +
				"    tp_notes, " +
				"    tp_location, " +
				"    com_system_id, " +
				"    com_company_name, " +
				"    com2_system_id, " +
				"    com2_company_name, " +
				"    proj_system_id, " +
				"    proj_contract_code, " +
				"    proj_contract_name, " +
				"    proj2_system_id, " +
				"    proj2_contract_code, " +
				"    proj2_contract_name, " +
				"    pl_system_id, " +
				"    d_ID, " +
				"    d_al_date, " +
				"    d_overnight, " +
				"    d_nf_sub_link, " +
				"    d_late, " +
				"    pl_system_id, " +
				"    pl_user_id, " +
				"    wp_system_id, " +
				"    wp_wp_name, " +
				"    wp_wp_num_days, " +
				"    tp_off_sick, " +
				"    tp_on_holiday, " +
				"    tp_label, " +
				"    tp_in_gui_use, " +
				"    tp_presales " +
				" FROM " +
				"    v_dv_GetTimePeriods " +
				" WHERE " +
				"   pl_system_id = " + intUserID +
				" AND " +
				"   d_al_date >= " + ConvertToSQLSafe(dtDateTime) +
				" AND " +
				"   d_al_date < " + ConvertToSQLSafe(dtTopDateRange) +
				" ORDER BY " +
				"    d_al_date, tp_time_period  ";
	
			try 
			{
				this.Logger.EnterFunction(conPROC_NAME);

				this.Locations = this.Locations;

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogSQL(strCurrentOpp);

				ExecuteSQLQuery(strSQL);

				intLastDayID = 0;

				while (DataReader.Read()) 
				{	
					if (intLastDayID == 0) 

						strCurrentOpp = "Creating new Person object with SystemID of: " + DataReader.GetInt32("pl_system_id");
						this.Logger.LogVerbose(strCurrentOpp);

						objPerson = new Person(DataReader.GetInt32("pl_system_id"), DataReader.GetString("pl_user_id"), this);

					intDayID = DataReader.GetInt32("d_ID");

					if (intLastDayID != intDayID) 
					{
						if (intLastDayID > 0)
						{
							strCurrentOpp = "Adding Day object to arraylist of days, with Day ID of: " + objDay.ID;
							this.Logger.LogSQL(strCurrentOpp);

							arrDays.Add(objDay);
						}

						if (DataReader.GetInt32("com2_system_id") > -1) 
						{
							objCompany2 = new Company(DataReader.GetInt32("com2_system_id"),DataReader.GetString("com2_company_name"),this);
							this.Logger.LogVerbose("Created objCompany2 with ID: " + objCompany2.ID);
						}
						else
						{
							objCompany2 = null;
						}

						if (objCompany2 != null && DataReader.GetInt32("proj2_system_id") > -1)
						{
							objProject2 = new Project(DataReader.GetInt32("proj2_system_id"),objCompany2, DataReader.GetString("proj2_contract_code"),
								DataReader.GetString("proj2_contract_name"), "",false);

							this.Logger.LogVerbose("Created objProject2 with ID: " + objProject2.ID);
						}
						else
						{
							objProject2 = null;
						}
 
						objDay = new Day(DataReader.GetInt32("d_ID"), DataReader.GetDate("d_al_date"),
							objPerson, DataReader.GetBoolean("d_overnight"), DataReader.GetInt32("d_nf_sub_link"), this, 
							objCompany2, objProject2, DataReader.GetBoolean("d_late"));

						this.Logger.LogVerbose("Created objDay with ID: " + objDay.ID);
					}

					if (DataReader.GetInt32("com_system_id") > -1) 
					{ 
						objCompany = new Company(DataReader.GetInt32("com_system_id"),
							DataReader.GetString("com_company_name"),this);

						this.Logger.LogVerbose("Created objCompany with ID: " + objCompany.ID);
					} 
					else
						objCompany = null;

					if (objCompany != null && DataReader.GetInt32("proj_system_id") > -1)
					{
						objProject = new Project(DataReader.GetInt32("proj_system_id"),
							objCompany, DataReader.GetString("proj_contract_code"),
							DataReader.GetString("proj_contract_name"), "",false);

						this.Logger.LogVerbose("Created objProject with ID: " + objProject.ID);

					} else
						objProject = null;


					if (objProject != null && DataReader.GetInt32("wp_system_id") > -1) 
					{
						objWorkPackage = new WorkPackage(DataReader.GetInt32("wp_system_id"),
							DataReader.GetString("wp_wp_name"),
							DataReader.GetInt32("wp_wp_num_days"),objProject,objPerson);

						this.Logger.LogVerbose("Created objWorkPackage with ID: " + objWorkPackage.ID);

					} else
						objWorkPackage = null;


					for(int i=0; i<this.Locations.Count; i++) {
						
						if(DataReader.GetString("tp_location").Trim() == ((Location)this.Locations[i]).Name) {
							
							objLocation = new Location(((Location)this.Locations[i]).ID,((Location)this.Locations[i]).Name,this);
						}
					}

					objDay.AddTimePeriod(DataReader.GetInt32("tp_id"),   
						DataReader.GetString("tp_time_period"), 
						DataReader.GetString("tp_notes"), 
						objCompany,
						objProject,
						objWorkPackage, 
						DataReader.GetBoolean("tp_off_sick"), 
						DataReader.GetBoolean("tp_on_holiday"),
						DataReader.GetBoolean("tp_in_gui_use"),
						objLocation,
						DataReader.GetBoolean("tp_presales"));

					this.Logger.LogVerbose("Adding TimePeriod with ID: " + DataReader.GetInt32("tp_id"));

							
					intLastDayID = intDayID;
				}
			
				strCurrentOpp = "Adding Day object to arraylist of days, with Day ID of: " + objDay.ID;
				this.Logger.LogVerbose(strCurrentOpp);

				arrDays.Add(objDay);

				mobjDBDataReader.Close();
				ClearCommand();

				return arrDays;
			}
			catch (Exception objException) 
			{
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* CheckCaseExistance(Person objPerson, DateTime dtStartOfWeek) 
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Used as part of the scheduled task.  Identifies if the current userID
		* and date already exists in the t_dv_almain table.  If user information does exist, 
		* the scheduled task will not create duplicate enteries.
		
		* Parameters: objPerson - Current Person object used as part of the SQL query.
		*			  dtStartOfWeek - The current 'Monday' date of the current week.
		*********************************************************************************/

		public bool CheckCaseExistance(Person objPerson, DateTime dtStartOfWeek)
		{
			const string conPROC_NAME = "CheckCaseExistance";
			string strCurrentOpp = "prior to TRY";

			int intCount=0;
			bool blCaseExists = false;

			ArrayList arrPeople = new ArrayList();

			string strSQL = 
				" SELECT COUNT (1) as count" +    
				" FROM " +       
				"     v_dv_GetTimePeriods " +
				" WHERE " +
				"  d_people_link = " + objPerson.ID +
				" AND " +
				"  d_al_date > " + ConvertToSQLSafe(dtStartOfWeek.AddDays(-1.0)) +
				" AND " +
				"  d_al_date < " + ConvertToSQLSafe(dtStartOfWeek.AddDays(1.0));

			arrPeople = new ArrayList();
	
			try 
			{
				this.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogSQL(strCurrentOpp);

				ExecuteSQLQuery(strSQL);

				while (DataReader.Read()) 
				{	
					intCount = DataReader.GetInt32("count");

				}
				mobjDBDataReader.Close();
				ClearCommand();

				if(intCount > 0)
				{
					blCaseExists = true;

				}
				strCurrentOpp = "Have Days been inserted into the database with this user? : " + blCaseExists.ToString();
				this.Logger.LogVerbose(strCurrentOpp);

				return blCaseExists;
			} 
			catch (Exception objException) 
			{
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}




		/********************************************************************************************************
		* CreateUserActivityObjects (Person objPerson, DateTime dtDateTime, int intSubCaseLink)
		* -------------------------------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Used as part of the scheduled task. Creates initial Day and TimePeriod 
		* objects before user updates can take place.
		
		* Parameters: objPerson - Current Person object used as part of the SQL query.
		*			  dtDateTime - The current 'Monday' date of the current week.
		*			  objCase - Case associated with objPerson. (Used to retrieve
		*             the sub external link value.
		*			  intSubCaseLink - NodeLink SubCase ID link
		*********************************************************************************/
		public void CreateUserActivityObjects (Person objPerson, DateTime dtDateTime, int intSubCaseLink)
		{
			const string conPROC_NAME = "CreateUserActivityObjects";
			string strCurrentOpp = "prior to TRY";

			ArrayList arrTimePeriodStrings = new ArrayList(); 
			bool blOverNight = false;

			Day objDay;
			TimePeriod objTimePeriod;

			try 
			{
				this.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "Adding Timeperiod strings to arraylist";
				this.Logger.LogVerbose(conPROC_NAME);

				arrTimePeriodStrings.Add("0900:1100");
				arrTimePeriodStrings.Add("1100:1300");
				arrTimePeriodStrings.Add("1330:1530");
				arrTimePeriodStrings.Add("1530:1730");

				strCurrentOpp = "Creating initial Day objects and TimePeriod objects, then " +
					"inserting them into the database ";
				this.Logger.LogVerbose(strCurrentOpp);

				for(int i=0; i<7; i++)
				{
					objDay = new Day(dtDateTime.AddDays(i), objPerson, blOverNight, intSubCaseLink, this  );

					this.Logger.LogVerbose("Created objDay with ID: " + objDay.ID);

					objDay.InsertIntoDatabase();

					strCurrentOpp = "Adding Day objects to arraylist, with Day ID of: " + objDay.ID;
					this.Logger.LogVerbose(conPROC_NAME);

					for (int j=0; j<4; j++)
					{	
						objTimePeriod = objDay.AddTimePeriodStamp((string)arrTimePeriodStrings[j]);

						this.Logger.LogVerbose("Created objTimePeriod with ID: " + objTimePeriod.ID);
						objTimePeriod.InsertIntoDatabase();
					}
				}
			} 
			catch (Exception objException) 
			{
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}


		/********************************************************************************
		* GetTechnicalPeople() 
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Build an arraylist of technical people from database.
		*********************************************************************************/
		public ArrayList GetTechnicalPeople() 
		{
			const string conPROC_NAME = "GetTechnicalPeople";
			string strCurrentOpp = "prior to TRY";

			ArrayList arrPeople = new ArrayList();

			string strSQL = 
				" SELECT " +    
				"    system_id, " +
				"    user_id " +
				" FROM " +       
				"    DOCSADM.v_dv_GetTechnicalPeople " +
				" ORDER BY " +
				" user_id ";

			arrPeople = new ArrayList();
	
			try 
			{
				this.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogSQL(strCurrentOpp);

				ExecuteSQLQuery(strSQL);

				while (DataReader.Read()) 
				{	
					strCurrentOpp = "Building arraylist of people objects with people_system_id of: " + DataReader.GetInt32("system_id");
					this.Logger.LogVerbose(strCurrentOpp);
					arrPeople.Add(new Person(DataReader.GetInt32("system_id"),DataReader.GetString("user_id"),this));

				}
				mobjDBDataReader.Close();
				ClearCommand();

				return arrPeople;
			} 
			catch (Exception objException) 
			{
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* GetTechnicalSupportPeople() 
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Build an arraylist of technical and support people from database.
		*********************************************************************************/
		public ArrayList GetTechnicalSupportPeople() {
			const string conPROC_NAME = "GetTechnicalSupportPeople";
			string strCurrentOpp = "prior to TRY";

			ArrayList arrPeople = new ArrayList();

			string strSQL = 
				" SELECT " +    
				"    system_id, " +
				"    user_id " +
				" FROM " +       
				"    DOCSADM.v_dv_GetTechnicalSupportPeople " +
				" ORDER BY " +
				" user_id ";

			arrPeople = new ArrayList();
	
			try {
				this.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogSQL(strCurrentOpp);

				ExecuteSQLQuery(strSQL);

				while (DataReader.Read()) {	
					strCurrentOpp = "Building arraylist of people objects with people_system_id of: " + DataReader.GetInt32("system_id");
					this.Logger.LogVerbose(strCurrentOpp);
					arrPeople.Add(new Person(DataReader.GetInt32("system_id"),DataReader.GetString("user_id"),this));

				}
				mobjDBDataReader.Close(); 
				ClearCommand();
				
				return arrPeople;
			} 
			catch (Exception objException) {
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* GetCompanys(string strCompanyName, bool blnAllCompanies)
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Build an arraylist of companies  from database based on a given
		* company name. (This function is used for the thin client activity logger).
		*
		* Parameters: strCompanyName - Company Name.
		*             blnAllCompanies - Flag.  True retrieves ALL companies. 
		* False retrieves companies that only have assoiciated projects.
		*********************************************************************************/
		public ArrayList GetCompanys (string strCompanyName, bool blnAllCompanies) {
			const string conPROC_NAME = "GetCompanys";
			string strCurrentOpp = "prior to TRY";

			string strSQL;

			if(blnAllCompanies) {
				strSQL = 
					" SELECT " +
					"     system_id, company_name " +
					" FROM " +
					"     docsadm.v_dv_GetALLCompanies " +
					" WHERE " +
					"     company_name LIKE '"+strCompanyName.Trim()+"%'" +
					" ORDER BY " +
					"    company_name ";
			}
			else {
				strSQL = 
					" SELECT " +
					"     system_id, company_name " +
					" FROM " +
					"     docsadm.v_dv_GetCompanies " +
					" WHERE " +
					"     company_name LIKE '"+strCompanyName.Trim()+"%'" +
					" ORDER BY " +
					"    company_name ";
			}

			ArrayList arrCompanies = new ArrayList();

			try {
				mobjLogger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "Executing SQL query: " + strSQL;
				mobjLogger.LogSQL(strCurrentOpp);

				ExecuteSQLQuery(strSQL);

				while (DataReader.Read()) {	
					strCurrentOpp = "Building company object with system ID of: " + DataReader.GetInt32("system_id");
					mobjLogger.LogVerbose(strCurrentOpp);

					mobjCompany = new Company(DataReader.GetInt32("system_id"), DataReader.GetString("company_name"), this);
					arrCompanies.Add(mobjCompany);
				}

				mobjDBDataReader.Close();
				ClearCommand();

				return arrCompanies;
			}
			catch (Exception objException) {
				mobjLogger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			}
			finally {
				mobjLogger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* GetCompanys()
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Build an arraylist of companies from the database.
		*********************************************************************************/
		public ArrayList GetCompanys () 
		{
			const string conPROC_NAME = "GetCompanys";
			string strCurrentOpp = "prior to TRY";

			string strSQL =
							" SELECT "+
							"    system_id, " +
							"    company_name " +
							" FROM " +
							"    docsadm.v_dv_GetALLCompanies " +
							" ORDER BY company_name ";
			
			ArrayList arrCompanies = new ArrayList();

			try 
			{
				mobjLogger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "Executing SQL query: " + strSQL;
				mobjLogger.LogSQL(strCurrentOpp);

				ExecuteSQLQuery(strSQL);

				while (DataReader.Read()) 
				{	
					strCurrentOpp = "Building company object with system ID of: " + DataReader.GetInt32("system_id");
					mobjLogger.LogVerbose(strCurrentOpp);

					mobjCompany = new Company(DataReader.GetInt32("system_id"), DataReader.GetString("company_name"), this);
					arrCompanies.Add(mobjCompany);
				}

				mobjDBDataReader.Close();
				ClearCommand();

				return arrCompanies;
			}
			catch (Exception objException)
			{
				mobjLogger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			}
			finally 
			{
				mobjLogger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* GetSelectedCompanys()
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Build an arraylist of companies that only have associated projects
		* from the database.
		*********************************************************************************/
		public ArrayList GetSelectedCompanys () {
			const string conPROC_NAME = "GetSelectedCompanys";
			string strCurrentOpp = "prior to TRY";

			string strSQL =	
				" SELECT "+
				"    system_id, " +
				"    company_name " +
				" FROM " +
				"    docsadm.v_dv_GetCompanies " +
				" ORDER BY company_name ";
			
			ArrayList arrCompanies = new ArrayList();

			try {
				mobjLogger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "Executing SQL query: " + strSQL;
				mobjLogger.LogSQL(strCurrentOpp);

				ExecuteSQLQuery(strSQL);

				while (DataReader.Read()) {	
					strCurrentOpp = "Building company object with system ID of: " + DataReader.GetInt32("system_id");
					mobjLogger.LogVerbose(strCurrentOpp);

					mobjCompany = new Company(DataReader.GetInt32("system_id"), DataReader.GetString("company_name"), this);
					arrCompanies.Add(mobjCompany);
				}

				mobjDBDataReader.Close();
				ClearCommand();

				return arrCompanies;
			}
			catch (Exception objException) {
				mobjLogger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			}
			finally {
				mobjLogger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* GetLocationValues()
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Build an arraylist of location names to populate the location
		* combo boxes.
		*********************************************************************************/
		public ArrayList GetLocationValues () {
			const string conPROC_NAME = "GetLocationValues";
			string strCurrentOpp = "prior to TRY";

			string strSQL = 
				" SELECT " + 
				"    col, " +
				"    ord " +
				" FROM " +
				"    v_dv_GetLocationValues ";

			ArrayList arrLocations = new ArrayList();

			try {
				mobjLogger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "Executing SQL query: " + strSQL;
				mobjLogger.LogSQL(strCurrentOpp);

				ExecuteSQLQuery(strSQL);

				while (DataReader.Read()) {	
				
					arrLocations.Add(new Location( DataReader.GetInt32("ord"),DataReader.GetString("col"),this));
					
				}

				mobjDBDataReader.Close();
				ClearCommand();

				return arrLocations;
			}
			catch (Exception objException) {
				mobjLogger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			}
			finally {
				mobjLogger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* MarkALCaseAsLate(int intSubCaseLink)
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: This method sets the 'late' flag in the database for a given subcase.
		*
		* Parameters: intSubCaseLink - SubCase Link i.e subcase ID in NodeLink
		*********************************************************************************/
		public void MarkALCaseAsLate(int intSubCaseLink) {

			const string conPROC_NAME = "MarkALCaseAsLate";
			string strCurrentOpp = "prior to TRY";

			try {
				mobjLogger.EnterFunction(conPROC_NAME);
			
				string strSQL;

				strSQL = 
					"UPDATE " +
					"    t_dv_ALMain " +
					"SET " +
					"    late = 1 " +
					"WHERE " +
					"    nf_sub_link = " + Database.ConvertToSQLSafe(intSubCaseLink) ;

				this.Logger.LogSQL("About to execute SQL string: " + strSQL);
				ExecuteSQLCommand(strSQL);
				ClearCommand();
			}
			catch (Exception objException) {
				mobjLogger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			}
			finally {
				mobjLogger.LeaveFunction(conPROC_NAME);
			}
		}
		
		/******************************************************************************************************************
		* SimpleNLCaseSearch(string strDescription, string strCaseTypeName, string strOwner, bool blnClosed)
		* ------------------------------------------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: This method searches for a NodeLinkCase based on the following paramters.
		*
		* Parameters: strDescription - Case description
		*			  strCaseTypeName - Case type
		*			  strOwner - Case owner
		*			  blnClosed - Case closed flag
		*
		********************************************************************************************************************/
		public ArrayList SimpleNLCaseSearch(string strDescription, string strCaseTypeName, string strOwner, bool blnClosed) {
			const string conPROC_NAME = "SimpleNLCaseSearch";
			string strCurrentOpp = "prior to TRY";

			try {
				mobjLogger.EnterFunction(conPROC_NAME);
				return mobjLBNLCore.SimpleCaseSearch(strDescription, strCaseTypeName, strOwner, blnClosed);
			}
			catch (Exception objException) {
				mobjLogger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			}
			finally {
				mobjLogger.LeaveFunction(conPROC_NAME);
			}
		}


		/********************************************************************************
		* CheckTimePeriodExistance(string strTimePeriodStamp, Day objDay)
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Cancel Event on Cancel button
		* Parameters: strTimePeriodStamp - TimePeriod range (Time from and Time To).
		*			  objDay - Current day object.
		*
		*********************************************************************************/

		public bool CheckTimePeriodExistance(string strTimePeriodStamp, Day objDay) {

			const string conPROC_NAME = "CheckTimePeriodExistance";
			string strCurrentOpp = "prior to TRY";

			int intCount=0;
			bool blTimePeriodExists = false;


			string strSQL = 
				" SELECT COUNT (1) as count" +    
				" FROM " + 
				"    v_dv_GetTimePeriods " +
				" WHERE " +
				"    tp_main_al_link = " + ConvertToSQLSafe(objDay.ID) +
				" AND " +
				"    tp_time_period = " + ConvertToSQLSafe(strTimePeriodStamp);
	
			try {
				this.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogSQL(strCurrentOpp);

				ExecuteSQLQuery(strSQL);

				while (DataReader.Read()) {	
					intCount = DataReader.GetInt32("count");

				}
				mobjDBDataReader.Close();
				ClearCommand();

				if(intCount > 0) {
					blTimePeriodExists = true;

				}
				strCurrentOpp = "Has TimePeriod (" + strTimePeriodStamp + ") been inserted into the database with this user? : " + blTimePeriodExists.ToString();
				this.Logger.LogVerbose(strCurrentOpp);

				return blTimePeriodExists;
			} 
			catch (Exception objException) {
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}


		/********************************************************************************
		* CreateNewTimePeriod (string strTimePeriodStamp, Day objDay)
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Cancel Event on Cancel button
		* Parameters: strTimePeriodStamp - TimePeriod range (Time from and Time To).
		*			  objDay - Current day object.
		*
		*********************************************************************************/
		public void CreateNewTimePeriod (string strTimePeriodStamp, Day objDay) {

			const string conPROC_NAME = "CreateNewTimePeriod";
			string strCurrentOpp = "prior to TRY";

			TimePeriod objTimePeriod;

			try {
				this.Logger.EnterFunction(conPROC_NAME);

				this.Logger.LogVerbose(strCurrentOpp);
				this.Logger.LogVerbose("Day object passed in with ID: " + objDay.ID);

				objDay.InsertIntoDatabase();
		
				objTimePeriod = objDay.AddTimePeriodStamp(strTimePeriodStamp);

				this.Logger.LogVerbose(strCurrentOpp);
				this.Logger.LogVerbose("Created objTimePeriod with ID: " + objTimePeriod.ID);

				objTimePeriod.InGUIUse =true;
				objTimePeriod.InsertIntoDatabase();
				
			} 
			catch (Exception objException) {
				this.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				this.Logger.LeaveFunction(conPROC_NAME);
			}
		}
	#endregion
	}
}
