using Eq.Unity;
using System.Collections.Generic;

namespace Eq.GoogleVR
{
    public class Permissions
    {

    }

    public class RuntimePermissionHelper
    {
        public readonly static Permission ACCESS_COARSE_LOCATION = new Permission("android.permission.ACCESS_COARSE_LOCATION");
        public readonly static Permission ACCESS_FINE_LOCATION = new Permission("android.permission.ACCESS_FINE_LOCATION");
        public readonly static Permission ADD_VOICEMAIL = new Permission("android.permission.ADD_VOICEMAIL");
        public readonly static Permission BODY_SENSORS = new Permission("android.permission.BODY_SENSORS");
        public readonly static Permission CALL_PHONE = new Permission("android.permission.CALL_PHONE");
        public readonly static Permission CAMERA = new Permission("android.permission.CAMERA");
        public readonly static Permission GET_ACCOUNTS = new Permission("android.permission.GET_ACCOUNTS");
        public readonly static Permission PROCESS_OUTGOING_CALLS = new Permission("android.permission.PROCESS_OUTGOING_CALLS");
        public readonly static Permission READ_CALENDAR = new Permission("android.permission.READ_CALENDAR");
        public readonly static Permission READ_CALL_LOG = new Permission("android.permission.READ_CALL_LOG");
        public readonly static Permission READ_CONTACTS = new Permission("android.permission.READ_CONTACTS");
        public readonly static Permission READ_EXTERNAL_STORAGE = new Permission("android.permission.READ_EXTERNAL_STORAGE");
        public readonly static Permission READ_PHONE_STATE = new Permission("android.permission.READ_PHONE_STATE");
        public readonly static Permission READ_SMS = new Permission("android.permission.READ_SMS");
        public readonly static Permission RECEIVE_MMS = new Permission("android.permission.RECEIVE_MMS");
        public readonly static Permission RECEIVE_SMS = new Permission("android.permission.RECEIVE_SMS");
        public readonly static Permission RECEIVE_WAP_PUSH = new Permission("android.permission.RECEIVE_WAP_PUSH");
        public readonly static Permission RECORD_AUDIO = new Permission("android.permission.RECORD_AUDIO");
        public readonly static Permission SEND_SMS = new Permission("android.permission.SEND_SMS");
        public readonly static Permission USE_SIP = new Permission("android.permission.USE_SIP");
        public readonly static Permission WRITE_CALENDAR = new Permission("android.permission.WRITE_CALENDAR");
        public readonly static Permission WRITE_CALL_LOG = new Permission("android.permission.WRITE_CALL_LOG");
        public readonly static Permission WRITE_CONTACTS = new Permission("android.permission.WRITE_CONTACTS");
        public readonly static Permission WRITE_EXTERNAL_STORAGE = new Permission("android.permission.WRITE_EXTERNAL_STORAGE");

        private LogController mLogger;

        public RuntimePermissionHelper(LogController logger)
        {
            mLogger = logger;
            if (logger == null)
            {
                mLogger = new LogController();
            }
        }

        /// <summary>
        /// check permission(return the result of GvrPermissionsRequester.IsPermissionGranted)
        /// </summary>
        /// <param name="targetPermission">target for check permission</param>
        /// <returns>True if it is allowed, False it it is not alllowed</returns>
        public bool CheckPermission(Permission targetPermission)
        {
            mLogger.CategoryLog(LogController.LogCategoryMethodIn, targetPermission);

            bool ret = false;
            GvrPermissionsRequester requester = GvrPermissionsRequester.Instance;

            if (requester != null)
            {
                ret = requester.IsPermissionGranted(targetPermission.Name);
            }

            mLogger.CategoryLog(LogController.LogCategoryMethodOut, targetPermission + ": " + ret);
            return ret;
        }

