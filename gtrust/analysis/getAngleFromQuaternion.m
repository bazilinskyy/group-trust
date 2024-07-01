function angleDegrees = getAngleFromQuaternion(q)
    % Ensure the quaternion is normalized
    q = q / norm(q);
    
    % Calculate the angle in radians
    angleRadians = 2 * acos(q(4)); % q(4) is the w component of the quaternion
    
    % Convert the angle to degrees
    angleDegrees = rad2deg(angleRadians);
end
