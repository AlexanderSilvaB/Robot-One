clear; close all; clc;
disconnectRobotOne(1);

handler = connectRobotOne('127.0.0.1');
traceRobotOne(handler, true);

MaxV = 5.0;

for n = 1:1000
   rgb = captureCamera(handler);
   hsv = rgb2hsv(rgb);
   
   minval = [0 0 0]/255;
   maxval = [255 90 180]/255;

   mask = true(size(hsv,1), size(hsv,2));
   for p = 1 : 3
        mask = mask & (hsv(:,:,p) >= minval(p) & hsv(:,:,p) <= maxval(p));
   end
   
   lineMask = mask(150:160, :);
   [row, col] = find(lineMask);
   
   w = 0;
   if length(row) > 0
        mrow = sum(row)/length(row);
        mcol = sum(col)/length(col);
        x = mcol;
        y = mrow + 150;
        w = 0.6*atan2(160-x, 80);
        disp(w)
   end
   velocity(handler, [MaxV w]);
   
   figure(1)
   imshow(rgb)
   figure(2)
   imshow(mask)
   waitRobotOne(handler)
end

traceRobotOne(handler, false);
disconnectRobotOne(handler);