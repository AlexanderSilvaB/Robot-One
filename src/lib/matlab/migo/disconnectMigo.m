function disconnectMigo(handler)
    if libisloaded('libmigo')
        
        if handler == 1
            disp('Disconnecting Migo')
            handler = calllib('libmigo', 'disconnectMigo');
            if handler == 1
                disp('Disconnected')
            else
                disp('Not disconnected')
            end
        end
        
        unloadlibrary('libmigo')
        disp('Unloaded Migo')
    end
end