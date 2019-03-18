function std = odometryStd(handler, std)
    if nargin < 2
        std = [0, 0];
        std(1) = getMigo(handler, 'Odometry.Std.Linear');
        std(2) = getMigo(handler, 'Odometry.Std.Angular');
    else
        std(1) = setMigo(handler, 'Odometry.Std.Linear', std(1));
        std(2) = setMigo(handler, 'Odometry.Std.Angular', std(2));
    end
end