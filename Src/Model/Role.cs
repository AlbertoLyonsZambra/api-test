namespace iCarus.Src.Model;
// Clase para representar el rol
public class Role()
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<User> Users { get; set; } = [];
}