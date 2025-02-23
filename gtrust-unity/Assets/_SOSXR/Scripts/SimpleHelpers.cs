using System.Net;
using UnityEngine;


public static class SimpleHelpers
{
    public static string GetLocalIP(string startsWith = "192")
    {
        var localIPs = Dns.GetHostAddresses(Dns.GetHostName());

        foreach (var local in localIPs)
        {
            if (local.ToString().StartsWith(startsWith))
            {
                Debug.Log(local);

                return local.ToString();
            }
        }

        return "No_IP_found";
    }
}