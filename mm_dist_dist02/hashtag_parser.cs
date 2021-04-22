using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Twitter_tools
{
/// <summary>
    /// This class reads all of the tweets from a database and parses all of the hashtags out of it.
    /// </summary>
    /// <remarks>This class is derived from the class SQL_reader. It reads all the tweets from the database 'future1' ansd seeks for all the hashtags.</remarks>
    class hashtag_parser : TwitterBase
    {
        List<Tuple<Int16, String, Int64>> tags = new List<Tuple<short, string, long>>();
        string tag;
        /// <summary>
        /// The "main" function of the class. Reads all of the tweets from a database and parses all of the hashtags out of it.
        /// </summary>
        public void run()
        {
            Regex anySpaces = new Regex("[ \t]+");
            Regex quot = new Regex("'");
            string[] arrWords;
            init("SELECT [run_id],[tweet_id],[created_at],[utc_offset],[user_id],[lon],[lat],[cx],[cy],[cz],[text] FROM [Twitter_1].[dbo].[tweet]");
            Console.WriteLine("Read tags");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (reader.Read())
            {
                arrWords = anySpaces.Split(reader["text"].ToString().Trim());
                foreach (string s in arrWords)
                {
                    if (s.StartsWith("#"))
                    {
                        tag = s.Substring(1);
                        if (quot.IsMatch(tag))
                        {
                            tag = quot.Replace(tag, "");
                        }
                        tags.Add(new Tuple<Int16, String, Int64>(Convert.ToInt16(reader["run_id"].ToString().Trim()),tag,Convert.ToInt64(reader["tweet_id"].ToString().Trim())));

                    }
                }
            }
            close();
            showTime(stopWatch);

            Console.WriteLine("Read done, started writing...");
            Stopwatch nyu = new Stopwatch();
            nyu.Start();
            foreach (Tuple<Int16, String, Int64> nyenyenyunyu in tags)
            {
                string query = "INSERT INTO rudolf.dbo.tweet_hashtag (run_id, tag, tweet_id) VALUES (" + nyenyenyunyu.Item1.ToString() + ", \'" + nyenyenyunyu.Item2 + "\', " + nyenyenyunyu.Item3.ToString() + ")";
                initNoExe(query);
                command.ExecuteNonQuery();
                close();
            }
            Console.Write("Done. ");
            showTime(nyu);
        }
    }
}
