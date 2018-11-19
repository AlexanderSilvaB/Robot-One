function p = odometry(handler)
    p = [0, 0, 0];
    p(1) = getMigo(handler, 'Odometry.X');
    p(2) = getMigo(handler, 'Odometry.Y');
    p(3) = getMigo(handler, 'Odometry.Theta');
end