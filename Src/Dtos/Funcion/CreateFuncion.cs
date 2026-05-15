namespace iCarus.Src.Dtos.Funcion;

public class CreateFuncion
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string DateFunction { get; set; } = null!;
    public string TimeFunction { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public IFormFile? Image { get; set; }
}

public class CreateFuncionData
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string DateFunction { get; set; } = null!;
    public string TimeFunction { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public string ImageUrl { get; set; } = null!;
}

public class CreateFuncionResponse
{
    public string Message { get; set; } = null!;
    public bool Success { get; set; }
    public CreateFuncionData? Data { get; set; }
}
