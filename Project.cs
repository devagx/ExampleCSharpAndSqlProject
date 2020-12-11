using System;
using System.Collections;
using System.Data.Odbc;

namespace Activity_Logger_05
{
	/// <summary>
	/// Summary description for Project.
	/// </summary>
	public class Project : AL05Base
	{

	/********************************************************************************
	* Project class
	* --------------------------------------------------------------------------------
	* Author: Alun Groome
	* 
	* This class provides functionality to:
	* - insert, update, delete projects
	* - Retrieve associated workpackages with a particular project.
	*********************************************************************************/

		#region Constant variables
		private const string mconPROJECT_NAME = "Activity_Logger_05";
		private const string mconCLASS_NAME = "Project";
		#endregion

		#region Variable declaration

		private Company mobjCompany;
		private string mstrProjectCode;
		private ArrayList marrWorkPackages;
		private string mstrProjectDetails;
		private bool mblnLiveProject;
	
		#endregion

		#region Properties

		public string ProjectCode
		{
			set{mstrProjectCode = value;}
			get{return mstrProjectCode;}
		}

		public bool LiveProject {
			set{mblnLiveProject = value;}
			get{return mblnLiveProject;}
		}

		public string ProjectDetails {
			set{mstrProjectDetails = value;}
			get{return mstrProjectDetails;}
		}

		public Company Company
		{
			set{mobjCompany = value;}
			get{return mobjCompany;}
		}

		/********************************************************************************
		* WorkPackages Property
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: This property allows retrieval of all associated workpackages with the
		* current project.
		*
		* If the arraylist is null then GetWorkPackages() is called to populate the workpackages
		* arraylist.  i.e. no matter what the circumstance is, a populated arraylist will
		* always be returned (unless an error has occured else where).
		*
		*********************************************************************************/
		public ArrayList WorkPackages
		{
			set{marrWorkPackages = value;}
			get
			{
				if (marrWorkPackages == null)
				{
					marrWorkPackages = GetWorkPackages();
				}
				return marrWorkPackages;
			}
		}

		#endregion

		#region Constructor/s

		/********************************************************************************
		* Constructors
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Several constructors are provided.
		*
		*********************************************************************************/
		
		/// <summary>
		/// Creates a Project. This constructor is used when retrieving projects from the database.
		/// </summary>
		public Project(int intSystemID, Company objCompany, string strProjectCode, string strProjectName, string strProjectDetails, bool blnLiveProject)
			: base (intSystemID, objCompany.Database, strProjectName)
		{
			this.ID = intSystemID;
			mobjCompany = objCompany;
			mstrProjectCode = strProjectCode;
			this.Name = strProjectName;
			this.Database = objCompany.Database;
			marrWorkPackages = null;
			mstrProjectDetails = strProjectDetails;
			mblnLiveProject = blnLiveProject;
		}

		/// <summary>
		/// Creates a Project. This constructor is used when adding projects to a company.
		/// </summary>
		public Project (Company objCompany, string strProjectCode, string strProjectName, bool blnLiveProject)
			: base (0, objCompany.Database, strProjectName)
		{
			mobjCompany = objCompany;
			this.Name = strProjectName;
			mstrProjectCode = strProjectCode;
			mblnLiveProject = blnLiveProject;
		}


		#endregion

		#region Private Methods
		
		/********************************************************************************
		* GetWorkPackages()
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Gets associated workpackages from THIS project from the database.
		*
		*********************************************************************************/

