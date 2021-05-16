/*
Name:       Austin Jones
Class:      CSCI 4727-001
Assignment: Homework 6 - Threads
File:       hw6.cpp
*/
#include <csignal>
#include <unistd.h>
#include <string>
#include <climits>
#include <sstream>
#include <vector>
#include <algorithm>
#include <sys/wait.h>
#include <pthread.h>
#include <semaphore.h>
#include "Knight.h"
#include "hwLog.h"
#include "Rabbit.h"
#include "SafeQueue.cpp"

using namespace std;
/*
Name: 		message
Purpose: 	static size of message to send across pipe.
*/
struct message {
    int from;   //0 = Rabbit; 1+ = Knight
    int type;   //0 = knight; 1 = bite; 2 = quick; 3 = throat; 4+ => specific strings
    int damage; //-1 = dead;
};

static void run_log(string& _file);
static void * run_rabbit(void* _r);
static void * run_knight(void* _k);
struct run_knight_parms {
public:
    Knight &_k;
    int _id;
};
static vector<Knight> create_army(string& _file);
static Rabbit create_rabbit(string& _file);
static Knight create_knight(vector<string> _keys, vector<string> _values);

static int log_pipe[2];
static int log_pid;
static vector<string> knight_names;
static vector<SafeQueue<message>> Queues;

/*
Name:       exit_handler
Purpose:    exits with appropriate code.
*/
void exit_handler(int signum) {
    exit(143);
}

/*
Name:       main
Purpose:    Entry-point for the program.
Arguments:  argc, argv - command line arguments.
*/
int main(int argc, char** argv) {
    /*Set up Log and Rabbit pipes.*/
    pipe(log_pipe);
    auto Usage = []() {
        fprintf(stdout, "USAGE: HW6 (-r rabbitFile -REQUIRED) [-k knightsFile] [-l logFile]");
        fflush(stdout);
        exit(-1);
    };

    /*Set up base objects.*/
    char cwd[PATH_MAX];
    getcwd(cwd, PATH_MAX);
    string noFile = string(cwd) + "/";
    string files[3] = {string(cwd) + "/",
                       string(cwd) + "/",
                       string(cwd) + "/"
    };   //0 - Knight; 1 - rabbit; 2 - log
    /*CLI arguments*/
    if (argc % 2 != 0) { //If each cli flag has a value,
        for (int i = 1; i < argc; i += 2) {
            if (string(argv[i]).front() == '-' && (string(argv[i + 1]).front() == '-' || i == argc - 1))
                Usage();
            else if (string(argv[i]) == "-r")
                files[1] += (string(argv[i + 1]));
            else if (string(argv[i]) == "-k")
                files[0] += string(argv[i + 1]);
            else if (string(argv[i]) == "-l")
                files[2] += (string(argv[i + 1]));
            else
                Usage();
        }//end for(i<argc)
    }

    if (files[1] == noFile) { Usage(); }
    Rabbit r = create_rabbit(files[1]);
    if (files[0] == noFile) { files[0] += "default"; }
    vector<Knight> army = create_army(files[0]);

    /*Create Log*/
    if (files[2].compare(noFile) == 0) { files[2].append("log.txt"); }
    /*Creating log proc*/
    int pid = fork();
    if (pid == -1)
        exit(-1);
    else if (pid == 0) {
        run_log(files[2]);
        exit(1);
    } else {
        log_pid = pid;
    }

    pthread_t tid;
    vector<pthread_t> knight_tids;
    vector<run_knight_parms> parms;

    int id;
    for (id = 1; id< army.size();++id)
        run_knight_parms rkp = {army[id], id};

    SafeQueue<message> temp;
    for (int i = 0; i <= parms.size() + 1; ++i)  //Pushes back parms knights and 1 for rabbit.
        Queues.push_back(temp);

    for (auto & parm : parms) {
        pthread_create(&tid, NULL, run_knight, (void *) &parm);
        knight_tids.push_back(tid);
    }
    pthread_create(&tid, NULL, run_rabbit, (void *) &r);

    pthread_join(tid, nullptr);    //Waits on Rabbit
    for (auto id:knight_tids)
        pthread_join(id, nullptr);  //Waits on the Knights

    message END = {-1, 11, -1};
    write(log_pipe[1], (char *) &END, sizeof(message));
    wait(&log_pid);                  //Wait for the log to close to close the program.
    exit(1);
}

