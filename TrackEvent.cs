using UnityEngine.Analytics;
using System.Collections.Generic;
using UnityEngine;

public class TrackerEvent
{
    public string trackerName;
    public string eventName;
    public Dictionary<string, object> parameters;
    private const string EVENT_VERSION = "1.0";
    private const string APP_NAME = "MiJuego";
    private const string APP_VERSION = "0.1a";
    private const string CLIENT_ID = "jugador_desconocido"; // Puedes sobreescribirlo

    public TrackerEvent(string eventName, string trackerName, Dictionary<string, object> extraParameters)
    {
        this.eventName = eventName;
        this.trackerName = trackerName;
        this.parameters = new Dictionary<string, object>();

        parameters["event_version"] = EVENT_VERSION;
        parameters["event_id"] = System.Guid.NewGuid().ToString();
        parameters["event_timestamp"] = System.DateTime.UtcNow.ToString("o");
        parameters["event_type"] = eventName;
        parameters["app_name"] = APP_NAME;
        parameters["app_version"] = APP_VERSION;
        parameters["client_id"] = CLIENT_ID;

        foreach(var elem in extraParameters)
        {
            parameters[elem.Key] = elem.Value;
        }

    }

    public void SendEvent()
    {
        Analytics.CustomEvent(eventName, parameters);
    }
}