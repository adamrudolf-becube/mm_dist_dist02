using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Twitter_tools
{
    /// <summary>
    /// Dummy class for testing new methods. Contents often change.
    /// </summary>
    class sigma_tester : TwitterBase
    {
        TwitterBase s1 = new TwitterBase();
        TwitterBase s2 = new TwitterBase();
        List<Int64> elements = new List<Int64>();

        /// <summary>
        /// The "main" function of the class.
        /// </summary>
        public void run()
        {
            Console.WriteLine("Reading file...");

            Stopwatch stopWatch = new Stopwatch();

            List<Pairs> sigmas = new List<Pairs>();
            List<geoPoint> subList = new List<geoPoint>();
            stopWatch.Start();
            string previous = "", twobefore = "";
            int temp_CNT_gt = 0;
            int temp_CNT = 0;
            bool firstrow = true;

            // Read data (from file) ------------------------------------------------------------
            string filename = "hashtag";
            StreamReader testFile = new StreamReader(@"" + defaultPath + filename + ".txt");
            string line;
            int counter = 0;
            string[] arrWords = new string[6]; // The container of split line
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<int> ints = new List<int>();

            while ((line = testFile.ReadLine()) != null)
            {
                
                // If the current tag and the previous is the same, we just add the geolocation to the list.
                // But if it's different, we save the tag's properties to the ListOfLists and clear the 'subList' temporary container

                if (counter > 117854518 && counter < 117854535)
                {
                    Console.WriteLine(line);
                }
                counter++;
            }

            testFile.Close();
            System.Console.WriteLine("There were {0} lines.", counter);
            showTime(watch);

            TextWriter tw1 = new StreamWriter(defaultPath + "afffff.txt");
            foreach (int i in ints)
                tw1.WriteLine(i.ToString());
        }
    }
}
