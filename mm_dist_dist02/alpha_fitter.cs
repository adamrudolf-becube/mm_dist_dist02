using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace Twitter_tools
{
    /// <summary>
    /// This class reads two series from files and determines the constant factor which minimizes the sum of squared differences of the two dataset by multipliing the first one.
    /// </summary>
    /// <remarks>This class contains methods to find a constant factor which minimizes the sum of squared differences between two datasets. It uses the Newton-Raphson method, and contains methods for first and second order derivatives.</remarks>
    class alpha_fitter
    {
        /// <summary>
        /// The 'main' function of the class. We call this method when we use the class. It's contents can change.
        /// </summary>
        public void run()
        {
            string filename2 = "hashtag-graf_avg-sigma_VS_CNT-gt_linlinlin";
            string filename1 = "nyeff3";
            read(filename1, filename2);
        }

        /// <summary>
        /// Reads two data files and finds the constant factor to multiply the first with. 
        /// </summary>
        /// <remarks>It reads the files from the path \\QSO\Public\rudolf\My Documents\ and appends .dat extension to them. </remarks>
        /// <param name="filename1">The first filename without path and extension. The program multiplies this by the fit factor.</param>
        /// <param name="filename2">The second filename without path and extension.</param>
		public void read(string filename1, string filename2)
		{
			// Read data (from file) ------------------------------------------------------------
            StreamReader testFile1 = new StreamReader(@"" + TwitterBase.defaultPath + filename1 + ".dat");
            StreamReader testFile2 = new StreamReader(@"" + TwitterBase.defaultPath + filename2 + ".dat");
            string line;
            int counter = 0;
            string[] arrWords = new string[2]; // The container of split line
            List<double> data1 = new List<double>();
            List<double> data2 = new List<double>();
            Regex anySpaces = new Regex("[ \t]+");
			
            while ((line = testFile1.ReadLine()) != null)
            {
                arrWords = anySpaces.Split(line);
                if (arrWords[0] != "#")
				    data1.Add(Double.Parse(arrWords[1].ToString().Trim()));		
            }
			
			while ((line = testFile2.ReadLine()) != null)
            {
                arrWords = anySpaces.Split(line);
                if (arrWords[0] != "#")
				    data2.Add(/*TwitterBase.R * */Double.Parse(arrWords[1].ToString().Trim()));			
            }
			
			testFile1.Close();
			testFile2.Close();

            double alpha = 1.0;
            alpha = Newton(S, data1, data2);
		}

        /// <summary>
        /// Sum of squared differences of alpha * list1 and list2.
        /// </summary>
        /// <remarks>This method finds the ninimum of the sizes of list1 and list2 and sums the squared differences over the common section.</remarks>
        /// <param name="list1">List object of double variables. Program will multiply this List by the parameter alpha.</param>
        /// <param name="list2">List object of double variables.</param>
        /// <param name="alpha">A constant double factor the method multiplies the first List with.</param>
        /// <returns>a double value whish is the sum of the squared differences of alpha * list1 and list2.</returns>
        public double S(List<double> list1, List<double> list2, double alpha)
		{
			//int minindex = (list1.Count < list2.Count) ? list1.Count : list2.Count;
            int minindex = 200;
			double sum = 0.0;
			for (int i = 0; i < minindex; i++)
			{
				sum += ((alpha * list1[i] - list2[i]) * (alpha * list1[i] - list2[i]));
			}
			return sum;
		}
		
		/// <summary>
		/// First order numerical derivative of the S_delegate type function f.
		/// </summary>
		/// <param name="list1">First double List type parameter of the function f.</param>
        /// <param name="list2">Second double List type parameter of the function f.</param>
		/// <param name="alpha">The double type parameter of the function f.</param>
		/// <param name="f">An S_delegate type function. The method will calculate the first derivative of this.</param>
		/// <returns>the first order numerical derivative of the S_delegate type function f at the given parameters.</returns>
		public double Diff(List<double> list1, List<double> list2, double alpha, S_delegate f)
		{
			double h = 0.1;
			return (f(list1, list2, alpha + h) - f(list1, list2, alpha)) / h;
		}

        /// <summary>
        /// Second order numerical derivative of the S_delegate type function f.
        /// </summary>
        /// <param name="list1">First double List type parameter of the function f.</param>
        /// <param name="list2">Second double List type parameter of the function f.</param>
        /// <param name="alpha">The double type parameter of the function f.</param>
        /// <param name="f">An S_delegate type function. The method will calculate the second derivative of this.</param>
        /// <returns>the second order numerical derivative of the S_delegate type function f at the given parameters.</returns>
        public double Diff_2(List<double> list1, List<double> list2, double alpha, S_delegate f)
        {
            double h = 0.1;
            return (f(list1, list2, alpha + 2.0 * h) - 2.0 * f(list1, list2, alpha + h) + f(list1, list2, alpha)) / (h * h);
        }

		/// <summary>
		/// A delegate function which fits for S, the sum of squared differences of alpha * list1 and list2.
		/// </summary>
        /// <param name="list1">First double List type parameter of the function f.</param>
        /// <param name="list2">Second double List type parameter of the function f.</param>
        /// <param name="alpha">The double type parameter of the function f.</param>
        /// <returns></returns>
        public delegate double S_delegate(List<double> list1, List<double> list2, double alpha);
		
        /// <summary>
        /// Minimizes the S_delegate type function S by changing it's parameter alpha.
        /// </summary>
        /// <remarks>Finds the minimum of S by using the Newton-Raphson method to find the root of the first derivative of S.</remarks>
        /// <param name="S">An S_delegate type function.</param>
        /// <param name="list1">First double List type parameter of the function S.</param>
        /// <param name="list2">Second double List type parameter of the function S.</param>
        /// <returns>a double value which minimizes S_delegate type S as the alpha parameter of it with the given parameters list1 and list2.</returns>
		public double Newton(S_delegate S, List<double> list1, List<double> list2)
		{
			double alpha = 2.0;
            double diff = 1.0;
            Console.WriteLine("Initial alpha value = {0},\t diff = {1}", alpha, diff);
			double epsilon = 0.00000001;
            int iterations = 1;
			while (Math.Abs(diff) >= epsilon)
			{
                double d = Diff_2(list1, list2, alpha, S);
				alpha = alpha - (Diff(list1, list2, alpha, S)) / (Diff_2(list1, list2, alpha, S));
                diff = Diff(list1, list2, alpha, S);
                Console.WriteLine("After {0} iterations, alpha value = {1},\t diff = {2}", iterations, alpha, diff);
                iterations++;
			}
            Console.WriteLine("After {0} iterations the fit converged. alpha = {1}. diff = {2}, S = {3}.", iterations, alpha, diff, S(list1, list2, alpha));
			return alpha;
		}
			
    }
}