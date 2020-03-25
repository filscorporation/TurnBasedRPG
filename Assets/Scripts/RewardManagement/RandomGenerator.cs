using System;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.RewardManagement
{
    /// <summary>
    /// Generates random numbers from a seed
    /// </summary>
    [Serializable]
    public class RandomGenerator
    {
        private static RandomGenerator instance;
        public static RandomGenerator Instance => instance ?? (instance = new RandomGenerator());

        public int Seed;

        public RandomGenerator()
        {
            Random rand = new Random();
            Seed = rand.Next(int.MinValue, int.MaxValue);
            UnityEngine.Random.InitState(Seed);
        }

        public RandomGenerator(int seed)
        {
            this.Seed = seed;
            UnityEngine.Random.InitState(seed);
        }

        /// <summary>
        /// Return random number from 0 to max exclusevly
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public int RandomInt(int max)
        {
            return UnityEngine.Random.Range(0, max);
        }

        /// <summary>
        /// Return random integer number that follows Gaussian distribution
        /// </summary>
        /// <param name="m">Mean</param>
        /// <param name="d">Dispersion</param>
        /// <returns></returns>
        public int RandomNormal(int m, float d)
        {
            float u1 = 1F - UnityEngine.Random.Range(0F, 1F);
            float u2 = 1F - UnityEngine.Random.Range(0F, 1F);
            float randStdNormal = Mathf.Sqrt(-2F * Mathf.Log(u1)) * Mathf.Sin(2F * Mathf.PI * u2);
            float randNormal = m + d * randStdNormal;

            return Mathf.RoundToInt(randNormal);
        }
    }
}
