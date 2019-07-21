function std = gpsStd(handler, std)
    if nargin < 2
        std = [0, 0, 0];
        std(1) = getRobotOne(handler, 'GPS.Std.X');
        std(2) = getRobotOne(handler, 'GPS.Std.Y');
        std(3) = getRobotOne(handler, 'GPS.Std.Theta');
    else
        std(1) = setRobotOne(handler, 'GPS.Std.X', std(1));
        std(2) = setRobotOne(handler, 'GPS.Std.Y', std(2));
        std(3) = setRobotOne(handler, 'GPS.Std.Theta', std(3));
    end
end