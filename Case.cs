using System;
using System.Collections;
using LateBoundNodeLink;
using System.Windows.Forms;

namespace Activity_Logger_05
{
	/// <summary>
	/// Summary description for Case.
	/// </summary>
	public class Case
	{
		/********************************************************************************
		* Case class
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* This class is used to build 'Case' held between NodeLink and AL05 Logger.
		*********************************************************************************/
		#region Constant variables

		private const string mconPROJECT_NAME = "Activity_Logger_05";
		private const string mconCLASS_NAME = "Case";

		#endregion
		
		#region Variable Declaration

		private Person mobjPerson;
		private DateTime mdtmWeekCommencing;
		private int mintSubLink;

		private bool mblnLate;

		private LBNode mobjLBNode;

		private LBNLCore mobjLBNLCore;

		#endregion

		#region Properties

		public string WeekBeginning {
			get { return mdtmWeekCommencing.ToString("dd/MM/yyyy") ; }
		}

		public DateTime WeekCommencing
		{
			get { return mdtmWeekCommencing ; }
		}

		public Person Person
		{
			get{return mobjPerson ; }
		}

		public bool Late {
			get { return mblnLate ; }
		}

		public LBNode LBNode {
			get {
				if(mobjLBNode == null) {
					mobjLBNode = GetNode();
				}
				return mobjLBNode;
			}
		}

		public bool LBNodeLockedStatus {
			get { 
				mobjLBNode = GetNode();
				return mobjLBNode.UnLock(); 
			}
		}

		#endregion

		#region Constructor/s
		/********************************************************************************
		* Constructors
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		*********************************************************************************/

		/// <summary>
		/// This constructor is used for storing 'day' data from table t_dv_almain which in turn
		/// constructs a Case object.
		/// </summary>
		public Case(Person objPerson, DateTime dtmWeekCommencing, bool blnLate, int intSubLink, LBNLCore objLBNLCore)
		{
			mdtmWeekCommencing = dtmWeekCommencing;
			mobjPerson = objPerson;
			mblnLate = blnLate;
			mintSubLink = intSubLink;
			mobjLBNLCore = objLBNLCore;
		}

		#endregion

		#region Public Methods

		public void FollowLink(string strDestination) {

			mobjLBNode.FollowLink(strDestination);
		}

		public bool UnLock() {
			return LBNode.UnLock();
		}

		public bool Lock() {
			return LBNode.Lock();
		}


		#endregion

		#region Private Methods

		private LBNode GetNode() {

			const string conPROC_NAME = "GetNode";
			string strCurrentOpp = "prior to TRY";

			ArrayList arrLBSubCases =null;

			try {
				mobjPerson.Database.Logger.EnterFunction(conPROC_NAME);

				try {
					arrLBSubCases = mobjLBNLCore.GetCase(mintSubLink).GetSubCases("Week Activity");
				}
				catch {

					MessageBox.Show ("Error retrieving LB Sub cases.", mconPROJECT_NAME,
						MessageBoxButtons.OK, MessageBoxIcon.Information);
					//throw new Exception("Error retrieving LB Sub cases");
				}

				try {
					mobjLBNode = ((LBSubCase) arrLBSubCases[arrLBSubCases.Count - 1]).GetLatestNode();
				}
				catch {
					MessageBox.Show ("Error getting LB node (TimePeriod maybe in use by another user).", mconPROJECT_NAME,
						MessageBoxButtons.OK, MessageBoxIcon.Information);
					//throw new Exception("Error getting latest LB node");
				}
				return mobjLBNode;
			}
			catch (Exception objException) {
				mobjPerson.Database.Logger.LogFunctionError(mconCLASS_NAME, conPROC_NAME,  strCurrentOpp, objException.Message);
				throw objException;
			} 
			finally {
				mobjPerson.Database.Logger.LeaveFunction(conPROC_NAME);
			}
		}
		#endregion
	}
}
