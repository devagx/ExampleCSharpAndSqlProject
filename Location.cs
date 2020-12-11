using System;

namespace Activity_Logger_05 {
	/// <summary>
	/// Summary description for Location.
	/// </summary>
	public class Location : AL05Base {
		/********************************************************************************
		* Person class
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* This class provides functionality to create Location objects.
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
		/// This constructor is used to assoicate a Location name with a Location ID.
		/// </summary>
		public Location(int intID, string strName, Database objDatabase) 
			:base (intID, objDatabase, strName) {
			this.ID = intID;
			this.Name = strName;
			this.Database = objDatabase;
			
		}
		#endregion

		#region Public Methods

		/********************************************************************************
		* ToString()
		* --------------------------------------------------------------------------------
		* Author: Alun Groome
		* 
		* Synopsis: Overides the ToString () method to provide the Location name.
		*
		*********************************************************************************/
		public override string ToString() {
			return this.Name.Trim();
		}

		#endregion

	}
}
