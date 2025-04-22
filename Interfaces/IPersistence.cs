using System.Collections.Concurrent;
using System.IO;
using System.Threading;

public interface IPersistence
{
    void Enqueue(string serializedEvent);
    void StartProcessing();
    void StopProcessing();
}