        /// <summary>
        /// check permissions(return the result of GvrPermissionsRequester.HasPermissionsGranted)
        /// </summary>
        /// <param name="targetPermissions">target for check permissions</param>
        /// <returns>the results for each permission</returns>
        public bool[] CheckPermission(Permission[] targetPermissions)
        {
            mLogger.CategoryLog(LogController.LogCategoryMethodIn, targetPermissions);

            bool[] retArray = new bool[targetPermissions.Length];
            GvrPermissionsRequester requester = GvrPermissionsRequester.Instance;

            if (requester != null)
            {
                for (int i = 0, size = targetPermissions.Length; i < size; i++)
                {
                    retArray[i] = requester.IsPermissionGranted(targetPermissions[i].Name);
                }
            }

            mLogger.CategoryLog(LogController.LogCategoryMethodOut, retArray);
            return retArray;
        }

        public void RequestPermission(Permission targetPermission, System.Action<GvrPermissionsRequester.PermissionStatus[]> callback)
        {
            mLogger.CategoryLog(LogController.LogCategoryMethodIn, targetPermission);
            GvrPermissionsRequester requester = GvrPermissionsRequester.Instance;

            if (requester != null)
            {
                if (CheckPermission(targetPermission))
                {
                    callback(new GvrPermissionsRequester.PermissionStatus[] { new GvrPermissionsRequester.PermissionStatus(targetPermission.Name, true) });
                }
                else
                {
                    requester.RequestPermissions(new string[] { targetPermission.Name }, callback);
                }
            }
            else
            {
                callback(new GvrPermissionsRequester.PermissionStatus[] { new GvrPermissionsRequester.PermissionStatus(targetPermission.Name, false) });
            }

            mLogger.CategoryLog(LogController.LogCategoryMethodOut, targetPermission);
        }

        public void RequestPermission(Permission[] targetPermissions, System.Action<GvrPermissionsRequester.PermissionStatus[]> callback)
        {
            mLogger.CategoryLog(LogController.LogCategoryMethodIn, targetPermissions);
            GvrPermissionsRequester requester = GvrPermissionsRequester.Instance;

            if (requester != null)
            {
                bool[] tempRetArray = CheckPermission(targetPermissions);
                bool allGranted = true;
                foreach(bool tempRet in tempRetArray)
                {
                    allGranted &= tempRet;
                }

                if (allGranted)
                {
                    GvrPermissionsRequester.PermissionStatus[] permissiontStatusArray = new GvrPermissionsRequester.PermissionStatus[targetPermissions.Length];
                    for (int i = 0, size = targetPermissions.Length; i < size; i++)
                    {
                        permissiontStatusArray[i] = new GvrPermissionsRequester.PermissionStatus(targetPermissions[i].Name, true);
                    }
                    callback(permissiontStatusArray);
                }
                else
                {
                    string[] targetPermissionStrArray = new string[targetPermissions.Length];
                    for (int i=0, size=targetPermissions.Length; i<size; i++)
                    {
                        targetPermissionStrArray[i] = targetPermissions[i].Name;
                    }

                    requester.RequestPermissions(targetPermissionStrArray, callback);
                }
            }
            else
            {
                GvrPermissionsRequester.PermissionStatus[] permissiontStatusArray = new GvrPermissionsRequester.PermissionStatus[targetPermissions.Length];
                for (int i = 0, size = targetPermissions.Length; i < size; i++)
                {
                    permissiontStatusArray[i] = new GvrPermissionsRequester.PermissionStatus(targetPermissions[i].Name, false);
                }

                callback(permissiontStatusArray);
            }

            mLogger.CategoryLog(LogController.LogCategoryMethodOut, targetPermissions);
        }

        public bool ShouldShowRational(Permission targetPermission)
        {
            mLogger.CategoryLog(LogController.LogCategoryMethodIn, targetPermission);
            bool ret = false;

            if (!CheckPermission(targetPermission))
            {
                GvrPermissionsRequester requester = GvrPermissionsRequester.Instance;

                if (requester != null)
                {
                    ret = requester.ShouldShowRational(targetPermission.Name);
                }
            }

            mLogger.CategoryLog(LogController.LogCategoryMethodOut, targetPermission + ", " + ret);
            return ret;
        }
    }

    public class Permission
    {
        private string mPermissionName;

        public Permission(string permissionName)
        {
            mPermissionName = permissionName;
        }

        public string Name
        {
            get
            {
                return mPermissionName;
            }
        }

        public override string ToString()
        {
            return mPermissionName;
        }
    }
}
