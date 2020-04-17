namespace QuizHut.Web.Infrastructure.Helpers
{
    using System;
    using System.Collections.Generic;

    public class Shuffler : IShuffler
    {
        public IList<T> Shuffle<T>(IList<T> list)
        {
            var rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
