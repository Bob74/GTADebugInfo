using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Diagnostics;

namespace GTADebugInfo
{
    public partial class DebugInfosGUI : Form
    {
        private string logFilePath = Application.StartupPath + "\\" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".log";
        private int timeOut = 20000;

        public DebugInfosGUI()
        {
            InitializeComponent();
            textBoxGamePath.Text = Tools.GetGamePath();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dial = new FolderBrowserDialog();
            if (dial.ShowDialog() == DialogResult.OK)
                textBoxGamePath.Text = dial.SelectedPath;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            richTextBoxLog.Clear();

            string gamePath = textBoxGamePath.Text;
            string gameExePath = textBoxGamePath.Text + "\\GTA5.exe";

            if (File.Exists(gameExePath))
            {
                if (File.Exists(logFilePath))
                {
                    int time = 0;
                    File.Delete(logFilePath);
                    while (File.Exists(logFilePath) || time > timeOut)
                    {
                        Thread.Sleep(1000);
                        time += 1000;
                    }

                    if (File.Exists(logFilePath))
                    {
                        MessageBox.Show("Cannot create the file " + logFilePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                using (StreamWriter sw = File.CreateText(logFilePath))
                {
                    // Windows Informations
                    sw.WriteLine("[Windows Informations]");
                    richTextBoxLog.AppendText("[Windows Informations]\r\n");
                    sw.WriteLine("Version: " + Tools.GetWindowsVersion());
                    richTextBoxLog.AppendText("Version: " + Tools.GetWindowsVersion() + "\r\n");

                    if (Environment.Is64BitOperatingSystem)
                    {
                        sw.WriteLine("Architecture: 64 bits");
                        richTextBoxLog.AppendText("Architecture: 64 bits\r\n");
                    }
                    else
                    {
                        sw.WriteLine("Architecture: 32 bits");
                        richTextBoxLog.AppendText("Architecture: 32 bits\r\n");
                    }

                    CultureInfo ci = CultureInfo.InstalledUICulture;
                    sw.WriteLine("Language: " + ci.DisplayName);
                    richTextBoxLog.AppendText("Language: " + ci.DisplayName + "\r\n");


                    // Game files
                    sw.WriteLine();
                    sw.WriteLine("[Game Files]");
                    richTextBoxLog.AppendText("\r\n[Game Files]\r\n");
                    FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(gameExePath);

                    sw.WriteLine("GTA5.exe: " + fileInfo.FileVersion);
                    richTextBoxLog.AppendText("GTA5.exe: " + fileInfo.FileVersion + "\r\n");

                    string[] scriptHookVDotNetFiles = Directory.GetFiles(gamePath, "ScriptHook*.dll");
                    foreach (string fileName in scriptHookVDotNetFiles)
                    {
                        FileInfo info = new FileInfo(fileName);
                        fileInfo = FileVersionInfo.GetVersionInfo(fileName);
                        sw.WriteLine(info.Name + ": " + fileInfo.FileVersion);
                        richTextBoxLog.AppendText(info.Name + ": " + fileInfo.FileVersion + "\r\n");
                    }

                    string[] scriptHookVMods = Directory.GetFiles(gamePath, "*.asi");
                    foreach (string fileName in scriptHookVMods)
                    {
                        FileInfo info = new FileInfo(fileName);
                        fileInfo = FileVersionInfo.GetVersionInfo(fileName);
                        sw.WriteLine(info.Name + ": " + fileInfo.FileVersion);
                        richTextBoxLog.AppendText(info.Name + ": " + fileInfo.FileVersion + "\r\n");
                    }

                    if (File.Exists(gamePath + "\\RAGEPluginHook.exe"))
                    {
                        fileInfo = FileVersionInfo.GetVersionInfo(gamePath + "\\RAGEPluginHook.exe");
                        sw.WriteLine("RagePluginHook: " + fileInfo.FileVersion);
                        richTextBoxLog.AppendText("RagePluginHook: " + fileInfo.FileVersion + "\r\n");
                    }
                    else
                    {
                        sw.WriteLine("RagePluginHook: No");
                        richTextBoxLog.AppendText("RagePluginHook: No\r\n");
                    }


                    // Mod files
                    sw.WriteLine();
                    sw.WriteLine("[Mod Files]");
                    richTextBoxLog.AppendText("\r\n[Mod Files]\r\n");

                    string[] modFiles = Directory.GetFiles(gamePath + "\\scripts", "*.dll");
                    foreach (string fileName in modFiles)
                    {
                        FileInfo info = new FileInfo(fileName);
                        fileInfo = FileVersionInfo.GetVersionInfo(fileName);
                        sw.WriteLine(info.Name + ": " + fileInfo.FileVersion);
                        richTextBoxLog.AppendText(info.Name + ": " + fileInfo.FileVersion + "\r\n");
                    }

                    sw.WriteLine();
                    sw.WriteLine("[Ini File]");
                    richTextBoxLog.AppendText("\r\n[Ini File]\r\n");

                    StreamReader sr = new StreamReader(gamePath + "\\scripts\\MMI\\config.ini");
                    string configFileText = sr.ReadToEnd();

                    sw.WriteLine(configFileText);
                    richTextBoxLog.AppendText(configFileText + "\r\n");


                    // Visual C++ Versions
                    string visualC = Tools.GetVisualCVersions();
                    sw.WriteLine();
                    sw.WriteLine("[Visual C++ Versions]");
                    richTextBoxLog.AppendText("\r\n[Visual C++ Versions]\r\n");
                    sw.WriteLine("Visual C++ version higher or equal to 2017 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2017));
                    sw.WriteLine("Visual C++ version higher or equal to 2015 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2015));
                    sw.WriteLine("Visual C++ version higher or equal to 2013 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2013));
                    sw.WriteLine("Visual C++ version higher or equal to 2012 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2012));
                    sw.WriteLine("Visual C++ version higher or equal to 2010 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2010));
                    sw.WriteLine("Visual C++ version higher or equal to 2008 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2008));
                    sw.WriteLine("Visual C++ version higher or equal to 2005 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2005));
                    richTextBoxLog.AppendText("Visual C++ version higher or equal to 2017 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2017) + "\r\n");
                    richTextBoxLog.AppendText("Visual C++ version higher or equal to 2015 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2015) + "\r\n");
                    richTextBoxLog.AppendText("Visual C++ version higher or equal to 2013 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2013) + "\r\n");
                    richTextBoxLog.AppendText("Visual C++ version higher or equal to 2012 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2012) + "\r\n");
                    richTextBoxLog.AppendText("Visual C++ version higher or equal to 2010 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2010) + "\r\n");
                    richTextBoxLog.AppendText("Visual C++ version higher or equal to 2008 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2008) + "\r\n");
                    richTextBoxLog.AppendText("Visual C++ version higher or equal to 2005 = " + Tools.IsVisualCVersionHigherOrEqual(Tools.VisualCVersion.Visual_2005) + "\r\n");


                    // NET Framework Version
                    string netf = Tools.GetNETFrameworkVersions();
                    sw.WriteLine();
                    sw.WriteLine("[NET Framework Version]");
                    richTextBoxLog.AppendText("\r\n[NET Framework Version]\r\n");
                    sw.WriteLine(netf);
                    richTextBoxLog.AppendText(netf + "\r\n");


                    // Done
                    richTextBoxLog.SelectionColor = System.Drawing.Color.ForestGreen;
                    richTextBoxLog.SelectionIndent = 50;
                    richTextBoxLog.SelectionFont = new System.Drawing.Font(richTextBoxLog.SelectionFont.FontFamily.Name, richTextBoxLog.SelectionFont.Size, System.Drawing.FontStyle.Bold);
                    richTextBoxLog.AppendText("\r\n\r\n======> Done!\r\n");

                    richTextBoxLog.ScrollToCaret();
                }
            }
            else
            {
                MessageBox.Show("File " + textBoxGamePath.Text + "\\GTA5.exe" + " not found!\r\n" + "Make sure you specified a valid path to the game directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
