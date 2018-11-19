clear; close all; clc;
disconnectMigo(1);

handler = connectMigo('127.0.0.1');
traceMigo(handler, true);

for n = 1:100
   readings = readLidar(handler)
   
   plot(readings.pos(:,1), readings.pos(:,2))
   grid on
   axis equal
   drawnow
   
   
   w = deg2rad(20)*randn();
   p = 4.0;
   velocity(handler, [p w]);
   waitMigo(handler)
end

traceMigo(handler, false);
disconnectMigo(handler);