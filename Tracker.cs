using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Timeline;
using Newtonsoft.Json.Linq;
using System.IO;

public enum EventType
{
    SessionStart,
    SessionEnd
}

public enum TrackerEventType
{
    ProgressionTracker,
    ResourceTracker
}

public class Tracker : MonoBehaviour
{
    public static Tracker Instance { get; private set; }

    public ISerializer serializer;
    public IPersistence persistence;
    Dictionary<string, bool> trackerMap = new Dictionary<string, bool>(); // Este mapa haría la función de un ITrackerAsset para saber como se agrupan los eventos y aceptarlos o no segun el archivo de configuracion
    private string sessionId;
    public string GetSessionId()
    {
        return sessionId;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            serializer = new JsonSerializer();
            persistence = new FilePersistence("guardado.json");

            string path = Path.Combine(Application.dataPath, "Telemetria\\conf.json");
            string json = File.ReadAllText(path);
            JObject config = JObject.Parse(json);
            sessionId = System.Guid.NewGuid().ToString();
            JObject trackers = config["trackers"] as JObject;

            if (trackers != null)
            {
                foreach (var pair in trackers)
                {
                    string key = pair.Key;
                    bool value = pair.Value?.Value<bool>() ?? false;

                    trackerMap[key] = value;

                    Debug.Log(key);
                }
            }
            else
            {
                Debug.LogError("No se encontró el objeto 'trackers' en el JSON.");
            }

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        persistence.StartProcessing();
    }

    private void OnDestroy()
    {
        persistence.StopProcessing();
    }
    private void OnApplicationQuit()
    {
        Debug.Log("Se está cerrando la aplicación.");

        persistence.StopProcessing();
    }

    public void TrackEvent(TrackerEvent eventToTrack)
    {
        if (trackerMap[eventToTrack.trackerName])
        {
            string serializedEvent = serializer.Serialize(eventToTrack.parameters);
            persistence.Enqueue(serializedEvent);
        }
    }

    // Eventos comunes a cualquier juego
    public void TrackSessionStartEvent(string sessionId, string startTime)
    {
        var data = new Dictionary<string, object>
        {
            { "session_id", sessionId },
            { "start_time", startTime }
        };

        TrackEvent(new TrackerEvent(EventType.SessionStart.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackSessionEndEvent(string sessionId)
    {
        var data = new Dictionary<string, object>
        {
            { "session_id", sessionId },
            { "end_time", System.DateTime.Now.ToString("o") }
        };

        TrackEvent(new TrackerEvent(EventType.SessionEnd.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }
}