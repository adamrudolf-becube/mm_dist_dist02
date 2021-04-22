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
    /// Calculates the cumulative distance distribution of the mutual mention graph.
    /// </summary>
    class dist_distribution : TwitterBase
    {
        // Custom variables -----------------------------------------------------------------
        int N = 100000; // Number of read lines
        List<double> distances = new List<double>();

        /// <summary>
        /// "Main" function of the class. Calculates the cumulative distance distribution of the mutual mention graph and writes it to a .dat file named tavolsageloszlas_vilag_PDF_scalar-product_nocold at the defaultPath.
        /// </summary>
        public void run()
        {
            int[] thresholds = new int[6] { 2, 5, 10, 20, 100, 500};
            int[] switchVar = new int[3] { 1, 2, 3 };
            int cnt = 0;

            double dist;
            int fails = 0;
            int invalids = 0;
            int counter = 0;
            init("SELECT * FROM [rudolf].[dbo].[mention_mutual_all] WHERE (cx_a <> 1) AND (cx_b <> 1)");
            while (reader.Read())
            {
                dist = Convert.ToDouble(reader["dist"].ToString().Trim());
                distances.Add(dist);
                counter++;
            }
            close();
            Distribution<double> DF = new Distribution<double>(distances, 100);

            foreach (int eff in switchVar)
            {
                // Reading connected user data, and making histogram ---------------------------
                // Temp
                //init("SELECT * FROM [rudolf].[dbo].[mention_mutual_all] WHERE (cx_a <> 1) AND (cx_b <> 1) AND atob >= 100 and btoa >= 100 and ratio <= 0.2");
                // World s
                //init("SELECT * FROM [rudolf].[dbo].[mention_mutual_all] WHERE (cx_a <> 1) AND (cx_b <> 1) AND atob >= "+s.ToString()+" and btoa >= "+s.ToString());
                switch (eff)
                {
                    case 1:
                        init("SELECT * FROM [rudolf].[dbo].[mention_mutual_all] WHERE (cx_a <> 1) AND (cx_b <> 1) AND ratio <= 0.2");
                        break;
                    case 2:
                        init("SELECT * FROM [rudolf].[dbo].[mention_mutual_all] WHERE (cx_a <> 1) AND (cx_b <> 1) AND ratio > 0.2 AND ratio <= 0.8 ");
                        break;
                    default:
                        init("SELECT * FROM [rudolf].[dbo].[mention_mutual_all] WHERE (cx_a <> 1) AND (cx_b <> 1) AND ratio > 0.8");
                        break;
                }
                    
                // USA
                //init("SELECT * FROM [rudolf].[dbo].[mention_mutual_all] WHERE (cx_a <> 1) AND (cx_b <> 1) AND (lon_a > -126 AND lon_a < -66 AND lat_a > 25 AND lat_a < 49) AND (lon_b > -126 AND lon_b < -66 AND lat_b > 25 AND lat_b < 49)");
                // Europe
                //init("SELECT * FROM [rudolf].[dbo].[mention_mutual_all] WHERE (cx_a <> 1) AND (cx_b <> 1) AND (lon_a > -12 AND lon_a < 38 AND lat_a > 36 AND lat_a < 72) AND (lon_b > -12 AND lon_b < 38 AND lat_b > 36 AND lat_b < 72)");
                //init("SELECT TOP 67000000 * FROM jszule.dbo.follower_2dir_coord");

                Console.WriteLine("Started reading mentions...");
                distances.Clear();

                while (reader.Read())
                {
                    dist = Convert.ToDouble(reader["dist"].ToString().Trim());
                    distances.Add(dist);
                    counter++;
                }
                close();
                int sizeOfDistances = distances.Count();
                Console.WriteLine("Load complete. {0} invalids, {1} failed,  {2} succeeded.", invalids, fails, (fails + sizeOfDistances));

                // Making Cumulative Distribution Function
                Distribution<double> DFs = new Distribution<double>(distances, 100);
                distances.Clear();

                // Writing out data
                Console.WriteLine("Writing out data...");

                TextWriter tw = new StreamWriter(defaultPath + "differ" + eff.ToString());
                for (int j = 0; j < DFs.PDF.Count; j++)
                    tw.WriteLine("{0} {1}", DFs.PDF[j].x, DFs.PDF[j].y - DF.interpolate(DFs.PDF[j].x));
                tw.Close();

                //DF.writeToFile("tavolsageloszlas_vilag_PDF_mention_rat-03", dataName: "distance");
                //DF.writeToFile("tavolsageloszlas_vilag_PDF_s=1rlt02", dataName: "distance");
            }
        }
    }
}
