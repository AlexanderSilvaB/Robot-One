// mobilerobotics.cpp : Defines the exported functions for the DLL application.
//

#include "TcpUdpSocket.h"
#include <iostream>
#include <string>
#include <sstream>
#include <fstream>

#define PORT 5046
#define VERSION 1.0
#define TIME_WAIT "Time.Wait"
#define MAX_DATA 921600

using namespace std;

TcpUdpSocket *s = NULL;
char dataIn[MAX_DATA];
char dataOut[MAX_DATA];
char dataContainer[MAX_DATA];
volatile static int retSize = 0;
volatile static float retValue = 0;
char *temp;

bool Handler(char *data)
{
	int c = (unsigned char)data[0];
	if (c == 0 || c == 1) // Get and Set sending
		return false;
	int status = (unsigned char)data[1];
	temp = data + 2;
	retValue = ((float*)temp)[0];
	if (c == 3) // Raw data
	{
		temp = data + 6;
		retSize = ((int*)temp)[0];
		temp = data + 10;
		for (int i = 0; i < retSize; i++)
			dataContainer[i] = temp[i];
	}
	return true;
}

extern "C" __declspec(dllexport) int connect(char *address)
{
	if (s != NULL)
		return 1;
	s = new TcpUdpSocket(PORT, address, FALSE);
	bool connected = s->isConnected();
	return connected ? 1 : 0;
}

extern "C" __declspec(dllexport) int disconnect()
{
	if (s == NULL)
		return 0;
	delete s;
	s = NULL;
	return 1;
}

extern "C" __declspec(dllexport) float version()
{
	return VERSION;
}

extern "C" __declspec(dllexport) float wait()
{
	if (s == NULL || !s->isConnected())
		return 0.0f;
	int len = strlen(TIME_WAIT);
	dataOut[0] = (char)0;
	*((int*)(dataOut + 1)) = len;
	for (int i = 0; i < len; i++)
		dataOut[5 + i] = TIME_WAIT[i];
	s->send(dataOut, len + 5 + 4);
	bool done = false;
	while (!done)
	{
		s->receive(dataIn, MAX_DATA);
		done = Handler(dataIn);
	}
	return retValue;
}

extern "C" __declspec(dllexport) char* getData()
{
	return dataContainer;
}

extern "C" __declspec(dllexport) float* getDataFloat()
{
	return (float*)dataContainer;
}

extern "C" __declspec(dllexport) float get(char *name)
{
	if (s == NULL || !s->isConnected())
		return 0.0f;
	int len = strlen(name);
	dataOut[0] = (char)0;
	*((int*)(dataOut + 1)) = len;
	for (int i = 0; i < len; i++)
		dataOut[5 + i] = name[i];
	s->send(dataOut, len + 5 + 4);
	bool done = false;
	while (!done)
	{
		s->receive(dataIn, MAX_DATA);
		done = Handler(dataIn);
	}
	return retValue;
}

extern "C" __declspec(dllexport) float set(char *name, float value)
{
	if (s == NULL || !s->isConnected())
		return 0.0f;
	int len = strlen(name);
	dataOut[0] = (char)1;
	*((int*)(dataOut + 1)) = len;
	for (int i = 0; i < len; i++)
		dataOut[5 + i] = name[i];
	temp = (char*)&value;
	for (int i = 0; i < 4; i++)
		dataOut[5 + len + i] = temp[i];
	s->send(dataOut, len + 5 + 4);
	bool done = false;
	while (!done)
	{
		s->receive(dataIn, MAX_DATA);
		done = Handler(dataIn);
	}
	return retValue;
}

extern "C" __declspec(dllexport) float setData(char *name, int size, char *data)
{
	if (s == NULL || !s->isConnected())
		return 0.0f;
	int len = strlen(name);
	dataOut[0] = (char)1;
	*((int*)(dataOut + 1)) = len;
	for (int i = 0; i < len; i++)
		dataOut[5 + i] = name[i];
	float tmpF = (float)size;
	temp = (char*)&tmpF;
	for (int i = 0; i < 4; i++)
		dataOut[5 + len + i] = temp[i];
	int index = len + 5 + 4;
	for (int i = 0; i < size; i++)
	{
		dataOut[index] = data[i];
		index++;
		if(index >= MAX_DATA)
			break;
	}
	s->send(dataOut, index);
	bool done = false;
	while (!done)
	{
		s->receive(dataIn, MAX_DATA);
		done = Handler(dataIn);
	}
	return retValue;
}

extern "C" __declspec(dllexport) float setDataFloat(char *name, int size, float *data)
{
	if (s == NULL || !s->isConnected())
		return 0.0f;
	int len = strlen(name);
	dataOut[0] = (char)1;
	*((int*)(dataOut + 1)) = len;
	for (int i = 0; i < len; i++)
		dataOut[5 + i] = name[i];
	float tmpF = (float)size;
	temp = (char*)&tmpF;
	for (int i = 0; i < 4; i++)
		dataOut[5 + len + i] = temp[i];
	int index = len + 5 + 4;
	char *tmpFV = (char*)data;
	for (int i = 0; i < size*4; i++)
	{
		dataOut[index] = tmpFV[i];
		index++;
		if(index >= MAX_DATA)
			break;
	}
	s->send(dataOut, index);
	bool done = false;
	while (!done)
	{
		s->receive(dataIn, MAX_DATA);
		done = Handler(dataIn);
	}
	return retValue;
}