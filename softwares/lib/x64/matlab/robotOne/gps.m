function p = gps(handler)
    p = [0, 0, 0];
    p(1) = getRobotOne(handler, 'GPS.X');
    p(2) = getRobotOne(handler, 'GPS.Y');
    p(3) = getRobotOne(handler, 'GPS.Theta');
end