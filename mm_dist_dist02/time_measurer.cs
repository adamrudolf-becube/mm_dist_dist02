using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using System.Data.SqlClient;
using System.Data;
using System.IO;


namespace Twitter_tools
{
    /// <summary>
    /// Ths class executes a particular query with different numbers of lines and measures the ecexution time in seconds of each execution.
    /// </summary>
    class time_measurer : TwitterBase
    {
        // Analysis objects
        Stopwatch watch = new Stopwatch();

        // Tuning parameters
        static int num = 1000; // The total number of executions

        /// <summary>
        /// The "main" function of the class. Executes a particular query with different numbers of lines and measures the ecexution time in seconds of each execution.
        /// Saves the results to a .dat file called time_measurer at the default path.
        /// </summary>
        /// <remarks>Saves results to the file 'time_measurer.dat' at defaultPath.</remarks>
        public void run()
        {
            int[] N = new int[num]; // Numbers of read lines in the ith execution
            double[] secs = new double[num]; // Elapsed seconds of the ith execution

            // Filling the N array
            for (int i = 0; i < num; i++)
            {
                N[i] = 100 * i;
            }

            TextWriter tw1 = new StreamWriter(defaultPath + "time_measurer.dat");
            tw1.WriteLine("#Query: SELECT top 1000 [run_id],[tweet_id],[created_at],[utc_offset],[user_id],[lon],[lat],[cx],[cy],[cz],[text] FROM [Twitter_1].[dbo].[tweet]");
            tw1.WriteLine("#N\t t (sec)");
            tw1.Close();
            // Reset the stopwatch before each execution, execute the query and save the running time
            for (int i = 0; i < num; i++)
            {
                tw1 = File.AppendText(defaultPath + "time_measurer2.dat");
            
                init("SELECT TOP " + N[i].ToString() + " [run_id],[tweet_id],[created_at],[utc_offset],[user_id],[lon],[lat],[cx],[cy],[cz],[text] FROM [Twitter_1].[dbo].[tweet]");
                //init("SELECT TOP " + N[i].ToString() + " * FROM [rudolf].[dbo].[hashtag_coords_counts] WHERE run_id = 1004 AND tag LIKE '%[0-9a-z]%' AND CNT > 100 ORDER BY tag");

                Console.Write("Exec. number: {0}, N = {1}. ", i);
                showTime(watch);
                //command = new SqlCommand("SELECT TOP " + N[i].ToString() + " * FROM [rudolf].[dbo].[hashtag_coords_counts] WHERE run_id = 1004 AND tag LIKE '%[0-9a-z]%' AND CNT > 100 ORDER BY tag");
                
                secs[i] = watch.Elapsed.TotalSeconds;
                close();

                tw1.WriteLine("{0}\t{1}", N[i], secs[i]);
                watch.Restart();
                tw1.Close();
            }

            // Writing output ---------------------------------------------------------------
            
        }
    }
}
