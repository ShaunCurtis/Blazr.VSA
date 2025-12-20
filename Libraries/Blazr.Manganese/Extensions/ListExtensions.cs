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
        public List<T> RemoveItem(T itemToRemove)
        {
            var newList = @this.ToList();
            if (newList.Remove(itemToRemove))
                return newList;
            return @this;
        }

        public List<T> AddItem(T itemToAdd)
        {
            @this.Add(itemToAdd);
            return @this;
        }
    }

    extension<T>(IEnumerable<T> @this)
    {
        public Return<IEnumerable<T>> ToReturn()
            => ReturnT.Success(@this);
    }
}
