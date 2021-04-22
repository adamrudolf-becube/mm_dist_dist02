using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using System.IO;

namespace Twitter_tools
{
    /// <summary>
    /// Class calculates 
    /// </summary>
    class sigma_calculator : TwitterBase
    {
        public bool tweets = true;
        public void run()
        {
            Random rand = new Random();
            List<geoPoint> gps = new List<geoPoint>();
            List<Tuple<int,double>> sigmas = new List<Tuple<int,double>>();
            double temp_sigma;
            double counter;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Hist2D<int, double> H = new Hist2D<int, double>(1, 100, 50, 200);
            List<geoPoint> geoList = new List<geoPoint>();
            int cunter = 0;
            if (tweets)
            {
                Stopwatch sw2 = new Stopwatch();
                sw2.Start();
                Console.WriteLine("Started reading data...");
                init("SELECT cx, cy, cz from Twitter_1.dbo.tweet tablesample(30000000 rows) where lat is not null and lon is not null");
                while (reader.Read())
                {
                    geoList.Add(new geoPoint(Double.Parse(reader["cx"].ToString().Trim()), Double.Parse(reader["cy"].ToString().Trim()), Double.Parse(reader["cz"].ToString().Trim())));
                    cunter++;
                }
                close();
                Console.Write("Done. {0} lines were read.", cunter);
                showTime(sw2);
            }

            StreamReader tr = new StreamReader(defaultPath + "ns.dat");
            string line;
            string[] arrWords = new string[2];
            List<int> counters = new List<int>();
            while ((line = tr.ReadLine()) != null)
            {
                arrWords = line.Split(' ');
                counters.Add(Convert.ToInt32(arrWords[1].Trim()));
            }
            // This loop goes trough the sizes of sets.
            for (int n = 1; n < 100; n++)
            {
                if (tweets)
                    counter = counters[n];
                else
                    counter = (int)(1000000 / (1.0 * n)); // Number of evaluated n sized sets
                temp_sigma = 0.0;

                // Goes trough the instances of n sized sets
                for (int i = 0; i < counter; i++)
                {
                    // We clear the gepPoint list and fill it with random n gps.
                    gps.Clear();
                    for (int j = 0; j < n; j++)
                    {
                        if (tweets)
                            gps.Add(geoList[rand.Next(0, cunter - 1)]);
                        else
                            gps.Add(new geoPoint(rand));
                    }
                    temp_sigma += GetStdDev(gps);
                    // Add data to the 2D histogram
                    H.addData(new Tuple<int,double>(n, R * GetStdDev(gps)));
                    //if (n > 10)
                    //    Console.WriteLine("{0} {1}", n, R * GetStdDev(gps));
                }
                sigmas.Add(new Tuple<int,double>(n, temp_sigma / (1.0 * counter)));
                Console.WriteLine("n = {0}, sigma = {1}\t{2}", n, temp_sigma / (1.0 * counter), R * temp_sigma / (1.0 * counter));
                showTime(sw);
            }
            TextWriter tw = new StreamWriter(defaultPath+"nyeff3");
            foreach (Tuple<int, double> t in sigmas)
                tw.WriteLine("{0} {1} {2}", t.Item1, t.Item2, R*t.Item2);
            H.writeToFile("hashtag-graf_sigma_CNT-gt_hist_linlinlin_2");
            tw.Close();
        }
    }
}
