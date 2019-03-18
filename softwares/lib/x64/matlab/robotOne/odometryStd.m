function std = odometryStd(handler, std)
    if nargin < 2
        std = [0, 0];
        std(1) = getRobotOne(handler, 'Odometry.Std.Linear');
        std(2) = getRobotOne(handler, 'Odometry.Std.Angular');
    else
        std(1) = setRobotOne(handler, 'Odometry.Std.Linear', std(1));
        std(2) = setRobotOne(handler, 'Odometry.Std.Angular', std(2));
    end
end