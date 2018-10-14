#ifndef MOBILEROBOTICS_H_
#define MOBILEROBOTICS_H_

int Connect(char *address);
int Disconnect();
float Version();
float Wait();
float Get(char *name);
float Set(char *name, float value);

#endif