using System;
using System.Windows.Forms;

namespace SpindleTalker2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWindow());
            }
            catch (Exception e)
            {
                if (e.ToString().Contains("Could not load file or assembly 'Microsoft.VisualBasic"))
                    Console.WriteLine("Please install: sudo apt-get install mono-vbnc");

                if(e.InnerException == null)
                    MessageBox.Show(e.ToString());
                else
                    MessageBox.Show(e.InnerException.ToString());
            }
        }
    }
}
