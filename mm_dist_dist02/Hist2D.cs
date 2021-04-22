using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twitter_tools
{
    using System.IO;

    /// <summary>
    /// This class represents a 2 dimensional histogram. Similar to the class Distribution, but 2 dimensional, bin size is constant and is not normalized.
    /// </summary>
    /// <typeparam name="T">Class puts this type of x data to bins. Must be able to be converted to double.</typeparam>
    /// <typeparam name="U">Class puts this type of y data to bins. Must be able to be converted to double.</typeparam>
    class Hist2D<T,U>
    {
        // Attributes -----------------------------------------------------------------------
        /// <summary>
        /// Number of x bins in the histogram.
        /// </summary>
        public int x_binNumber;
        /// <summary>
        /// Size (= width) of one x bin. "Right side" of the histogram is x_binNumber * x_binSize.
        /// </summary>
        public double x_binSize;
        /// <summary>
        /// Number of y bins in the histogram.
        /// </summary>
        public int y_binNumber;
        /// <summary>
        /// Size (= width) of one y bin. "Bottom" of the histogram is y_binNumber * y_binSize.
        /// </summary>
        public double y_binSize;
        /// <summary>
        /// Class stores the values in this array. This is the histogram itself.
        /// </summary>
        public int[,] hist;

        // Methods --------------------------------------------------------------------------
        /// <summary>
        /// Returns the index of the bin in the histogram which contains the 'data' if the bin size is given.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when method can not convert T to double.</exception>
        /// <param name="data">The numerical data which we want to put in a bin. Type must be able to be converted to double.</param>
        /// <returns>an integer which is the index of the bin data belongs to.</returns>
        public int whichBin<V>(V data, double binSize)
        {
            double dataDouble;
            try
            {
                dataDouble = Convert.ToDouble(data);
            }
            catch
            {
                throw new InvalidOperationException
                ("T must be a type convertable to double.");
            }
            int ret = (int)Math.Floor((1.0 * dataDouble) / (1.0 * binSize));
            return ret;
        }

        /// <summary>
        /// Adds 1 to the bin data belongs to.
        /// </summary>
        /// <param name="data">The numerical data which we want to put in a bin. Expects a tuple with the x and y values. Both's type must be able to be converted to double.</param>
        public void addData(Tuple<T,U> data)
        {
            int x_index = whichBin<T>(data.Item1, x_binSize);
            int y_index = whichBin<U>(data.Item2, y_binSize);
            if ((x_index < x_binNumber) && (y_index < y_binNumber))
                hist[x_index, y_index]++;
        }

        /// <summary>
        /// Adds 1 to the bin data belongs to.
        /// </summary>
        /// <param name="x_data">The numerical x data which we want to put in a bin. Type T must be able to be converted to double.</param>
        /// <param name="x_data">The numerical y data which we want to put in a bin. Type U must be able to be converted to double.</param>
        public void addData(T x_data, U y_data)
        {
            int x_index = whichBin<T>(x_data, x_binSize);
            int y_index = whichBin<U>(y_data, y_binSize);
            if ((x_index < x_binNumber) && (y_index < y_binNumber))
                hist[x_index, y_index]++;
        }

        /// <summary>
        /// Fills all of the bins with the given value.
        /// </summary>
        /// <param name="value">Sets all of the bin values to this value.</param>
        public void fillWith(int value)
        {
            for (int i = 0; i < x_binNumber; i++)
                for (int j = 0; j < y_binNumber; j++)
                    hist[i,j] = value;
        }

        // Constructors ---------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of Hist1D with tha given bin size and bin number, filled with the elements of dataSet.
        /// </summary>
        /// <param name="dataSet">List of T, U type Tuples. T and U types must be able to be converted to double.</param>
        /// <param name="x_binSize_">Size (= width) of one x bin.</param>
        /// <param name="x_binNumber_">Number of the x_binSize_ sized bins.</param>
        /// <param name="y_binSize_">Size (= width) of one y bin.</param>
        /// <param name="y_binNumber_">Number of the y_binSize_ sized bins.</param>
        public Hist2D(List<Tuple<T,U>> dataSet, double x_binSize_, int x_binNumber_, double y_binSize_, int y_binNumber_)
        {
            x_binSize = x_binSize_;
            x_binNumber = x_binNumber_;
            y_binSize = y_binSize_;
            y_binNumber = y_binNumber_;
            hist = new int[x_binNumber, y_binNumber];
            fillWith(0);
            foreach (Tuple<T,U> data in dataSet)
                addData(data);
        }

        /// <summary>
        /// Initializes a new instance of Hist1D with tha given bin size and bin number, filled with the elements of zeros.
        /// </summary>
        /// <param name="x_binSize_">Size (= width) of one x bin.</param>
        /// <param name="x_binNumber_">Number of the x_binSize_ sized bins.</param>
        /// <param name="y_binSize_">Size (= width) of one y bin.</param>
        /// <param name="y_binNumber_">Number of the y_binSize_ sized bins.</param>
        public Hist2D(double x_binSize_, int x_binNumber_, double y_binSize_, int y_binNumber_)
        {
            x_binSize = x_binSize_;
            x_binNumber = x_binNumber_;
            y_binSize = y_binSize_;
            y_binNumber = y_binNumber_;
            hist = new int[x_binNumber, y_binNumber];
            fillWith(0);
        }

        /// <summary>
        /// Initializes a new instance of Hist1D with tha given bin size and bin number, filled with the elements stored in a list of Hist1D objects.
        /// </summary>
        /// <remarks>Each column (y) will be one of the Hist1D objects of the hists parameter. If the Hist1D objects have different binSize attibutes, the y_binSize
        /// attribute of this object will be the maximum of them. The missing elements will be initialized as zeros.
        /// The y_binSize of this object will be the binSize of the binSize first Hist1D object, no matter what are the others.
        /// The x_binNumber of this object will be the number of the Hist1D object hists parameter contains.</remarks>
        /// <param name="hists">List of Hist1D objects. Each object of this List will be one of the columns of the Hist2D object.</param>
        /// <param name="x_binSize_">The size (= width) of the x bins in the Hist2D object.</param>
        public Hist2D(List<Hist1D<T>> hists, double x_binSize_)
        {
            x_binSize = x_binSize_;
            x_binNumber = hists.Count();
            y_binSize = hists[0].binSize;
            int maxIndex = 0;
            foreach (Hist1D<T> h in hists)
                if (h.binNumber > maxIndex)
                    maxIndex = h.binNumber;
            y_binNumber = maxIndex;

            hist = new int[x_binNumber, y_binNumber];
            fillWith(0);

            for(int x = 0; x < x_binNumber; x++)
            {
                for (int y = 0; y < hists[x].binSize; y++)
                    hist[x, y] = hists[x].hist[y];
            }
        }

        // One more method... ---------------------------------------------------------------
        /// <summary>Writes the data stored in the histogram to a .dat file with the given name at the given path in matrix format.</summary>
        /// <remarks>This method uses the System.TextWriter class to write to a file. The first two lines are comments,
        /// beginning with the '#' mark for further working with the file in GNUPLOT. User can name the data so in the comments
        /// that name will be shown.
        /// The method writes the integer values of the bin in matrix format.</remarks>
        /// <param name="fileName">Method will name the .dat file after this at the given path. Must not contain extension.</param>
        /// <param name="path">Method will save the file at this path. Can be omitted. If omitted, method will save file to TwitterBase.defaultPath.</param>
        /// <param name="x_dataName">Name of the x data. Appear in the dat description line of the file. Default value is "x".</param>
        /// <param name="y_dataName">Name of the y data. Appear in the dat description line of the file. Default value is "y".</param>
        /// <param name="comment">Optional comment line. Method will write this string at the first line of the file after a '#' mark. If omitted, first line is left uot entirely.</param>
        /// <param name="transposed">If true, rows will represent the fixed x values, and columns are fixed y values. If false, rows are ys, columns are xs. By default, transposed is false.</param>
        /// <param name="substitute">If not 0, the method will substitute zero values with the given double number. Useful when plotting in logscale. Equals to 0 if omitted.</param>
        public void writeToFile(string fileName, string path = TwitterBase.defaultPath, string x_dataName = "x", string y_dataName = "y", string comment = "", bool transposed = false, double substitute = 0.0)
        {
            TextWriter tw1 = new StreamWriter(path + fileName + ".dat");
            if (comment != "")
                tw1.WriteLine("#" + comment);
            if (!transposed)
            {
                tw1.WriteLine("# " + y_dataName + "/" + x_dataName);
                for (int y = 0; y < y_binNumber; y++)
                {
                    for (int x = 0; x < x_binNumber; x++)
                        if ((substitute != 0) && (hist[x, y] == 0))
                            tw1.Write("{0} ", substitute);
                        else
                            tw1.Write("{0} ", hist[x, y]);
                    tw1.Write("\n");
                }
            }
            else
            {
                tw1.WriteLine("# " + y_dataName + "/" + x_dataName);
                for (int x = 0; x < x_binNumber; x++)
                {
                    for (int y = 0; y < y_binNumber; y++)
                        if ((substitute != 0) && (hist[x, y] == 0))
                            tw1.Write("{0} ", substitute);
                        else
                            tw1.Write("{0} ", hist[x, y]);
                    tw1.Write("\n");
                }
            }
            tw1.Close();
        }
    }
}
