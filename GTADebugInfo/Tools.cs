using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;
using System.Diagnostics;

namespace GTADebugInfo
{
    class Tools
    {
        public enum VisualCVersion
        {
            Visual_2017,
            Visual_2015,
            Visual_2013,
            Visual_2012,
            Visual_2010,
            Visual_2008,
            Visual_2005,
        }

        private static readonly Dictionary<VisualCVersion, Version> visualCVersion = new Dictionary<VisualCVersion, Version>
        {
            { VisualCVersion.Visual_2017, new Version("14.1.0.0")},
            { VisualCVersion.Visual_2015, new Version("14.0.0.0")},
            { VisualCVersion.Visual_2013, new Version("12.0.0.0")},
            { VisualCVersion.Visual_2012, new Version("11.0.0.0")},
            { VisualCVersion.Visual_2010, new Version("10.0.0.0")},
            { VisualCVersion.Visual_2008, new Version("9.0.0.0")},
            { VisualCVersion.Visual_2005, new Version("8.0.0.0")}
        };

        public static bool IsVisualCVersionHigherOrEqual(VisualCVersion visualC)
        {
            Version targetVersion = visualCVersion[visualC];

            string[] filters = { "msvcp*.dll", "msvcr*.dll" };
            List<string> files = new List<string>();

            foreach (string filter in filters)
                files.AddRange(Directory.GetFiles(Environment.SystemDirectory, filter));

            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(file);

                if (new Version(fileVersion.ProductVersion).CompareTo(targetVersion) >= 0)
                    return true;
            }

            return false;
        }

        private static readonly List<string> visual2017 = new List<string> {
            "Installer\\Dependencies\\,,x86,14.0,bundle\\Dependents\\{404c9c27-8377-4fd1-b607-7ca635db4e49}",
            "Installer\\Dependencies\\,,amd64,14.0,bundle\\Dependents\\{6c6356fe-cbfa-4944-9bed-a9e99f45cb7a}"
        };
        private static readonly List<string> visual2015 = new List<string> {
            "SOFTWARE\\Classes\\Installer\\Dependencies\\{e2803110-78b3-4664-a479-3611a381656a}",
            "SOFTWARE\\Classes\\Installer\\Dependencies\\{d992c12e-cab2-426f-bde3-fb8c53950b0d}"
        };
        private static readonly List<string> visual2013 = new List<string> {
            "SOFTWARE\\Classes\\Installer\\Dependencies\\{f65db027-aff3-4070-886a-0d87064aabb1}",
            "SOFTWARE\\Classes\\Installer\\Dependencies\\{050d4fc8-5d48-4b8f-8972-47c82c46020f}"
        };
        private static readonly List<string> visual2012 = new List<string> {
            "SOFTWARE\\Classes\\Installer\\Dependencies\\{33d1fd90-4274-48a1-9bc1-97e33d9c2d6f}",
            "SOFTWARE\\Classes\\Installer\\Dependencies\\{ca67548a-5ebe-413a-b50c-4b9ceb6d66c6}"
        };
        private static readonly List<string> visual2010 = new List<string> {
            "SOFTWARE\\Classes\\Installer\\Products\\1D5E3C0FEDA1E123187686FED06E995A",
            "SOFTWARE\\Classes\\Installer\\Products\\1926E8D15D0BCE53481466615F760A7F"
        };
        private static readonly List<string> visual2008 = new List<string> {
            "SOFTWARE\\Classes\\Installer\\Products\\6E815EB96CCE9A53884E7857C57002F0",
            "SOFTWARE\\Classes\\Installer\\Products\\67D6ECF5CD5FBA732B8B22BAC8DE1B4D"
        };
        private static readonly List<string> visual2005 = new List<string> {
            "SOFTWARE\\Classes\\Installer\\Products\\c1c4f01781cc94c4c8fb1542c0981a2a",
            "SOFTWARE\\Classes\\Installer\\Products\\1af2a8da7e60d0b429d7e6453b3d0182"
        };

