function disconnectRobotOne(handler)
    if libisloaded('librobotOne')
        
        if handler == 1
            disp('Disconnecting Robot One')
            handler = calllib('librobotOne', 'disconnectRobotOne');
            if handler == 1
                disp('Disconnected')
            else
                disp('Not disconnected')
            end
        end
        
        unloadlibrary('librobotOne')
        disp('Unloaded Robot One')
    end
end