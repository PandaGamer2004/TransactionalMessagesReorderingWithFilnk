namespace RocketGateway.Extensions;

public static class LinkedListExtensions
{
    public static LinkedList<TResult> Prepend<TResult>(this LinkedList<TResult> results, TResult result)
    {
        results.AddFirst(result);
        return results;
    }
}