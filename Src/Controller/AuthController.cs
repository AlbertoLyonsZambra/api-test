using Microsoft.AspNetCore.Mvc;
using iCarus.Src.Dtos.Users;
using iCarus.Src.Services.Interfaces;
using iCarus.Src.Services.interfaces;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace iCarus.Src.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthServices authServices, ISendGridEmailServices emailService) : ControllerBase
{
    private readonly IAuthServices _authServices = authServices;
    private readonly ISendGridEmailServices _emailService = emailService;


    /**
     * Recibe la solicitud HTTP para iniciar sesión.
     *
     *	@param {LoginDto} loginDto - Credenciales enviadas desde el cliente.
     *	@returns {Task<IActionResult>} Respuesta HTTP con BadRequest si falla u Ok si el login es exitoso.
     */
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var response = await _authServices.Login(loginDto);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    /**
     * Recibe la solicitud HTTP para registrar un nuevo usuario.
     *
     *	@param {RegisterDto} registerDto - Datos personales y de contacto del usuario a registrar.
     *	@returns {Task<IActionResult>} Respuesta HTTP con BadRequest si falla u Ok si el registro es exitoso.
     */
    [HttpPost("testEmail")]
    public async Task<IActionResult> TestEmail(string text)
    {
        var response = await _emailService.SendEmailAsync(
        "revatoto15@gmail.com",
        "Bienvenido - Tu contraseña temporal",
        text
        );
        if (!response)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var response = await _authServices.Register(registerDto);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    /**
     * Recibe la solicitud HTTP para cambiar la contraseña del usuario autenticado.
     *
     *	@param {ChangePasswordDto} dto - Contraseña actual, nueva contraseña y confirmación.
     *	@returns {Task<IActionResult>} Respuesta HTTP con Unauthorized si no hay usuario, BadRequest si falla u Ok si se actualiza.
     */
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(userIdClaim == null)
        {
            return Unauthorized(new 
            {
                Success = false,
                Message = "Usuario no autenticado",
                Data = (string?)null
            });
        }

        int userId = int.Parse(userIdClaim);

        var response = await _authServices.ChangePassword(userId, dto);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
