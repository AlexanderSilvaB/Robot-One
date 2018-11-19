handler = connectMigo('127.0.0.1');
traceMigo(handler, true);

Kp = 0.1818181818181818;
Ka = 0.4191980558930741;
Kb = -0.1818181818181818;

MaxV = 100.0;
MaxD = 0.2;

GL = [  0, 0, 0
        0, 20, 0
        20, 20, 0
        20, 0, 0
        0, 0, 0];
    
for i = 1:size(GL, 1)
   G = GL(i, :); 
   while true
       R = pose(handler);
       dx = G(1) - R(1);
       dy = G(2) - R(2);
       p = sqrt(dx*dx + dy*dy);
       if p > MaxD
           gamma = atan2(dy, dx);
           alpha = fixAngle(gamma - R(3));
           beta = fixAngle(G(3) - gamma);
           v = min(Kp * p, MaxV);
           w = Ka * alpha + Kb * beta;
           velocity(handler, [v, w]);
       else
           break
       end
       t = waitMigo(handler)
   end
end

velocity(handler, [0, 0]);
R = pose(handler);
disp(G);
disp(R);

traceMigo(handler, false);
disconnectMigo(handler);