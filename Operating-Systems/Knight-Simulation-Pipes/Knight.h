//
// Created by sonictm on 2/16/20.
//

#ifndef HOMEWORK2_RABBITATTACK_KNIGHT_H
#define HOMEWORK2_RABBITATTACK_KNIGHT_H
#include<string>
using namespace std;
class Knight{
private:
    string name;
    int hp;
    int bravery;
    int attackSpeed;
    int attackPower;
public:
    Knight();
    Knight(string n, int h, int b, int as, int ap);
    string getName();
    int getHP();
    int takeDamage(int dmg);
    int getDMG();
    int getRate();
    bool run();
    bool isDead();
    string to_string();
};

#endif //HOMEWORK2_RABBITATTACK_KNIGHT_H
