#ifndef HOMEWORK2_RABBITATTACK_RABBIT_H
#define HOMEWORK2_RABBITATTACK_RABBIT_H
#include<vector>
#include<string>
#include"Knight.h"

class Rabbit {
public:
    struct atk {
    public:
        bool aoe;
        bool weak;
    };
private:
    int hp;
    int attackSpeed;
    int biteAttackChance;
    int quickAttackChance;
    int throatAttackChance;
    int weakAttackDmg;
    int strongAttackDmg;
    int evadeChance;
    bool hasTargets;
    vector<string> targets;
    atk attack(int attackNum);
    int evasionRoll();

public:
    Rabbit();
    Rabbit(int h, int as, int bac, int qac,
           int tac, int wad, int sad, int ec);
    atk attackRoll();
    void setTargets(vector<Knight> &knights);
    vector<string> getTargets();
    int getDamage(bool weak);
    int takeDamage(int dmg);
    int getHealth();
    int getRate();
    int canAttack(int timer);
    bool isDead();
    string to_string();
    void removeTarget(Knight &k);
    void removeTargetAt(int n);
};
#endif //HOMEWORK2_RABBITATTACK_RABBIT_H
