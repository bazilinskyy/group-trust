function mappedValue = mapAngleToValue(angleDegrees)
    % Normalize the angle to a 0-1 range
    normalizedAngle = angleDegrees / 360;
    
    % Map the normalized value to the 1-100 range
    mappedValue = normalizedAngle * 99 + 1;
end
