using Newtonsoft.Json;
using System.Collections.Generic;

public class JsonSerializer : ISerializer
{
    public string Serialize(Dictionary<string, object> data)
    {
        return JsonConvert.SerializeObject(data);
    }
}