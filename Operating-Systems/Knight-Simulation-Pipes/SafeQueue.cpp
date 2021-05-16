/*
Name:       Austin Jones
Class:      CSCI 4727-001
Assignment: Homework 6 - Threading
File:       SafeQueue.cpp
*/
#ifndef JONESAUSTINHW6_SAFEQUEUE_H
#define JONESAUSTINHW6_SAFEQUEUE_H
#include <queue>
#include <semaphore.h>
#include <pthread.h>
using namespace std;
template<class T>
class SafeQueue {
private:
    queue<T> _q;
    sem_t _count{};
    pthread_mutex_t _lock;
public:
    SafeQueue<T>();
    void enqueue(T t);
    bool is_empty();
    T dequeue();
};

/*
Name:       SafeQueue<T>::SafeQueue
Purpose:    _init w sem at 0 and mutex unlocked.
*/
template<class T>
SafeQueue<T>::SafeQueue() {
    _lock = PTHREAD_MUTEX_INITIALIZER;
    sem_init(&_count,0,0);
}
/*
Name:       SafeQueue<T>::enqueue
Purpose:    Accepts a Generic type and throws it on a queue.
*/
template<class T>
void SafeQueue<T>::enqueue(T t) {
    pthread_mutex_lock(&_lock);
    _q.push(t);
    pthread_mutex_unlock(&_lock);
}
/*
Name:       SafeQueue<T>::dequeue
Purpose:    Returns the top object on the Queue and deletes it from the stack.
*/
template<class T>
T SafeQueue<T>::dequeue() {
    sem_wait(&_count);
    pthread_mutex_lock(&_lock);
    T mess = _q.front();
    _q.pop();
    pthread_mutex_unlock(&_lock);
    sem_post(&_count);
    return mess;
}

/*
Name:       SafeQueue<T>::is_empty
Purpose:    Sees if the Queue is empty.
*/
template<typename T>
bool SafeQueue<T>::is_empty() {
    return _q.empty();
}

#endif //JONESAUSTINHW6_SAFEQUEUE_H
