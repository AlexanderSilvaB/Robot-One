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

// Implementations
extern "C" __declspec(dllexport) void initLidar(LidarData &lidarData, int size)
{
	if(lidarData.readings != NULL)
		delete[] lidarData.readings;
	if(lidarData.angles != NULL)
		delete[] lidarData.angles;
	if(lidarData.x != NULL)
		delete[] lidarData.x;
	if(lidarData.y != NULL)
		delete[] lidarData.y;
	lidarData.size = size;
	lidarData.readings = new float[size];
	lidarData.angles = new float[size];
	lidarData.x = new float[size];
	lidarData.y = new float[size];
}

extern "C" __declspec(dllexport) void readLidar(LidarData &lidarData)
{
	int szI = (int)get("Lidar.Read");
	if(lidarData.size != szI)
		initLidar(lidarData, szI);
	float *readings = getDataFloat();
	float sz = szI;
	float inc = M_PI / sz;
	float start = -(0.5f * sz)*inc;
	float end = -start;
	float inc_ = M_PI_2;
	for(int i = 0; i < sz; i++)
	{
		lidarData.readings[i] = readings[i];
		lidarData.angles[i] = start + i*inc;
		if(lidarData.readings[i] > 0)
		{
			lidarData.x[i] = -lidarData.readings[i]*cos(inc_ + lidarData.angles[i]);
			lidarData.y[i] =  lidarData.readings[i]*sin(inc_ + lidarData.angles[i]);
		}
		else
		{
			lidarData.x[i] = lidarData.y[i] = 0;
		}
	}
}

extern "C" __declspec(dllexport) void initCamera(CameraData &cameraData)
{
	if(cameraData.data != NULL)
		delete[] cameraData.data;
	cameraData.width = 320;
	cameraData.height = 240;
	cameraData.channels = 3;
	cameraData.size = cameraData.width * cameraData.height * cameraData.channels;
	cameraData.data = new char[cameraData.size];
 }


extern "C" __declspec(dllexport) void captureCamera(CameraData &cameraData)
{
	int sz = (int)get("Camera.Capture");
	if(cameraData.size != sz)
		initCamera(cameraData);
	char *data = getData();
	memcpy(cameraData.data, data, sz);
}

extern "C" __declspec(dllexport) void getPose(Value3 &pose)
{
	pose.values[0] = get("Pose.X");
	pose.values[1] = get("Pose.Y");
	pose.values[2] = get("Pose.Theta");
}

extern "C" __declspec(dllexport) void setPose(Value3 &pose)
{
	set("Pose.X", pose.values[0]);
	set("Pose.Y", pose.values[1]);
	set("Pose.Theta", pose.values[2]);
}

extern "C" __declspec(dllexport) void getOdometry(Value3 &pose)
{
	pose.values[0] = get("Odometry.X");
	pose.values[1] = get("Odometry.Y");
	pose.values[2] = get("Odometry.Theta");
}

extern "C" __declspec(dllexport) void setOdometry(Value3 &pose)
{
	set("Odometry.X", pose.values[0]);
	set("Odometry.Y", pose.values[1]);
	set("Odometry.Theta", pose.values[2]);
}

extern "C" __declspec(dllexport) void getOdometryStd(Value2 &odometryStd)
{
	odometryStd.values[0] = get("Odometry.Std.Linear");
	odometryStd.values[1] = get("Odometry.Std.Angular");	
}

extern "C" __declspec(dllexport) void setOdometryStd(Value2 &odometryStd)
{
	set("Odometry.Std.Linear", odometryStd.values[0]);
	set("Odometry.Std.Angular", odometryStd.values[1]);
}

extern "C" __declspec(dllexport) void getVelocity(Value2 &velocity)
{
	velocity.values[0] = get("Velocity.Linear");
	velocity.values[1] = get("Velocity.Angular");		
}

extern "C" __declspec(dllexport) void setVelocity(Value2 &velocity)
{
	set("Velocity.Linear", velocity.values[0]);
	set("Velocity.Angular", velocity.values[1]);
}

extern "C" __declspec(dllexport) bool getLowLevelControl()
{
	return get("Controller.LowLevel") > 0;
}

extern "C" __declspec(dllexport) void setLowLevelControl(bool enabled)
{
	set("Controller.LowLevel", enabled ? 1.0f : 0.0f);
}

extern "C" __declspec(dllexport) bool getTrace()
{
	return get("Trace") > 0;
}

extern "C" __declspec(dllexport) void setTrace(bool enabled)
{
	set("Trace", enabled ? 1.0f : 0.0f);
}

extern "C" __declspec(dllexport) void getWheels(Value2 &wheels)
{
	wheels.values[0] = get("Velocity.Angular.Left");
	wheels.values[1] = get("Velocity.Angular.Right");
}

extern "C" __declspec(dllexport) void setWheels(Value2 &wheels)
{
	set("Velocity.Angular.Left", wheels.values[0]);
	set("Velocity.Angular.Right", wheels.values[1]);
}
