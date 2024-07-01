% Read the Excel file into a table
filename = '2024-06-26-17-16';
dataTable = readtable(filename);

% Display the first few rows to understand the structure
disp(dataTable(1:5, :));

% Extract the quaternion strings from column 28
quaternionStrings = dataTable{:, 28}; 

% Display the first few quaternion strings to check the format
disp(quaternionStrings(1:5));

% Initialize an array to store the parsed quaternions
numQuaternions = length(quaternionStrings);
quaternions = zeros(numQuaternions, 4); % 4 columns for x, y, z, w

% Parse each quaternion string
for i = 1:numQuaternions
    % Remove parentheses and spaces
    cleanString = erase(quaternionStrings{i}, {'(', ')', ' '});
    
    % Split the cleaned string by commas and convert to numbers
    qComponents = str2double(strsplit(cleanString, ','));
    
    % Check if the conversion was successful
    if any(isnan(qComponents)) || length(qComponents) ~= 4
        error(['Invalid quaternion data at row ', num2str(i), ': ', quaternionStrings{i}]);
    end
    
    % Store the components in the quaternions array
    quaternions(i, :) = qComponents;
end

% Filter out quaternions with zero values
validQuaternions = quaternions(all(quaternions, 2), :);

% Define the function to get angle from quaternion
function angleDegrees = getAngleFromQuaternion(q)
    % Ensure the quaternion is normalized
    qNorm = norm(q);
    if qNorm == 0
        error('Quaternion has zero norm, cannot normalize.');
    end
    q = q / qNorm;
    
    % Check for valid quaternion normalization
    if any(isnan(q))
        error('Quaternion normalization resulted in NaN values.');
    end
    
    % Calculate the angle in radians
    angleRadians = 2 * acos(q(4)); % q(4) is the w component of the quaternion
    
    % Convert the angle to degrees
    angleDegrees = rad2deg(angleRadians);
end

% Define the function to map angle to value between 1 and 100
function mappedValue = mapAngleToValue(angleDegrees)
    % Normalize the angle to a 0-1 range
    normalizedAngle = angleDegrees / 360;
    
    % Map the normalized value to the 1-100 range
    mappedValue = normalizedAngle * 99 + 1;
end

% Initialize an array to store the mapped values
numValidQuaternions = size(validQuaternions, 1);
mappedValues = zeros(numValidQuaternions, 1);

% Loop through each valid quaternion and process it
for i = 1:numValidQuaternions
    q = validQuaternions(i, :);
    
    % Check for valid quaternion components
    if any(isnan(q))
        error(['Invalid quaternion components at row ', num2str(i)]);
    end
    
    % Calculate the angle and map it to the desired range
    try
        angle = getAngleFromQuaternion(q);
        mappedValues(i) = mapAngleToValue(angle);
    catch ME
        disp(['Error processing quaternion at row ', num2str(i)]);
        disp(['Quaternion: ', num2str(q)]);
        rethrow(ME);
    end
end

% Display the mapped values
disp(mappedValues);
