namespace iCarus.Src.Dtos.Users;
// Clase que representa el data transfer object
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// Clase que representa la estructura de la respuesta que se devuelve al cliente despues de un intento de inicio de sesión.
public class DataResponseLogin
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

// Clase que representa la estructura de la respuesta general para el proceso de inicio de sesión
public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DataResponseLogin? Data { get; set; }
}

public class ChangePasswordDto
{
    public string ConfirmPassword { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}