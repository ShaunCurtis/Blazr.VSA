/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class ListFunctionalExtensions
{
    extension<T>(List<T> @this)
    {
        public Return<List<T>> ToReturn()
            => ReturnT.Success(@this);
    }

    extension<T>(IEnumerable<T> @this)
    {
        public Return<IEnumerable<T>> ToReturn()
            => ReturnT.Success(@this);
    }
}
