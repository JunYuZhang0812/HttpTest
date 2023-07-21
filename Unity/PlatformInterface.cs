public class PlatformInterface
{
    //only ios
    public static string IOSGetAddressInfo(string host)
    {
#if UNITY_IPHONE
		    return lsGetAddressInfo(host);
#else
        return string.Empty;
#endif
    }
}