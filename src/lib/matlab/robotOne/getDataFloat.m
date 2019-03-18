function data = getData(handler)
    if libisloaded('librobotOne') && handler == 1
        data = calllib('librobotOne', 'getDataFloat');
    else
        data = 0;
    end
end