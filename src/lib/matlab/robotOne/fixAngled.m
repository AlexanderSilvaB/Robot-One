function angle = fixAngled(angle)
    angle = mod(angle, 360);
    if angle > 180
        angle = angle - 360;
    end
end