using System;
using System.Collections;
using System.Data.Odbc;
using System.Windows.Forms;
using LateBoundNodeLink;

namespace Activity_Logger_05
{
	/// <summary>
	/// Summary description for Day.
	/// </summary>
	public class Day : AL05Base
	{
		#region Constant variables

		private const string mconPROJECT_NAME = "Activity_Logger_05";
		private const string mconCLASS_NAME = "Day";

		#endregion

		#region Variable Declaration

		private Company mobjCompany;
		private Project mobjProject;
		private Person mobjPerson;

		private bool mblOverNight;
		
		private int mintNFSubExternalLink;

		private ArrayList marrTimePeriods = new ArrayList();
		private DateTime mdtALDate;
		
		private bool mblHasChaged = false;
		private bool mblnIsComplete = false;
		private bool mblnLate;

		private bool blnLocked;
 
		#endregion

		#region Properties

		public bool HasChanged {
			set{mblHasChaged = value;}
			get{return mblHasChaged;}
		}

		public bool IsComplete 
		{
			set{mblnIsComplete = value;}
			get{return mblnIsComplete;}
		}
		
		public ArrayList TimePeriods 
		{
			set{marrTimePeriods = value;}
			get{return marrTimePeriods;}
		}

		public DateTime Date {
			set{mdtALDate = value;}
			get{return mdtALDate;}
		}

		public Person Person {
			set{mobjPerson = value;}
			get{return mobjPerson;}
		}

		public Company Company {
			set{mobjCompany = value;}
			get{return mobjCompany;}
		}

		public Project Project {
			set{mobjProject = value;}
			get{return mobjProject;}
		}

		public bool Late {
			set { mblnLate = value ; }
			get { return mblnLate ; }
		}

		public bool OverNight {
			set { mblOverNight = value ; }
			get { return mblOverNight ; }
		}

		public bool LockedProperty {
			set{blnLocked = value;}
			get{return blnLocked;}
		}

		public int NFSubExternalLinkValue {
			get{return mintNFSubExternalLink;}
		}

		#endregion
		
		#region Constructors
		/********************************************************************************
		* Constructors
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		*********************************************************************************/

		/// <summary>
		/// This constructor is used for retrieveing day data from table t_dv_almain
		/// </summary>
		public Day(int intID, DateTime dtALDate, Person objPerson, bool blOverNight, int intNFSubExternalLink, Database objDatabase, Company objCompany, Project objProject, bool blnLate)
		: base(intID, objDatabase, null)
		{
			this.ID = intID;
			mdtALDate = dtALDate;
			mobjPerson = objPerson;
		    mblOverNight = blOverNight;
			mintNFSubExternalLink = intNFSubExternalLink;
			this.Database = objDatabase;
			mobjCompany = objCompany;
			mobjProject = objProject;
			mblnLate = blnLate;
		}

		/// <summary>
		/// This methods is used for adding days to the t_dv_almain table.
		/// </summary>
		public Day(DateTime dtALDate, Person objPerson, bool blOverNight, int intNFSubExternalLink, Database objDatabase)
			: base(0, objDatabase, null)
		{
			mdtALDate = dtALDate;
			mobjPerson = objPerson;
			mblOverNight = blOverNight;
			mintNFSubExternalLink = intNFSubExternalLink;
			this.Database = objDatabase;
	
		}

		#endregion

		#region Public Methods

		/********************************************************************************
		* AddTimePeriodStamp()
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Constructs a TimePeriod object based on the created DayObject.
		* This method is only used with conjunction with the scheduled task for inserting
		* days and time periods into the database once a week.
		*
		* Parameters: strTimePeriod - The TimePeriod stamp e.g. "0900:1100"
		*********************************************************************************/
		public TimePeriod AddTimePeriodStamp (string strTimePeriod)
		{
			const string conPROC_NAME = "InsertIntoDatabase";
			string strCurrentOpp = "prior to TRY";

			try 
			{
				this.Logger.EnterFunction(conPROC_NAME);
				strCurrentOpp = "Returning TimePeriod object";
				this.Logger.LogVerbose(strCurrentOpp);

				return new TimePeriod(this, strTimePeriod);
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
		* InsertIntoDatabase()
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Inserts Days into table t_dv_alday.
		*
		*********************************************************************************/
		public void InsertIntoDatabase() 
		{
			const string conPROC_NAME = "InsertIntoDatabase";
			string strCurrentOpp = "prior to TRY";
			string strSQL;

			string strCompany;
			string strProject;

			try 
			{
				this.Logger.EnterFunction(conPROC_NAME);

				if(this.Company == null)
					strCompany = " null ";
				else
					strCompany = Database.ConvertToSQLSafe(this.Company.ID.ToString().Trim());

				if(this.Project == null)
					strProject = " null ";
				else
					strProject = Database.ConvertToSQLSafe(this.Project.ID.ToString().Trim());

				if(this.ID == 0) {

					this.ID = this.Database.GetNextID("t_dv_ALMain");

					strCurrentOpp = "Inserting Day into database with ID of: " + this.ID.ToString();
					this.Logger.LogSQL(strCurrentOpp);

					strSQL = 
						"INSERT INTO t_dv_ALMain " +
						"    ([ID], [AL_DATE], [PEOPLE_LINK],  [OVERNIGHT], [NF_SUB_LINK]) " +
						"VALUES (" +
							Database.ConvertToSQLSafe(this.ID) + ", " +
							Database.ConvertToSQLSafe(mdtALDate) + " ," + 
							Database.ConvertToSQLSafe(mobjPerson.ID) + ", " +
							Database.ConvertToSQLSafe(mblOverNight) + ", " + 
							Database.ConvertToSQLSafe(mintNFSubExternalLink) + ")";
					
				} else {
					
					strCurrentOpp = "Updating Day in database with a ID of: " + this.ID;
					this.Logger.LogSQL(strCurrentOpp);

					strSQL = 
						"UPDATE " +
						"    t_dv_ALMain " +
						"SET " +
						"    [NF_SUB_LINK] = " + Database.ConvertToSQLSafe(mintNFSubExternalLink) + ", " +
						"    [ON_COMP_LINK] = " + strCompany + ", " +
						"    [ON_PROJ_LINK] = " + strProject + ", " +
						"    [OVERNIGHT] = " + Database.ConvertToSQLSafe(mblOverNight) + " " +
						"WHERE " +
						"    [ID] = " + Database.ConvertToSQLSafe(this.ID);
									
				} 

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogVerbose(strCurrentOpp);

				this.Database.ExecuteSQLCommand(strSQL);

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
		* AddTimePeriod()
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Adds a TimePeriod to the ArrayList of TimePeriods
		*
		* Parameters: Parameters are the TimePeriod's constructor.
		*********************************************************************************/
		public void AddTimePeriod(int intID, string strTimePeriod, string strNotes, Company objCompany, 
			Project objProject, WorkPackage objWorkPackage, bool blnOffSick, bool blnOnHoliday, bool blnInGUIUse, Location objLocation, bool blnPreSales) 
		{
			const string conPROC_NAME = "GetTimePeriods";
			string strCurrentOpp = "prior to TRY";
			
			try
			{
				strCurrentOpp = "Adding TimePeriod objects to arraylist with ID of: " + intID;
				this.Logger.LogVerbose(strCurrentOpp);

				marrTimePeriods.Add(new TimePeriod(intID, this, strTimePeriod, strNotes, objCompany, objProject, objWorkPackage, 
					blnOffSick, blnOnHoliday, this.Database, blnInGUIUse, objLocation, blnPreSales));
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
	}
}
