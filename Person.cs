using System;

namespace Activity_Logger_05
{
	/// <summary>
	/// Summary description for Person.
	/// </summary>
	public class Person : AL05Base
	{
	/********************************************************************************
	* Person class
	* --------------------------------------------------------------------------------
	* Author: Alun Groome
	* 
	* This class provides functionality to create person objects.
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
		/// This constructor is used to assoicate a person name with a person ID.
		/// </summary>
		public Person(int intID, string strName, Database objDatabase) 
			:base (intID, objDatabase, strName)
		{
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
		* Synopsis: Overides the ToString () method to provide the Persons name.
		*
		*********************************************************************************/
		public override string ToString() {
			return this.Name.Trim();
		}

		#endregion

	}
}
