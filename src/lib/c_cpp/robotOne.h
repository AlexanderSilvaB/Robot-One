#ifndef ROBOT_ONE_H_
#define ROBOT_ONE_H_

#define extern_c extern "C" __declspec(dllexport)

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

extern_c int connectRobotOne(char *address);
extern_c int disconnectRobotOne();
extern_c float versionRobotOne();
extern_c float waitRobotOne();
extern_c float get(char *name);
extern_c float set(char *name, float value);
extern_c char* getData();
extern_c float* getDataFloat();
extern_c float setData(char *name, int size, char *data);
extern_c float setDataFloat(char *name, int size, float *data);

extern_c void initLidar(LidarData *lidarData, int size);
extern_c void readLidar(LidarData *lidarData);
extern_c void initCamera(CameraData *cameraData);
extern_c void captureCamera(CameraData *cameraData);
extern_c void getPose(Value3 *pose);
extern_c void setPose(Value3 *pose);
extern_c void getOdometry(Value3 *pose);
extern_c void setOdometry(Value3 *pose);
extern_c void getOdometryStd(Value2 *odometryStd);
extern_c void setOdometryStd(Value2 *odometryStd);
extern_c void getVelocity(Value2 *velocity);
extern_c void setVelocity(Value2 *velocity);
extern_c bool getLowLevelControl();
extern_c void setLowLevelControl(bool enabled);
extern_c bool getTrace();
extern_c void setTrace(bool enabled);
extern_c void getWheels(Value2 *wheels);
extern_c void setWheels(Value2 *wheels);

#endif