namespace QuizHut.Web.Infrastructure.Helpers
{
    using System.Collections.Generic;

    public interface IShuffler
    {
        IList<T> Shuffle<T>(IList<T> list);
    }
}
