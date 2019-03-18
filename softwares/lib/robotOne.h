#ifndef ROBOT_ONE_H_
#define ROBOT_ONE_H_

typedef struct
{
    float values[3];
}Value3;

typedef struct
{
    float values[2];
}Value2;

typedef struct
{
    int width, height, channels, size;
    char *data;
}CameraData;

typedef struct
{
    int size;
    float *readings, *angles, *x, *y;
}LidarData;

int connectRobotOne(char *address);
int disconnectRobotOne();
float versionRobotOne();
float waitRobotOne();
float get(char *name);
float set(char *name, float value);
char* getData();
float* getDataFloat();
float setData(char *name, int size, char *data);
float setDataFloat(char *name, int size, float *data);

void initLidar(LidarData *lidarData, int size);
void readLidar(LidarData *lidarData);
void initCamera(CameraData *cameraData);
void captureCamera(CameraData *cameraData);
void getPose(Value3 *pose);
void setPose(Value3 *pose);
void getOdometry(Value3 *pose);
void setOdometry(Value3 *pose);
void getOdometryStd(Value2 *odometryStd);
void setOdometryStd(Value2 *odometryStd);
void getVelocity(Value2 *velocity);
void setVelocity(Value2 *velocity);
bool getLowLevelControl();
void setLowLevelControl(bool enabled);
bool getTrace();
void setTrace(bool enabled);
void getWheels(Value2 *wheels);
void setWheels(Value2 *wheels);

#endif