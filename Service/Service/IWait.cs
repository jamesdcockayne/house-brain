namespace Service;

public interface IWait
{
    Task LongWaitAsync(CancellationToken token);
    Task ShortWaitAsync(CancellationToken token);
}
