function v = versionMigo(handler)
    if libisloaded('libmigo') && handler == 1
        v = calllib('libmigo', 'versionMigo');
    else
        v = -1;
    end
end