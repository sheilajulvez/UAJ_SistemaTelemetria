using System.Collections.Concurrent;
using System.IO;
using System.Threading;

public interface IPersistence
{
    void Enqueue(TrackerEvent evt);
    void StartProcessing();
    void StopProcessing();
}