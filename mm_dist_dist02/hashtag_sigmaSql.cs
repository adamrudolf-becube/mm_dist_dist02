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
    /// This class reads all the hashtags from an SQL table. Makes a histogram from the counts (CNT_gt) and Standard Deviance of them (sigma).
    /// </summary>
    class hashtag_sigmaSql : TwitterBase
    {
        /// <summary>
        /// The "main" function of the class. Reads all the hashtags from an SQL table grouped by tags. It calculates the average and standard deviance of the particular tags and writes them to files.
        /// </summary>
        /// <remarks>Works in the directory stored at defaultPath. Reads data from a file called 'hashtag.txt' and writes 2 dimensional histogram to
        /// 'hashtag-graf_sigma_CNT-gt_hist_linlinlin.dat', 'average to hashtag-graf_avg-sigma_VS_CNT-gt_linlinlin.dat', and
        /// detailed hashtag info to 'hashtag-graf_sigma_VS_count_linlin_2.dat'.
        /// </remarks>
        public void run()
        {
            Console.WriteLine("Reading database...");
            Stopwatch stopWatch = new Stopwatch();
            List<Tuple<int, double>> data = new List<Tuple<int, double>>();
            List<double> sigmas = new List<double>();
            List<int> CNTk = new List<int>();

            stopWatch.Start();
            init("SELECT * FROM rudolf.dbo.hashtag_sigmas WHERE CNT_gt > 1000 and sigma < 50");
            while (reader.Read())
            {
                data.Add(new Tuple<int,double>(Convert.ToInt32(reader["CNT_gt"].ToString().Trim()), Convert.ToDouble(reader["sigma"].ToString().Trim())));
                sigmas.Add(Convert.ToDouble(reader["sigma"].ToString().Trim()));
                CNTk.Add(Convert.ToInt32(reader["CNT_gt"].ToString().Trim()));
            }
            close();
            showTime(stopWatch);

            Hist2D<int, double> h = new Hist2D<int, double>(data, 50, 1000, 50, 200);
            h.writeToFile("hashtag-graf_03", x_dataName: "CNT_gt", y_dataName: "sigma_R");

            Hist1D<double> h1 = new Hist1D<double>(sigmas, 1, 50);
            h1.writeToFile("h1d");

            Hist1D<int> h2 = new Hist1D<int>(CNTk, 10, 3000);
            h2.writeToFile("h2d");

            // Caculating average
            TextWriter avg;
            avg = new StreamWriter(defaultPath + "hashtag-graf_avg-sigma_VS_CNT-gt_linlinlin.dat");
            double temp_avg = 0.0;
            int temp_sum = 0;
            for (int C = 0; C < 200; C++)
            {
                for (int S = 0; S < h.y_binNumber; S++)
                {
                    temp_avg += 1.0 * h.hist[C, S] * (S) * h.y_binSize;
                    temp_sum += h.hist[C, S];
                }
                temp_avg = (1.0 * temp_avg) / (1.0 * temp_sum);
                avg.WriteLine("{0} {1}", (C + 1), temp_avg);
                temp_avg = 0.0;
                temp_sum = 0;
            }
            int step = 20;
            for (int C = 200; C < h.x_binNumber; C += step)
            {
                for (int c = 0; c < step; c++)
                {
                    for (int S = 0; S < h.y_binNumber; S++)
                    {
                        temp_avg += 1.0 * h.hist[(C + c), S] * (S) * h.y_binSize;
                        temp_sum += h.hist[(C + c), S];
                    }
                }
                temp_avg = (1.0 * temp_avg) / (1.0 * temp_sum);
                avg.WriteLine("{0} {1}", (C + 1), temp_avg);
                temp_avg = 0.0;
                temp_sum = 0;
            }
            avg.Close();

            TextWriter ns = new StreamWriter(defaultPath + "ns.dat");
            int counter = 1;
            for (int x = 0; x < h.x_binNumber; x++)
            {
                int sum = 0;
                for (int y = 0; y < h.y_binNumber; y++)
                    sum += h.hist[x, y];
                ns.WriteLine("{0} {1}", counter, sum);
                counter++;
            }

            Console.WriteLine("Done.");
        }
    }
}