using System;

namespace StriBot.Application.Extensions
{
    public static class RandomHelper
    {
        public static readonly Random Random = new Random();

        public static string GetRandomOfArray(string[] array)
            => array[Random.Next(0, array.Length - 1)];
    }
}