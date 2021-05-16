/*
Name:       Austin Jones
Class:      CSCI 4727-001
Assignment: Homework 1 - Log
File:       hwLog.h
*/
#ifndef HWLOG_H
#define HWLOG_H
#include<iostream>
#include<fstream>
#include<string>
using namespace std;

class hwLog {
private:
    ifstream in;
    ofstream out;
public:
    string file;
    hwLog();
    hwLog(string&);
    void write(string);
    int writeLog();
};
#endif
