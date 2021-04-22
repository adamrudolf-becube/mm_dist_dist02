using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twitter_tools
{
    using System.Data.SqlClient;
    using System.Data;
    using System.IO;
    using System.Diagnostics;

    /// <summary>
    /// This class is intended to be the base class all of the other classes which read from an SQL database and proces Twitter data.
    /// </summary>
    /// <remarks>The class contains all of teh attributes we need in our particular project and methods which help to initialize them, and other useful attributes and methods used by many classes.</remarks>
    class TwitterBase
    {
        // Function declarations ------------------------------------------------------------
        /// <summary>
        /// Gives the distance of two geoPoint objects n the surface of the unit sphere.
        /// </summary>
        /// <remarks>Based on the scalar product of the two unit vectors. The order of the two arguments does not matter. May return NaN.</remarks>
        /// <param name="u">The first geoPoint object.</param>
        /// <param name="v">The second geoPoint object.</param>
        /// <returns>with the distance of the two given geoPoint objects in radian. The return type is double.</returns>
        public double getDistance(geoPoint u, geoPoint v)
        {
            double theta = ((u.cx * v.cx) + (u.cy * v.cy) + (u.cz * v.cz)) / (Math.Sqrt((u.cx * u.cx) + (u.cy * u.cy) + (u.cz * u.cz)) * Math.Sqrt((v.cx * v.cx) + (v.cy * v.cy) + (v.cz * v.cz)));
            if (theta > 1 && theta < 1.1)
                return 0.0;
            double theta_real = Math.Acos(theta);
            return theta_real;
        }

        /// <summary>
        /// Gives the approximate distance of two close geoPoint objects on the surface of the unit sphere.
        /// </summary>
        /// <remarks>Approximates the surface with a plan and gives the magnitude of the difference vector of the two given vectors as the distance of the points. The order of the two arguments does not matter.</remarks>
        /// <param name="u">The first geoPoint object.</param>
        /// <param name="v">The second geoPoint object.</param>
        /// <returns>with the approximate distance of the two given close geoPoint objects in radian. The return type is double.</returns>
        public double getApproxDistance(geoPoint u, geoPoint v)
        {
            double diffSq = Math.Pow((u.cx - v.cx), 2.0) + Math.Pow((u.cy - v.cy), 2.0) + Math.Pow((u.cz - v.cz), 2.0);
            return Math.Sqrt(diffSq);
        }

        /// <summary>
        /// Gives the average of the given set of geoPoint objects.
        /// </summary>
        /// <remarks>Sums all the unit vectors and normalizes after, so the result vector will be on the surface of the unit sphere as well. The method initialzes a new geoPoint object with the resulting unt vector.</remarks>
        /// <param name="gps">List object of geoPoint objects. Can contain any number of geoPoint objects.</param>
        /// <returns>a geoPoint object which is the "middle" of the set given as the parameter.</returns>
        public geoPoint GetAvg(List<geoPoint> gps)
        {
            geoPoint gp_avg = new geoPoint(0, 0, 0);

            foreach (geoPoint g in gps)
            {
                gp_avg.cx += g.cx;
                gp_avg.cy += g.cy;
                gp_avg.cz += g.cz;
            }

            gp_avg.normalize();

            return gp_avg;
        }

        /// <summary>
        /// Calculates the standard deviance of the given set of geoPoint objects.
        /// </summary>
        /// <remarks>Returns a double number which is the standard deviance of the given set of geoPoint objects. The method calculates the average of the geoPoint set, then it sums the squared differences and takes the square root of the average of squared differences. Returns -1 is the average is a NaN.</remarks>
        /// <param name="gps">List object of geoPoint objects. Can contain any number of geoPoint objects.</param>
        /// <returns>a double value which is the standard deviance of the given set of geoPoint objects in radian. The return value is -1 if sigma is NaN for some reason.</returns>
        public double GetStdDev(List<geoPoint> gps)
        {
            geoPoint avg = new geoPoint();
            avg = GetAvg(gps);
            double dist = 0;
            double avg_dist_sq = 0;

            foreach (geoPoint g in gps)
            {
                dist = getDistance(g, avg);
                avg_dist_sq += dist * dist;
            }

            avg_dist_sq = avg_dist_sq / gps.Count;

            double sigma = Math.Sqrt(avg_dist_sq);
            if (Double.IsNaN(sigma))
            {
                return -1;
            }
            else
            {
                if (sigma < 0.000001)
                    return 0;
                else
                    return sigma;
            }
        }

        /// <summary>
        /// Calculates the average absolute deviation of the given set of geoPoint objects.
        /// </summary>
        /// <remarks>Returns a double number which is the average absolute deviation of the given set of geoPoint objects. The method calculates the average of the geoPoint set, then it sums the differences and takes the average of differences. Returns -1 is the average is a NaN.</remarks>
        /// <param name="gps">List object of geoPoint objects. Can contain any number of geoPoint objects.</param>
        /// <returns></returns>
        public double GetAvgAbsDev(List<geoPoint> gps)
        {
            geoPoint avg = new geoPoint();
            avg = GetAvg(gps);
            double dist = 0;
            double avg_dist_sq = 0;

            foreach (geoPoint g in gps)
            {
                dist = getDistance(g, avg);
                avg_dist_sq += dist;
            }

            avg_dist_sq = avg_dist_sq / gps.Count;

            double sigma = avg_dist_sq;
            if (Double.IsNaN(sigma))
            {
                return -1;
            }
            else
            {
                if (sigma < 0.000001)
                    return 0;
                else
                    return sigma;
            }
        }

        /// <summary>
        /// Stops the given Stopwatch instance and writes it's current state to the screen.
        /// </summary>
        /// <remarks>The method expects a System.Diagnostics.Stopwatch instance as a parameter and writes it's current Elapsed property to the screen in "Elapsed time: HH:MM:SS.SS" format.</remarks>
        /// <param name="stopWatch">System.Diagnostics.Stopwatch object instance.</param>
        public void showTime(Stopwatch stopWatch)
        {
            // Show time! -------------------------------------------------------------------
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            //            Console.WriteLine("Query executed in " + elapsedTime);
            Console.WriteLine("Elapsed time: " + elapsedTime);
        }

        // SQL connection objects -----------------------------------------------------------
        /// <summary>
        /// SqlConnection class for connecting to the Twitter database
        /// </summary>
        public SqlConnection con;
        /// <summary>
        /// SqlConnectionStringBuilder class for 
        /// </summary>
        public SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
        /// <summary>
        /// Contains the SQL query.
        /// </summary>
        public SqlCommand command;
        /// <summary>
        /// Iterates through the database and reads the table line by line.
        /// </summary>
        public SqlDataReader reader;

        // Other attributes
        /// <summary>
        /// The average radius of Earth.
        /// </summary>
        /// <remarks>This has nothing to do with databases, but almost all of the classes which implement SQL_reader use this constant so it is useful to teclare it here.</remarks>
        public const double R = 6372.797;

        /// <summary>
        /// The default directory each class writes to and reads from.
        /// </summary>
        /// <remarks>Originally it is set to \\\\QSO\\Public\\rudolf\\My Documents\\</remarks>
        public const string defaultPath = "\\\\QSO\\Public\\rudolf\\My Documents\\";

        // SQL related initializations and finalizations
        /// <summary>
        /// Sets all of the attributes of SQL_reader superclass to the most commonly used values. Does noot start to execute the query.
        /// </summary>
        /// <remarks>Sets the datasource to 'future1', and the initial catalog to 'rudolf'.
        /// Integrated Security to 'true', ConnectionTimeOut to '100 000'. Sets the command to
        /// the given string in the parameter, opens the connection, so after calling this method
        /// we only have to execute the query and use the reader.Read() command.</remarks>
        /// <param name="com">Expects a string variable containing a valid SQL query. reader will execute this query.</param>
        /// <param name="noExecute">A boolean variable. The value of it does not have any effect on the behavior of the method. Only the existence of it prevents the function from executing the query.</param>
        public void initNoExe(string com)
        {
            command = new SqlCommand(com);

            csb.DataSource = "future1";
            csb.InitialCatalog = "rudolf";
            csb.IntegratedSecurity = true;
            csb.ConnectTimeout = 100000;

            con = new SqlConnection(cs.bConnectionString);
            con.Open();
            command.Connection = con;
            command.CommandTimeout = 100000;
        }

        /// <summary>
        /// Sets all of the attributes of SQL_reader superclass to the most commonly used values.
        /// </summary>
        /// <remarks>Sets the datasource to 'future1', and the initial catalog to 'rudolf'.
        /// Integrated Security to 'true', ConnectionTimeOut to '100 000'. Sets the command to
        /// the given string in the parameter, opens the connection and starts to execute the
        /// query so after calling this method we only have to use the reader.Read() command.</remarks>
        /// <param name="com">Expects a string variable containing a valid SQL query. reader will execute this query.</param>
        public void init(string com)
        {
            initNoExe(com);
            reader = command.ExecuteReader();
        }

        /// <summary>
        /// Closes the SqlConnection con connection property of the SQL_reader superclass (probably) opened in the init(string com) method.
        /// </summary>
        public void close()
        {
            con.Close();
        }
    }
}