		private ArrayList GetWorkPackages() 
		{
			const string conPROC_NAME = "GetWorkPackages";
			string strCurrentOpp = "prior to TRY";

			WorkPackage objWorkPackage;
			

			string strSQL = 
				" SELECT " +    
				"    wp_system_id, " +
				"    wp_contract_id, " +
				"    wp_wp_name, " +
				"    wp_wp_num_days, " +
				"    p_user_id, " +
				"    wp_people_id " +
				" FROM " +        
				"    DOCSADM.v_dv_GetWorkPackages " +
				" WHERE " +
				"    wp_contract_id = " + this.ID +
				" ORDER BY wp_wp_name ";

			marrWorkPackages = new ArrayList();
	
			try 
			{
				this.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogSQL(strCurrentOpp);

				this.Database.ExecuteSQLQuery(strSQL);

				while (this.Database.DataReader.Read()) 
				{	
					strCurrentOpp = "Building workpackage object with workpackage systemID of: " +
						Int32.Parse(this.Database.DataReader.GetString("wp_system_id"));
					this.Logger.LogVerbose(strCurrentOpp);

					objWorkPackage = new WorkPackage(Int32.Parse(this.Database.DataReader.GetString("wp_system_id")), 
						this.Database.DataReader.GetString("wp_wp_name"), 
						this.Database.DataReader.GetInt32("wp_wp_num_days"),this, 
						new Person(Int32.Parse(this.Database.DataReader.GetString("wp_people_id")), 
						this.Database.DataReader.GetString("p_user_id"), 
						this.Database));
					marrWorkPackages.Add(objWorkPackage);

				}
				this.Database.DataReader.Close();
				this.Database.ClearCommand();

				
				return marrWorkPackages;
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
		* ClearWorkpackages()
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Clears the workpackage arraylist.  This methods should be run each
		* time a new project has been selected from the projects combo.
		*
		*********************************************************************************/

		public void ClearWorkpackages() 
		{
			const string conPROC_NAME = "ClearWorkpackages";
			string strCurrentOpp = "prior to TRY";
	
			try {
				this.Logger.EnterFunction(conPROC_NAME);
				marrWorkPackages = null;
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
		* InsertIntoDatabase
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Transfers a boolean value into its string representation BIT value ('0' or '1')
		*
		*********************************************************************************/
		public string ConvertBoolToString (bool blnVal) {
			const string conPROC_NAME = "ConvertBoolToString";
			string strCurrentOpp = "prior to TRY";
		
			try {
				if (blnVal)
					return " 1 ";
				else
					return " 0 ";
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
		* InsertIntoDatabase
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Inserts and updates projects in the database.
		*
		*********************************************************************************/
		public void InsertIntoDatabase() 
		{
			const string conPROC_NAME = "InsertIntoDatabase";
			string strCurrentOpp = "prior to TRY";
			string strSQL;
		
			try 
			{
				this.Logger.EnterFunction(conPROC_NAME);


				if(this.ID == 0) 
				{
					this.ID = this.Database.GetNextID("SYSTEMKEY");

					strSQL= 
						"exec docsadm.sp_dv_InsertProject " +
						"'" + this.mstrProjectCode + "'" + ", " + 
						"'" + this.Name + "'" + ", " + 
						this.Company.ID +"," + this.ID + ", " +
						ConvertBoolToString(this.LiveProject);

					strCurrentOpp = "About to execute SQL query: " + strSQL;
					strCurrentOpp = "Inserting Project into database with a project system ID of: " + this.ID + " and Project code of: " + this.mstrProjectCode;
					this.Logger.LogSQL(strCurrentOpp);

					this.Database.ExecuteSQLCommand(strSQL);

					strCurrentOpp = "Adding project to arraylist with ID of: " + this.ID;
					this.Logger.LogSQL(strCurrentOpp);

					mobjCompany.Projects.Add(this);

				}
				else 
				{
					strSQL= 
						"exec docsadm.sp_dv_UpdateProject " +
						this.ID + ", '" + this.mstrProjectCode + "' , '" + this.Name +"' ,"+ ConvertBoolToString(this.LiveProject); 

					strCurrentOpp = "About to execute SQL query: " + strSQL;
					strCurrentOpp = "Updating Project in database with a system ID of: " + this.ID + 
										" and Project code of: " + this.mstrProjectCode +
						                " and Live Project sttus: " + this.LiveProject;
					this.Logger.LogSQL(strCurrentOpp);

					this.Logger.LogSQL(strCurrentOpp);
					this.Database.ExecuteSQLCommand(strSQL);
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
		#endregion
	}
}
