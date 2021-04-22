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
    /// This class stores certain properties of a particular hashtag. Contains all of the coordinates of the occurances.
    /// </summary>
    class ListAndCNT
    {
        /// <summary>
        /// The string representation of the tag.
        /// </summary>
        /// <remarks>The 'tag' is case insensitive.</remarks>
        public string tag;
        /// <summary>
        /// Stores a list of geo-coordinates where the hashtag was published.
        /// </summary>
        public List<geoPoint> List;
        /// <summary>
        /// The count of the tag was tagged.</summary>
        /// <remarks>Not all tweets are geo-tagged so it can be different from CNT_gt.</remarks>
        public int CNT;
        /// <summary>
        /// The count of the tag was tagged in tweets with coordinates.
        /// </summary>
        /// <remarks>Not all tweets are geo-tagged so it can be different from CNT.</remarks>
        public int CNT_gt;

        /// <summary>
        /// Initializes a ListAndCNT instance with the given attributes.
        /// </summary>
        /// <param name="tag_">The tag attribute of the ListAndCNT class.</param>
        /// <param name="List_">The List attribute of the ListAndCNT class.</param>
        /// <param name="CNT_">The CNT attribute of the ListAndCNT class.</param>
        /// <param name="CNT_gt_">The CNT_gt attribute of the ListAndCNT class.</param>
        public ListAndCNT(string tag_, List<geoPoint> List_, int CNT_, int CNT_gt_)
        {
            this.tag = tag_;
            this.List = List_;
            this.CNT = CNT_;
            this.CNT_gt = CNT_gt_;
        }
    }

    /// <summary>
    /// This class stores certain properties of a particular hashtag. Contans only the standard deviance about the location information.
    /// </summary>
    class Pairs
    {
        /// <summary>
        /// The string representation of the tag.
        /// </summary>
        /// <remarks>The 'tag' is case insensitive.</remarks>
        public string tag;
        /// <summary>
        /// The standard deviance of coordinates of the geotagged occurances of the hashtag.
        /// </summary>
        public double sigma;
        /// <summary>
        /// The count of the tag was tagged.</summary>
        /// <remarks>Not all tweets are geo-tagged so it can be different from CNT_gt.</remarks>
        public int CNT;
        /// <summary>
        /// The count of the tag was tagged in tweets with coordinates.
        /// </summary>
        /// <remarks>Not all tweets are geo-tagged so it can be different from CNT.</remarks>
        public int CNT_gt;

        /// <summary>
        /// Initializes a Pairs instance with the given attributes.
        /// </summary>
        /// <param name="tag_">The tag attribute of the Pairs class.</param>
        /// <param name="sigma_">The sigma attribute of the Pairs class.</param>
        /// <param name="CNT_">The CNT attribute of the Pairs class.</param>
        /// <param name="CNT_gt_">The CNT_gt attribute of the Pairs class.</param>
        public Pairs(string tag_, double sigma_, int CNT_, int CNT_gt_)
        {
            this.tag = tag_;
            this.sigma = sigma_;
            this.CNT = CNT_;
            this.CNT_gt = CNT_gt_;
        }
    }

    /// <summary>
    /// The minimalist version of the Pairs class. Contains only the number of occurances and the standard deviance with memory friendly types.
    /// </summary>
    class PairsLowMem
    {
        /// <summary>
        /// The standard deviance of coordinates of the geotagged occurances of the hashtag.
        /// </summary>
        public float sigma;
        /// <summary>
        /// The count of the tag was tagged in tweets with coordinates.
        /// </summary>
        public ushort CNT_gt;

        /// <summary>
        /// Initializes a new PairsLowMem instance with the given attributes.
        /// </summary>
        /// <param name="sigma_">The sigma attribute of te PairsLowMem class.</param>
        /// <param name="CNT_gt_">The CNT_gt attribute of te PairsLowMem class.</param>
        public PairsLowMem(double sigma_, int CNT_gt_)
        {
            this.sigma = (float)sigma_;
            this.CNT_gt = (ushort)CNT_gt_;
        }
    }

    /// <summary>
    /// This class reads all the hashtags from a file grouped by tags. It calculates the average and standard deviance of the particula tags.
    /// </summary>
    class hashtag_sigma : TwitterBase
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
            Console.WriteLine("Reading file...");

            Stopwatch stopWatch = new Stopwatch();

            //List<Pairs> sigmas = new List<Pairs>();
            List<geoPoint> subList = new List<geoPoint>();
            stopWatch.Start();
            string previous = "", twobefore = "";
            int temp_CNT = 0, temp_CNT_gt = 0;
            bool firstrow = true;

            // Read data (from file) ------------------------------------------------------------
            string filename = "hashtag";
            StreamReader testFile = new StreamReader(@"" + defaultPath + filename + ".txt");
            string line;
            int counter = 0;
            string[] arrWords = new string[6]; // The container of split line

            string query = "delete FROM [rudolf].[dbo].[hashtag_sigmas]";
            initNoExe(query);
            command.ExecuteNonQuery();
            close();

            while ((line = testFile.ReadLine()) != null)
            {
                arrWords = line.Split(' ');
                // If the current tag and the previous is the same, we just add the geolocation to the list.
                // But if it's different, we save the tag's properties to the ListOfLists and clear the 'subList' temporary container
                string current = arrWords[0].ToString().Trim();
                if (!current.Equals(previous, StringComparison.InvariantCultureIgnoreCase) && !current.Equals(twobefore, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!firstrow)
                    {
                        //sigmas.Add(new Pairs(previous, GetStdDev(subList), temp_CNT, temp_CNT_gt));
                        query = "INSERT INTO rudolf.dbo.hashtag_sigmas (tag, sigma, CNT, CNT_gt) VALUES ('" + previous + "'," + GetStdDev(subList).ToString() + "," + temp_CNT.ToString() + "," + temp_CNT_gt.ToString() + ")";
                        initNoExe(query);
                        command.ExecuteNonQuery();
                        close();
                        subList.Clear();
                    }
                    else
                        firstrow = false;
                    try
                    {
                        temp_CNT = Convert.ToInt32(arrWords[4]);
                        temp_CNT_gt = Convert.ToInt32(arrWords[5]);
                    }
                    catch { }
                }
                try
                {
                    subList.Add(new geoPoint(Double.Parse(arrWords[1].ToString().Trim()), Double.Parse(arrWords[2].ToString().Trim()), Double.Parse(arrWords[3].ToString().Trim())));
                    twobefore = previous;
                    previous = current;
                }
                catch { }
                counter++;
                if ((counter % 1000000) == 0)
                {
                    Console.WriteLine(current + ", count = " + counter.ToString());
                }
            }

            testFile.Close();
            System.Console.WriteLine("There were {0} lines.", counter);

            showTime(stopWatch);
            { }
            // Makin' da histogram ----------------------------------------------------------
            /*           Hist2D<int, double> h = new Hist2D<int, double>(1, 2000, 100, 100);
                       foreach (Pairs p in sigmas)
                           h.addData(p.CNT_gt, p.sigma);
                       h.writeToFile("hashtag-graf_sigma_CNT-gt_hist_linlinlin", x_dataName: "CNT_gt", y_dataName: "sigma");

                       // Writig out avg ---------------------------------------------------------------
                       TextWriter avg;
                       avg = new StreamWriter(defaultPath + "hashtag-graf_avg-sigma_VS_CNT-gt_linlinlin.dat");
                       double temp_avg = 0.0;
                       int temp_sum = 0;
                       for (int C = 0; C < 200; C++)
                       {
                           for (int S = 0; S < h.y_binNumber; S++)
                           {
                               temp_avg += (1.0 * h.hist[C, S] * S * h.y_binSize) / 2.0;
                               temp_sum += h.hist[C, S];
                           }
                           temp_avg = (1.0 * temp_avg) / (1.0 * temp_sum);
                           avg.WriteLine("{0} {1}", (C+1), temp_avg);
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
                                   temp_avg += (1.0 * h.hist[(C+c), S] * S * h.y_binSize) / 2.0;
                                   temp_sum += h.hist[(C + c), S];
                               }
                           }
                           temp_avg = (1.0 * temp_avg) / (1.0 * temp_sum);
                           avg.WriteLine("{0} {1}", (C + 1), temp_avg);
                           temp_avg = 0.0;
                           temp_sum = 0;
                       }
                       avg.Close();

                       //sigmas.Sort();
                       // Writing output ---------------------------------------------------------------
                       Console.WriteLine("Writing putput...");
                       TextWriter tw1 = new StreamWriter(defaultPath + "hashtag-graf_sigma_VS_count_linlin_2.dat");
                       tw1.WriteLine("#CNT\t CNT_gt\t sigma (rad)\t sigma (km)\t tag");
                       foreach (Pairs p in sigmas)
                           if (p.CNT_gt > 1000) tw1.WriteLine("{0}\t{1}\t{2}", p.CNT_gt, p.sigma, p.sigma * R);
                       tw1.Close();
           */
            Console.WriteLine("Done.");
        }
    }
}