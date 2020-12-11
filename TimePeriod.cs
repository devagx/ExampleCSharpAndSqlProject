using System;

namespace Activity_Logger_05
{
	/// <summary>
	/// Summary description for Day.
	/// </summary>
	public class TimePeriod : AL05Base
	{
		#region Constant variables

		private const string mconPROJECT_NAME = "Activity_Logger_05";
		private const string mconCLASS_NAME = "TimePeriod";

		#endregion

		#region Variable Declaration

		private string mstrNotes;
	    private Company mobjCompany;
		private Project mobjProject;
		private WorkPackage mobjWorkPackage;
		private bool mblOffSick;
		private bool mblOnHoliday;
		private Location mobjLocation;

		private bool mblnInGUIUse;
		private Day mobjDay;
		private bool mblnPreSales;

		#endregion

		#region Properties

		public Day Day {
			set{ mobjDay = value ; }
			get{ return mobjDay ; }
		}

		public string Notes {
			set { mstrNotes = value ; }
			get { return mstrNotes ; }
		}

		public Location LocationObj {
			set { mobjLocation = value ; }
			get { return mobjLocation ; }
		}

		public Company Company {
			set { mobjCompany = value ; }
			get { return mobjCompany; }
		}

		public Project Project {
			set { mobjProject = value ; }
			get { return mobjProject ;}
		}

		public WorkPackage WorkPackage {
			set{mobjWorkPackage = value;}
			get{return mobjWorkPackage;}
		}

		public bool OffSick {
			set{mblOffSick = value;}
			get{return mblOffSick;}
		}

		public bool OnHoliday {
			set{mblOnHoliday = value;}
			get{return mblOnHoliday;}
		}

		public bool PreSales {
			set{mblnPreSales = value;}
			get{return mblnPreSales;}
		}

		public bool InGUIUse {
			set {
				mblnInGUIUse = value ; 
			}
			get { return mblnInGUIUse ; }
		}

		#endregion

		#region Constructors

		/********************************************************************************
		* Constructors
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		*********************************************************************************/

		/// <summary>
		/// This constructor is used for TimePeriod database Updates
		/// </summary>
		public TimePeriod(int intID, Day objDay, string strTimePeriod, string strNotes, Company objCompany, 
			Project objProject, WorkPackage objWorkPackage, bool blnOffSick, bool blnOnHoliday, 
			Database objDatabase, bool blnInGUIUse, Location objLocation, bool blnPreSales)
			: base(intID, objDatabase, strTimePeriod)
		{
			this.ID = intID;
			mobjDay = objDay;
			this.Name = strTimePeriod;
			mstrNotes = strNotes;
			mobjCompany = objCompany;
			mobjProject = objProject;
			mobjWorkPackage = objWorkPackage;
			mblOffSick = blnOffSick;
			mblOnHoliday = blnOnHoliday;
			this.Database = objDatabase;	
			mblnInGUIUse = blnInGUIUse;
			mobjLocation = objLocation;
			mblnPreSales = blnPreSales;

		}
	
		/// <summary>
		/// This constructor is used for TimePeriod database Inserts
		/// </summary>
		public TimePeriod(Day objDay, string strTimePeriod)
			: base(0, objDay.Database, strTimePeriod)
		{	
			mobjDay = objDay;
			this.Name= strTimePeriod;
			this.Database = objDay.Database;			
		}

		#endregion

		#region Public Methods

		/********************************************************************************
		* InsertIntoDatabase()
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Inserts and updates the t_dv_altimeperiod table.
		*
		*********************************************************************************/
		public void InsertIntoDatabase() 
		{
			const string conPROC_NAME = "InsertIntoDatabase";
			string strCurrentOpp = "prior to TRY";
			string strSQL;

			string strCompany;
			string strProject;
			string strWorkPackage;
			string strLocation;

			try 
			{
				if(this.Company == null)
					strCompany = " null ";
				else
					strCompany = this.Company.ID.ToString().Trim();

				if(this.Project == null)
					strProject = " null ";
				else
					strProject = this.Project.ID.ToString().Trim();

				if(this.WorkPackage == null)
					strWorkPackage = " null ";
				else
					strWorkPackage = this.WorkPackage.ID.ToString().Trim();

				if(this.LocationObj == null)
					strLocation = " null ";
				else
					strLocation = this.LocationObj.Name.ToString().Trim();



				if(this.ID ==0) 
				{
					this.ID = this.Database.GetNextID("t_dv_ALTimePeriod");

					strSQL= 
						"INSERT INTO t_dv_ALTimePeriod ( " +
						"    [ID], " +
						"    [MAIN_AL_LINK], " +
						"    [TIME_PERIOD], " +
						"    [NOTES], " +
						"    [COMPANY_LINK], " +
						"    [PROJECT_LINK], " +
						"    [WORKPACKAGE_LINK], " +
						"    [OFF_SICK], " +
						"    [ON_HOLIDAY], " +
						"    [IN_GUI_USE], " +
						"    [LOCATION], " +
						"    [PRESALES] " +
						"    ) " +
						"VALUES ( " +
						    Database.ConvertToSQLSafe(this.ID) + ", " + 
							Database.ConvertToSQLSafe(mobjDay.ID) + ", " + 
						    Database.ConvertToSQLSafe(this.Name) + ", " +
							"'', " +
							"null, " +
							"null, " +
							"null, " +
							"0, " +
							"0, " +
							Database.ConvertToSQLSafe(this.mblnInGUIUse) +", " +
							"'Other',"  +
							Database.ConvertToSQLSafe(this.mblnPreSales)  +")";
				}
				else
				{
					this.Logger.EnterFunction(conPROC_NAME);
					strCurrentOpp = "Updating TimePeriod in database with a ID of: " + this.ID;
					this.Logger.LogSQL(strCurrentOpp);

					strSQL = 
						" UPDATE " +
						"    t_dv_ALTimePeriod " +
						" SET " +
						"    [NOTES] = " + Database.ConvertToSQLSafe(mstrNotes) +", " +
						"    [TIME_PERIOD] = '" + this.Name + "' , " +
						"    [OFF_SICK] = " + Database.ConvertToSQLSafe(mblOffSick)+ ", " +
						"    [ON_HOLIDAY] = " + Database.ConvertToSQLSafe(mblOnHoliday) + ", " +
						"    [IN_GUI_USE] = " + Database.ConvertToSQLSafe(mblnInGUIUse) + ", " +
						"    [COMPANY_LINK] = " + strCompany + ", " +
						"    [PROJECT_LINK] = " + strProject + ", " +
						"    [LOCATION] = " + Database.ConvertToSQLSafe(strLocation) + ", " +
						"    [WORKPACKAGE_LINK] = " + strWorkPackage + ", " +
						"    [PRESALES] = " + Database.ConvertToSQLSafe(mblnPreSales) + " " +
						" WHERE " +
						"    [ID] = " + this.ID;
				
				}

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogSQL(strCurrentOpp);

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

		#endregion	
	}
}
