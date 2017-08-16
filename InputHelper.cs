using Eq.Unity;
using System;
using UnityEngine;

namespace Eq.GoogleVR
{
    public class InputHelper
    {
        private GvrLaserPointer mLaserPointer;
        private LogController mLogger;

        public InputHelper(LogController logger) : this(logger, "GvrControllerPointer", "Laser")
        {
        }

        public InputHelper(LogController logger, string controllerPointerName, string laserName)
        {
            GameObject controllerPointerGO = GameObject.Find(controllerPointerName);
            GameObject laserGO = null;

            mLogger = logger;
            if(mLogger == null)
            {
                mLogger = new LogController();
            }

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
}
