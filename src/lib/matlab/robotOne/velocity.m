function vel = velocity(handler, vel)
    if nargin < 2
        vel = [0, 0];
        vel(1) = getRobotOne(handler, 'Velocity.Linear');
        vel(2) = getRobotOne(handler, 'Velocity.Angular');
    else
        vel(1) = setRobotOne(handler, 'Velocity.Linear', vel(1));
        vel(2) = setRobotOne(handler, 'Velocity.Angular', vel(2));
    end
end