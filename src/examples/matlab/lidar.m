clear; close all; clc;
disconnectRobotOne(1);

handler = connectRobotOne('127.0.0.1');
traceRobotOne(handler, true);

for n = 1:100
   readings = readLidar(handler)
   
   plot(readings.pos(:,1), readings.pos(:,2))
   grid on
   axis equal
   drawnow
   
   
   w = deg2rad(20)*randn();
   p = 4.0;
   velocity(handler, [p w]);
   waitRobotOne(handler)
end

traceRobotOne(handler, false);
disconnectRobotOne(handler);