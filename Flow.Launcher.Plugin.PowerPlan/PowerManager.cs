using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Flow.Launcher.Plugin.PowerPlan
{
    public static class PowerManager
    {
        public static IEnumerable<PowerProperty> GetAll()
        {
            var schemeGuid = Guid.Empty;

            uint sizeSchemeGuid = (uint)Marshal.SizeOf(typeof(Guid));
            uint schemeIndex = 0;

            while (PowerAPI.PowerEnumerate(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, (uint)PowerAPI.AccessFlags.ACCESS_SCHEME, schemeIndex, ref schemeGuid, ref sizeSchemeGuid) == 0)
            {
                string friendlyName = ReadPowerName(schemeGuid);
                bool isActive = IsActive(schemeGuid);
                yield return new PowerProperty { Id = schemeGuid, Name = friendlyName, IsActive = isActive };
                schemeIndex++;
            }
        }

        public static void Active(Guid id)
        {
            PowerAPI.PowerSetActiveScheme(IntPtr.Zero, ref id);
        }

        private static bool IsActive(Guid id)
        {
            IntPtr pCurrentSchemeGuid = IntPtr.Zero;
            PowerAPI.PowerGetActiveScheme(IntPtr.Zero, ref pCurrentSchemeGuid);
            var currentSchemeGuid = (Guid)Marshal.PtrToStructure(pCurrentSchemeGuid, typeof(Guid));
            return currentSchemeGuid == id;
        }

        private static PowerProperty GetPower(Guid id)
        {
            return GetAll().FirstOrDefault(x => x.Id.Equals(id));
        }

        private static string ReadPowerName(Guid id)
        {
            uint sizeName = 1024;
            IntPtr pSizeName = Marshal.AllocHGlobal((int)sizeName);

            string powerName;

            try
            {
                PowerAPI.PowerReadFriendlyName(IntPtr.Zero, ref id, IntPtr.Zero, IntPtr.Zero, pSizeName, ref sizeName);
                powerName = Marshal.PtrToStringUni(pSizeName);
            }
            finally
            {
                Marshal.FreeHGlobal(pSizeName);
            }
            return powerName;
        }
    }
}
