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
    /// Class calculates the distance distribution of the mutual follower graph.
    /// </summary> 
    class dist_distribution_2 : TwitterBase
    {
        // Custom variables -----------------------------------------------------------------
        List<double> distances = new List<double>();

        /// <summary>
        /// "Main" function of the object. Calculates the cumulative distance distribution of the mutual follower graph and writes it to a .dat file named tavolsageloszlas_vilag_PDF_follow_mutual at defaultPath.
        /// </summary>
        public void run()
        {
            // Reading connected user data, and making histogram ----------------------------
            TextWriter tw1 = new StreamWriter(defaultPath + "tavolsageloszlas_vilag_PDF_follow_mutual.dat");
            //tw1.WriteLine("#run_id;user_id;initCount;endCount;sigma;lat;lon;cx;cy;cz;iterations");

            Console.WriteLine("Connecting database...");

            init("SELECT dist AS d FROM [rudolf].[dbo].[follow_mutual_distances] ORDER BY d ASC");

            Console.WriteLine("Started reading data...");

            double d = new Double();
            int fails = 0;
            while (reader.Read())
            {
                // Getting data
                d = Double.Parse(reader["d"].ToString().Trim());
                if (Double.IsNaN(d))
                    fails++;
                else
                    distances.Add(d);
            }
            close();
            int sizeOfDistances = distances.Count();
            Console.WriteLine("Load complete. {0} failed, {1} succeeded.", fails, sizeOfDistances);

            // Making Cumulative Distribution Function
            Distribution<double> DF = new Distribution<double>(distances, 100);

            // Writing out data
            Console.WriteLine("Writing out data...");
            DF.writeToFile("tavolsageloszlas_vilag_PDF_follow_mutual", dataName: "distance");
        }
    }
}
