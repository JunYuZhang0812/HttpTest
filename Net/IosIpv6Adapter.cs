﻿// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by pdl.
//      Mono Runtime Version: 4.0.30319.1
// 
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
//using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class IosIpv6Adapter
{
    public static Regex IPRegex = new Regex("(\\d*\\.){3}\\d*");
    /// <summary>
    /// 判断地址是IPV4地址还是域名,IPV6地址暂不用支持
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static bool IsIPorDomain(string host)
    {
        return IPRegex.IsMatch(host);
    }
    public static IPAddress[] IpAddressConvert(string hostIp, out AddressFamily af)
    {
#if !UNITY_IPHONE
        af = AddressFamily.InterNetwork;
        IPAddress[] address = Dns.GetHostAddresses("www.google.com");
        for (int i = 0; i < address.Length; i++)
        {
            if (address[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                UDebug.LogError("SocketClient", "CurNetState=IPV6");
                af = AddressFamily.InterNetworkV6;
                break;
            }
        }
        if (af == AddressFamily.InterNetworkV6)
        {
            hostIp = "::" + hostIp;
            IPAddress[] returnAddress = new IPAddress[1];
            returnAddress[0] = IPAddress.Parse(hostIp);
            return returnAddress;
        }
        else
        {
            return null;
        }
#endif

        af = AddressFamily.InterNetwork;
        var outstr = PlatformInterface.IOSGetAddressInfo(hostIp);
        UDebug.Log("IOSGetAddressInfo: " + outstr);
        if (outstr.StartsWith("ERROR"))
        {
            return null;
        }
        var addressliststr = outstr.Split('|');
        var addrlist = new List<IPAddress>();
        foreach (string s in addressliststr)
        {
            if (String.IsNullOrEmpty(s.Trim()))
                continue;
            switch (s)
            {
                case "ipv6":
                    {
                        af = AddressFamily.InterNetworkV6;
                    }
                    break;
                case "ipv4":
                    {
                        af = AddressFamily.InterNetwork;
                    }
                    break;
                default:
                    {
                        addrlist.Add(IPAddress.Parse(s));
                    }
                    break;
            }
        }
        if (af == AddressFamily.InterNetworkV6)
            return addrlist.ToArray();
        else
            return null;
    }

}

