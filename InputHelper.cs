﻿using Eq.Unity;
using System;
using UnityEngine;

namespace Eq.GoogleVR
{
    public class PointerDeviceHelper
    {
        internal static readonly string GvrLaserVisualTypeName = "GvrLaserVisual, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

        internal LogController mLogger;

        public PointerDeviceHelper(LogController logger)
        {
            mLogger = logger;
        }

        public ButtonPushStatus AppButtonStatus 
        {
            get
            {
                ButtonPushStatus ret = ButtonPushStatus.None;

                if (GvrController.AppButton)
                {
                    if (GvrController.AppButtonDown)
                    {
                        ret = ButtonPushStatus.Down;
                    }
                }
                else
                {
                    if (GvrController.AppButtonUp)
                    {
                        ret = ButtonPushStatus.Up;
                    }
                }

                return ret;
            }
        }
    }

    public class TouchPadHelper : PointerDeviceHelper
    {
        public TouchPadHelper(LogController logger) : base(logger)
        {
            mLogger = logger;
            if (mLogger == null)
            {
                mLogger = new LogController();
            }
        }

        public TouchStatus TouchStatus
        {
            get
            {
                TouchStatus status = TouchStatus.None;

                if (GvrController.IsTouching)
                {
                    if (GvrController.TouchDown)
                    {
                        status = TouchStatus.Down;
                    }
                }
                else
                {
                    if (GvrController.TouchUp)
                    {
                        status = TouchStatus.Up;
                    }
                }

                return status;
            }
        }

        public Vector2 TouchPositionOnTouchPad
        {
            get
            {
                return GvrController.TouchPos;
            }
        }
    }

    public class TouchPadClickHelper : TouchPadHelper
    {
        private GvrLaserPointer mLaserPointer;
        private Component mLaserVisual;

        public TouchPadClickHelper(LogController logger) : this(logger, "GvrControllerPointer", "Laser")
        {
        }

        public TouchPadClickHelper(LogController logger, string controllerPointerName, string laserName) : base(logger)
        {
            GameObject controllerPointerGO = GameObject.Find(controllerPointerName);
            GameObject laserGO = null;

            if (controllerPointerGO != null)
            {
                Transform laserTF = controllerPointerGO.transform.Find(laserName);
                if (laserTF != null)
                {
                    laserGO = laserTF.gameObject;
                    if (laserGO != null)
                    {
                        if(Common.GvrSdkVersion >= Common.GoogleVRSDKVersion.v1_100_0)
                        {
                            mLaserVisual = laserGO.GetComponent(GvrLaserVisualTypeName);
                        }
                        else
                        {
                            mLaserPointer = laserGO.GetComponent<GvrLaserPointer>();
                        }
                    }
                }
            }

            if ((mLaserPointer == null) && (mLaserVisual == null))
            {
                throw new ArgumentException("not found GameObject: " + (controllerPointerGO == null ? controllerPointerName : laserName));
            }
        }

        public ClickStatus ClickStatus
        {
            get
            {
                ClickStatus status = ClickStatus.None;

                if (GvrController.ClickButton)
                {
                    if (GvrController.ClickButtonDown)
                    {
                        status = ClickStatus.Down;
                    }
                }
                else
                {
                    if (GvrController.ClickButtonUp)
                    {
                        status = ClickStatus.Up;
                    }
                }

                return status;
            }
        }

        public GameObject ClickedGameObject
        {
            get
            {
                GameObject ret = null;
                Ray ray = Camera.main.ScreenPointToRay(ClickScreenPosition);
                RaycastHit raycastHit = new RaycastHit();

                if (Physics.Raycast(ray, out raycastHit))
                {
                    ret = raycastHit.collider.gameObject;
                }

                return ret;
            }
        }
        
        public Vector3 ClickScreenPosition
        {
            get
            {
                mLogger.CategoryLog(LogController.LogCategoryMethodIn, "pointer position(" + ClickWorldPosition.x + ", " + ClickWorldPosition.y + ", " + ClickWorldPosition.z + ")");

                Vector3 ret = Camera.main.WorldToScreenPoint(ClickWorldPosition);

                mLogger.CategoryLog(LogController.LogCategoryMethodOut, "click world position(" + ret.x + ", " + ret.y + ", " + ret.z + ")");
                return ret;
            }
        }

        public Vector3 ClickWorldPosition
        {
            get
            {
                if(Common.GvrSdkVersion >= Common.GoogleVRSDKVersion.v1_100_0)
                {
                    Type laserVisualType = Type.GetType(GvrLaserVisualTypeName);
                    GameObject reticle = laserVisualType.GetField("reticle").GetValue(mLaserVisual) as GameObject;

                    if(reticle != null)
                    {
                        return reticle.transform.position;
                    }
                    else
                    {
                        return (Vector3)(laserVisualType.GetProperty("Laser").GetValue(mLaserVisual, null));
                    }
                }
                else
                {
                    Type laserPointerType = mLaserPointer.GetType();
                    GameObject reticle = laserPointerType.GetField("reticle").GetValue(mLaserPointer) as GameObject;
                    if(reticle != null)
                    {
                        return reticle.transform.position;
                    }
                    else
                    {
                        return (Vector3)(laserPointerType.GetProperty("LineEndPoint").GetValue(mLaserPointer, null));
                    }
                }
            }
        }
    }

    public enum ButtonPushStatus
    {
        None, Down, Up
    }

    public enum ClickStatus
    {
        None, Down, Up
    }

    public enum TouchStatus
    {
        None, Down, Up
    }
}
