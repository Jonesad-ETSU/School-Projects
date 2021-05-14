//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project:	    Project 4 - Convention Simulation
//	File Name:		Tools.cs
//	Description:	Makes various general purpose tools which may be used for the rest of the semester.
//	Course:			CSCI 2210-001 - Data Structures
//	Author:			Austin Jones, jonesad@etsu.edu, Logan Fossett, fossettl@etsu.edu
//	Created:		Sunday, November 20, 2018
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Project4_Simulation
{
    /// <summary>
    /// Provides General Purpose Tools for use in future assignments.
    /// </summary>
    public class Tools
    {

        private static readonly Random R = new Random();

        #region promptUser
        /// <summary>
        /// Prompts the user and sends the result back
        /// </summary>
        /// <param name="message">Message to be shown to the user</param>
        /// <param name="type">The type to convert the answer to</parm>
        /// <return>Returns the answer of the prompt</return>
        public static String promptUser(String message, String type = "String")
        {
            Console.WriteLine(message);
            String answer = Console.ReadLine();
            if (type == "int" || type == "Int")
            {
                Convert.ToInt32(answer);
            }//end if(type)
            if (type == "double")
            {
                Convert.ToDouble(answer);
            }//end if(type)
            if (type == "String" || type == "string")
            { }//end if(type)
            else
            {
                throw new Exception("That is not an acceptable type -promptUser method");
            }//end else
            return answer;
        }//end promptUser(String)
        #endregion

        #region Setup
        /// <summary>
        /// Provides a general purpose tool to set up the console window.
        /// </summary>
        /// <param name="bg">Specifies the color to be set as background</param>
        /// <param name="fg">Specifies the color to be set as foreground</param>

        public static void Setup(ConsoleColor bg = ConsoleColor.Black, ConsoleColor fg = ConsoleColor.White)
        {
            DrawLine('-');
            Console.Clear();
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
            DrawLine('-');
        }//end Setup()
        #endregion

        #region ConsoleColor
        ///<summary>
        ///Changes console colors
        ///</summary>
        ///<param name="fg">Changes the foreground color</param>
        ///<param name="bg">Changed the background color</param>

        public static void ChangeConsoleColor(ConsoleColor fg = System.ConsoleColor.White, ConsoleColor bg = System.ConsoleColor.Black)
        {
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
        }//end ConsoleColor(ConsoleColor, ConsoleColor)

        #endregion

        #region CleanString
        /// <summary>
        /// Makes a method to clean a string- replacing certain characters for easier workability.
        /// </summary>
        /// <param name="work">A prewritten String to be converted</param>
        /// <param name="delims">Determines the delims which CleanString will use</param>
        /// <returns>Returns the cleaned String</returns>

        public static string CleanString(ref string work, string delims = " _")
        {
            work = work.Trim(delims.ToArray());
            work = work.Replace(" ", "");
            return (work.Replace("\r\n", "\n"));
        }//end CleanString(ref string)
        #endregion

        #region Parse
        /// <summary>
        /// Parses a String and returns every word in the String in a List.
        /// </summary>
        /// <param name="original">The String to be converted into a List.</param>
        /// <param name="delimiters">The String of delims to be parsed.</param>
        /// <returns>parsedString returns the finalized parsedString.</return>

        public static List<String> Parse(string original, string delimiters = " \n")
        {
            String temp = CleanString(ref original);                       //Gives the base string being manipulated to extract the List.
            List<String> parsedStrings = new List<string>(5);
            bool blnContinues = true;
            int pos = 0,
                length = temp.Length;

            while (blnContinues)
            {

                temp = temp.Trim(" _".ToCharArray());

                if (temp.IndexOfAny(delimiters.ToCharArray()) != -1)
                {
                    pos = temp.IndexOfAny(delimiters.ToCharArray());           //Gets index of next delimiter
                }//end if(temp.IndexOfAny(char[]))
                else
                    pos = temp.Length - 1;                                       //If no more delimiters, get length as default return.

                pos++;                                                     //Adds 1 to the pos, preventing an exception.

                if (!(pos == temp.Length))
                {
                    parsedStrings.Add(temp.Substring(0, pos - 1));             //adds a substring upto and not including the delimiter to parsedStrings[]
                }//end if(pos)
                else
                {
                    parsedStrings.Add(temp.Substring(0, pos));                 //adds a substring upto and including the last character to parsedStrings[]
                }//end else

                temp = (temp.Substring(pos, temp.Length - pos));               //removes the previously used Strings from the original string.
                if (temp.Length <= 0)
                {
                    blnContinues = false;
                }//end if(temp.Length)
            }//end while(blnContinues)

            for (int i = 0; i < parsedStrings.Count(); i++)
            {
                if (parsedStrings[i].Contains("\n"))
                {
                    parsedStrings[i] = parsedStrings[i].Replace("\n", "");
                }//end if(parsedStrings[i])
            }//end for()
            return parsedStrings;
        }//end Parse(String, Strings)
        #endregion

        #region Format
        /// <summary>
        /// Formats a List<String> object and converts it into a String.
        /// </summary>
        /// <param name="list">The LIst to be formatted and displayed</param>
        /// <param name="leftMargin">The number of spaces from the left that the string beings</param>
        /// <param name="rightMargin">The number of spaces from the left that the string ends</param>
        /// <returns>Returns the formattedString</returns>

        public static String Format(List<string> list, int leftMargin = 5, int rightMargin = 75)
        {
            if (rightMargin < 0 || rightMargin <= leftMargin)               //Error checks values of margins
            {
                Console.WriteLine("Left: " + leftMargin + "\nRight: " + rightMargin);
                PressAny();
                throw new Exception("The Right Margin must be greater than both 0 and the Left Margin!");
            }
            String formattedString = "",
                   leftString = "";
            int availableSpaces = rightMargin - leftMargin;

            for (int i = 0; i < leftMargin; i++)
            {
                leftString += " ";
            }//end for(i)

            formattedString = leftString;
            for (int i = 0; i < list.Count; i++)
            {
                availableSpaces -= list[i].Length;
                if (availableSpaces <= 0)
                {
                    formattedString += "\n" + leftString + list[i];
                    availableSpaces = rightMargin - leftMargin - list[i].Length;
                }//end if(availableSpaces)
                else
                {
                    formattedString += " " + list[i];
                }//end else
            }//end for(i)

            return formattedString;
        }//end Format(List<String>,int,int)
        #endregion

        #region Sort 

        /// <summary>
        /// Sorts a List of Generic Objects.
        /// </summary>
        /// <param name="list">The LIst to be formatted and displayed</param>
        /// <param name="sortType">The Number that represents which sorting type is used.</param>
        /// <returns>Returns the SortedList</returns>
        public static List<T> Sort<T>(List<T> list, int sortType = 0) where T : IComparable<T>
        {
            if (sortType == 0) //Bubble Sort
            {
                return BubbleSort(list);
            }
            else
                throw new Exception("Sort Type not yet implemented.");
        }//end Sort<T>(List<T>)

        /// <summary>
        /// Performs a BubbleSort on a list of Objects
        /// </summary>
        /// <param name="list">The List to be Sorted</param>
        /// <returns>Returns the BubbleSorted List</returns>
        public static List<T> BubbleSort<T>(List<T> list) where T : IComparable<T>
        {
            for (int i = 1; i <= list.Count; i++)
            {
                for (int j = 0; j < (list.Count - 1); j++)
                {
                    if (list[j + 1].CompareTo(list[j]) < 0)
                    {
                        T temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }//end if(list[j])
                }//end for(j)
            }//end for(i)
            return list;
        }//end BubbleSort<T>(List<T>)
        #endregion

        #region DrawLine
        /// <summary>
        /// Returns a line of count c's.
        /// </summary>
        /// <param name="c">The character to be drawn</param>
        /// <param name="count">The number of character c to be drawn</param>
        /// <returns>Returns a Line of count c's</returns>>

        public static String DrawLine(char c, int count = 15)
        {
            string line = "\n";
            for (int i = 0; i <= count; i++)
            {
                line += c;
            }
            line += "\n";
            return line;
        }//end DrawLine(char)
        #endregion

        #region PressAnyKey
        /// <summary>
        /// Provides a method to stop execution until the user inputs a key.
        /// </summary>
        public static void PressAny()
        {
            Console.WriteLine("Press any key to continue. . . .");
            Console.ReadKey();
        }//end pressAny()
        #endregion

        #region NegativeExponential
        /// <summary>
        /// Returns a value based on the Expected Value and the Negative Exponentional.
        /// </summary>
        /// <param name="ExpectedValue">Holds the Expected Value</param>
        /// <returns>Returns A Value influenced by both the Expected and Exponentional.</returns>>
        public static double NegExp(double ExpectedValue)
        {
            return -ExpectedValue * Math.Log(R.NextDouble(), Math.E);
        }//end NegExp(double)
        #endregion

        #region Poisson
        /// <summary>
        /// Returns a value based on the Expected Value and the Poisson Generation.
        /// </summary>
        /// <param name="ExpectedValue">Holds the Expected Value</param>
        /// <returns>Returns A Value influenced by both the Expected and Poisson.</returns>>
        public static int Poisson(double ExpectedValue)
        {
            double dLimit = -ExpectedValue;
            double dSum = Math.Log(R.NextDouble( ));

            int Count;
            for (Count = 0; dSum > dLimit; Count++)
                dSum += Math.Log(R.NextDouble( ));
            return Count;
        }//end Poisson(double)
        #endregion

    }//end Tools
}
