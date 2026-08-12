//=================================================================
// NetworkThrottle.cs - MW0LGE 2021
//=================================================================

using System;
using Microsoft.Win32;
using System.Windows.Forms;

namespace Thetis
{
    static class NetworkThrottle
    {
        public static bool GetNetworkThrottle(out int throttle, bool showErrors = true)
        {
            bool bRet = false;
            throttle = 0;

            RegistryKey hklm = null;
            try
            {
                hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
            }
            catch
            {
                if (showErrors)
                {
                    MessageBox.Show("无法打开 LocalMachine 注册表基础键。",
                        "错误",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Common.MB_TOPMOST);
                }
            }
            if (hklm != null)
            {
                RegistryKey key = null;
                try
                {
                    key = hklm.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile", false);
                }
                catch
                {
                    if (showErrors)
                    {
                        MessageBox.Show("无法打开 SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile 注册表键。",
                            "错误",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Common.MB_TOPMOST);
                    }
                }

                if (key != null)
                {
                    try
                    {
                        object o = key.GetValue("NetworkThrottlingIndex");
                        if (o != null)
                        {
                            if (o is int)
                            {
                                throttle = (int)o;
                                bRet = true;
                            }
                            else
                            {
                                if (showErrors)
                                {
                                    MessageBox.Show("NetworkThrottlingIndex 键中的值不合适。",
                                        "错误",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Common.MB_TOPMOST);
                                }
                            }
                        }
                    }
                    catch
                    {
                        if (showErrors)
                        {
                            MessageBox.Show("无法读取 NetworkThrottlingIndex 注册表项的值。",
                                "错误",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Common.MB_TOPMOST);
                        }
                    }
                    key.Close();
                }
                hklm.Close();
            }
            return bRet;
        }
        public static bool SetNetworkThrottle(int throttle)
        {
            bool bRet = false;

            if (Common.IsAdministrator())
            {
                RegistryKey hklm = null;
                try
                {
                    hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
                }
                catch
                {
                    MessageBox.Show("无法打开 LocalMachine 注册表基础键。",
                        "错误",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Common.MB_TOPMOST);
                }

                if (hklm != null)
                {
                    RegistryKey key = null;
                    try
                    {
                        key = hklm.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile", true);
                    }
                    catch
                    {
                        MessageBox.Show("无法打开 SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile 注册表键。",
                            "错误",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Common.MB_TOPMOST);
                    }

                    if (key != null)
                    {
                        try
                        {
                            key.SetValue("NetworkThrottlingIndex", throttle, RegistryValueKind.DWord);             //unchecked((int)0xffffffffu)

                            bRet = true;
                        }
                        catch
                        {
                            MessageBox.Show("无法设置 NetworkThrottlingIndex 注册表项的值。",
                            "错误",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Common.MB_TOPMOST);
                        }
                        key.Close();
                    }
                    hklm.Close();
                }
            }
            else
            {
                //msgbox need to be admin !
                MessageBox.Show("您需要是管理员。请以“管理员身份”运行 Thetis。",
                    "无管理员权限",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Common.MB_TOPMOST);
            }

            return bRet;
        }
    }
}
