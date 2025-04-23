using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

public class FilePersistence : IPersistence
{
    private BlockingCollection<string> eventQueue = new BlockingCollection<string>();
    private Thread processingThread;
    private string filename;
    private bool isRunning = true;
    private string path;

    public FilePersistence(string filename)
    {
        this.filename = filename;
        // Descomentar si se quiere que se guarde en el ordenador y asi que se pueda guardar en la build
        /*path = Path.Combine(Application.persistentDataPath, "Telemetria", filename);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        Debug.Log("Ruta de telemetría: " + Application.persistentDataPath);*/
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
                // Descomentar si se quiere que se guarde en el ordenador y asi que se pueda guardar en la build
                //File.AppendAllText(path, serializedEvent + "\n");
                File.AppendAllText("Assets\\Telemetria\\" + filename, serializedEvent + "\n");
            }
        }
    }
}