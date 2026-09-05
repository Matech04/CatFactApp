HttpClient httpClient = new();

var response = await httpClient.GetAsync("https://catfact.ninja/fact");

string jsonContent = await response.Content.ReadAsStringAsync();

Console.WriteLine(jsonContent);