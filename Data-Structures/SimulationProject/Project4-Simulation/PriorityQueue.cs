//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project:	    Project 4 - Convention Simulation
//	File Name:		PriorityQueue.cs
//	Description:	Creates a Priority Queue implementation by using a Max Heap.
//	Course:			CSCI 2210-001 - Data Structures
//	Author:			Austin Jones, jonesad@etsu.edu, Logan Fossett, fossettl@etsu.edu
//	Created:		Sunday, November 20, 2018
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace Project4_Simulation
{
    /// <summary>
    /// Handles the Max Heap Priority Queue implementation.
    /// </summary>
    public class PriorityQueue
    {
        #region Variables
        public int Hours { get; set; }
        public int Registrants { get; set; }
        public double Expected { get; set; }

        public List<Event> Data;
        public List<Queue<Registrant>> Lines = new List<Queue<Registrant>>();
        public int HeapSize { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new PriorityQueue Object.
        /// </summary>
        /// <param name="size">Gives a new initial size the Event List</param>
        public PriorityQueue(int size = 0)
        {
            Data = new List<Event>(size);
        }//end PriorityQueue
        #endregion

        #region FindNodes
        /// <summary>
        /// Gets the top Node of The Priority Queue
        /// </summary>
        /// <returns>Returns the top node of the PQ</returns>>
        public Event GetMinimum()
        {
            Event top;
            if (IsEmpty())
                throw new Exception("Heap is empty");
            else
            {
                top = Data[0];
                RemoveMin();
                return top;
            }//end else
        }//end GetMinimum

        /// <summary>
        /// Find the index of the Current Node's left child
        /// </summary>
        /// <param name="Parent">Index of Parent</param>
        /// <returns>Index of Left Child</returns>
        public int FindLeftChildIndex(int Parent)
        {
            return 2 * Parent + 1;
        }//end FindLeftChild(int)

        /// <summary>
        /// Find the index of the Current Node's Right child
        /// </summary>
        /// <param name="Parent">Index of Parent</param>
        /// <returns>Index of Right Child</returns>
        public int FindRightChildIndex(int Parent)
        {
            return 2 * Parent + 2;
        }//end FindLeftChild(int)

        /// <summary>
        /// Find the index of the Current Node's Parent
        /// </summary>
        /// <param name="child">Index of Child</param>
        /// <returns>Index of Parent</returns>
        public int FindParentIndex(int child)
        {
            return (child - 1) / 2;
        }//end FindParent(int)
        #endregion

        #region Adding to PriorityQueue
        /// <summary>
        /// Adds a new Event to The Priority Queue
        /// </summary>
        /// <param name="value">The Event</param>
        public void Insert(Event value)
        {
            /*if (HeapSize == Data.Count)
                throw new Exception("Heap Storage is overflow");
            else
            {*/
                HeapSize++;
                Data.Add(value);
                SiftUp(HeapSize - 1);
            //}//end else
        }//end Insert(Event)

        /// <summary>
        /// sorts the Queue after the Insertion
        /// </summary>
        /// <param name="nodeIndex">Index of current Node to be sorted</param>
        private void SiftUp(int nodeIndex)
        {
            int parentIndex;
            Event tmp;
            if (nodeIndex != 0)
            {
                parentIndex = FindParentIndex(nodeIndex);
                if (Data[parentIndex] > Data[nodeIndex])
                {
                    tmp = Data[parentIndex];
                    Data[parentIndex] = Data[nodeIndex];
                    Data[nodeIndex] = tmp;
                    SiftUp(parentIndex);
                }//end if
            }//end if(nodeIndex)
        }//end SiftUp(int)
        #endregion

        #region Removing from PriorityQueue
        /// <summary>
        /// Removes the Data[0] Node
        /// </summary>
        public void RemoveMin()
        {
            if (IsEmpty())
                throw new Exception("Heap is empty.");
            else
            {
                Data[0] = Data[HeapSize - 1];
                HeapSize--;
                if (HeapSize > 0)
                    SiftDown(0);
            }//end else
        }//end RemoveMin

        /// <summary>
        /// Sorts the current Heap after Node Removal
        /// </summary>
        /// <param name="nodeIndex">Index of Current Node</param>
        private void SiftDown(int nodeIndex)
        {
            Event tmp;
            int leftChildIndex,
                rightChildIndex,
                minIndex;

            leftChildIndex = FindLeftChildIndex(nodeIndex);
            rightChildIndex = FindRightChildIndex(nodeIndex);

            if (rightChildIndex >= HeapSize)
            {
                if (leftChildIndex >= HeapSize)
                    return;
                else
                    minIndex = leftChildIndex;
            }//end if(rightChildIndex)
            else
            {
                if (Data[leftChildIndex] <= Data[rightChildIndex])
                    minIndex = leftChildIndex;
                else
                    minIndex = rightChildIndex;
            }//end else
            if (Data[nodeIndex] > Data[minIndex])
            {
                tmp = Data[minIndex];
                Data[minIndex] = Data[nodeIndex];
                Data[nodeIndex] = tmp;
                SiftDown(minIndex);
            }//end if(data[nodeIndex])
        }//end SiftDown(int)
        #endregion

        #region Line Manipulation
        /// <summary>
        /// Finds the Line with the Fewest people.
        /// </summary>
        /// <returns>Returns the Smallest Line's Number</returns>
        public int FindMinLine()
        {
            int MinLine = 0;
            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i].Count < Lines[MinLine].Count)
                {
                    MinLine = i;
                }//if (Lines[i])
            }//end for(i)
            return MinLine;
        }//end FindMinLine()

        /// <summary>
        /// Adds a Registrant to the shortest Line.
        /// </summary>
        /// <param name="r">The Registrant to be added</param>
        public void AddToLine(Registrant r)
        {
            if (Lines.Count == 0)
                return;
            int Line = FindMinLine();
            r.LineNum = Line;
            Lines[Line].Enqueue(r);
        }//end AddToLine(Registrant)

        /// <summary>
        /// Removes a Registrant from a Line
        /// </summary>
        /// <param name="lineNumber">The Line to be "popped".</param>
        public void RemoveFromLine(int lineNumber)
        {
            Lines[lineNumber].Dequeue();
        }//end RemoveFromLine(Registrant)
        #endregion

        #region IsEmpty
        /// <summary>
        /// Finds if the current PriorityQueue is empty
        /// </summary>
        /// <returns>Returns a boolean representing if the PQ is empty.</returns>
        public bool IsEmpty()
        {
            return Data.Count == 0;
        }//end IsEmpty()
        #endregion

        #region Operators
        /// <summary>
        /// Adds an event to the PriorityQueue.
        /// </summary>
        /// <param name="pq">The Priority Queue to add the event</param>
        /// <param name="e">The Event</param>
        /// <returns>Returns a PriortyQueue with the Event added.</returns>
        public static PriorityQueue operator+ (PriorityQueue pq, Event e)
        {
            pq.Insert(e);
            return pq;
        }//end operator+(PriorityQueue, Event)
        #endregion
    }//end PriorityQueue
}