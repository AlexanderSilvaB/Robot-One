function r = setRobotOne(handler, name, value)
    if libisloaded('librobotOne') && handler == 1
        r = calllib('librobotOne', 'set', name, single(value));
    else
        r = 0;
    end
end