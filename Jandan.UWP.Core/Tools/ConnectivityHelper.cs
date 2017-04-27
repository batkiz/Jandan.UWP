using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;

namespace Jandan.UWP.Core.Tools
{
    public class ConnectivityHelper
    {
        public static bool isInternetAvailable
        {
            get { return NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable; }
        }

        public static bool isMeteredConnection
        {
            get
            {
                if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable) return false;

                switch (NetworkHelper.Instance.ConnectionInformation.ConnectionType)
                {
                    case ConnectionType.Ethernet:
                    case ConnectionType.WiFi:
                    case ConnectionType.Unknown:
                    default:
                        if (NetworkHelper.Instance.ConnectionInformation.IsInternetOnMeteredConnection)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case ConnectionType.Data:
                        return true;
                }
            }
        }

        public static string NetworkStatus()
        {
            if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                return "无网络连接";
            }

            switch (NetworkHelper.Instance.ConnectionInformation.ConnectionType)
            {
                case ConnectionType.Ethernet:
                    if (NetworkHelper.Instance.ConnectionInformation.IsInternetOnMeteredConnection)
                    {
                        return "按流量计费的以太网连接";
                    }
                    else
                    {
                        return "以太网";
                    }
                case ConnectionType.WiFi:
                    if (NetworkHelper.Instance.ConnectionInformation.IsInternetOnMeteredConnection)
                    {
                        return "按流量计费的WiFi";
                    }
                    else
                    {
                        return "WiFi";
                    }
                case ConnectionType.Data:
                    return "蜂窝网络";
                case ConnectionType.Unknown:
                    return "未知网络类型";
                default:
                    break;
            }

            return "无法识别的网络类型";
        }
    }
}
