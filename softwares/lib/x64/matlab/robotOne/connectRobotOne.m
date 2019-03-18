function handler = connectRobotOne(address)
    
    if nargin < 1
        address = '127.0.0.1';
    end

    v = ['R' version('-release')];
    if not(libisloaded('librobotOne'))
        try
            loadlibrary(['librobotOne_' v], 'robotOne.h', 'alias', 'librobotOne');
            disp('Loaded Robot One')
        catch EX
            handler = 0;
            return
        end
    end
    
    disp('Connecting Robot One')
    handler = calllib('librobotOne', 'connectRobotOne', address);
    if handler == 1
        disp('Connected')
    else
        disp('Not connected')
    end
end