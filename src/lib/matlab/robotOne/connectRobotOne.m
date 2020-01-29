function handler = connectRobotOne(address)
    
    if nargin < 1
        address = '127.0.0.1';
    end

    v = ['R' version('-release')];
    if not(libisloaded('librobotOne'))
        try
            if ispc
                strLib = ['librobotOne_' v];
            else
                strLib = ['librobotOne'];
            end
            loadlibrary(strLib, 'robotOne.h', 'alias', 'librobotOne');
            disp('Loaded Robot One')
        catch EX1
            try
                if ispc
                    strLib = ['C:/robot-one/lib/matlab/librobotOne_' v];
                else
                    strLib = ['/usr/local/lib/librobotOne.so'];
                end
                loadlibrary(strLib, 'robotOne.h', 'alias', 'librobotOne');
                disp('Loaded Robot One')
            catch EX
                txt = getReport(EX);
                disp(txt)
                handler = 0;
                return
            end
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