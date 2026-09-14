namespace CatFactApp.WebApp.Models;

public class CatFactDto
{
    public DateTime CreatedAt { get; set; }
    public string Fact { get; set; } = string.Empty;
    public int Length { get; set; }
}