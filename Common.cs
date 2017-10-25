﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eq.GoogleVR
{
    public class Common
    {
        private Common()
        {
            // 処理なし
        }

        public enum GoogleVRSDKVersion
        {
            Init = -1,
            v1_70_0 = 0,
            v1_100_0
        };

        private static GoogleVRSDKVersion _GvrSdkVersion = GoogleVRSDKVersion.Init;
        public static GoogleVRSDKVersion GvrSdkVersion
        {
            get
            {
                return _GvrSdkVersion;
            }
            set
            {
                if(_GvrSdkVersion == GoogleVRSDKVersion.Init)
                {
                    _GvrSdkVersion = value;
                }
            }
        }
    }
}
