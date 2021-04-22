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
    /// Class reads geolocations of users and makes tweets user-pairs. Then it calculates the
    /// distance distribution of the result graph.
    /// </summary>
    class correlation : TwitterBase
    {
        // Custom variables -----------------------------------------------------------------
        int N = 1000000; // Number of read lines
        int N_2 = 10000000; // Number of tweets connections
        List<double> distances = new List<double>();

        /// <summary>
        /// The 'main' function of the class. Reads geolocations of users and makes tweets user-pairs. Then it calculates the
        /// cumulative distance distribution of the result graph and writes it to a file named 'tavolsageloszlas_vilag_korrelacio' at defaultPath.
        /// </summary>
        public void run()
        {
            TextWriter tw1 = new StreamWriter(defaultPath + "tavolsageloszlas_vilag_korrelacio.dat");
            //command = new SqlCommand("SELECT TOP " + N.ToString() + " [cx],[cy],[cz] FROM [jszule].[dbo].[user_location_cold] WHERE [cluster_id] = 0");
            init("SELECT [cx],[cy],[cz] FROM [Twitter_1].[dbo].[user_location_cluster] WHERE [cluster_id] = 0 AND cx <> 1");

            Console.WriteLine("Started reading data...");

            // Read user coordinates --------------------------------------------------------
            double dist;
            List<geoPoint> geoList = new List<geoPoint>();
            geoPoint tgp = new geoPoint();
            int i = 0;
            while (reader.Read())
            {
                // Getting data
                tgp = new geoPoint(Double.Parse(reader["cx"].ToString().Trim()), Double.Parse(reader["cy"].ToString().Trim()), Double.Parse(reader["cz"].ToString().Trim()));
                geoList.Add(tgp);
                i++;
            }
            close();

            // Connecting tweets users ------------------------------------------------------
            Random random = new Random();
            int c, d; // Random indices of the compared geopoints
            for (int j = 0; j < N_2; j++)
            {
                c = random.Next(0, i - 1);
                d = random.Next(0, i - 1);
                dist = R * getDistance(geoList[c], geoList[d]);
                distances.Add(dist);
            }
            int sizeOfDistances = distances.Count();
            Console.WriteLine("Load complete.");

            // Making Cumulative Distribution Function --------------------------------------
            Distribution<double> DF = new Distribution<double>(distances, 100);

            // Writing out data
            Console.WriteLine("Writing out data...");
            tw1.WriteLine("#dist_index distance\t\t\t\tCDF\t\t\t\t\tPDF\t\t\t\tlog(dist)\t\t\t\tlog(CDF)\t\t\tlog(PDF)\t\t\t theoPDF\t\t\t theoPDF\t\t\t");
            double theoreticCDF, theoreticPDF;
            for (int j = 0; j < DF.binNumber - 1; j++)
            {
                double RealDist = DF.PDF[j].x;
                double logDist;

                if (RealDist == 0)
                    logDist = 0;
                else
                    logDist = Math.Log10(RealDist);

                double logCDF, logPDF;
                if (DF.CDF[j].y == 0)
                    logCDF = 0;
                else
                    logCDF = Math.Log10(DF.CDF[j].y);

                if (DF.PDF[j].y == 0)
                    logPDF = 0;
                else
                    logPDF = Math.Log10(DF.PDF[j].y);
                theoreticCDF = Math.Cos((1.0 * RealDist) / (1.0 * R));
                theoreticPDF = (1.0* Math.Sin((1.0 * RealDist) / (1.0 * R))) / (2.0 * R);
                tw1.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}", j, RealDist, DF.CDF[j].y, DF.PDF[j].y, logDist, logCDF, logPDF, theoreticCDF, theoreticPDF);
            }

            con.Close();
            tw1.Close();
        }
    }
}
