//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project:	    Project 4 - Convention Simulation
//	File Name:		Event.cs
//	Description:	Handles the Event Object to be stored in the Priority Queue.
//	Course:			CSCI 2210-001 - Data Structures
//	Author:			Austin Jones, jonesad@etsu.edu, Logan Fossett, fossettl@etsu.edu
//	Created:		Sunday, November 20, 2018
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace Project4_Simulation
{
  
    enum Type { ENTER, LEAVE };

    /// <summary>
    /// Stores Event Data.
    /// </summary>
    public class Event : IComparable
    {
        #region Variables
        private Type EventType { get; set; }
        public DateTime Time { get; set; }
        public int Patron { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new Event Object.
        /// </summary>
        /// <param name="eventType">Type of Event</param>
        /// <param name="time">Time of the Event</param>
        /// <param name="patron">ID of Registrant</param>
        public Event(String eventType, DateTime time, int patron)
        {
            switch (eventType)
            {
                case "Enter":
                case "enter":
                case "ENTER":
                    EventType = Type.ENTER;
                    Driver.pq += this;
                    break;

                case "Leave":
                case "leave":
                case "LEAVE":
                    EventType = Type.LEAVE;
                    Driver.pq += this;
                    break;

                default:
                    throw new Exception("Event Type Invalid.");
            }//end switch(eventType)
           Time = time;
           Patron = patron;
        }//end Event(String,DateTime,int)
        #endregion

        #region Operators
        /// <summary>
        /// Determines if e1 is greater than or equal to e2
        /// </summary>
        /// <param name="e1">Event 1</param>
        /// <param name="e2">Event 2</param>
        /// <returns>Returns true if e1 >= e2 according to CompareTo.</returns>
        public static bool operator >= (Event e1, Event e2)
        {
            return e1.CompareTo(e2) >= 0;
        }//end operator>=(Event, Event)

        /// <summary>
        /// Determines if e1 is less than or equal to e2
        /// </summary>
        /// <param name="e1">Event 1</param>
        /// <param name="e2">Event 2</param>
        /// <returns>Returns true if e1 <= e2 according to CompareTo.</returns>
        public static bool operator <= (Event e1, Event e2)
        {
            return e1.CompareTo(e2) <= 0;
        }//end operator<=(Event, Event)

        /// <summary>
        /// Determines if e1 is greater than e2
        /// </summary>
        /// <param name="e1">Event 1</param>
        /// <param name="e2">Event 2</param>
        /// <returns>Returns true if e1 > e2 according to CompareTo.</returns>
        public static bool operator > (Event e1, Event e2)
        {
            return e1.CompareTo(e2) > 0;
        }//end operator>=(Event, Event)

        /// <summary>
        /// Determines if e1 is less than e2
        /// </summary>
        /// <param name="e1">Event 1</param>
        /// <param name="e2">Event 2</param>
        /// <returns>Returns true if e1 >= e2 according to CompareTo.</returns>
        public static bool operator < (Event e1, Event e2)
        {
            return e1.CompareTo(e2) < 0;
        }//end operator<=(Event, Event)
        #endregion

        #region ToString
        /// <summary>
        /// Makes a String Representation of the Event.
        /// </summary>
        /// <returns>Returns a String Representation of an Event.</returns>
        public override string ToString()
        {
           String str = "";

           str += String.Format("Patron {0} ", Patron.ToString().PadLeft(3));
           str += EventType + "'s";
           str += String.Format(" at {0}", Time.ToShortTimeString().PadLeft(8));

           return str;
        }
        #endregion

        #region CompareTo
        /// <summary>
        /// Uses the DateTime CompareTo method to compare the time of two Events
        /// </summary>
        /// <param name="obj">The Object to be compared with.</param>
        /// <returns>An integer representation of the two's difference.</returns>
        public int CompareTo(Object obj)
        {
           if (!(obj is Event))
              throw new ArgumentException("The argument is not an Event object."); 
    
           Event e = (Event)obj;
           return (e.Time.CompareTo(Time));
        }//end CompareTo
        #endregion
    }//end Event
}