function r = setMigo(handler, name, value)
    if libisloaded('libmigo') && handler == 1
        r = calllib('libmigo', 'set', name, single(value));
    else
        r = 0;
    end
end