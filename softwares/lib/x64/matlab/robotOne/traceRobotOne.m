function enabled = traceRobotOne(handler, enable)
    if nargin < 2
        enabled = getRobotOne(handler, 'Trace');
    else
        if enable
            enable = 1;
        else
            enable = 0;
        end
        enabled = setRobotOne(handler, 'Trace', enable);
    end
    enabled = enabled > 0;
end