function readings = readLidar(handler)
    if libisloaded('librobotOne') && handler == 1
        sz = getRobotOne(handler, 'Lidar.Read');
        readings.size = sz;
        if sz == 0
            readings.distances = 0;
            readings.angles = 0;
            readings.pos = 0;
            return;
        end

        data = getDataFloat(handler);
        setdatatype(data, 'singlePtr', sz);

        readings.distances = data.value;
        readings.distances = readings.distances';
        inc = pi / sz;
        start = -double(int32(sz/2))*inc;
        readings.angles = linspace(start, -start, sz);
        inc = pi / 2;
        readings.pos = zeros(sz, 2);
        for i = 1:sz
            readings.pos(i, 1) = -readings.distances(i)*cos(inc + readings.angles(i));
            readings.pos(i, 2) =  readings.distances(i)*sin(inc + readings.angles(i));
        end
    else
        readings.size = 0;
        readings.distances = 0;
        readings.angles = 0;
        readings.pos = 0;
    end
end