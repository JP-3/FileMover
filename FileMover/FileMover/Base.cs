using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace FileMover
{
    static internal class Base
    {
        public static Dictionary<string, string> data = new Dictionary<string, string>();
        static public Dictionary<string, string> GetProperties()
        {
            if (data.Count == 0)
            {
                foreach (var row in File.ReadAllLines(@"C:\\git\key.txt"))
                    data.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
                return data;
            }
            else return data;
        }

        public static Dictionary<string, bool> processedFiles = new Dictionary<string, bool>();


        public static string pattern1900s = @"\.19\d\d\.";   //19??
        public static string pattern2000s = @"\.20\d\d\.";   //20??
        public static string patternTvShow1 = @"s\d\de\d\d"; //S10EE11
        public static string patternTvShow2 = @"s\d\de\d";   //S10E1
        public static string patternTvShow3 = @"s\de\d\d";   //S1E11
        public static string patternTvShow4 = @"s\de\d";   //S1E1

        public static string patternSeason1 = @"s\d\d";   //S1E11
        public static string patternSeason2 = @"s\d";   //S1E1
    }
}
