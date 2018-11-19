function vel = wheels(handler, vel)
    if nargin < 2
        vel = [0, 0];
        vel(1) = getMigo(handler, 'Velocity.Angular.Left');
        vel(2) = getMigo(handler, 'Velocity.Angular.Right');
    else
        vel(1) = setMigo(handler, 'Velocity.Angular.Left', vel(1));
        vel(2) = setMigo(handler, 'Velocity.Angular.Right', vel(2));
    end
end