function data = getData(handler)
    if libisloaded('libmigo') && handler == 1
        data = calllib('libmigo', 'getData');
    else
        data = 0;
    end
end