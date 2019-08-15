function image = captureCamera(handler)
    if libisloaded('librobotOne') && handler == 1
        sz = getRobotOne(handler, 'Camera.Capture');
        data = getDataFloat(handler);
        setdatatype(data, 'uint8Ptr', sz);
        image = reshape(fliplr(reshape(data.value, 3, numel(data.value)/3).'),[320 240 3]);
        % image = flip(permute(image, [2 1 3]), 1);
	% image = flip(image, 2);
	image = permute(image, [2 1 3]);
        tmp = image(:,:,1);
        image(:,:,1) = image(:,:,3);
        image(:,:,3) = tmp;
    else
        image = zeros(320, 240, 3, 'uint8');
    end
end