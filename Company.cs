using System;
using System.Collections;
using System.Data.Odbc;

namespace Activity_Logger_05
{
	/// <summary>
	/// Summary description for Company.
	/// </summary>
	public class Company : AL05Base
	{
		/********************************************************************************
		* Company class
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* This class provides functionality to:
		* - insert, update, delete companies
		* - Retrieve associated projects with a particular project.
		*********************************************************************************/

		#region Constant variables
		private const string mconPROJECT_NAME = "Activity_Logger_05";
		private const string mconCLASS_NAME = "Company";
		#endregion

		#region Variable declaration

		private ArrayList marrLiveProjects;
		private ArrayList marrAllProjects;

		#endregion

		#region Properties

		/********************************************************************************
		* Projects Property
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: This property allows retrieval of all associated Projects with the
		* current company that are LIVE.
		*
		* If the arraylist is null then GetProjects() is called to populate the projects
		* arraylist.  i.e. no matter what the circumstance is, a populated arraylist will
		* always be returned (unless an error has occured else where).
		*
		*********************************************************************************/
		public ArrayList Projects
		{
			set{marrLiveProjects = value;}
			get
			{
				if(marrLiveProjects == null)
				{
					marrLiveProjects = GetLiveProjects();
				}
				return marrLiveProjects;
			}
		}

		/********************************************************************************
		* AllProjects Property
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: This property allows retrieval of all associated Projects with the
		* current company. LIVE and NON LIVE projects.
		*
		* If the arraylist is null then GetProjects() is called to populate the projects
		* arraylist.  i.e. no matter what the circumstance is, a populated arraylist will
		* always be returned (unless an error has occured else where).
		*
		*********************************************************************************/
		public ArrayList AllProjects 
		{
			set{marrAllProjects = value;}
			get {
				if(marrAllProjects == null) {
					marrAllProjects = GetAllProjects();
				}
				return marrAllProjects;
			}
		}

		#endregion

		#region Constructor/s

		/********************************************************************************
		* Constructors
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		*
		*********************************************************************************/
		
		/// <summary>
		/// Creates a Company. This constructor is used for retrieving companies from the
		/// database.
		/// </summary>
		public Company(int intCompanyID, string strCompanyName, Database objDatabase)
			: base (intCompanyID, objDatabase, strCompanyName)
		{
			this.ID = intCompanyID;
			this.Name = strCompanyName;
			this.Database = objDatabase;
			marrLiveProjects = null;

		}

		#endregion

		#region Private Methods

		/********************************************************************************
		* GetProjects () 
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: This method builds project objects based on THIS company.
		*********************************************************************************/
		private ArrayList GetLiveProjects () 
		{
			const string conPROC_NAME = "GetLiveProjects";
			string strCurrentOpp = "prior to TRY";
			Project objProject;

			string strSQL = 
				" SELECT " + 
				"    system_id, "+
				"    contract_id, " +
				"    contract_name, " +
				"    company_id, " +
				"    proj_details, " +
				"    live_project " +
				" FROM " +
				"    docsadm.v_dv_GetProjects " +
				" WHERE " +
				"    company_id = " + this.ID +
			    " AND " +
				"    live_project = 1 ";

			ArrayList arrProjects = new ArrayList();

			try 
			{
				this.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogSQL(strCurrentOpp);

				this.Database.ExecuteSQLQuery(strSQL);

				while (this.Database.DataReader.Read()) 
				{	
					strCurrentOpp = "Building project object with project code of: " + 
						this.Database.DataReader.GetInt32("system_id");
					this.Logger.LogVerbose(strCurrentOpp);

					objProject = new Project(this.Database.DataReader.GetInt32("system_id"), this, 
						this.Database.DataReader.GetString("contract_id"), 
						this.Database.DataReader.GetString("contract_name"),
						this.Database.DataReader.GetString("proj_details"),
						this.Database.DataReader.GetBoolean("live_project"));
					arrProjects.Add(objProject);

				}

				this.Database.DataReader.Close();
				this.Database.ClearCommand();

				return arrProjects;

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
		* GetAllProjects () 
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: This method builds project objects based on THIS company.
		*********************************************************************************/
		private ArrayList GetAllProjects () {
			const string conPROC_NAME = "GetAllProjects";
			string strCurrentOpp = "prior to TRY";
			Project objProject;

			string strSQL = 
				" SELECT " + 
				"    system_id, "+
				"    contract_id, " +
				"    contract_name, " +
				"    company_id, " +
				"    proj_details, " +
				"    live_project " +
				" FROM " +
				"    docsadm.v_dv_GetProjects " +
				" WHERE " +
				"    company_id = " + this.ID;

			ArrayList arrProjects = new ArrayList();

			try {
				this.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "About to execute SQL query: " + strSQL;
				this.Logger.LogSQL(strCurrentOpp);

				this.Database.ExecuteSQLQuery(strSQL);

				while (this.Database.DataReader.Read()) {	
					strCurrentOpp = "Building project object with project code of: " + 
						this.Database.DataReader.GetInt32("system_id");
					this.Logger.LogVerbose(strCurrentOpp);

					objProject = new Project(this.Database.DataReader.GetInt32("system_id"), this, 
						this.Database.DataReader.GetString("contract_id"), 
						this.Database.DataReader.GetString("contract_name"),
						this.Database.DataReader.GetString("proj_details"),
						this.Database.DataReader.GetBoolean("live_project"));
					arrProjects.Add(objProject);

				}

				this.Database.DataReader.Close();
				this.Database.ClearCommand();

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
        
		#endregion

	

		#region Public Methods

		/********************************************************************************
		* ClearProjects()  
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: This method assigns the array of project to NULL.  This method should
		* be run each time a new company has been selected.
		*********************************************************************************/
		public void ClearProjects() 
		{
			const string conPROC_NAME = "ClearProjects";
			string strCurrentOpp = "prior to TRY";

			try {
				this.Logger.EnterFunction(conPROC_NAME);

				marrLiveProjects = null;
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
