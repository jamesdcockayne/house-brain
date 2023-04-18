namespace Service;

internal class Waiter : IWait
{
    public async Task LongWaitAsync() => await Task.Delay(5 * 60 * 1000);

    public async Task ShortWaitAsync() => await Task.Delay(5 * 1000);
}