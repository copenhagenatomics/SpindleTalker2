using System;
using System.Diagnostics;
using System.IO;
using static System.Environment;

namespace SpindleTalker2
{
    public class InstallFont
    {
        public static void UsingShell32(string filename)
        {
            var fullpath = Path.GetFullPath(filename);
            Shell32.Shell shell = new Shell32.Shell();
            Shell32.Folder fontFolder = shell.NameSpace(0x14);
            var distination = Path.Combine(Environment.GetFolderPath(SpecialFolder.Windows), "Fonts", Path.GetFileName(filename));
            if (!File.Exists(distination))
                fontFolder.CopyHere(fullpath);
        }

    }
}
