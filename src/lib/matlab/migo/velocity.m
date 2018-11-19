function vel = velocity(handler, vel)
    if nargin < 2
        vel = [0, 0];
        vel(1) = getMigo(handler, 'Velocity.Linear');
        vel(2) = getMigo(handler, 'Velocity.Angular');
    else
        vel(1) = setMigo(handler, 'Velocity.Linear', vel(1));
        vel(2) = setMigo(handler, 'Velocity.Angular', vel(2));
    end
end