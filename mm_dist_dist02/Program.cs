using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twitter_tools
{
    /// <summary>
    /// Contains the Main function. Only initializes an instance of an other object and call the "run()" method of it.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The Main function of the project. Only calls the run() method of another object.
        /// </summary>
        /// <param name="args">Optional parameters.</param>
        static void Main(string[] args)
        {
            hashtag_sigmaSql d1 = new hashtag_sigmaSql();
            d1.run();
        }
    }
}
