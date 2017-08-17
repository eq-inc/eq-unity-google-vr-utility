using Eq.Unity;
using System;
using UnityEngine;

namespace Eq.GoogleVR
{
    public class TouchPadHelper
    {
        internal LogController mLogger;

        public TouchPadHelper(LogController logger)
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
                        mLaserPointer = laserGO.GetComponent<GvrLaserPointer>();
                    }
                }
            }

            if (mLaserPointer == null)
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
                return mLaserPointer.reticle != null ? mLaserPointer.reticle.transform.position : mLaserPointer.LineEndPoint;
            }
        }
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
