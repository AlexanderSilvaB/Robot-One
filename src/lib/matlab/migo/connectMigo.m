function handler = connectMigo(address)
    
    if nargin < 1
        address = '127.0.0.1';
    end

    v = ['R' version('-release')];
    if not(libisloaded('libmigo'))
        try
            loadlibrary(['libmigo_' v], 'migo.h', 'alias', 'libmigo');
            disp('Loaded Migo')
        catch EX
            handler = 0;
            return
        end
    end
    
    disp('Connecting Migo')
    handler = calllib('libmigo', 'connectMigo', address);
    if handler == 1
        disp('Connected')
    else
        disp('Not connected')
    end
end