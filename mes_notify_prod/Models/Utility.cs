using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.InteropServices;

namespace DELTA_MES.Utility
{
    /// <summary>
    /// Network drive management
    /// Add By : Anusorn
    /// Date : 2020/05/27
    /// </summary>
    public static class MyNetworkDrive
    {
        public enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET,
            RESOURCE_REMEMBERED,
            RESOURCE_RECENT,
            RESOURCE_CONTEXT
        }

        public enum ResourceType
        {
            RESOURCETYPE_ANY,
            RESOURCETYPE_DISK,
            RESOURCETYPE_PRINT,
            RESOURCETYPE_RESERVED
        }

        public enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010,
            RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
        }

        public enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC,
            RESOURCEDISPLAYTYPE_DOMAIN,
            RESOURCEDISPLAYTYPE_SERVER,
            RESOURCEDISPLAYTYPE_SHARE,
            RESOURCEDISPLAYTYPE_FILE,
            RESOURCEDISPLAYTYPE_GROUP,
            RESOURCEDISPLAYTYPE_NETWORK,
            RESOURCEDISPLAYTYPE_ROOT,
            RESOURCEDISPLAYTYPE_SHAREADMIN,
            RESOURCEDISPLAYTYPE_DIRECTORY,
            RESOURCEDISPLAYTYPE_TREE,
            RESOURCEDISPLAYTYPE_NDSCONTAINER
        }

        [StructLayout(LayoutKind.Sequential)]
        private class NETRESOURCE
        {
            public ResourceScope dwScope = 0;
            public ResourceType dwType = 0;
            public ResourceDisplayType dwDisplayType = 0;
            public ResourceUsage dwUsage = 0;
            public string lpLocalName = null;
            public string lpRemoteName = null;
            public string lpComment = null;
            public string lpProvider = null;
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NETRESOURCE lpNetResource, string lpPassword, string lpUsername, int dwFlags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);

        /// <summary>
        /// Access authorization on network drive
        /// </summary>
        /// <param name="IpRemoteName">Ex.//192.168.213.198</param>
        /// <param name="User">Admin</param>
        /// <param name="Password">1234</param>
        /// <returns></returns>
        public static int MapNetworkDrive(string IpRemoteName, string User, string Password)
        {
            NETRESOURCE myNetResource = new NETRESOURCE();
            myNetResource.dwType = ResourceType.RESOURCETYPE_DISK;
            myNetResource.lpRemoteName = IpRemoteName;
            myNetResource.lpProvider = "";
            int result = WNetAddConnection2(myNetResource, Password, User, 0);
            return result;
        }

        /// <summary>
        /// Cancel to access authorization on network drive
        /// </summary>
        /// <param name="drive">Ex.//192.168.213.198</param>
        /// <returns></returns>
        public static int CancelMapNetworkDrive(string drive)
        {
            int result = WNetCancelConnection2(drive, 0, false);
            return result;
        }
    }
}
