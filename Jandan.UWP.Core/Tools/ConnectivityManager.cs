using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace Jandan.UWP.Core.Tools
{
    public enum NetworkType { NotConnected, Unknown, Wwan2G, Wwan3G, Wwan4G, WLAN, LAN };

    public class ConnectivityManager
    {
        private static ConnectivityManager _current;
        public static ConnectivityManager Current
        { get { if (null == _current) { _current = new ConnectivityManager(); }return _current; } }

        public NetworkType Network { get; private set; }

        public string NetworkTitle
        {
            get
            {
                switch (Network)
                {
                    case NetworkType.NotConnected:
                        return "无网络连接";
                    case NetworkType.Unknown:
                        return "无法识别的网络";
                    case NetworkType.Wwan2G:
                        return "2G";
                    case NetworkType.Wwan3G:
                        return "3G";
                    case NetworkType.Wwan4G:
                        return "4G";
                    case NetworkType.WLAN:
                        return "Wifi";
                    case NetworkType.LAN:
                        return "LAN";
                    default:
                        return "无网络连接";
                }
            }
        }

        public event NetworkStatusChangedEventHandler NetworkStatusChanged;

        public ConnectivityManager()
        {
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
            Network = GetConnectionGeneration();
        }

        private NetworkType GetConnectionGeneration()
        {
            bool isConnected = false;
            NetworkType nt = NetworkType.Unknown;

            ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile == null)
            {
                nt = NetworkType.NotConnected;
            }
            else
            {
                NetworkConnectivityLevel cl = profile.GetNetworkConnectivityLevel();
                isConnected = (cl != NetworkConnectivityLevel.None);
            }

            if (!isConnected)
            {
                nt = NetworkType.NotConnected;
                return nt;
            }

            if (profile.IsWwanConnectionProfile)
            {
                if (null == profile.WwanConnectionProfileDetails)
                {
                    nt = NetworkType.Unknown;
                }
                WwanDataClass connectionClass = profile.WwanConnectionProfileDetails.GetCurrentDataClass();
                switch (connectionClass)
                {
                    // 2G
                    case WwanDataClass.Gprs:
                    case WwanDataClass.Edge:
                        nt = NetworkType.Wwan2G;
                        break;
                    // 3G
                    case WwanDataClass.Cdma1xRtt:
                    case WwanDataClass.Cdma1xEvdo:
                    case WwanDataClass.Cdma1xEvdoRevA:
                    case WwanDataClass.Cdma1xEvdv:
                    case WwanDataClass.Cdma3xRtt:
                    case WwanDataClass.Cdma1xEvdoRevB:
                    case WwanDataClass.CdmaUmb:
                    case WwanDataClass.Hsdpa:
                    case WwanDataClass.Hsupa:
                    case WwanDataClass.Umts:
                        nt = NetworkType.Wwan3G;
                        break;
                    // 4G
                    case WwanDataClass.LteAdvanced:
                        nt = NetworkType.Wwan4G;
                        break;

                    // No Internet Connection
                    case WwanDataClass.None:
                        nt = NetworkType.Unknown;
                        break;                    
                    
                    case WwanDataClass.Custom:
                    default:
                        nt = NetworkType.Unknown;
                        break;
                }
            }
            else if (profile.IsWlanConnectionProfile)
            {
                nt = NetworkType.WLAN;
            }
            else
            {
                //不是Wifi也不是蜂窝数据判断为Lan
                nt = NetworkType.LAN;
            }

            return nt;
        }



        private void NetworkInformation_NetworkStatusChanged(object sender)
        {
            Network = GetConnectionGeneration();
            NetworkStatusChanged?.Invoke(this);
        }
    }
}