/*
Name:       run_log
Purpose:    Runs the log process, continually reading struct messages and writing to the log file.
Arguments:  _file: string path of the file to open
*/
static void run_log(string& _file) {
    message m;
    string Message;

    /*Messages in array*/
    vector<string> Messages = {
            "Sir _name tried to hit the Rabbit. . . ",                                                  //0
            ". . . but it failed",                                                                      //1
            ". . . and was successful for _dmg",                                                        //2
            "Sir _name KILLED the rabbit!",                                                             //3
            "Sir _name RUNS away",                                                                      //4
            "Sir _name was BIT by the Rabbit for _dmg",                                                 //5
            "Every Knight was hit by Rabbit's QUICK attack for _dmg",                                   //6
            "Sir _name's THROAT was mauled by the Rabbit for _dmg",                                     //7
            "Sir _name was KILLED by Rabbit's _type Attack.",                                           //8
            "The Knights won!",                                                                         //9
            "The Rabbit won!",                                                                          //10
            "END",                                                                                      //11
            "Rabbit: Could not read file. Unknown Key",                                                 //12
            "Knight: Could not read file. Unknown Key",                                                 //13
            "Rabbit: Could not read file. Value Out of Bounds",                                         //14
            "Knight: Could not read file. Value Out of Bounds",                                         //15
            "Rabbit: Creation Success",                                                                 //16
            "Knight: Creation Success",                                                                 //17
            "Army: Knights File not read successfully",                                                 //18
            "Army: Incorrect Number of Knights in File",                                                //19
            "Rabbit: Attack Chances must add to 100%"                                                   //20
    };
    close(log_pipe[1]);

    hwLog l(_file);
    l.write("BEGIN");
    while(true) {
        try {
            int bytes_read = read(log_pipe[0], (char *) &m, sizeof(message));   //Reads the message
            if(bytes_read == sizeof(message)) {
                Message = Messages[m.type];
                Message = Message.replace(Message.find("_name"),5 ,knight_names[m.from-1]);
                Message = Message.replace(Message.find("_dmg"),4,to_string(m.damage));
                Message = Message.replace(Message.find("_type"),5,
                                          ((m.type==6)?"QUICK":(m.type==5)?"BITE":"THROAT"));
                fprintf(stdout,"%s\n", Message.c_str());
                fflush(stdout);
                l.write(Message);
                if (Message == "END") {
                    fprintf(stdout,"Message %s tells me to exit.\n",Message.c_str());
                    fflush(stdout);
                    break;
                }
            } else {
                if(bytes_read > 0) {
                    fprintf(stdout,"%d Bytes Read",bytes_read);
                    fflush(stdout);
                } else {
                    fprintf(stdout,"Nothing Read");
                    fflush(stdout);
                }
            }
        } catch (exception e) {
            fprintf(stdout, "Caught an exception");
            fflush(stdout);
            l.write("Logger encountered an error. Exiting.");
            kill(0,SIGTERM);
        }
    }//end LoggingLoop
    log_pid = -1;
    exit(0);
}

