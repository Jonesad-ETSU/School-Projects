/*
Name:       Austin Jones
Class:      CSCI 4727-001
Assignment: Homework 2 - Monty_Python_Rabbit_Attack
File:       Knight.cpp
*/
#include <iostream>
#include "Knight.h"
/*
Name:       Knight::Knight
Purpose:    Creates the ultimate one-man army.
*/
Knight::Knight() {
    this->name = "Leroy Jenkins";
    this->hp = 20;
    this->bravery = 10;
    this->attackSpeed = 25;
    this->attackPower = 5;
}

/*
Name:       Knight::Knight
Purpose:    Creates a knight with the specified parameters.
Arguments:  n,h,b,as,ap - name, health, bravery, attackSpeed, attackPower
*/
Knight::Knight(string n, int h, int b, int as, int ap) {
    bool err = false;
    if (!(n.length() >= 2 && n.length() <= 50)) {
        std::cout << "Name of Knight " << n << " does not fit the "
                  << "length requirements. It must be between 2 and 50"
                  << "characters.";
        err = true;
    }
    else if(!(h>=10 && h<=40)) {
        std::cout << "Health of Knight " << n << " must be between 10 and 40. Given: "+ ::to_string(h);
        err = true;
    }
    else if(!(b>=0 && b<h)) {
        std::cout << "Bravery of Knight " <<n << " must be between 0 and"
                  <<  " his health.";
        err = true;
    }
    else if(!(as>=10 && as<=50)) {
        std::cout << "Attack Speed of Knight "<<n<< " must be between"
                  << " 10 and 50.";
        err = true;
    }
    else if(!(ap>=1 && ap<=8)) {
        std::cout << "Attack Power of Knight "<<n<< "must be between"
                  << "1 and 8.";
        err = true;
    }
    if(err) exit(-1);

    this->name = n;
    this->hp = h;
    this->bravery = b;
    this->attackSpeed = as;
    this->attackPower = ap;
}

/*
Name:       Knight::getHP
Purpose:    Return Health
*/
int Knight::getHP() {
    return this->hp;
}

/*
Name:       Knight::getDMG
Purpose:    Return Damage
*/
int Knight::getDMG() {
    return this->attackPower;
}

/*
Name:       Knight::run
Purpose:    Uses Knight's Health and Bravery stats to determine if he is running.
*/
bool Knight::run() {
    return !isDead() && this->bravery>=this->hp;
}

/*
Name:      Knight::getRate
Return:    Returns the current Knight's attackSpeed.
*/
int Knight::getRate() {
    return this->attackSpeed;
}
/*
Name:       Knight::takeDamage
Purpose:    Subtracts damage from Knight's Health and returns the new health.
Arguments:  The amount of health to be taken.
*/
int Knight::takeDamage(int dmg) {
    this->hp -= dmg;    //Dead People still get punished for being in the rabbit's territory.
    return this->hp;
}

/*
Name:       Knight::isDead
Purpose:    Returns if health is zero or less.
*/
bool Knight::isDead() {
    return this->hp <= 0;
}

/*
Name:       Knight::getName
Purpose:    Return the Knight's Name.
*/
string Knight::getName() {
    return this->name;
}

/*
Name:       Knight::to_string
Purpose:    Return Name and Health as a string.
*/
string Knight::to_string() {
    return "Name: "+this->getName()+"    HP: "+
           std::to_string(this->getHP());
}