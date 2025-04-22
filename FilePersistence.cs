using System.IO;
using System.Collections.Concurrent;
using System.Threading;

public class FilePersistence : IPersistence
{
    private BlockingCollection<string> eventQueue = new BlockingCollection<string>();
    private Thread processingThread;
    private string filename;
    private bool isRunning = true;

    public FilePersistence(string filename)
    {
        this.filename = filename;
    }

    public void Enqueue(string serializedEvent)
    {
        eventQueue.Add(serializedEvent);
    }

    public void StartProcessing()
    {
        processingThread = new Thread(ProcessEvents);
        processingThread.Start();
    }

    public void StopProcessing()
    {
        isRunning = false;
        if (processingThread != null && processingThread.IsAlive)
        {
            processingThread.Join();
        }
    }

    private void ProcessEvents()
    {
        while (isRunning || eventQueue.Count > 0)
        {
            if (eventQueue.TryTake(out string serializedEvent, 100))
            {
                File.AppendAllText("Assets\\Telemetria\\" + filename, serializedEvent + "\n");
            }
        }
    }
}