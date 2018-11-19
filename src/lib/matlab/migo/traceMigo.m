function enabled = traceMigo(handler, enable)
    if nargin < 2
        enabled = getMigo(handler, 'Trace');
    else
        if enable
            enable = 1;
        else
            enable = 0;
        end
        enabled = setMigo(handler, 'Trace', enable);
    end
    enabled = enabled > 0;
end