/*
Name:       run_rabbit
Purpose:    Runs the rabbit process until death or winning.
Arguments:  _r: The rabbit object to run.
*/
static void* run_rabbit(void* _r) {
    close(log_pipe[0]);
    auto rabbit = (Rabbit*)_r;

    /*Combat Loop*/
    while (true) {
        fprintf(stdout,"run_rabbit_running");
        fflush(stdout);
        /*Attacks*/
        Rabbit::atk atk = rabbit->attackRoll();

        message out = {0,(atk.aoe)? 6:(atk.weak)? 5:7, rabbit->getDamage(atk.weak)};
        if (atk.aoe) {
            for(int i = 1; i<Queues.size();++i)
                Queues[i].enqueue(out);
            write(log_pipe[1], (char *) &out, sizeof(out));
        } else {
            unsigned long rand = random() % rabbit->getTargets().size();
            Queues[rand+1].enqueue(out);
        }
        /*Checks for deaths*/
        int health_old = rabbit->getHealth();
        fprintf(stdout, "Potential Read Lock Inbound");
        fflush(stdout);
        while(!Queues[0].is_empty()) {
            message in = Queues[0].dequeue();
            fprintf(stdout,"run_rabbit_while_read");
            fflush(stdout);
            if (in.type == 8) {   //Dead Knight
                rabbit->removeTargetAt(in.from - 1);  //Remove from Targets
                fprintf(stdout, "Rabbit: Removing target %i from list.",in.from-1);
                fflush(stdout);
                if(rabbit->getTargets().empty()) {    //If rabbit has won
                    in.from = 0;
                    in.type = 10;    //type(10)=>Rabbit Wins
                    write(log_pipe[1], (char*)&in, sizeof(message));
                    exit(1);
                }
            } else if(in.type == 4) {  //Knight Running
                fprintf(stdout, "Rabbit: What a coward you are %s",rabbit->getTargets()[in.from-1].c_str());
                fflush(stdout);
                rabbit->removeTargetAt(in.from - 1);
            } else if (in.type == 0) {
                int health_new = rabbit->takeDamage(in.damage);
                if (rabbit->isDead()) {
                    fprintf(stdout, "Rabbit: I have been unfairly slain.");
                    fflush(stdout);
                    message deadBunny = {0, 9, 9};
                    Queues[in.from].enqueue(deadBunny);
                    pthread_exit(0);
                } else if(health_old == health_new) {
                    fprintf(stdout,"Rabbit: Haha, you missed.");
                    fflush(stdout);
                    in.type = 1;
                    write(log_pipe[1], (char *) &in, sizeof(message));    //tell log k missed
                } else {
                    fprintf(stdout,"Rabbit: Ouch");
                    fflush(stdout);
                    in.type = 2;
                    write(log_pipe[1], (char *) &in, sizeof(message));    //tell log k hit
                }
            }
        }
    }
}

/*
Name:       run_knight
Purpose:    Runs the knight's combat simulation.
Arguments:  _k: Knight -> the knight to run; _id:int -> the index of that knight.
*/
static void* run_knight(void* _k) {
    close(log_pipe[0]);
    auto args = (run_knight_parms*)_k;
    while (true) {
        fprintf(stdout,"run_knight_running\n");
        fflush(stdout);
        message in;

        /*deals dmg*/
        message knight_attack = {args->_id, 0, args->_k.getDMG()};
        Queues[0].enqueue(knight_attack);
        fprintf(stdout, "%s tried to attack the rabbit.",args->_k.getName().c_str());
        fflush(stdout);

        while(!Queues[args->_id].is_empty()) {
            message in = Queues[args->_id].dequeue();
            fprintf(stdout,"run_knight_while_read");
            fflush(stdout);
            if (in.type == 9) { //Knights have won.
                if (in.from == 0) {  //If from the rabbit.
                    fprintf(stdout, "Knight: I, %d, killed the Rabbit!",getpid());
                    fflush(stdout);
                    message win_message = {args->_id, 9, in.damage};
                    write(log_pipe[1], (char*)&win_message, sizeof(message)); //Tell the logger I killed it.
                    for(int i = 1; i<Queues.size();++i) {
                        if(i==args->_id) continue;
                        Queues[i].enqueue(win_message);
                    }
                    pthread_exit(0);    //Killer Knight exits with 2
                } else {
                    fprintf(stdout, "Knight: I, %d, didn't do it, but ayy, someone killed the rabbit!",getpid());
                    fflush(stdout);
                    pthread_exit(0);
                } //Rest exit with 1. -1 means ran away.
            } else {    //If the rabbit is still alive,
                /*takes dmg*/
                args->_k.takeDamage(in.damage);
                if (args->_k.isDead()) {
                    fprintf(stdout,"I, proc %d, am dead.",getpid());
                    fflush(stdout);
                    message knight_dead = {args->_id, 8, in.type};    //Knight, dead, attack type that killed him.
                    Queues[0].enqueue(knight_dead);
                    write(log_pipe[1], (char *) &knight_dead, sizeof(message));
                    pthread_exit(0);
                } else if(args->_k.run()) {
                    fprintf(stdout,"%s is getting out of dodge",args->_k.getName().c_str());
                    fflush(stdout);
                    message knight_run = {args->_id, 4, 0};   //Knight, runs, junk
                    write(log_pipe[1], (char*)&knight_run, sizeof(message)); 	//Tells the logger k is running- coward.
                    Queues[0].enqueue(knight_run);
                    pthread_exit(0);
                }
            }
        }//end read
        //usleep(_k.getRate());
    }
}//end KnightProcLoop

