using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarFinderXamarin
{
    static class Car
    {
        public static string Year { get; set; }
        public static string Make { get; set; }
        public static string Model { get; set; }
        public static string Weight { get; set; }
        public static string EnginePower { get; set; }
        public static string EngineSize { get; set; }
        public static List<string> Trim { get; set; }
    }
}
