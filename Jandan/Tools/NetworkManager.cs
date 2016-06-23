﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace Jandan.UWP.Tools
{
    class NetworkManager
    {
        private static NetworkManager _current;
        public static NetworkManager Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new NetworkManager();
                }
                return _current;
            }
        }
        public const WwanDataClass Wwan2G = (WwanDataClass.Edge
                        | WwanDataClass.Gprs);
        public const WwanDataClass Wwan3G = (WwanDataClass.Hsupa
                        | WwanDataClass.Hsdpa
                        | WwanDataClass.Umts
                        | WwanDataClass.CdmaUmb
                        | WwanDataClass.Cdma3xRtt
                        | WwanDataClass.Cdma1xRtt
                        | WwanDataClass.Cdma1xEvdv
                        | WwanDataClass.Cdma1xEvdoRevB
                        | WwanDataClass.Cdma1xEvdoRevA
                        | WwanDataClass.Cdma1xEvdo);
        public const WwanDataClass Wwan4G = WwanDataClass.LteAdvanced;

        public string NetworkTitle
        {
            get
            {
                if (_network == 0)
                {
                    return "2G";
                }
                else if (_network == 1)
                {
                    return "3G";
                }
                else if (_network == 2)
                {
                    return "4G";
                }
                else if (_network == 3)
                {
                    return "WIFI";
                }
                else
                {
                    return "无网络访问";
                }
            }
        }

        private int _network;

        public int Network
        {
            get
            {
                return _network;
            }
        }

        public event NetworkStatusChangedEventHandler NetworkStatusChanged;

        public NetworkManager()
        {
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
            _network = GetConnectionGeneration();
            //_network = 3;
        }

        private void NetworkInformation_NetworkStatusChanged(object sender)
        {
            _network = GetConnectionGeneration();
            if (NetworkStatusChanged != null)
            {
                NetworkStatusChanged(this);
            }
        }

        /// <summary>
        ///  0:2G 1:3G 2:4G  3:wifi  4:无连接
        /// </summary>
        /// <returns></returns>
        private int GetConnectionGeneration()
        {
            try
            {
                ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();
                if (profile.IsWwanConnectionProfile)
                {
                    WwanDataClass connectionClass = profile.WwanConnectionProfileDetails.GetCurrentDataClass();
                    if((connectionClass & Wwan4G) != 0)
                    {
                        // not worse than 4G
                        return 2;
                    }
                    else if((connectionClass & Wwan3G) != 0)
                    {
                        // not worse than 3G
                        return 1;
                    }
                    else if((connectionClass & Wwan2G) != 0)
                    {
                        // not worse than 2G
                        return 0;
                    }
                    else
                    {
                        return 4;
                    }
                    //switch (connectionClass)
                    //{
                    //    //2G-equivalent
                    //    case WwanDataClass.Edge:
                    //    case WwanDataClass.Gprs:
                    //        return 0;
                    //    //3G-equivalent
                    //    case WwanDataClass.Cdma1xEvdo:
                    //    case WwanDataClass.Cdma1xEvdoRevA:
                    //    case WwanDataClass.Cdma1xEvdoRevB:
                    //    case WwanDataClass.Cdma1xEvdv:
                    //    case WwanDataClass.Cdma1xRtt:
                    //    case WwanDataClass.Cdma3xRtt:
                    //    case WwanDataClass.CdmaUmb:
                    //    case WwanDataClass.Umts:
                    //    case WwanDataClass.Hsdpa:
                    //    case WwanDataClass.Hsupa:
                    //        return 1;
                    //    //4G-equivalent
                    //    case WwanDataClass.LteAdvanced:
                    //        return 2;

                    //    //not connected
                    //    case WwanDataClass.None:
                    //        return 4;

                    //    //unknown
                    //    case WwanDataClass.Custom:
                    //    default:
                    //        return 4;
                    //}
                }
                else if (profile.IsWlanConnectionProfile)
                {
                    return 3;
                }
                else if (!string.IsNullOrEmpty(profile.ProfileName))
                {
                    return 3; // PC or something?
                }
                return 4;
            }
            catch (Exception)
            {
                return 4; //as default
            }

        }
    }
}
