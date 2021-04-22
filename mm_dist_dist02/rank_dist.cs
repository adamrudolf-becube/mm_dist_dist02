using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Twitter_tools
{
    /// <summary>
    /// Class reads links with weights above a certain threshold from the mutual mention graph and calculates the rank distribution of it.
    /// </summary>
    class rank_dist : TwitterBase
    {
        // Custom variables -----------------------------------------------------------------
        List<int> ranks = new List<int>();

        /// <summary>
        /// The "main" funtcion of the class. Reads links with weights above a certain threshold from the mutual mention graph and calculates the rank distribution of it, then it writes the results to .dat files called mention_mutual_num and mention_mutual_rank_dist_tr.
        /// </summary>
        /// <remarks>Saves results to .dat files at defaultPath.
        /// Saves the number of read lines (= links) for each threshold to 'mention_mutual_num'.
        /// Saves the rank distribution for each threshold to 'mention_mutual_rank_dist'. Here each row represents a distribution for a particular rank, and each column is for a fixed threshold.
        /// Saves the transposed matrix of the distribution written to 'mention_mutual_rank_dist' to the file 'mention_mutual_rank_dist_tr'. Here each row has a fixed threshold and each column has a ficed rank.
        /// Writing both of the matrix and it's transposed matrix helps in data visualization.
        /// </remarks>
        /// <param name="thresholds">An int list of the thresholds. The method will calculate the histogram for each threshold given in this List.</param>
        public void run(int[] thresholds)
        {
            int cnt = thresholds.Count();
            List<Hist1D<int>> matrix = new List<Hist1D<int>>();
            int[] sizeOfRatios = new int[cnt];
                        
            foreach(int threshold in thresholds)
            {
                init("SELECT user_a_is, COUNT(*) rank FROM [rudolf].[dbo].[mention_mutual_all] WHERE atob > " + threshold.ToString() + " AND btoa > " + threshold.ToString() + " GROUP BY user_a_is");
                //init("SELECT user_a_is, COUNT(*) rank FROM [rudolf].[dbo].[mutual_mention_table] GROUP BY user_a_id");

                int rank;
                ranks.Clear();
                while (reader.Read())
                {
                    // Getting data
                    rank = Convert.ToInt32(reader["rank"].ToString().Trim());
                    ranks.Add(rank);
                }
                close();
                Console.WriteLine("Threshold = {1}.\tLoad complete. {2} numbers loaded.", threshold, ranks.Count());

                // Making histogram --------------------------------------------------------
                matrix.Add(new Hist1D<int>(ranks, 1, ranks.Max()));
            }

            // Fill the missing elements with ones (zeros would be correct. We add ones becouse of the logscale)
            Hist2D<int, int> h = new Hist2D<int, int>(matrix, 1);
            
            // Writing out data
            TextWriter nums;
            nums = new StreamWriter(defaultPath + "mention_mutual_num.dat");
            for(int i = 0; i < sizeOfRatios.Count(); i++)
                nums.WriteLine("{0}\t{1}", thresholds[i], sizeOfRatios[i]);
            nums.Close();

            h.writeToFile("mention_mutual_rank_dist", x_dataName: "counts", y_dataName: "rank");
            h.writeToFile("mention_mutual_rank_dist_tr", x_dataName: "counts", y_dataName: "rank", transposed: true);
        }
    }
}
