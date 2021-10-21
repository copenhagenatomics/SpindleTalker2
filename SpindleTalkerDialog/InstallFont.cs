using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Security.Principal;

namespace SpindleTalker2
{
#pragma warning disable CA1416 // Validate platform compatibility
    public class InstallFont
    {
        [DllImport("gdi32", EntryPoint = "AddFontResource")]
        public static extern int AddFontResourceA(string lpFileName);

        [DllImport("gdi32.dll")]
        private static extern int AddFontResource(string lpszFilename);

        [DllImport("gdi32.dll")]
        private static extern int CreateScalableFontResource(uint fdwHidden, string lpszFontRes, string lpszFontFile, string lpszCurrentPath);


        /// <summary>
        /// Installs font on the user's system and adds it to the registry so it's available on the next session
        /// Your font must be included in your project with its build path set to 'Content' and its Copy property
        /// set to 'Copy Always'
        /// </summary>
        /// <param name="contentFontName">Your font to be passed as a resource (i.e. "myfont.tff")</param>
        public static void RegisterFont(string filename)
        {
            if (IsLinux)
                return;

            var fullpath = Path.GetFullPath(filename);

            // Creates the full path where your font will be installed
            var fontDestination = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts), Path.GetFileName(filename));

            if (!File.Exists(fontDestination))
            {
                if (!IsRunAsAdmin())
                {
                    MessageBox.Show("Run as Admin to install fonts");
                    return;
                }

                // Copies font to destination
                System.IO.File.Copy(fullpath, fontDestination);

                // Retrieves font name
                // Makes sure you reference System.Drawing
                PrivateFontCollection fontCol = new PrivateFontCollection();
                fontCol.AddFontFile(fontDestination);
                var actualFontName = fontCol.Families[0].Name;

                //Add font
                AddFontResource(fontDestination);
                //Add registry entry   
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", actualFontName, fontDestination, RegistryValueKind.String);
            }
        }

        private static bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

    }
}
