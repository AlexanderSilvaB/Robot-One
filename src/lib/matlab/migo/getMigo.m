function r = getMigo(handler, name)
    if libisloaded('libmigo') && handler == 1
        r = calllib('libmigo', 'get', name);
    else
        r = 0;
    end
end