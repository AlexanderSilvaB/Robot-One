function p = odometry(handler)
    p = [0, 0, 0];
    p(1) = getRobotOne(handler, 'Odometry.X');
    p(2) = getRobotOne(handler, 'Odometry.Y');
    p(3) = getRobotOne(handler, 'Odometry.Theta');
end