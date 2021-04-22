using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twitter_tools
{
    /// <summary>
    /// This class represents a point on the surface of the Earth or a point on a unit sphere
    /// </summary>
    class geoPoint
    {
        /// <summary>
        /// The x coordinate of the unit vector representation of the geoPoint.
        /// </summary>
        public double cx;
        /// <summary>
        /// The y coordinate of the unit vector representation of the geoPoint.
        /// </summary>
        public double cy;
        /// <summary>
        /// The z coordinate of the unit vector representation of the geoPoint.
        /// </summary>
        public double cz;
        /// <summary>
        /// The latitude coordinate of the geoPoint.
        /// </summary>
        public double lat;
        /// <summary>
        /// The longitude coordinate of the geoPoint.
        /// </summary>
        public double lon;

        /// <summary>
        /// Returns the norm (length) of the x, y, z vector.
        /// </summary>
        /// <returns></returns>
        public double norm()
        {
            return Math.Sqrt((this.cx * this.cx) + (this.cy * this.cy) + (this.cz * this.cz));
        }

        /// <summary>
        /// Normalizes the vector. Changes the length to 1 and leaves the direction.
        /// </summary>
        public void normalize()
        {
            double norm = this.norm();
            this.cx = (1.0 * this.cx) / (1.0 * norm);
            this.cy = (1.0 * this.cy) / (1.0 * norm);
            this.cz = (1.0 * this.cz) / (1.0 * norm);
        }

        /// <summary>
        /// Initializes a geoPoint instance with 0, 0, 0 coordinates.
        /// </summary>
        public geoPoint()
        {
            this.cx = 0.0;
            this.cy = 0.0;
            this.cz = 0.0;
        }

        /// <summary>
        /// Initializes a geoPoint instance with the givel longitude and latitude coordinates,
        /// </summary>
        /// <param name="setlon">The longitude coordinate.</param>
        /// <param name="setlat">The latitude coordinate.</param>
        public geoPoint(double setlon, double setlat)
        {
            this.lat = setlat;
            this.lon = setlon;
            this.setXYZ();
        }

        /// <summary>
        /// Initializes a geoPoint instance with the given x,y,z coordinates.
        /// </summary>
        /// <param name="x">The x coordinate of the geoPoint.</param>
        /// <param name="y">The y coordinate of the geoPoint.</param>
        /// <param name="z">The z coordinate of the geoPoint.</param>
        public geoPoint(double x, double y, double z)
        {
            this.cx = x;
            this.cy = y;
            this.cz = z;
        }

        /// <summary>
        /// Initializes a geoPoint instance equals to the givel geoPoint instance.
        /// </summary>
        /// <param name="GP"></param>
        public geoPoint(geoPoint GP)
        {
            this.cx = GP.cx;
            this.cy = GP.cy;
            this.cz = GP.cz;
            this.lat = GP.lat;
            this.lon = GP.lon;
        }

        /// <summary>
        /// Generates a geoPoint on a tweets point of the unit sphere with uniform spherical distribution.
        /// </summary>
        /// <remarks>Mathematical background at http://mathworld.wolfram.com/SpherePointPicking.html</remarks>
        /// <param name="tweets">Random class of the System.Random library.</param>
        public geoPoint(Random random)
        {
            double x1 = 1.0;
            double x2 = 1.0;
            while((x1 * x1 + x2 * x2) >= 1)
            {
                x1 = random.NextDouble() * 2.0 - 1.0;
                x2 = random.NextDouble() * 2.0 - 1.0;
            }

            this.cx = 2.0 * x1 * Math.Sqrt(1.0 - x1 * x1 - x2 * x2);
            this.cy = 2.0 * x2 * Math.Sqrt(1.0 - x1 * x1 - x2 * x2);
            this.cz = 1.0 - 2.0 * (x1 * x1 + x2 * x2);
            this.normalize();
        }

        /// <summary>
        /// Sets the latitude and longitude coordinates based on the x, y, z coordinates.
        /// </summary>
        public void setLatLon()
        {
            this.normalize();
            this.lat = 180.0 * Math.Acos(this.cz) / Math.PI;
            this.lon = 180.0 * Math.Atan2(this.cy, this.cx) / Math.PI; ;
        }

        /// <summary>
        /// Sets the x, y, z coordinates based on tha latitude and longitude coordinates.
        /// </summary>
        public void setXYZ()
        {
            this.cx = Math.Sin(Math.PI * this.lat / 180.0) * Math.Cos(Math.PI * this.lon / 180.0);
            this.cy = Math.Sin(Math.PI * this.lat / 180.0) * Math.Sin(Math.PI * this.lon / 180.0);
            this.cz = Math.Cos(Math.PI * this.lon / 180.0);
        }

        /// <summary>
        /// Returns the String representation of the geoPoint instance.
        /// </summary>
        /// <remarks>Returns the x, y, z coordinates in String.</remarks>
        /// <returns></returns>
        public override string ToString()
        {
            return cx.ToString() + " " + cy.ToString() + " " + cz.ToString();
        }

        /// <summary>
        /// Returns the String representation of the geoPoint instance.
        /// </summary>
        /// <remarks>Returns the longitude and latitude coordinates in String.</remarks>
        /// <returns></returns>
        public string getLatLonString()
        {
            return lon.ToString() + " " + lat.ToString();
        }
    }
}
