//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project:	    Project 4 - Convention Simulation
//	File Name:		Driver.cs
//	Description:	Provides User GUI and launches the Rest of the program.
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
    /// Main Entry Point; Menu-based.
    /// </summary>
    public class Driver
    {

        public static PriorityQueue pq;
        public static Random random = new Random();

        /// <summary>
        /// Provides the Main entry point for the program.
        /// </summary>
        protected static void Main(string[] args)
        {
            short choice = -1,
                continues = 1,
                error = 0;


            while (continues == 1)
            {
                while (choice == -1)
                {
                    Tools.Setup();
                    choice = DisplayMenu();
                }//end while(choice)
                switch (choice)
                {
                    case 1:
                        error = NumRegistrants();
                        break;
                    case 2:
                        error = SetHours();
                        break;
                    case 3:
                        error = NumWindows();
                        break;
                    case 4:
                        error = SetCheckout();
                        break;
                    case 5:
                        error = Run();
                        break;
                    case 6:
                        Environment.Exit(0);
                        break;
                    default:
                        error = 1;
                        break;
                }//end switch(choice)
                if (error == 1)
                    Console.WriteLine("The Operation returned an error.");
                Tools.PressAny();
                choice = -1;
            }//end while(continues)
        }//end Main(String[])

        #region DisplayMenu
        /// <summary>
        /// Displays the Menu Prompt for the User.
        /// </summary>
        /// <returns>Returns a short value with an error code</returns>
        private static short DisplayMenu()
        {
            Tools.ChangeConsoleColor(ConsoleColor.Red);
            Console.Write("\t-----------------------------");
            Tools.ChangeConsoleColor(ConsoleColor.Cyan);
            Console.Write("\n\tWhat would you like to do? ");
            Console.Write("\n\t1. ");
            Tools.ChangeConsoleColor(ConsoleColor.Green);
            Console.Write("Set Number of Registrants");
            Tools.ChangeConsoleColor(ConsoleColor.Cyan);
            Console.Write("\n\t2. ");
            Tools.ChangeConsoleColor(ConsoleColor.Green);
            Console.Write("Set Number Of Open Hours");
            Tools.ChangeConsoleColor(ConsoleColor.Cyan);
            Console.Write("\n\t3. ");
            Tools.ChangeConsoleColor(ConsoleColor.Green);
            Console.Write("Set the Number of Windows");
            Tools.ChangeConsoleColor(ConsoleColor.Cyan);
            Console.Write("\n\t4. ");
            Tools.ChangeConsoleColor(ConsoleColor.Green);
            Console.Write("Set the Expected Service Time");
            Tools.ChangeConsoleColor(ConsoleColor.Cyan);
            Console.Write("\n\t5. ");
            Tools.ChangeConsoleColor(ConsoleColor.Green);
            Console.Write("Run the Simulation");
            Tools.ChangeConsoleColor(ConsoleColor.Cyan);
            Console.Write("\n\t6. ");
            Tools.ChangeConsoleColor(ConsoleColor.Green);
            Console.WriteLine("Exit");
            String strChoice = Console.ReadLine();
            short choice = 0;

            try
            {
                choice = Convert.ToInt16(strChoice);
                return choice;
            }//end try
            catch (Exception e)
            {
                Console.WriteLine("Invalid Option Selected.");
                Tools.PressAny();
                return -1;
            }//end catch(Exception)
        }//end DisplayMenu()
        #endregion

        #region MenuOptions
        /// <summary>
        /// Sets the number of Registrants
        /// </summary>
        /// <returns>Error Code if necessary</returns>
        private static byte NumRegistrants()
        {
            Console.WriteLine("How many people will register?");
            String answer = Console.ReadLine();
            try
            {
                int Answer = (Convert.ToInt32(answer));
                pq = new PriorityQueue(Answer);
                pq.Registrants = (Answer);
                return 0;
            }//end try
            catch (Exception e)
            {
                return 1;
            }
        }//end NumRegistrants

        /// <summary>
        /// Sets the number of Open Hours
        /// </summary>
        /// <returns>Error Code if necessary</returns>
        private static byte SetHours()
        {
            Console.WriteLine("How many Hours?");
            String answer = Console.ReadLine();
            try
            {
                pq.Hours = (Convert.ToInt32(answer));
                ConventionRegistration.AddHours(Convert.ToInt32(answer));
                return 0;
            }//end try
            catch (Exception e)
            {
                return 1;
            }
        }//end SetHours()

        /// <summary>
        /// Sets the number of Windows Available
        /// </summary>
        /// <returns>Error Code if necessary</returns>
        private static byte NumWindows()
        {
            Console.WriteLine("How many Windows?");
            String answer = Console.ReadLine();
            try
            {
                pq.Lines = new List<Queue<Registrant>>(Convert.ToInt16(answer));
                for (int i = 0; i < pq.Lines.Capacity; i++)
                {
                    pq.Lines.Add(new Queue<Registrant>());
                }//end for(int)
                return 0;
            }//end try
            catch (Exception e)
            {
                return 1;
            }
        
        }//end NumWindows()

        /// <summary>
        /// Sets the Expected Checkout Duration
        /// </summary>
        /// <returns>Error Code if necessary</returns>
        private static byte SetCheckout()
        {
            Console.WriteLine("What is the Expected Service Time?");
            String answer = Console.ReadLine();
            try
            {
                pq.Expected = (Convert.ToDouble(answer));
                return 0;
            }//end try
            catch (Exception e)
            {
                return 1;
            }
        }//end SetCheckout()

        /// <summary>
        /// Runs the Simulation
        /// </summary>
        /// <returns>Error Code if necessary</returns>
        private static byte Run()
        {
            //try
            //{
                ConventionRegistration.RunSimulation();
                return 0;
            /*}//end try
            catch (Exception e)
            {
                return 1;
            }//end catch(Exception) */
        }//end Run()
        #endregion

    }//end Driver
}
