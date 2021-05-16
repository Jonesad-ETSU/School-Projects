all:
hw6: hw6.o Knight.o Rabbit.o hwLog.o SafeQueue.o
	g++ -o hw6 hw6.o Rabbit.o Knight.o hwLog.o SafeQueue.o -lpthread
hw6.o: hw6.cpp
	g++ -c hw6.cpp
Rabbit.o: Rabbit.cpp Rabbit.h Knight.o hwLog.o
	g++ -c Rabbit.cpp
Knight.o: Knight.cpp Knight.h
	g++ -c Knight.cpp
hwLog.o: hwLog.cpp hwLog.h
	g++ -c hwLog.cpp
SafeQueue.o: SafeQueue.cpp
	g++ -c SafeQueue.cpp
clean:
	rm -rf hw6

