using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twitter_tools
{
    using System.IO;
    /// <summary>
    /// This class represents both Probability Distribution Function (PDF) and Cumulative Distribution Function (CDF) of any numeric data with variable bin size.
    /// </summary>
    /// <remarks>Both of the PDF and CDF functions are normalized to 1.</remarks>
    /// <typeparam name="T">Numeric type. This class will put this type of numbers to bins. Must implement IComparable interface and must be able to be converted to double.</typeparam>
    class Distribution<T> where T : IComparable
    {
        
        /// <summary>
        /// This struct represents a point of a Distribution Function. Just an x-y pair of variables. Type of x is T, and y is double.
        /// </summary>
        public struct DFpoint
        {
            /// <summary>
            /// Element of the domain of DF.
            /// </summary>
            public T x;
            /// <summary>
            /// Element of the codomain of DF. Output to the element x.
            /// </summary>
            public double y;
        }

        /// <summary>
        /// Number of the bins in the DFs.
        /// </summary>
        public int binNumber;
        /// <summary>
        /// Number of the samples (= values) stored in one bin.
        /// </summary>
        public int sampleNumber;
        /// <summary>
        /// The Cumulative Distribution Function itself as a List of DFpoint structs.
        /// </summary>
        public List<DFpoint> CDF = new List<DFpoint>();
        /// <summary>
        /// The Probability Distributon Function itself as a List of DFpoint structs.
        /// </summary>
        public List<DFpoint> PDF = new List<DFpoint>();

        /// <summary>
        /// Initializes a distribution instance. Puts elements of data to binNumber_arg variable sized bins.
        /// </summary>
        /// <remarks>This method sorts the List data, and splits it to (approximately) binNumber_arg parts.
        /// Each part becomes a bin, so the size of the bin is variable, but the y size of the bin is (apprximately) fixed.
        /// This is the Probability Distribution Function. After it, the method numerically derives the PDF so we get the
        /// Cumulative distribution function, CDF.</remarks>
        /// <exception cref="System.InvalidOperationException">Thrown when method can not convert T to double.</exception>
        /// <param name="data">List of T type objects. T must implement System.IComparabe interface, and can be converted to double. If not, this method throws a System.InvalidOperationException exception.</param>
        /// <param name="binNumber_arg">The number of the bins, i.e. the number of parts we split the data into.</param>
        public Distribution(List<T> data, int binNumber_arg)
        {
            // Making Cumulative Distribution Function --------------------------------------
            int size = data.Count;
            data.Sort();
            binNumber = binNumber_arg;
            sampleNumber = Convert.ToInt32((1.0 * size) / (1.0 * binNumber));
            DFpoint temp;
            for (int index = sampleNumber; index < size; index += sampleNumber)
            {
                temp.x = data[index];
                temp.y = (1.0 * index) / (1.0 * size);
                CDF.Add(temp);
            }

            // Numerical differentation so calculating CDF
            double CDFx, CDFx2;
            try
            {
                CDFx = Convert.ToDouble(CDF[0].x);
            }
            catch
            {
                throw new InvalidOperationException
                ("T must be a type convertable to double.");
            }
            temp.x = CDF[0].x;
            temp.y = CDF[0].y / CDFx;
            PDF.Add(temp);

            for (int j = 1; j < CDF.Count(); j++)
            {
                temp.x = CDF[j].x;
                try
                {
                    CDFx = Convert.ToDouble(CDF[j].x);
                    CDFx2 = Convert.ToDouble(CDF[j - 1].x);
                }
                catch
                {
                    throw new InvalidOperationException
                    ("T must be a type convertable to double.");
                }
                temp.y = (1.0 * sampleNumber) / (size * (CDFx - CDFx2));
                PDF.Add(temp);
            }
        }

        /// <summary>
        /// Writes the CDF and PDF data to a .dat file at the given path with the given filename.
        /// </summary>
        /// <remarks>This method uses the System.TextWriter class to write to a file. The first two lines are comments,
        /// beginning with the '#' mark for further working with the file in GNUPLOT. User can name the data so in the comments
        /// that name will be shown.
        /// Each line of the file represents a bin.
        /// The method writes the index of the bin, the element of data at the bin limits (= the x value of the bin),
        /// PDF and CDF value (= the y value) of the bin, and the logarithm of each respectively for easier data visualization.
        /// Method will write 0s instead of the log of the zero values. Base of logarithms is 10.</remarks>
        /// <exception cref="System.InvalidOperationException">Thrown when method can not convert T to double.</exception>
        /// <param name="fileName">Method will name the .dat file afterthis at the given path. Must not contain extension.</param>
        /// <param name="path">Method will save the file at this path. Can be omitted. If omitted, method will save file to TwitterBase.defaultPath.</param>
        /// <param name="dataName">Name of the data. Appear in the dat description line of the file. Default value is "data".</param>
        /// <param name="comment">Optional comment line. Method will write this string at the first line of the file after a '#' mark. If omitted, first line is left uot entirely.</param>
        public void writeToFile(string fileName, string path = TwitterBase.defaultPath, string dataName = "data", string comment = "")
        {
            TextWriter tw1 = new StreamWriter(path + fileName + ".dat");
            if (comment != "")
                tw1.WriteLine("#" + comment);
            tw1.WriteLine("#index " + dataName + "\t\t\t\tCDF\t\t\t\t\tPDF\t\t\t\tlog(" + dataName + ")\t\t\t\tlog(CDF)\t\t\tlog(PDF)");
            double logData;
            double RealData;
            for (int j = 0; j < PDF.Count - 1; j++)
            {
                try
                {
                    RealData = Convert.ToDouble(PDF[j].x);
                    if (RealData == 0)
                        logData = 0;
                    else
                        logData = Math.Log10(RealData);
                }
                catch
                {
                    throw new InvalidOperationException
                    ("T must be a type convertable to double.");
                }
                
                double logCDF, logPDF;
                if (CDF[j].y == 0)
                    logCDF = 0;
                else
                    logCDF = Math.Log10(CDF[j].y);

                if (PDF[j].y == 0)
                    logPDF = 0;
                else
                    logPDF = Math.Log10(PDF[j].y);
                tw1.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", j, RealData, CDF[j].y, PDF[j].y, logData, logCDF, logPDF);
            }
            tw1.Close();
        }

        /// <summary>
        /// Writes the CDF and PDF data to a .dat file at the given path with the given filename, but without their logarithms.
        /// </summary>
        /// <remarks>This method uses the System.TextWriter class to write to a file. The first two lines are comments,
        /// beginning with the '#' mark for further working with the file in GNUPLOT. User can name the data so in the comments
        /// that name will be shown.
        /// Each line of the file represents a bin.
        /// The method writes the index of the bin, the element of data at the bin limits (= the x value of the bin),
        /// PDF and CDF value (= the y value) of the bin</remarks>
        /// <param name="fileName">Method will name the .dat file afterthis at the given path. Must not contain extension.</param>
        /// <param name="path">Method will save the file at this path. Can be omitted. If omitted, method will save file to TwitterBase.defaultPath.</param>
        /// <param name="dataName">Name of the data. Appear in the dat description line of the file. Default value is "data".</param>
        /// <param name="comment">Optional comment line. Method will write this string at the first line of the file after a '#' mark. If omitted, first line is left uot entirely.</param>
        public void writeToFileNoLog(string fileName, string path = TwitterBase.defaultPath, string dataName = "data", string comment = "")
        {
            TextWriter tw1 = new StreamWriter(path + fileName + ".dat");
            if (comment != "")
                tw1.WriteLine("#" + comment);
            tw1.WriteLine("#index " + dataName + "\t\t\t\tCDF\t\t\t\t\tPDF\t\t\t\tlog(" + dataName + ")\t\t\t\tlog(CDF)\t\t\tlog(PDF)");
            for (int j = 0; j < binNumber - 1; j++)
                tw1.WriteLine("{0}\t{1}\t{2}\t{3}", j, PDF[j].x, CDF[j].y, PDF[j].y);
            tw1.Close();
        }

        /// <summary>
        /// Gives the value of the PDF between two point of the discrete domain.
        /// </summary>
        /// <remarks>Distribution is a set of discrete DFpoint structures. So PDF can only be evaluated at the exact x points of the domain. This
        /// function allows the developer to evaluate the PDF anywhere.</remarks>
        /// <param name="x">The </param>
        /// <returns> Between the datapoint it evaluates the function by linear interpolation between
        /// the neightboring datapoints. Out of the range it returns the closer limiter.</returns>
        public double interpolate(double x)
        {
            double min1 = Convert.ToDouble(PDF[0].x);
            double max1 = Convert.ToDouble(PDF[PDF.Count - 1].x);
            if (x < min1)
                return PDF[0].y;
            else
            {
                if (x > max1)
                    return PDF[PDF.Count - 1].y;
                else
                {
                    for (int i = 0; i < PDF.Count - 1; i++)
                    {
                        if ((Convert.ToDouble(PDF[i].x) < x) && (Convert.ToDouble(PDF[i + 1].x) > x))
                        {
                            double ret = PDF[i].y + (PDF[i + 1].y - PDF[i].y) * (x - Convert.ToDouble(PDF[i].x)) / (Convert.ToDouble(PDF[i + 1].x) - Convert.ToDouble(PDF[i].x));
                            return ret;
                        }
                    }
                }
            }
            return -1;
        }
    }
}
