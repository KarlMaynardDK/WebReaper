using Polly;
using Polly.Retry;

namespace WebReaper.Infra;

public class Executor
{
    protected static AsyncRetryPolicy asyncPolicy;
    protected static RetryPolicy policy;

    public Executor()
    {
        asyncPolicy = Policy.Handle<Exception>().RetryAsync(3);
        policy = Policy.Handle<Exception>().Retry(3);
    }

    public static async Task<T> Retry<T>(Func<Task<T>> func) =>
        await asyncPolicy.ExecuteAsync(async () => await func());

    public static void Retry<T>(Action action) => policy.Execute(() => action());
}