using System.Collections.Concurrent;
using System.Threading;

public abstract class CustomThread
{
    public abstract void InsertData<T>(T value);
}
