namespace iCarus.Src.Model;

public class Funcion
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string DateFunction { get; set; } = null!;
    public string TimeFunction { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
