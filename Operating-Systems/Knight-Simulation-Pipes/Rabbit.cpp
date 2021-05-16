/*
Name:       Austin Jones
Class:      CSCI 4727-001
Assignment: Homework 2 - Monty_Python_Rabbit_Attack
File:       Rabbit.cpp
*/
#include <cstdlib>
#include <random>
#include "Rabbit.h"

/*
Name:       Rabbit::Rabbit
Purpose:    Creates a default rabbit.
*/
Rabbit::Rabbit() {
    this->hp = -1;
    this->hasTargets = false;
    this->targets;
    this->quickAttackChance = -1;
    this->biteAttackChance = -1;
    this->throatAttackChance = -1;
    this->weakAttackDmg = -1;
    this->strongAttackDmg = -1;
    this->evadeChance = -1;
}

/*
Name:       Rabbit::Rabbit
Purpose:    Create a rabbit with the specified attributes.
Arguments:  h,as,bac,qac,tac,wad,sad,ec - health, attackSpeed, biteAttackChance, quickAttackChance, throatAttackChance,
                                            weakAttackDamage, strongAttackDamage, evadeChance
*/
Rabbit::Rabbit(int h, int as, int bac, int qac,
               int tac, int wad, int sad, int ec) {
    this->hp = h;
    this->attackSpeed = as;
    this->biteAttackChance = bac;
    this->quickAttackChance = qac;
    this->throatAttackChance = tac;
    this->weakAttackDmg = wad;
    this->strongAttackDmg = sad;
    this->evadeChance = ec;
    this->hasTargets = false;
}

/*
Name:       Rabbit::attackRoll
Purpose:    Rolls for the type of attack and returns that attack.
*/
Rabbit::atk Rabbit::attackRoll() {
    default_random_engine randEngine;
    uniform_int_distribution<int> distribution(1,100);
    int attackNumber = distribution(randEngine);
    if(attackNumber<=this->biteAttackChance)
        return attack(0);
    else if(attackNumber<=biteAttackChance+quickAttackChance)
        return attack(1);
    else return attack(2);
}

/*
Name:       Rabbit::attack
Purpose:    Return the type of attack specified by input.
Arguments:  attackNum - the type of attack to be returned. 0 => bite; 1=> quick; 2=> throat
*/
Rabbit::atk Rabbit::attack(int attackNum){
    if(attackNum==0) {
        struct atk bite;
        bite.aoe = false;
        bite.weak = true;
        return bite;
    }
    else if(attackNum==1) {
        struct atk quick;
        quick.aoe = true;
        quick.weak = true;
        return quick;
    }
    else if(attackNum==2){
        struct atk throat;
        throat.aoe = false;
        throat.weak = false;
        return throat;
    }
    else {
        exit(-1);
    }
}

/*
Name:      Rabbit::getRate
Purpose:   Stores the rate of the Rabbit's attack.
Return:    Returns the rabbit's attackSpeed
*/
int Rabbit::getRate() {
    return this->attackSpeed;
}
/*
Name:       Rabbit::setTargets
Purpose:    Stores the names of the Rabbit's targets in a vector.
Arguments:  army - the knights to be set as initial target.
*/
void Rabbit::setTargets(vector<Knight> &army) {
    for(Knight k: army) {
        this->targets.push_back(k.getName());
    }
    hasTargets = true;
}

/*
Name:       Rabbit::getTargets
Purpose:    Return Names of Targets.
*/
vector<string> Rabbit::getTargets() {
    return this->targets;
}

/*
Name:       Rabbit::takeDamage
Purpose:    Rabbit takes the input amount of damage and returns the new health.
Arguments:  dmg - the amount of damage to take.
*/
int Rabbit::takeDamage(int dmg) {
    if(!evasionRoll())
        this->hp-=dmg;
    return this->hp;
}

/*
Name:       Rabbit::getHealth
Purpose:    Return Health
*/
int Rabbit::getHealth() {
    return this->hp;
}

/*
Name:       Rabbit::evasionRoll
Purpose:    Rolls to see if the rabbit avoids the attack.
*/
int Rabbit::evasionRoll() {
    default_random_engine randEngine;
    uniform_int_distribution<int> distribution(1,100);
    return this->evadeChance >= distribution(randEngine);
}

/*
Name:       Rabbit::canAttack
Purpose:    Accepts an integer timer and returns if the Rabbit can attack this turn
Arguments:  timer - the current time state of the game.
*/
int Rabbit::canAttack(int timer) {
    return hasTargets && (timer % attackSpeed == 0);
}

/*
Name:       Rabbit::getDamage
Purpose:    Returns the damage of the rabbit attack based on whether it is a weak atk or not.
Arguments:  weak - bool stating to return weak damage if true or strong if false.
*/
int Rabbit::getDamage(bool weak) {
    return (weak)? this->weakAttackDmg: strongAttackDmg;
}

/*
Name:       Rabbit::isDead
Purpose:    Return if health is less than or equal to zero.
*/
bool Rabbit::isDead() {
    return this->hp <= 0;
}

/*
Name:       Rabbit::removeTarget
Purpose:    Removes a Knight from the list of targets.
Arguments:  k - Knight to remove.
*/
void Rabbit::removeTarget(Knight &k) {
    for(int i = 0;i<targets.size();++i) {
        if(targets[i]==k.getName()) {  //If same knight
            targets.erase(targets.begin()+i);   //delete it
            break;  //Only 1 knight with the same name.
        }
    }
}

/*
Name:       Rabbit::removeTargetAt
Purpose:    Removes a Knight from the list of targets.
Arguments:  n - num of Knight to remove.
*/
void Rabbit::removeTargetAt(int n) {
    targets.erase(targets.begin()+n);
}

/*
Name:       Rabbit::to_string
Purpose:    Returns a string representation of the rabbit.
*/
string Rabbit::to_string() {
    string str = "";
    str+="Rabbit:\nHP: "+::to_string(this->hp);
    return str;
}
