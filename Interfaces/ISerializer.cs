using System.Collections.Generic;
using Newtonsoft.Json; // Necesitas instalar el paquete Newtonsoft.Json

public interface ISerializer
{
    string Serialize(Dictionary<string, object> data);
}