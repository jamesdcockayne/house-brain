namespace Service;

public interface IWait
{
    Task LongWaitAsync();
    Task ShortWaitAsync();
}
