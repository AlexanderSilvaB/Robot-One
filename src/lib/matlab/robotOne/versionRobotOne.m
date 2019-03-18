function v = versionRobotOne(handler)
    if libisloaded('librobotOne') && handler == 1
        v = calllib('librobotOne', 'versionRobotOne');
    else
        v = -1;
    end
end