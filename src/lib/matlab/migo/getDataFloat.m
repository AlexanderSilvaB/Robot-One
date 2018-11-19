function data = getData(handler)
    if libisloaded('libmigo') && handler == 1
        data = calllib('libmigo', 'getDataFloat');
    else
        data = 0;
    end
end