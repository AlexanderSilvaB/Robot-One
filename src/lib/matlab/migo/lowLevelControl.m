function enabled = lowLevelControl(handler, enable)
    if nargin < 2
        enabled = getMigo(handler, 'Controller.LowLevel');
    else
        if enable
            enable = 1;
        else
            enable = 0;
        end
        enabled = setMigo(handler, 'Controller.LowLevel', enable);
    end
    enabled = enabled > 0;
end