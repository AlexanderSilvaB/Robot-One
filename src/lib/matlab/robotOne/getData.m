function data = getData(handler)
    if libisloaded('librobotOne') && handler == 1
        data = calllib('librobotOne', 'getData');
    else
        data = 0;
    end
end