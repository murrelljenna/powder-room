using System.Net.Http.Json;

public class NetworkObject
{
    
}

await client.GetFromJsonAsync<NetworkObject>("users/1");