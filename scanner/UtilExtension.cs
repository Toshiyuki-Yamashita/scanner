using System.Reflection;

namespace scanner
{
    namespace Util
    {
        internal static class Extension
        {
            public static string ToMacAddressString(this ulong addr)
            {
                var tempMac = addr.ToString("X12");
                //tempMac is now 'E7A1F7842F17'

                var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
                var replace = "$1:$2:$3:$4:$5:$6";
                return System.Text.RegularExpressions.Regex.Replace(tempMac, regex, replace);
            }

            public static object? SetProperty(this Control ctrl, string name, params object[] args)
            {
                return ctrl.GetType().InvokeMember(name,
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                    null,
                    ctrl,
                    args);
            }
        }

    }
}
