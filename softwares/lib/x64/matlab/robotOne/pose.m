function p = pose(handler, p)
    if nargin < 2
        p = [0, 0, 0];
        p(1) = getRobotOne(handler, 'Pose.X');
        p(2) = getRobotOne(handler, 'Pose.Y');
        p(3) = getRobotOne(handler, 'Pose.Theta');
    else
        p(1) = setRobotOne(handler, 'Pose.X', p(1));
        p(2) = setRobotOne(handler, 'Pose.Y', p(2));
        p(3) = setRobotOne(handler, 'Pose.Theta', p(3));
    end
end