namespace Service;

internal class Waiter : IWait
{
    public async Task LongWaitAsync(CancellationToken token) => await Task.Delay(5 * 60 * 1000, token);

    public async Task ShortWaitAsync(CancellationToken token) => await Task.Delay(5 * 1000, token);
}