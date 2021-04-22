using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.IO;

namespace Twitter_tools
{
    /// <summary>
    /// Test object o call GNUPLOT from a C# console application.
    /// </summary>
    class gnuplot_caller
    {
        /// <summary>
        /// The "main" funciotn of the object.
        /// </summary>
        public void run()
        {
            string Pgm = @"C:\Program Files (x86)\gnuplot\bin\wgnuplot.exe";
            Process extPro = new Process();
            extPro.StartInfo.FileName = Pgm;
            extPro.StartInfo.UseShellExecute = false;
            extPro.StartInfo.RedirectStandardInput = true;
            extPro.Start();

            StreamWriter gnupStWr = extPro.StandardInput;
            gnupStWr.WriteLine("plot sin(x)\n");
            gnupStWr.Flush();
        }
    }
}
