function enabled = manualRobotOne(handler, enable)
    if nargin < 2
        enabled = getRobotOne(handler, 'Controller.Manual');
    else
        if enable
            enable = 1;
        else
            enable = 0;
        end
        enabled = setRobotOne(handler, 'Controller.Manual', enable);
    end
    enabled = enabled > 0;
end