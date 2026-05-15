using iCarus.Src.Dtos.Funcion;
using iCarus.Src.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iCarus.Src.Controller;

/// <summary>
/// Controlador que expone los endpoints HTTP para el requerimiento FUN-001: Registrar Función.
/// Solo accesible para usuarios autenticados con rol Admin.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class FuncionController(IFuncionServices funcionServices) : ControllerBase
{
    private readonly IFuncionServices _funcionServices = funcionServices;

    /// <summary>
    /// Endpoint POST para registrar una nueva función en la cartelera del teatro.
    /// Delega las validaciones del requerimiento FUN-001 al servicio correspondiente.
    ///
    /// @param request - Datos de la función: nombre, descripción, fecha, hora y precio base.
    /// @returns 200 OK con los datos de la función creada si el registro fue exitoso.
    ///          400 BadRequest si alguna validación falla.
    ///          500 Internal Server Error si ocurre un error inesperado.
    /// </summary>
    [HttpPost("Register")]
    public async Task<IActionResult> CreateFuncion([FromForm] CreateFuncion request)
    {
        try
        {
            // Se delega la lógica de validación y registro al servicio de funciones
            var response = await _funcionServices.CreateFuncionAsync(request);

            if (!response.Success)
            {
                // Alguna validación FUN-001 falló — se retorna el mensaje de error correspondiente
                return BadRequest(response);
            }

            // Función registrada exitosamente
            return Ok(response);
        }
        catch (Exception ex)
        {
            // Error inesperado del servidor
            return StatusCode(500, new CreateFuncionResponse
            {
                Success = false,
                Message = $"Error al crear la función: {ex.Message}",
                Data = null
            });
        }
    }
}