using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriBot
{
    public static class RandomHelper
    {
        public static Random random = new Random();
        public static string GetRandomOfArray(string[] array)
        {
            return array[random.Next(0, array.Length - 1)];
        }
    }
}
