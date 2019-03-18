function t = waitRobotOne(handler)
    if libisloaded('librobotOne') && handler == 1
        t = calllib('librobotOne', 'waitRobotOne');
    else
        t = 0;
    end
end