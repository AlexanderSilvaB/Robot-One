function enabled = lowLevelControl(handler, enable)
    if nargin < 2
        enabled = getRobotOne(handler, 'Controller.LowLevel');
    else
        if enable
            enable = 1;
        else
            enable = 0;
        end
        enabled = setRobotOne(handler, 'Controller.LowLevel', enable);
    end
    enabled = enabled > 0;
end