        public static string GetGamePath(string key = "SOFTWARE\\WOW6432Node\\Rockstar Games\\Grand Theft Auto V")
        {
            object value = "";

            RegistryKey reg = Registry.LocalMachine.OpenSubKey(key, false);
            value = reg.GetValue("InstallFolder");

            if (value != null)
                return value.ToString();
            else
                return "";
        }

        public static string GetWindowsVersion()
        {
            var name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault();
            return name != null ? name.ToString() : "Unknown";
        }

        public static string GetVisualCVersions()
        {
            string value = "";

            // Visual C++ 2017
            RegistryKey reg = Registry.ClassesRoot.OpenSubKey(visual2017[0], false);
            if (reg != null)
                value += "Visual C++ 2017 x86\r\n";

            reg = Registry.ClassesRoot.OpenSubKey(visual2017[1], false);
            if (reg != null)
                value += "Visual C++ 2017 x64\r\n";


            // Visual C++ 2015
            reg = Registry.LocalMachine.OpenSubKey(visual2015[0], false);
            if (reg != null)
                value += "Visual C++ 2015 x86\r\n";

            reg = Registry.LocalMachine.OpenSubKey(visual2015[1], false);
            if (reg != null)
                value += "Visual C++ 2015 x64\r\n";


            // Visual C++ 2013
            reg = Registry.LocalMachine.OpenSubKey(visual2013[0], false);
            if (reg != null)
                value += "Visual C++ 2013 x86\r\n";

            reg = Registry.LocalMachine.OpenSubKey(visual2013[1], false);
            if (reg != null)
                value += "Visual C++ 2013 x64\r\n";


            // Visual C++ 2012
            reg = Registry.LocalMachine.OpenSubKey(visual2012[0], false);
            if (reg != null)
                value += "Visual C++ 2012 x86\r\n";

            reg = Registry.LocalMachine.OpenSubKey(visual2012[1], false);
            if (reg != null)
                value += "Visual C++ 2012 x64\r\n";


            // Visual C++ 2010
            reg = Registry.LocalMachine.OpenSubKey(visual2010[0], false);
            if (reg != null)
                value += "Visual C++ 2010 x86\r\n";

            reg = Registry.LocalMachine.OpenSubKey(visual2010[1], false);
            if (reg != null)
                value += "Visual C++ 2010 x64\r\n";


            // Visual C++ 2008
            reg = Registry.LocalMachine.OpenSubKey(visual2008[0], false);
            if (reg != null)
                value += "Visual C++ 2008 x86\r\n";

            reg = Registry.LocalMachine.OpenSubKey(visual2008[1], false);
            if (reg != null)
                value += "Visual C++ 2008 x64\r\n";


            // Visual C++ 2005
            reg = Registry.LocalMachine.OpenSubKey(visual2005[0], false);
            if (reg != null)
                value += "Visual C++ 2005 x86\r\n";

            reg = Registry.LocalMachine.OpenSubKey(visual2005[1], false);
            if (reg != null)
                value += "Visual C++ 2005 x64\r\n";

            return value;
        }

        public static string GetNETFrameworkVersions()
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    return CheckFor45DotVersion((int)ndpKey.GetValue("Release"));
                }
                else
                {
                    return "";
                }
            }
        }

        // Checking the version using >= will enable forward compatibility, 
        // however you should always compile your code on newer versions of
        // the framework to ensure your app works the same.
        private static string CheckFor45DotVersion(int releaseKey)
        {
            if (releaseKey >= 461308)
            {
                return "4.7.1 or later";
            }
            if (releaseKey >= 460798)
            {
                return "4.7 or later";
            }
            if (releaseKey >= 394802)
            {
                return "4.6.2 or later";
            }
            if (releaseKey >= 394254)
            {
                return "4.6.1 or later";
            }
            if (releaseKey >= 393295)
            {
                return "4.6 or later";
            }
            if (releaseKey >= 379893)
            {
                return "4.5.2 or later";
            }
            if (releaseKey >= 378675)
            {
                return "4.5.1 or later";
            }
            if (releaseKey >= 378389)
            {
                return "4.5 or later";
            }

            // This line should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "No 4.5 or later version detected";
        }
    }
}
