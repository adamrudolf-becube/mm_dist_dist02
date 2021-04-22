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
    /// This class reads all the hashtags from a file grouped by tags. It calculates the average and standard deviance of the particula tags.
    /// </summary>
    class hashtag_sigma_hist : TwitterBase
    {
        /// <summary>
        /// The "main" function of the class. Reads all the hashtags from a txt file grouped by tags. It calculates the average and standard deviance of the particular tags and writes them to files.
        /// </summary>
        /// <remarks>Works in the directory stored at defaultPath. Reads data from a file called 'hashtag.txt' and writes 2 dimensional histogram to
        /// 'hashtag-graf_sigma_CNT-gt_hist_linlinlin.dat', 'average to hashtag-graf_avg-sigma_VS_CNT-gt_linlinlin.dat', and
        /// detailed hashtag info to 'hashtag-graf_sigma_VS_count_linlin_2.dat'.
        /// </remarks>
        public void run()
        {
            // Control panel
            ushort threshold = 1000;
            ushort xwidth = 20;
            Console.WriteLine("Reading file...");

            Stopwatch stopWatch = new Stopwatch();

            List<PairsLowMem> sigmas = new List<PairsLowMem>();
            List<geoPoint> subList = new List<geoPoint>();
            stopWatch.Start();

            // Read data (from file) ------------------------------------------------------------
            string filename = "hashtag-graf_sigma_VS_count_linlin_2";
            StreamReader testFile = new StreamReader(@"" + defaultPath + filename + ".dat");
            List<double> sigmaSet = new List<double>();
            List<Tuple<int, double>> data = new List<Tuple<int, double>>();

            int number = 10;
            Hist1D<double>[] datas = new Hist1D<double>[number];
            for (int i = 0; i < number; i++)
                datas[i] = new Hist1D<double>(5, 600);

            init("SELECT * FROM [rudolf].[dbo].[hashtag_sigmas]");
            while (reader.Read())
            {
                int CNT_temp = Convert.ToInt32(reader["CNT_gt"].ToString().Trim());
                double sigma_temp = Convert.ToDouble(reader["sigma"].ToString().Trim());
                //sigmaSet.Add(sigma_temp);
                //data.Add(new Tuple<int,double>(CNT_temp, sigma_temp));
                if (CNT_temp < number)
                    datas[CNT_temp].addData(sigma_temp);
            }
            close();

            showTime(stopWatch);
            for (int i = 1; i < number; i++)
            {
                TextWriter tw = new StreamWriter(defaultPath + "hashtag-graf_sigma_DIST_" + i.ToString() + ".dat");
                int sum = 0;
                for (int j = 0; j < datas[i].binNumber; j++)
                    sum += datas[i].hist[j];
                for (int j = 0; j < datas[i].binNumber; j++)
                    tw.WriteLine("{0} {1} {2}", j, j * datas[i].binSize, (1.0 * datas[i].hist[j]) / (1.0 * sum));
                tw.Close();
            }

            /*Hist1D<double> h1d = new Hist1D<double>(sigmaSet, 0.5, 100);
            h1d.writeToFile("hashtag-graf_h1d", dataName: "sigma", comment: "binSize = 100, binNumber = 200");

            // Makin' da histogram ----------------------------------------------------------
            Hist2D<int, double> h = new Hist2D<int, double>(data, 1, 20000, 50, 400);
            h.writeToFile("hashtag-graf_01", x_dataName: "CNT_gt", y_dataName: "sigma");
            Hist2D<int, double> h2 = new Hist2D<int, double>(data, 20, 1000, 50, 400);
            h2.writeToFile("hashtag-graf_02", x_dataName: "CNT_gt", y_dataName: "sigma");
            Hist2D<int, double> h3 = new Hist2D<int, double>(data, 50, 400, 50, 400);
            h3.writeToFile("hashtag-graf_03", x_dataName: "CNT_gt", y_dataName: "sigma");
            Hist2D<int, double> h4 = new Hist2D<int, double>(data, 100, 200, 50, 400);
            h4.writeToFile("hashtag-graf_04", x_dataName: "CNT_gt", y_dataName: "sigma");
             * */
            Console.WriteLine("Done.");
        }
    }
}