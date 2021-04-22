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
    /// Class reads the ratios of back and forth mention counts and makes the PDF of it.
    /// </summary>
    class ratio_dist : TwitterBase
    {
        // Custom variables -----------------------------------------------------------------
        int N = 100000; // Number of read lines
        List<double> ratios = new List<double>();

        /// <summary>
        /// The "main" function of the class. Reads the ratios of back and forth mention counts and makes the PDF of their ratio.
        /// </summary>
        /// <remarks>Saves results to a .dat file named 'ratio-dist' at the defaultPath.</remarks>
        public void run()
        {
            // Reading connected user data, and making histogram ---------------------------
            //init("select * from [rudolf].[dbo].[mention_mutual_all] where atob >= 10 and btoa >= 10");

            // Data read --------------------------------------------------------------------

            double binSize = 0.02;
            int binNumber = (int)(1 / binSize) + 2;
            Console.WriteLine("Started reading data...");
            //Hist2D<int, int> ms = new Hist2D<int, int>(10, 800, 10, 800);
            /*while (reader.Read())
                ratios.Add(Convert.ToDouble(reader["ratio"].ToString().Trim()));
            close();
            int sizeOfRatios = ratios.Count();
            Console.WriteLine("Load complete. {0} numbers loaded.", sizeOfRatios);
            
            //ms.writeToFile("ratio_dist_hist");
            Console.WriteLine("Calculate histogram...");
            //Distribution<double> DF = new Distribution<double>(ratios, 50);
            Hist1D<double> HF = new Hist1D<double>(ratios, binSize, binNumber);
            int summ1 = 0;
            foreach (int s in HF.hist)
                summ1 += s;

            // Write normalized hist to file
            Console.WriteLine("Write histogram to file...");
            TextWriter tw1 = new StreamWriter(defaultPath + "ratio_dist_h2.dat");
            for (int j = 0; j < HF.binNumber - 1; j++)
                tw1.WriteLine("{0}\t{1}\t{2}", j, j * HF.binSize, (1.0 * HF.hist[j]) / (1.0 * summ1));
            tw1.Close();
            */
            // Calculate random ratios
            Random rand = new Random();
            Console.WriteLine("Calculate random histogram...");
            List<Tuple<int, int>> data = loadData();
            Hist1D<double> HF2 = new Hist1D<double>(binSize, binNumber);
            for (int i = 0; i < 60000000; i++)
            {
                double asdf = getRandomR(data, rand, 10);
                if (asdf > 0)
                    HF2.addData(asdf);
            }

            int summ2 = 0;
            foreach (int s in HF2.hist)
                summ2 += s;

            // Write them to file
            Console.WriteLine("Write random histogram...");
            TextWriter tw2 = new StreamWriter(defaultPath + "ratio_dist_rand.dat");
            for (int j = 0; j < HF2.binNumber - 1; j++)
                tw2.WriteLine("{0}\t{1}\t{2}", j, j * HF2.binSize, (1.0 * HF2.hist[j]) / (1.0 * summ2));
            tw2.Close();
            
            //HF.writeToFile("ratio_dist_hs", dataName: "ratio");
            //DF.writeToFile("ratio_dist", dataName: "ratio");
        }

        public List<Tuple<int, int>> loadData()
        {
            // Read the file
            List<Tuple<int, int>> data = new List<Tuple<int,int>>();
            string[] arrWords = new string[5];
            string line;
            StreamReader testFile = new StreamReader(@"" + defaultPath + "temp.txt");

            data.Add(new Tuple<int, int>(0, 0));
            while ((line = testFile.ReadLine()) != null)
            {
                arrWords = line.Split('\t');
                data.Add(new Tuple<int, int>(Convert.ToInt32(arrWords[0]), Convert.ToInt32(arrWords[2])));
            }
            return data;
        }

        public int getRandomW(List<Tuple<int, int>> data, Random rand, int min)
        {
            // Stores the sum of the weights for normalizing
            int sum = data[data.Count-1].Item2 + 1;
            // For the random generating
            int ret = -1;
            while (ret < min)
            {
                int temp = rand.Next(data[min].Item2, sum + 1);
                int i = min;
                while(!((data[i - 1].Item1 <= temp) && (data[i].Item2 > temp)) && i < (data.Count-1))
                {
                    ret = data[i].Item1;
                    i++;
                }
            }
            return ret;
        }

        public double getRandomR(List<Tuple<int, int>> data, Random rand, int min)
        {
            int W1 = getRandomW(data, rand, min);
            int W2 = getRandomW(data, rand, min);
            if (W1 <= W2)
                return (1.0 * W1) / (1.0 * W2);
            else
                return (1.0 * W2) / (1.0 * W1);
        }
    }
}
