using System;
using System.Collections.Generic;

namespace QBot
{
    public static class QRandom
    {
        private static Random Rand { get; } = new Random();

        public static int Number(int min = 1, int max = 100) => Rand.Next(min, max + 1);

        private static int Next(int min, int max) => Rand.Next(min, max);

        /// <summary>
        /// Gets a random index from the array
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ArrayRandom<T>(T[] array) => array[Next(0, array.Length)];

        /// <summary>
        /// Gets a random index from the array
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ArrayRandom<T>(IList<T> array) => array[Next(0, array.Count)];

        public static List<int> EmulateRolls(int numSides, int rolls)
        {
            List<int> numbers = new List<int>();
            ArrayRandom(numbers);
            for (int i = 0; i < rolls; i++)
            {
                numbers.Add(Number(1, numSides));
            }
            return numbers;
        }

        public static int Roll(int numOfSides, int rolls)
        {
            if (numOfSides < 0 || rolls < 0)
            {
                throw new Exception("Not a valid roll!");
            }
            int total = 0;
            for (int i = 0; i < rolls; i++)
            {
                total += Number(1, numOfSides);
            }
            return total;
        }
    }
}