/*
Name:       Austin Jones
Class:      CSCI 4727-001
Assignment: Homework 1 - Log
File:       hwLog.cpp
*/
#include<string>
#include<iostream>
#include<fstream>
#include<ctime>
#include <climits>
#include "hwLog.h"
using namespace std;
const string currentDateTime();

/*
 Name:      hwLog::hwLog
 Purpose:   Opens a default hwLog in the working directory.
 */
hwLog::hwLog() {
    char cwd[PATH_MAX];
    string path(cwd);
    path.append("/../default_log.txt");
    this->in.close();
    this->out.open(path,ios::app);
}

/*
Name:       hwLog::hwLog
Purpose:    initialize the output stream
Arguments:  fileOut - String path for the file to write to.
*/
hwLog::hwLog(string& fileOut) {
    this->in.close();
    this->file = fileOut;
    this->out.open(fileOut,ios::app);
}

/*
Name:       hwLog::write
Purpose:    Writes a record (complete with timestamp) to the output stream.
Arguments:  in - string to write to stream.
*/
void hwLog::write(string in) {
    if(out.is_open()) {
        this->out << currentDateTime() << endl << in << endl;
    } else {
        cerr << "No file opened";
    }
}

/*
Name:       hwLog::writeLog
Purpose:    Uses the opened files, writes to the output file, and closes the streams
*/
int hwLog::writeLog()
{
    string temp;
    out << currentDateTime()<< endl << "BEGIN" ;    //START LOGGING
    if(in.is_open()) {
        while (getline(in, temp))
            if (out.is_open())
                out << currentDateTime() << endl << temp << endl;
            else
                cerr << "Check the Out file's permissions";
        in.close();
    } else {
        out.close();
        return -1;
    }
    out << currentDateTime() << endl << "END";       //STOP LOGGING
    out.close();
    return 0;
}

/*Vaibhav Patle's method from
        https://stackoverflow.com/questions/997946/how-to-get-current-time-and-date-in-c */
const string currentDateTime() {
    time_t     now = time(0);
    struct tm  tstruct;
    char       buf[80];
    tstruct = *localtime(&now);
    // Visit http://en.cppreference.com/w/cpp/chrono/c/strftime
    // for more information about date/time format
    strftime(buf, sizeof(buf), "%Y-%m-%d.%X", &tstruct);
    return buf;
}
