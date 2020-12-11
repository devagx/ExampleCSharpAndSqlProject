using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Activity_Logger_05
{
	/********************************************************************************
	* frmWorkPackageDetails class
	* --------------------------------------------------------------------------------
	* Author: Alun Groome
	* 
	* This class builds the WorkPackage search form.
	*********************************************************************************/
	public class frmWorkPackageDetails : System.Windows.Forms.Form
	{
		#region Constant variables
		private const string mconPROJECT_NAME = "Activity_Logger_05";

		private const string mconPROJECT_NAME_FORM = "Activity Logger";
		private const string mconCLASS_NAME = "frmWorkPackage";
		#endregion

		#region Variable Declaration

		private System.Windows.Forms.Button cmdClose;
		private System.Windows.Forms.TextBox txtWPName;
		private System.Windows.Forms.TextBox txtNumDays;
		private System.Windows.Forms.ComboBox cboAssigned;
		private System.Windows.Forms.GroupBox gpbPackageDetails;
		private System.Windows.Forms.Label lblWPName;
		private System.Windows.Forms.Label lblNumDays;
		private System.Windows.Forms.Label lblAssignedTo;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button cmdOK;

		private WorkPackage mobjWorkPackage;
		//Indicates if the user entered valid data in the textbox 'txtNumDays'
		private bool mblValid;

		#endregion

		#region Constructors

		/********************************************************************************
		* Constructors
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		*
		*********************************************************************************/
		
		/// <summary>
		/// This constructor is used to build the workpackage details form.
		/// Only the WorkPackage object is associated with this class.
		/// </summary>
		public frmWorkPackageDetails(WorkPackage objWorkPackage)
		{
			mobjWorkPackage = objWorkPackage;

			InitializeComponent();
		}

		#endregion

		#region Private methods

		/********************************************************************************
		* PopulatePeopleCombo()
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Populates the people combo with technical group members.
		*********************************************************************************/
		private void PopulatePeopleCombo()
		{

			const string conPROC_NAME = "PopulatePeopleCombo";
			string strCurrentOpp = "prior to TRY";

			try 
			{
				for (int i=0; i<mobjWorkPackage.Database.TechnicalPeople.Count; i++)
				{
					mobjWorkPackage.Database.Logger.LogVerbose("FOR LOOP - mobjWorkPackage.Database.TechnicalPeople[i] = " + i);
					strCurrentOpp = "Adding User_id to combo, people_system_id = " + ((Person)mobjWorkPackage.Database.TechnicalPeople[i]).ID;
					mobjWorkPackage.Logger.LogVerbose(strCurrentOpp);
					cboAssigned.Items.Add(((Person)mobjWorkPackage.Database.TechnicalPeople[i]).Name);
				}

				cboAssigned.SelectedIndex = 0;
			}
			catch (Exception objException) 
			{
				mobjWorkPackage.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				mobjWorkPackage.Logger.LeaveFunction(conPROC_NAME);
			}
		}


		/********************************************************************************
		* InsertBlankPerson()
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Insers a blank person object with ID of -1.
		*********************************************************************************/
		private void InsertBlankPerson () {

			const string conPROC_NAME = "InsertBlankPerson";
			string strCurrentOpp = "prior to TRY";

			Person objPerson;

			try {

				strCurrentOpp = "Creating Blank person object with ID of -1";
				mobjWorkPackage.Logger.LogVerbose(strCurrentOpp);

				objPerson = new Person (-1, "", mobjWorkPackage.Database);

				mobjWorkPackage.Logger.LogVerbose("Created Person object with ID: " + objPerson.ID);

				strCurrentOpp = "Inserting blank person object to database arraylist of technical people";
				mobjWorkPackage.Logger.LogVerbose(strCurrentOpp);

				mobjWorkPackage.Database.TechnicalPeople.Insert(0, objPerson);
			}

			catch (Exception objException) {
				mobjWorkPackage.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				mobjWorkPackage.Logger.LeaveFunction(conPROC_NAME);
			}
	

		}

		/********************************************************************************
		* InitializeWorkPackageDetails
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Populates the workpackage form based on the workpackage object values.
		*
		*********************************************************************************/
		private void InitializeWorkPackageDetails()
		{

			const string conPROC_NAME = "InitializeWorkPackageDetails";
			string strCurrentOpp = "prior to TRY";

			try 
			{
				mobjWorkPackage.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "Building arraylist of technical people";
				mobjWorkPackage.Logger.LogVerbose(strCurrentOpp);

				mobjWorkPackage.Logger.LogVerbose("mobjWorkPackage has ID: " + mobjWorkPackage.ID);

				if (mobjWorkPackage.ID!= 0)
				{
					this.txtNumDays.Text = mobjWorkPackage.NumOfDays.ToString().Trim();
					this.txtWPName.Text = mobjWorkPackage.Name;
					this.Text = "Edit Work Package";

					PopulatePeopleCombo();

					cboAssigned.Text = mobjWorkPackage.Person.Name;
				}
				else
				{
					this.Text = "Add Work Package";
					PopulatePeopleCombo();
				}
			}

			catch (Exception objException) 
			{
				mobjWorkPackage.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				mobjWorkPackage.Logger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* IsInteger
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: This method uses the Regex class to create a regular expression.  Method returns
		* 'true' if strNumber is valid, and 'false' if strNumber is not valid.
		*
		* Parameters: strNumber - String of text to apply pattern match too.
		*
		*********************************************************************************/
		private bool IsInteger(String strNumber)
		{
			const string conPROC_NAME = "IsInteger";
			string strCurrentOpp = "prior to TRY";

			try 
			{
				mobjWorkPackage.Logger.EnterFunction(conPROC_NAME);

				strCurrentOpp = "Building regular expression [^0-9-]";
				mobjWorkPackage.Logger.LogVerbose(strCurrentOpp);

				Regex objNotIntPattern=new Regex("[^0-9-]");

				strCurrentOpp = "Building regular expression -[0-9]+$|^[0-9]+$";
				mobjWorkPackage.Logger.LogVerbose(strCurrentOpp);

				Regex objIntPattern=new Regex("^-[0-9]+$|^[0-9]+$");

				return  !objNotIntPattern.IsMatch(strNumber) &&  objIntPattern.IsMatch(strNumber);
			}
			catch (Exception objException) 
			{
				mobjWorkPackage.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				mobjWorkPackage.Logger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* cmdClose_Click
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Disposes of workpackage form.
		*
		*********************************************************************************/
		private void cmdClose_Click(object sender, System.EventArgs e)
		{
			const string conPROC_NAME = "cmdClose_Click";
			string strCurrentOpp = "prior to TRY";

			try 
			{
				mobjWorkPackage.Logger.EnterFunction(conPROC_NAME);

				this.Dispose();
			}
			catch (Exception objException) 
			{
				mobjWorkPackage.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				mobjWorkPackage.Logger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* frmWorkPackageDetails_Load(object sender, System.EventArgs e)
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Executes methods on form load.
		*
		*********************************************************************************/

		private void frmWorkPackageDetails_Load(object sender, System.EventArgs e) {

			
			const string conPROC_NAME = "frmWorkPackageDetails_Load";
			string strCurrentOpp = "prior to TRY";

			try {

				if(((Person)mobjWorkPackage.Database.TechnicalPeople[0]).ID != -1)
					InsertBlankPerson();

				InitializeWorkPackageDetails();
			}
			catch (Exception objException) {
				mobjWorkPackage.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				mobjWorkPackage.Logger.LeaveFunction(conPROC_NAME);
			}
		}

		/********************************************************************************
		* cmdOK_Click
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Inserts or updates a workpackage if field values are valid.
		*
		*********************************************************************************/
		private void cmdOK_Click(object sender, System.EventArgs e)
		{
			const string conPROC_NAME = "cmdOK_Click";
			string strCurrentOpp = "prior to TRY";

			try 
			{
				mobjWorkPackage.Logger.EnterFunction(conPROC_NAME);

				//Use regular expression to check weather user has input non integer values for 
				//Num of Days
				mblValid = IsInteger(txtNumDays.Text);

				if(txtWPName.Text.Trim() != "")
				{
						if (mblValid == true)
						{
							//Update current workpackage if object values have changed.
							if(mobjWorkPackage.ID != 0) 
							{
								if(txtWPName.Text.Trim()!= mobjWorkPackage.Name.Trim())
								{
									strCurrentOpp = "Updating workpackage with ID : " +mobjWorkPackage.ID;
									mobjWorkPackage.Logger.LogVerbose(strCurrentOpp);

									mobjWorkPackage.Name = txtWPName.Text.Trim();
									mobjWorkPackage.InsertIntoDatabase();
								}
								if(Int32.Parse(txtNumDays.Text.Trim())!= mobjWorkPackage.NumOfDays)
								{
									mobjWorkPackage.NumOfDays = Int32.Parse(txtNumDays.Text.Trim());

									strCurrentOpp = "Updating workpackage with ID : " +mobjWorkPackage.ID;
									mobjWorkPackage.Logger.LogVerbose(strCurrentOpp);

									mobjWorkPackage.InsertIntoDatabase();
								}
								if(mobjWorkPackage.Person.Name.Trim() != ((Person)mobjWorkPackage.Database.TechnicalPeople[cboAssigned.SelectedIndex]).Name.Trim())
								{
									mobjWorkPackage.Person = (Person)mobjWorkPackage.Database.TechnicalPeople[cboAssigned.SelectedIndex];

									strCurrentOpp = "Updating workpackage with ID : " +mobjWorkPackage.ID;
									mobjWorkPackage.Logger.LogVerbose(strCurrentOpp);

									mobjWorkPackage.InsertIntoDatabase();
								}
							}
								//Insert new workpackage
							else
							{
								strCurrentOpp = "Inserting new workpackage with ID : " +mobjWorkPackage.ID;
								mobjWorkPackage.Logger.LogVerbose(strCurrentOpp);
				
								if(cboAssigned.SelectedIndex == -1)
									cboAssigned.SelectedIndex =0;

								mobjWorkPackage = new WorkPackage(txtWPName.Text, Int32.Parse(txtNumDays.Text), mobjWorkPackage.Project, (Person)mobjWorkPackage.Database.TechnicalPeople[cboAssigned.SelectedIndex]);
								mobjWorkPackage.ID = mobjWorkPackage.InsertIntoDatabase();
							}
			
							this.Dispose();
						}
						else 
						{
							MessageBox.Show ("You must enter Integer values only for 'Num of Days'", mconPROJECT_NAME_FORM,
								MessageBoxButtons.OK, MessageBoxIcon.Information);
						}

				}
				else
				{
					MessageBox.Show ("You must assign a work package a name", mconPROJECT_NAME_FORM,
						MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception objException) 
			{
				mobjWorkPackage.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				mobjWorkPackage.Logger.LeaveFunction(conPROC_NAME);
			}
		}
		#endregion

		#region Protected Methods
		protected override void Dispose( bool disposing )
		{
			const string conPROC_NAME = "Dispose";
			string strCurrentOpp = "prior to TRY";

			try 
			{
				if( disposing )
				{
					if (components != null) 
					{
						strCurrentOpp = "About to execute line : 	components.Dispose();";
						mobjWorkPackage.Logger.LogDebug(strCurrentOpp);
						components.Dispose();
					}
				}
				base.Dispose( disposing );
			}
			catch (Exception objException) 
			{
				mobjWorkPackage.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally 
			{
				mobjWorkPackage.Logger.LeaveFunction(conPROC_NAME);
			}

		}

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmWorkPackageDetails));
			this.txtWPName = new System.Windows.Forms.TextBox();
			this.txtNumDays = new System.Windows.Forms.TextBox();
			this.cboAssigned = new System.Windows.Forms.ComboBox();
			this.lblWPName = new System.Windows.Forms.Label();
			this.lblNumDays = new System.Windows.Forms.Label();
			this.lblAssignedTo = new System.Windows.Forms.Label();
			this.cmdOK = new System.Windows.Forms.Button();
			this.cmdClose = new System.Windows.Forms.Button();
			this.gpbPackageDetails = new System.Windows.Forms.GroupBox();
			this.gpbPackageDetails.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtWPName
			// 
			this.txtWPName.Location = new System.Drawing.Point(104, 24);
			this.txtWPName.Name = "txtWPName";
			this.txtWPName.Size = new System.Drawing.Size(144, 20);
			this.txtWPName.TabIndex = 1;
			this.txtWPName.Text = "";
			// 
			// txtNumDays
			// 
			this.txtNumDays.Location = new System.Drawing.Point(104, 48);
			this.txtNumDays.Name = "txtNumDays";
			this.txtNumDays.Size = new System.Drawing.Size(144, 20);
			this.txtNumDays.TabIndex = 2;
			this.txtNumDays.Text = "";
			// 
			// cboAssigned
			// 
			this.cboAssigned.Location = new System.Drawing.Point(104, 72);
			this.cboAssigned.Name = "cboAssigned";
			this.cboAssigned.Size = new System.Drawing.Size(144, 21);
			this.cboAssigned.TabIndex = 3;
			// 
			// lblWPName
			// 
			this.lblWPName.Location = new System.Drawing.Point(8, 24);
			this.lblWPName.Name = "lblWPName";
			this.lblWPName.TabIndex = 3;
			this.lblWPName.Text = "WP Name:";
			// 
			// lblNumDays
			// 
			this.lblNumDays.Location = new System.Drawing.Point(8, 48);
			this.lblNumDays.Name = "lblNumDays";
			this.lblNumDays.TabIndex = 4;
			this.lblNumDays.Text = "Num of Days";
			// 
			// lblAssignedTo
			// 
			this.lblAssignedTo.Location = new System.Drawing.Point(8, 72);
			this.lblAssignedTo.Name = "lblAssignedTo";
			this.lblAssignedTo.TabIndex = 5;
			this.lblAssignedTo.Text = "Assigned to:";
			// 
			// cmdOK
			// 
			this.cmdOK.Location = new System.Drawing.Point(64, 120);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.TabIndex = 1;
			this.cmdOK.Text = "OK";
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// cmdClose
			// 
			this.cmdClose.Location = new System.Drawing.Point(144, 120);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.TabIndex = 2;
			this.cmdClose.Text = "Close";
			this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
			// 
			// gpbPackageDetails
			// 
			this.gpbPackageDetails.Controls.Add(this.txtWPName);
			this.gpbPackageDetails.Controls.Add(this.txtNumDays);
			this.gpbPackageDetails.Controls.Add(this.cboAssigned);
			this.gpbPackageDetails.Controls.Add(this.lblWPName);
			this.gpbPackageDetails.Controls.Add(this.lblNumDays);
			this.gpbPackageDetails.Controls.Add(this.lblAssignedTo);
			this.gpbPackageDetails.Location = new System.Drawing.Point(8, 8);
			this.gpbPackageDetails.Name = "gpbPackageDetails";
			this.gpbPackageDetails.Size = new System.Drawing.Size(264, 104);
			this.gpbPackageDetails.TabIndex = 0;
			this.gpbPackageDetails.TabStop = false;
			this.gpbPackageDetails.Text = "Work Package Details";
			// 
			// frmWorkPackageDetails
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(280, 149);
			this.Controls.Add(this.gpbPackageDetails);
			this.Controls.Add(this.cmdClose);
			this.Controls.Add(this.cmdOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "frmWorkPackageDetails";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Work Package";
			this.Load += new System.EventHandler(this.frmWorkPackageDetails_Load);
			this.gpbPackageDetails.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

	

		

	}
}
