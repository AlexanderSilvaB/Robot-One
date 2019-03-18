function angle = fixAngle(angle)
    angle = mod(angle, 2*pi);
    if angle > pi
        angle = angle - (2 * pi);
    end
end