clear; close all; clc;
disconnectRobotOne(1);

handler = connectRobotOne('127.0.0.1');
traceRobotOne(handler, true);

Vlinear = 10.0
Radius = 10.0
Vangular = Vlinear / Radius

for i = 1:200
    velocity(handler, [Vlinear Vangular]);
    waitRobotOne(handler)
    R = pose(handler);
    disp(R)
end

velocity(handler, [0 0]);
traceRobotOne(handler, false);
disconnectRobotOne(handler);