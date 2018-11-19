function t = waitMigo(handler)
    if libisloaded('libmigo') && handler == 1
        t = calllib('libmigo', 'waitMigo');
    else
        t = 0;
    end
end