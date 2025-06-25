using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

public class FilePersistence : IPersistence
{
    private BlockingCollection<TrackerEvent> eventQueue = new BlockingCollection<TrackerEvent>();
    private Thread processingThread;
    private string filename;
    private bool isRunning = true;
    private string path;
    private ISerializer serializer;

    public FilePersistence(string filename, ISerializer serializer)
    {
        this.filename = filename;
        this.serializer = serializer;

        path = Path.Combine(Application.dataPath, "Telemetria", filename);

        // Crea la carpeta si no existe
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        Debug.Log("Ruta de telemetría: " + path);
    }
    // Descomentar si se quiere que se guarde en el ordenador y asi que se pueda guardar en la build
    /*path = Path.Combine(Application.persistentDataPath, "Telemetria", filename);
    Directory.CreateDirectory(Path.GetDirectoryName(path));
    Debug.Log("Ruta de telemetría: " + Application.persistentDataPath);*/


    public void Enqueue(TrackerEvent evt)
    {
        eventQueue.Add(evt);
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
            if (eventQueue.TryTake(out TrackerEvent Event, 100))
            {
              
                // Descomentar si se quiere que se guarde en el ordenador y asi que se pueda guardar en la build
                //File.AppendAllText(path, serializedEvent + "\n");
                //  File.AppendAllText("Assets\\Telemetria\\" + filename, serializedEvent + "\n");
                string serializedEvent = serializer.Serialize(Event.parameters);
                File.AppendAllText(path, serializedEvent + "\n");
            }
        }
    }
}