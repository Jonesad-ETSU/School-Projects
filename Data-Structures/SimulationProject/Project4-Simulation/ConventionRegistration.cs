//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project:	    Project 4 - Convention Simulation
//	File Name:		ConventionRegistration.cs
//	Description:	Manages the Simulation and its processes.
//	Course:			CSCI 2210-001 - Data Structures
//	Author:			Austin Jones, jonesad@etsu.edu, Logan Fossett, fossettl@etsu.edu
//	Created:		Sunday, November 20, 2018
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Project4_Simulation
{
    /// <summary>
    /// Manages the Simulation Processes and its outputs.
    /// </summary>
    public class ConventionRegistration
    {
        #region Variables
        public static DateTime Timer { get; set; } = new DateTime();
        public static DateTime EndTime { get; set; } = new DateTime().AddHours(8);
        private static bool blnNoMoreEntrants = false;
        private static int attendeeCounter = 0;
        private static int Counter = 10;
        #endregion

        #region RunSimulation
        /// <summary>
        /// Runs the Simulation Procedure
        /// </summary>
        public static void RunSimulation()
        {
            byte continues = 1;
            List<Registrant> Registrants = new List<Registrant>();
            Timer = new DateTime().AddHours(8);
            int tickrate = 10; //Controls the length of time for each update. Effectively changing the speed of emulation.
            int secondsPerTick = 1; //Controls the amount of seconds that pass in a single run of the while loop.

            for (int i = 0; i < Tools.Poisson(Driver.pq.Registrants); i++)
            {
                Registrant r = new Registrant();
                Registrants.Add(r);

            }//end for(i)

            while (continues == 1)
            {
                Thread.Sleep(tickrate);
                Timer = Timer.AddSeconds(secondsPerTick);

                foreach (Registrant r in Registrants)
                {
                    if (r.Arrival.Hour == Timer.Hour && r.Arrival.Minute == Timer.Minute
                        && r.Arrival.Second == Timer.Second)
                    {
                        Driver.pq.AddToLine(r);
                        new Event("enter", r.Arrival, r.Id);
                        attendeeCounter++;
                    }//end if(r.Arrival.Hour)
                }//end foreach(Registrant)

                for (int i = 0; i < Driver.pq.Lines.Count; i++)
                {
                    if (Driver.pq.Lines[i].Count != 0)
                    {
                        Registrant tmp = Driver.pq.Lines[i].Peek();
                        if (!tmp.DepartureCalculated)
                        {
                            double dTmp = Tools.NegExp(Driver.pq.Expected);
                            if (dTmp < 1.5)
                                dTmp = 1.5;
                            tmp.Departure = Timer.AddMinutes(dTmp);
                            tmp.DepartureCalculated = true;
                        }//end if(!DepartureCalculated)
                    }//end if(Driver.pq.Lines[i].Count)
                }//end for(i)

                for (int i = 0; i < Registrants.Count; i++)
                {
                    if (Registrants[i].Departure.Hour <= Timer.Hour && Registrants[i].Departure.Minute <= Timer.Minute
                        && Registrants[i].Departure.Second <= Timer.Second && Registrants[i].Departure != DateTime.MinValue)
                    {
                        new Event("leave", Registrants[i].Departure, Registrants[i].Id);
                        Driver.pq.RemoveFromLine(Registrants[i].LineNum);
                        Registrants.Remove(Registrants[i]);
                    }//end if(Registrants[i])
                }//end foreach(Registrant)

                if (EndTime.Hour == Timer.Hour)
                    blnNoMoreEntrants = true;

                if (blnNoMoreEntrants)
                {
                    continues = 0;
                    foreach (Queue<Registrant> regs in Driver.pq.Lines)
                    {
                        if (regs.Count != 0)
                            continues = 1;
                    }//end foreach(Queue<Registrant>)
                }//if(blnNoMoreEntrants)

                Counter--;
                if (Counter <= 0)
                {
                    DisplayLines();
                    Counter = 10;
                }
            }//end while(continues == 1)
            DisplayLines();

            foreach (Event e in Driver.pq.Data)
                Console.WriteLine($"\t{e}");

            Clear();
        }//end RunSimulation()
        #endregion

        #region AddHours
        /// <summary>
        /// Adds the Number of Hours to the EndTimer
        /// </summary>
        /// <param name="hours">The Number of Hours to be added</param>
        public static void AddHours(int hours)
        {
            EndTime = EndTime.AddHours(hours);
        }//end AddHours(int)
        #endregion

        #region Clear
        /// <summary>
        /// Clears all the data for the Priority Queue and Simulation.
        /// </summary>
        private static void Clear()
        {
            Timer = new DateTime();
            EndTime = new DateTime();
            blnNoMoreEntrants = false;
            Driver.pq = new PriorityQueue();
            attendeeCounter = 0;
            Registrant.Count = 0;

        }//end Clear()
        #endregion

        #region DisplayLines
        /// <summary>
        /// Displays the Registrants and the Windows.
        /// </summary>
        private static void DisplayLines()
        {
            Registrant[] reg = null;

            Tools.Setup();

            Tools.ChangeConsoleColor(ConsoleColor.Red);
            Console.WriteLine("\n\t-----------------------------------------------------------------------------------------------------------");

            for (int i = 0; i < Driver.pq.Lines.Count; i++)
            {
                reg = Driver.pq.Lines[i].ToArray<Registrant>();

                Tools.ChangeConsoleColor(ConsoleColor.Cyan);
                Console.Write($"\tWindow {i + 1}: ");
                Tools.ChangeConsoleColor();
                for (int j = 0; j < reg.Length; j++)
                    Console.Write($" {reg[j].Id} ");

                Console.WriteLine();
            }//end for(int)

            Tools.ChangeConsoleColor(ConsoleColor.Yellow);
            Console.Write($"\n\tNumber of Events: ");
            Tools.ChangeConsoleColor(ConsoleColor.Magenta);
            Console.Write( Driver.pq.HeapSize);
            Tools.ChangeConsoleColor(ConsoleColor.Yellow);
            Console.Write($"\tNumber of Patrons: ");
            Tools.ChangeConsoleColor(ConsoleColor.Magenta);
            Console.Write(attendeeCounter);
            Tools.ChangeConsoleColor(ConsoleColor.Red);
            Tools.ChangeConsoleColor(ConsoleColor.Yellow);
            Console.Write($"\tTime: ");
            Tools.ChangeConsoleColor(ConsoleColor.Magenta);
            Console.Write($"{Timer.ToLongTimeString()}");
            Console.WriteLine("\n\t-----------------------------------------------------------------------------------------------------------");
        }
        #endregion

    }//end ConventionRegistration
}

