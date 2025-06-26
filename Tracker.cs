using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Timeline;
using Newtonsoft.Json.Linq;
using System.IO;



public class Tracker 
{
    public static Tracker Instance { get; private set; } = new Tracker();

    public ISerializer serializer ;
    public IPersistence persistence ;
    private float StartTime;
    private float levelstart;
   
    Dictionary<string, bool> trackerMap = new Dictionary<string, bool>(); // Este mapa haría la función de un ITrackerAsset para saber como se agrupan los eventos y aceptarlos o no segun el archivo de configuracion
    private string sessionId;

    public string GetSessionId() => sessionId;

    public void Initialize(ISerializer serializer, IPersistence persistence, Dictionary<string, bool> trackerMap)
    {
       
        this.serializer = serializer;
        this.persistence = persistence;
      
        this.trackerMap = trackerMap;
        this.sessionId = System.Guid.NewGuid().ToString();
    }

    public void Stop()
    {
        persistence.StopProcessing();
    }
    public void Start()
    {
        Debug.Log("START");
        persistence.StartProcessing();
    }

    public void TrackEvent(TrackerEvent eventToTrack)
    {
        if (trackerMap[eventToTrack.trackerName])
        {
          
            persistence.Enqueue(eventToTrack);
        }
    }

    // Eventos comunes a cualquier juego
    public void TrackSessionStartEvent(string sessionId)
    {
        StartTime = Time.time; // Guarda el tiempo de inicio en segundos
        var data = new Dictionary<string, object>
        {
            { "session_id", sessionId },
           
        };

        TrackEvent(new TrackerEvent(EventType.SessionStart.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }

    public void TrackSessionEndEvent(string sessionId)
    {
        float sessionDuration = Time.time - StartTime;
        var data = new Dictionary<string, object>
        {
            { "session_id", sessionId },
            { "session_duration", sessionDuration.ToString("F2")}
        };

        TrackEvent(new TrackerEvent(EventType.SessionEnd.ToString(), TrackerEventType.ProgressionTracker.ToString(), data));
    }



}