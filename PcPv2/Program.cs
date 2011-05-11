using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PcPv2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            String filename = "";

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // check the arguments, remove any quotes
            if (args.Length > 0)
            {
                filename = args[0].Replace("\"", "");
            }

            // if the file path is specified, make sure it exists
            if (filename != "")
            {
                try
                {
                    filename = System.IO.Path.GetFullPath(filename);
                }
                catch
                {
                    MessageBox.Show("Invalid file argument, starting without file specified.");
                    filename = "";
                }
            }
            Application.Run(new Dummy(filename));
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Dummy());
        }
    }
}
