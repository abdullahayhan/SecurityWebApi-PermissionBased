using System.Net;

namespace WebApi.Helpers
{
    public static class IpAddressHelper
    {
        public static string GetIpAddress(IPAddress? remoteIpAddress)
        {
            string ip = string.Empty;

            if (remoteIpAddress != null)
            {
                if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                        .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
                ip = remoteIpAddress.ToString();
            }

            return ip;
        }
    }
}
