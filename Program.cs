HttpClient httpClient = new();

var response = await httpClient.GetAsync("https://catfact.ninja/fact");

string jsonContent = await response.Content.ReadAsStringAsync();

await System.IO.File.AppendAllTextAsync("facts.txt", $"{jsonContent}\n");

Console.WriteLine(jsonContent);