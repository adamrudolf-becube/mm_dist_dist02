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
    /// Class generates a scatterplot and histogram of the users in mutual follower degree-mutual mention degree space.
    /// </summary>
    class scatter : TwitterBase
    {
        // Custom variables -----------------------------------------------------------------
        List<Tuple<int, int>> pairs = new List<Tuple<int, int>>();

        /// <summary>
        /// The "main function of the class. Generates a scatterplot and histogram of the users in mutual follower degree-mutual mention degree space. Writes results to the .dat file called scatter_hist.
        /// </summary>
        /// <remarks>Saves results to a .dat file named 'scatter_hist' at defaultPath.</remarks>
        public void run()
        {
            init("SELECT [follow_deg],[mention_deg] FROM [rudolf].[dbo].[users_intersection_degree] ORDER BY mention_deg, follow_deg");

            // Data read --------------------------------------------------------------------
            Console.WriteLine("Started reading data...");

            int foldeg, mendeg;
            int maxfoldeg = 0, maxmendeg = 0;
            while (reader.Read())
            {
                // Getting data
                foldeg = Convert.ToInt32(reader["follow_deg"].ToString().Trim());
                mendeg = Convert.ToInt32(reader["mention_deg"].ToString().Trim());
                if (foldeg > maxfoldeg) maxfoldeg = foldeg;
                if (mendeg > maxmendeg) maxmendeg = mendeg;

                pairs.Add(new Tuple<int, int>(mendeg, foldeg));
            }
            close();
            int sizeOfRatios = pairs.Count();
            Console.WriteLine("Load complete. {0} numbers loaded.", sizeOfRatios);

            // Calculating averages
            int N = 1000;
            int n = 0;
            int cnt_temp = 0;
            double temp_x = 0, temp_y = 0;
            List<Tuple<double, double>> avgs = new List<Tuple<double, double>>();
            foreach (Tuple<int, int> pair in pairs)
            {
                n++;
                cnt_temp++;
                temp_x += 1.0 * pair.Item1;
                temp_y += 1.0 * pair.Item2;
                if (pair.Item1 < 10)
                {
                    if (n % 300000 == 0)
                    {
                        temp_x = temp_x / (1.0 * cnt_temp);
                        temp_y = temp_y / (1.0 * cnt_temp);
                        avgs.Add(new Tuple<double, double>(temp_x, temp_y));
                        temp_x = 0.0;
                        temp_y = 0.0;
                        cnt_temp = 0;
                    }
                }
                else
                {
                    if (pair.Item1 < 20)
                    {
                        if (n % 100000 == 0)
                        {
                            temp_x = temp_x / (1.0 * cnt_temp);
                            temp_y = temp_y / (1.0 * cnt_temp);
                            avgs.Add(new Tuple<double, double>(temp_x, temp_y));
                            temp_x = 0.0;
                            temp_y = 0.0;
                            cnt_temp = 0;
                        }
                    }
                    else
                    {
                        if (n % 10000 == 0)
                        {
                            temp_x = temp_x / (1.0 * cnt_temp);
                            temp_y = temp_y / (1.0 * cnt_temp);
                            avgs.Add(new Tuple<double, double>(temp_x, temp_y));
                            temp_x = 0.0;
                            temp_y = 0.0;
                            cnt_temp = 0;
                        }
                    }
                }
            }

            TextWriter tw1 = new StreamWriter(defaultPath + "scatter_hist_avgs_2.dat");
            foreach (Tuple<double, double> pair in avgs)
                tw1.WriteLine("{0} {1}", pair.Item1, pair.Item2);
            tw1.Close();

        /*    // Making histogram --------------------------------------------------------
            Hist2D<int, int> h = new Hist2D<int, int>(pairs, 1, 3000, 1, 100);
            h.writeToFile("scatter_hist", x_dataName: "follow degree", y_dataName: "mention_degree", substitute: 0.1);
         */   
        }
    }
}
