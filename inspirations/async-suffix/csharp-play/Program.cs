using System.Text.Json.Nodes;

var client = new HttpClient();
client.DefaultRequestHeaders.UserAgent.ParseAdd("request");

var response = await client.GetFromJsonAsync<JsonObject>("https://api.github.com/users/astorDev");

Console.WriteLine(response!.ToJsonString(new () { WriteIndented = true }));