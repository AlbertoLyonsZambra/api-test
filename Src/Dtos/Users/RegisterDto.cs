using System.ComponentModel.DataAnnotations;

namespace iCarus.Src.Dtos.Users;

/// <summary>
/// DTO que contiene los datos requeridos para el
/// registro de cliente en el sistema:
/// Nombre, Apellido, Correo electronico, RUT y Telefono de contacto.
/// Incluye validacion de formato en las entradas que lo requieran.
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// Nombre del usuario. Campo obligatorio.
    /// </summary>
    [Required(ErrorMessage = "Falta completar el campo nombre")]
    public string Name {get; set; } = String.Empty;

    /// <summary>
    /// Apellido del usuario. Campo obligatorio.
    /// </summary>
    [Required(ErrorMessage = "Falta completar el campo apellido")]
    public string LastName {get; set; } = String.Empty;

    /// <summary>
    /// Correo electrónico del usuario. Campo obligatorio y debe tener formato válido.
    /// Debe ser único en el sistema, de lo contrario, despliega un mensaje de error.
    /// </summary>
    [Required(ErrorMessage = "Falta completar el campo correo electronico")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
    public string Email {get; set; } = String.Empty;

    /// <summary>
    /// RUT del usuario sin puntos ni guion (ejemplo: 12456789K). Campo obligatorio.
    /// Debe cumplir el formato y ser válido según el algoritmo de dígito verificador de Chile.
    /// Debe ser único en el sistema, de lo contrario despliega un mensaje de error.
    /// </summary>
    [Required(ErrorMessage = "Falta completar el campo rut")]
    [RegularExpression(@"^[0-9]{7,8}[0-9kK]{1}$", 
    ErrorMessage = "El RUT debe ingresarse sin puntos ni guion (ej: 12456789K)")]
    public string Rut {get; set; } = String.Empty;

    /// <summary>
    /// Teléfono de contacto del usuario. Campo obligatorio.
    /// </summary>
    [Required(ErrorMessage = "Falta completar el campo Teléfono de contacto")]
    public string PhoneNumber { get; set; } = string.Empty;
} 

/// <summary>
/// DTO de respuesta para el proceso de registro de un cliente.
/// Indica si el registro fue exitoso y contiene un mensaje descriptivo del resultado.
/// </summary>
public class RegisterResponse
{
     /// <summary>
    /// Indica si el registro fue exitoso (true) o si ocurrió un error (false).
    /// </summary>
    public bool Success { get; set; }

     /// <summary>
    /// Mensaje descriptivo del resultado del registro.
    /// En caso de error, contiene el motivo del fallo.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