/*
Name:       create_rabbit
Purpose:    Initializes a Rabbit object and returns it.
Arguments:  _file: string path of the file to open
*/
static Rabbit create_rabbit(string& _file) {
    const int attribNum = 8;
    string keysExpected[attribNum] = {"hp","rate","bite","quick","throat","weak","strong","evade"};
    int lowBounds[attribNum]  =      {25,3,60,10,5,1,30,5};
    int highBounds[attribNum] =      {100,10,75,20,20,9,40,95};
    int rabbitAttr[attribNum] =      {-1,-1,-1,-1,-1,-1,-1,-1};
    message m;

    auto expectedKey = [&](const string& key){
        m.type = 12;    //Unknown Key
        write(log_pipe[1], (char*)&m, sizeof(message));
        m.type = 11;    //END
        write(log_pipe[1], (char*)&m, sizeof(message));
        exit(-1);
    };

    auto expectedValue = [&](const string& value,int index) {
        m.type = 14;     //Value Out Of Bounds
        write(log_pipe[1], (char*)&m, sizeof(message));
        m.type = 11;    //END
        write(log_pipe[1], (char*)&m, sizeof(message));
        exit(-1);
    };

    /*Read from file into string*/
    ifstream in;
    in.open(_file);
    string temp, line, file;
    if(in.is_open()) {
        while(getline(in,temp)) {
            file+=temp+"\n";
        }
        in.close();
    }
    /*Gets index of parsed key*/
    istringstream f;
    f.str(file);
    int index = -1, attrib;
    while(getline(f, temp, ':')) {   //While there are keys
        for(int i = 0; i<attribNum;++i) {
            if(temp==keysExpected[i]) {
                index = i;
                break;
            }
            else if(i==attribNum-1) expectedKey(temp);
        }
        /*Checks range of acceptable values for value*/
        getline(f, temp);              //Gets value.
        if((attrib = (stoi(temp)))>highBounds[index] || attrib<lowBounds[index])  {//Error checking
            expectedValue(temp,index);
        }
        rabbitAttr[index]=attrib;
    }
    /*Makes sure all values are set*/
    for(int i = 0;i < attribNum;++i) {      //Checks to make sure all attribs are set
        if(rabbitAttr[i]==-1) {
            expectedValue(to_string(rabbitAttr[i]),i);
        }
    }
    /*Makes sure attack chances + => 100*/
    if(rabbitAttr[2]+rabbitAttr[3]+rabbitAttr[4]!=100) {
        m.type = 20;
        write(log_pipe[1], (char*)&m, sizeof(message));
        m.type = 11;
        write(log_pipe[1], (char*)&m, sizeof(message));
        exit(-1);
    }
    Rabbit r(rabbitAttr[0],rabbitAttr[1],rabbitAttr[2],rabbitAttr[3],
             rabbitAttr[4],rabbitAttr[5],rabbitAttr[6],rabbitAttr[7]);

    m.type = 16;    //Success
    write(log_pipe[1], (char*)&m, sizeof(message));

    return r;
}

