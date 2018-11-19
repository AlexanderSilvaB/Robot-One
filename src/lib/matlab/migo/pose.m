function p = pose(handler, p)
    if nargin < 2
        p = [0, 0, 0];
        p(1) = getMigo(handler, 'Pose.X');
        p(2) = getMigo(handler, 'Pose.Y');
        p(3) = getMigo(handler, 'Pose.Theta');
    else
        p(1) = setMigo(handler, 'Pose.X', p(1));
        p(2) = setMigo(handler, 'Pose.Y', p(2));
        p(3) = setMigo(handler, 'Pose.Theta', p(3));
    end
end