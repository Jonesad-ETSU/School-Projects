//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project:	    Project 4 - Convention Simulation
//	File Name:		Registrant.cs
//	Description:	Handles the properties of the Registrants.
//	Course:			CSCI 2210-001 - Data Structures
//	Author:			Austin Jones, jonesad@etsu.edu, Logan Fossett, fossettl@etsu.edu
//	Created:		Sunday, November 20, 2018
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Project4_Simulation
{
    /// <summary>
    /// Stores Data for each Registrant.
    /// </summary>
    public class Registrant
    {
        #region Variables
        public static int Count = 0;
        public int LineNum { get; set; } = -1;
        public bool DepartureCalculated { get; set; } = false;
        public DateTime Arrival,
                        Departure;
        public TimeSpan Ts { get; set; }
        public int Id;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for the Registrant. Assigns them an id and gives them an arrival time.
        /// </summary>
        public Registrant()
        {
           Id = Count + 1;
           Arrival = new DateTime(1, 1, 1, Driver.random.Next(Driver.pq.Hours),
            Driver.random.Next(60), Driver.random.Next(60)).AddHours(8);
           Count++;
        }//end Registrant()
        #endregion
        
    }//end Registrant
}
