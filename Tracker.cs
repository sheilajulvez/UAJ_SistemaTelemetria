using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Timeline;
using Newtonsoft.Json.Linq;
using System.IO;

public enum EventType
{
    SessionStart,
    SessionEnd,
    LevelStart,
    LevelEnd,
    LevelProgress,
    Jump,
    Death,
    BlueSlime,
    Pause,
    FallPlatform
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

    public void TrackPlayerDeath(string levelId, string deathType, float position)
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "session_id", sessionId },
            { "level_id", levelId },
            { "death_type", deathType },
            { "position in X", position.ToString() },
            { "event_timestamp", System.DateTime.Now.ToString("o") }
        };

        TrackEvent(new TrackerEvent(EventType.Death.ToString(), TrackerEventType.ResourceTracker.ToString(), data));
    }
    public void TrackLevelStart(string levelId)
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "session_id", sessionId },
            { "level_id", levelId },
            { "start_time", System.DateTime.Now.ToString("o") }
        };

        TrackEvent(new TrackerEvent(EventType.LevelStart.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }
    public void TrackLevelEnd(string levelId, string time)
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "session_id", sessionId },
            { "level_id", levelId },
            { "end_time", System.DateTime.Now.ToString("o") },
            { "time", time }
        };

        TrackEvent(new TrackerEvent(EventType.LevelEnd.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }
    // Eventos del Juego (AmonRA)





    public void TrackLevelProgress(string levelId, string deathnumber1, string deathnumber2, string deathnumber3)
    {
        var data = new Dictionary<string, object>
        {
            { "level_id", levelId },
            { "number_of_falls_deaths", deathnumber1 },
            { "number_of_spike_deaths", deathnumber2 },
            { "number_of_spike_appears_deaths", deathnumber3 },
        };

        Tracker.Instance.TrackEvent(new TrackerEvent(EventType.LevelProgress.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackJump()
    {
        var data = new Dictionary<string, object>
    {
        { "session_id", sessionId },
        { "jump_time", System.DateTime.Now.ToString("o") }
    };

        TrackEvent(new TrackerEvent(EventType.Jump.ToString(), TrackerEventType.ResourceTracker.ToString(), data));
    }

    public void TrackBlueSlime( float position)
    {
        var data = new Dictionary<string, object>
    {
        { "level_id", 1 },
        { "position_x", position.ToString() }
    };

        TrackEvent(new TrackerEvent(EventType.BlueSlime.ToString(), TrackerEventType.ResourceTracker.ToString(), data));
    }
    public void TrackFallPlatform(string levelId, float position)
    {
        var data = new Dictionary<string, object>
    {
        { "level_id", levelId },
        { "position_x", position.ToString() }
    };

        TrackEvent(new TrackerEvent(EventType.FallPlatform.ToString(), TrackerEventType.ResourceTracker.ToString(), data));
    }

    public void TrackPause(string levelId, int npause)
    {
        var data = new Dictionary<string, object>
    {
        { "level_id", levelId },
         {"number_of_times_pauses_game",npause.ToString() }
       
           
    };

        TrackEvent(new TrackerEvent(EventType.Pause.ToString(), TrackerEventType.ResourceTracker.ToString(), data));
    }













}