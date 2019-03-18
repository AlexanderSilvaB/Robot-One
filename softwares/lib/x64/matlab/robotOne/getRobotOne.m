function r = getRobotOne(handler, name)
    if libisloaded('librobotOne') && handler == 1
        r = calllib('librobotOne', 'get', name);
    else
        r = 0;
    end
end