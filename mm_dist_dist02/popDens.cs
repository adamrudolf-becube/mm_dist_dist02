using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twitter_tools
{
    class popDens : TwitterBase
    {
        public void run()
        {
            init("select lon, lat from Twitter_1.dbo.user_location_cluster where cluster_id = 0");

            List<Tuple<double, double>> coords = new List<Tuple<double, double>>();
            while (reader.Read())
                coords.Add(new Tuple<double,double>(Convert.ToDouble(reader["lon"].ToString().Trim()) + 180.0, Convert.ToDouble(reader["lat"].ToString().Trim()) + 90.0));

            Hist2D<double, double> H = new Hist2D<double, double>(coords, 0.5, 720, 0.5, 360);
            H.writeToFile("popDens");
        }
    }
}
