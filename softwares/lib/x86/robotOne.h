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

int connectRobotOne(const char *address);
int disconnectRobotOne();
float versionRobotOne();
float waitRobotOne();
float get(const char *name);
float set(const char *name, float value);
char* getData();
float* getDataFloat();
float setData(const char *name, int size, char *data);
float setDataFloat(const char *name, int size, float *data);

void initLidar(LidarData *lidarData, int size);
int readLidar(LidarData *lidarData);
void initCamera(CameraData *cameraData);
int captureCamera(CameraData *cameraData);
void getPose(Value3 *pose);
void setPose(Value3 *pose);
void getOdometry(Value3 *pose);
void setOdometry(Value3 *pose);
void getOdometryStd(Value2 *odometryStd);
void setOdometryStd(Value2 *odometryStd);
void getGPS(Value3 *gps);
void getGPSStd(Value3 *gpsStd);
void setGPSStd(Value3 *gpsStd);
void getVelocity(Value2 *velocity);
void setVelocity(Value2 *velocity);
bool getLowLevelControl();
void setLowLevelControl(bool enabled);
bool getTrace();
void setTrace(bool enabled);
void getWheels(Value2 *wheels);
void setWheels(Value2 *wheels);
bool getManualController();
void setManualController(bool enabled);

#endif