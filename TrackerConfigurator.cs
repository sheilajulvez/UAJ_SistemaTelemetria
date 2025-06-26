using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
public enum EventType
{
    SessionStart,
    SessionEnd,
    LevelStart,
    LevelEnd,
    Jump,
    Death,
    SlimeDeath,
    FallDeath,
    SpikeDeath,
    Pause,
    FallPlatform
}

public enum TrackerEventType
{
    ProgressionTracker,
    ResourceTracker
} 


public class TrackerConfigurator : MonoBehaviour
{
  
    private void Awake()
    {
       

            var serializer = new JsonSerializer();
            var persistence = new FilePersistence("guardado.json", serializer);

            string path = Path.Combine(Application.dataPath, "Telemetria\\conf.json");

            if (!File.Exists(path))
            {
                Debug.LogError("No se encontró el archivo de configuración en: " + path);
                return;
            }
            string json = File.ReadAllText(path);

            JObject config = JObject.Parse(json);
            JObject trackers = config["trackers"] as JObject;

            var trackerMap = new Dictionary<string, bool>();

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
            Tracker.Instance.Initialize(serializer, persistence, trackerMap);
            Tracker.Instance.Start();

    }

    private void OnDestroy()
    {
        Tracker.Instance.Stop();
    }

    private void OnApplicationQuit()
    {
        Tracker.Instance.Stop();
    }
    // Update is called once per frame
   
}
