using System;


[Serializable]
public struct SOSXRData
{
    public string FocusName;


    public SOSXRData(string focusName)
    {
        FocusName = focusName;
    }
}