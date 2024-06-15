using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Events;

public class LocalIPFetcher : MonoBehaviour
{
    public List<string> m_localIpFound;
    public UnityEvent<string[]> m_onLocalIpFound = new UnityEvent<string[]>();

    private void Start()
    {
        RefreshLocalIp();
    }


    [ContextMenu("Refresh Local IP")]
    public void RefreshLocalIp()
    {
        m_localIpFound = GetLocalIPv4Addresses();
    }

    private List<string> GetLocalIPv4Addresses()
    {
        List<string> ipAddresses = new List<string>();

        // Get all network interfaces
        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface networkInterface in networkInterfaces)
        {
            // Filter out loopback and non-operational interfaces
            if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                networkInterface.OperationalStatus != OperationalStatus.Up)
            {
                continue;
            }

            IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
            foreach (UnicastIPAddressInformation ipAddressInfo in ipProperties.UnicastAddresses)
            {

                if (ipAddressInfo.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    continue;
                }
                ipAddresses.Add(ipAddressInfo.Address.ToString());
            }
        }

        return ipAddresses;
    }
}