using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twitter_tools
{
    using System.Text.RegularExpressions;
    using System.Data;
    using System.IO;
    using System.Diagnostics;

    class rptToTxt
    {
        public string lineToTuple(System.IO.StreamReader reader, string line)
        {
            string result;
            Regex anySpaces = new Regex("[ ]+"); // One or more spaces as separator
            string line_ = line; // Temporary container
            string[] arrWords = new string[6]; // The container of split line
            int proba = -1; // The two int values. -1 is invalid: we seek for lines until we find valid pair
            if (line_ != null)
            {
                while (proba == -1) // Do it until we find a valid pair
                {
                    try // If we can convert, do it
                    {
                        arrWords = anySpaces.Split(line_);
                        proba = Convert.ToInt32(arrWords[0].ToString().Trim());
                    }
                    catch (Exception e) // If we can't, read the next line instead
                    {
                        if ((line_ = reader.ReadLine()) == null)
                        {
                            arrWords = new string[1];
                            arrWords[0] = " ";
                            return string.Join(" ", arrWords);
                        }
                    }
                }
            }
            result = string.Join(" ", arrWords);
            return result;
        }

        public void run(string filename)
        {
            StreamReader testFile = new StreamReader(TwitterBase.defaultPath + filename + ".rpt");
            StreamWriter outFile = new StreamWriter(TwitterBase.defaultPath + filename + ".txt");
            string line;
            string temp1;
            int counter = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while ((line = testFile.ReadLine()) != null)
            {
                temp1 = lineToTuple(testFile, line);
                if ((counter % 1000000) == 0)
                {
                    Console.WriteLine(temp1 + "\t{0}", (counter + 1));

                    // Get the elapsed time as a TimeSpan value.
                    TimeSpan ts = sw.Elapsed;

                    // Format and display the TimeSpan value. 
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                    double percent = ((1.0 * counter)/3186558.9);
                    Console.WriteLine("Elapsed: " + elapsedTime + "\t"+percent.ToString("0.")+"% done");
                }
                outFile.WriteLine(temp1);
                counter++;
            }

            testFile.Close();
            outFile.Close();
            System.Console.WriteLine("There were {0} lines.", counter);
            // Suspend the screen.
            System.Console.ReadLine();
        }
    }
}