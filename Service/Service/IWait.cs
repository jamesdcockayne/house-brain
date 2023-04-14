namespace Service;

public interface IWait
{
    Task LongWait();
    Task ShortWait();
}
