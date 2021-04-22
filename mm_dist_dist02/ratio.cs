using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace Twitter_tools
{
    /// <summary>
    /// This class reads the counts of hashtags and calculatesthe average ratio of get-tagged and all hashtags.
    /// </summary>
    class ratio : TwitterBase
    {
        /// <summary>
        /// The "main" function of the class. Reads the counts of hashtags and calculatesthe average ratio of get-tagged and all hashtags.
        /// </summary>
        public void run()
        {
            // SQL related initializations
            init("SELECT distinct tag, CNT, CNT_gt FROM [rudolf].[dbo].[hashtag_coords_counts_2] WHERE run_id = 1004 AND tag LIKE '%[0-9a-z]%'");
            //init("SELECT TOP 100 * FROM [rudolf].[dbo].[hashtag_coords_counts] WHERE run_id = 1004 AND tag LIKE '%[0-9a-z]%' AND CNT > 10 ORDER BY TAG");
            //init("SELECT TOP 1000000 * FROM [rudolf].[dbo].[hashtag_coords_counts] WHERE run_id = 1004 AND tag LIKE '%[0-9a-z]%' AND CNT > 100 ORDER BY tag");
            
            Console.WriteLine("Running query...");

            List<Tuple<int,int>> listOfTuples = new List<Tuple<int,int>>();
            
            // Read data --------------------------------------------------------------------
            while (reader.Read())
                listOfTuples.Add(Tuple.Create(Convert.ToInt32(reader["CNT"]), Convert.ToInt32(reader["CNT_gt"])));
            close();

            // Calculating average ----------------------------------------------------------
            Console.WriteLine("Calculating average...");
            double rat = 0.0;
            foreach (Tuple<int, int> t in listOfTuples)
                rat += (1.0 * t.Item2) / (1.0 * t.Item1);

            rat = (1.0 * rat) / (1.0 * listOfTuples.Count);

            // Writing output ---------------------------------------------------------------
            Console.WriteLine("Number of read lines: {0}. Average ratio: {1}", listOfTuples.Count, rat);

            Console.WriteLine("Done.");
        }
    }
}