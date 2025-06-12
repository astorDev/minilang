using System.Text.Json.Nodes;

var client = new HttpClient();

var response = await client.GetFromJsonAsync<JsonObject>(
    requestUri: "https://raw.githubusercontent.com/astorDev/minilang/refs/heads/main/hello.json"
);

Console.WriteLine(response!.ToJsonString(new () { WriteIndented = true }));