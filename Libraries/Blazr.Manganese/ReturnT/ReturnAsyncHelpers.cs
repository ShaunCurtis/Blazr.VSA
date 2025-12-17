using Blazr.Manganese;

public static class ReturnAsyncHelpers
{
    public static Return<T> CheckForTaskException<T>(Task<T> @this)
        => @this.IsCompletedSuccessfully
            ? ReturnT.Read(@this.Result)
            : Return<T>.Failure(@this.Exception
                ?? new Exception("The Task failed to complete successfully"));

    public static Return<T> CheckForTaskException<T>(Task<Return<T>> @this)
    => @this.IsCompletedSuccessfully
        ? @this.Result
        : Return<T>.Failure(@this.Exception
            ?? new Exception("The Task failed to complete successfully"));


    public static Return CheckForTaskException(Task<Return> @this)
        => @this.IsCompletedSuccessfully
            ? @this.Result
            : Return.Failure(@this.Exception
                ?? new Exception("The Task failed to complete successfully"));

    public static Return CheckForTaskException(Task @this)
        => @this.IsCompletedSuccessfully
            ? Return.Success()
            : Return.Failure(@this.Exception
                ?? new Exception("The Task failed to complete successfully"));

}