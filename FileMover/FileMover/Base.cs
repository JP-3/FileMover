using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