/*
Name:       create_knight
Purpose:    Parses Keys and Values and returns a knight with the appropriate attributes.
Arguments:  _keys, _values:vector<string> The keys and values used to create the knight.
*/
static Knight create_knight(vector<string> _keys, vector<string> _values){
    const int numAttr = 5;
    const string expectedKeys[numAttr] = {"name","hp","damage","bravery","rate"};
    int highBounds[numAttr]   =          {50,40,8,-1,50};
    int lowBounds[numAttr]    =          {2,10,1,0,10};
    int knightAttr[numAttr-1] =          {-1,-1,-1,-1};
    message m;
    string name = "";

    auto error = [&](int type) {
        m.type = type;     //Value Out Of Bounds
        write(log_pipe[1], (char*)&m, sizeof(message));
        m.type = 11;    //END
        write(log_pipe[1], (char*)&m, sizeof(message));
    };

    /*Makes sure all keys are valid*/
    if(_keys.size()!=numAttr)
        error(13);
    for(int i = 0; i<_keys.size();++i) {
        for(int j = 0; j< numAttr;++j) {
            if (_keys[i] == expectedKeys[j]) {
                break;
            } else if(j == 4) {
                error(13);
            }
        }
    }
    /*Matches all values based on their keys and the corresponding index in the stats. HP must go before Bravery still.*/
    int index, val;
    for(int i = 0; i < _keys.size(); ++i) {
        if(_keys[i].compare("hp")==0) { index = 0; }
        else if(_keys[i].compare("bravery")==0) { index = 1; }
        else if(_keys[i].compare("damage")==0) { index = 3; }
        else if(_keys[i].compare("rate")==0) { index = 2; }
        else if(_keys[i].compare("name")==0) {
            if(_values[i].size() < lowBounds[0] || _values[i].size() > highBounds[0])
                error(15);
            name = _values[i];
            continue;
        }
        if((val = stoi(_values[i])) < lowBounds[index] || val > highBounds[index])
            error(15);
        if(_keys[i].compare("hp")) { highBounds[2] = val; }
        knightAttr[index] = val;
    }

    m.type = 17;    //Success
    write(log_pipe[1], (char*)&m, sizeof(message));

    Knight k(name,knightAttr[0],knightAttr[1],knightAttr[2],knightAttr[3]);
    return k;
}

/*
Name:       create_army
Purpose:    Parses a text file and returns a collection of Knight objects from the file.
Arguments:  _file: string path of the file to open
*/
static vector<Knight> create_army(string& _file) {
    /*Default Army*/
    vector<Knight> army;
    string defaultName = "Camelot";
    int defaultStats[4]= {20,18,25,2};
    if(_file.substr(_file.find_last_of('/')) == "default") {
        Knight k(defaultName,defaultStats[0],defaultStats[1],defaultStats[2],defaultStats[3]);
        army.push_back(k);
        return army;
    }

    vector<string> key, value;    //Gets the entire contents of the file into the string.
    string temp, line, file;
    message m;
    /*Read in file to string*/
    ifstream kInput;
    kInput.open(_file);
    int count = 0;

    if(kInput.is_open()) {
        getline(kInput,temp,':');   //Ignore the first key- assume its count.
        getline(kInput,temp);              //Get First Value - which is count.
        count = stoi(temp);

        while(kInput.peek()=='\n')        //Skip any whitespace between count and first Knight.
            getline(kInput,temp);
        while(getline(kInput,temp))  //While not at the end of the file, add to file
            file+=temp+"\n";
        kInput.close();
    }
    else {
        m.type = 18;
        write(log_pipe[1], (char*)&m, sizeof(message));
        m.type = 11;
        write(log_pipe[1], (char*)&m, sizeof(message));
    }
    /*Add to army*/
    istringstream f;
    f.str(file);
    while(getline(f,line)) {
        /*An empty line means a new knight- so make the old one.*/
        if(line.empty()) {
            army.push_back(create_knight(key, value)); //Adds a new Knight to army
            key.clear();
            value.clear();
        }
            /*Adds in the Key:Value pair from the parsed line*/
        else {
            istringstream ln;  //Reads this non-empty line.
            ln.str(line);
            getline(ln, temp, ':');//Reads the key
            key.push_back(temp);
            getline(ln, temp);    //Reads Value
            value.push_back(temp);
        }
    }
    army.push_back(create_knight(key, value));  //Pushes back last knight
    if(count == army.size()) {
        return army;
    } else {
        m.type = 19;
        write(log_pipe[1], (char*)&m, sizeof(message));
        m.type = 11;
        write(log_pipe[1], (char*)&m, sizeof(message));
        exit(-1);
    }
}


