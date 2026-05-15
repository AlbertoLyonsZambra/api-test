using Microsoft.EntityFrameworkCore;
using iCarus.Src.Db;
using iCarus.Src.Dtos.Users;
using iCarus.Src.Services.interfaces;
using iCarus.Src.Utils;
using iCarus.Src.Model;
using iCarus.Src.Services.Interfaces;

namespace iCarus.Src.Services;
public class AuthServices(ContextDb contextDb, IConfiguration config, ISendGridEmailServices emailService) : IAuthServices
{

    private readonly ContextDb _contextDb = contextDb;

    private readonly IConfiguration _config = config;
    private readonly ISendGridEmailServices _emailService = emailService;

    /**
     * Valida las credenciales del usuario e inicia sesión.
     *
     *	@param {LoginDto} loginDto - Datos de inicio de sesión con correo electrónico y contraseña.
     *	@returns {Task<LoginResponse>} Respuesta con el resultado del login, mensaje y token JWT si fue exitoso.
     *	@throws {Exception} Si ocurre un error inesperado durante el proceso de autenticación.
     */
    public Task<LoginResponse> Login(LoginDto loginDto)
    {
        try
        {   

            var user = _contextDb.Users
                .Include(u => u.Role)
                .FirstOrDefault(u=> u.Email == loginDto.Email);
            if(user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return Task.FromResult(new LoginResponse
                {
                    Success = false,
                    Message = "Correo electrónico o contraseña incorrectos",
                    Data = null
                });
            }
            if (!user.Status)
            {
                return Task.FromResult(new LoginResponse
                {
                    Success = false,
                    Message = "Usuario inactivo, por favor contacte al administrador",
                    Data = null
                });
            }

            return Task.FromResult(new LoginResponse
            {
                Success = true,
                Message = "Login exitoso",
                Data = new DataResponseLogin
                {
                    Token = GenerateToken.CreateToken(user, _config),
                    UserId = user.Id,
                    Name = user.Name,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role.Name
                }
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new LoginResponse
            {
                Success = false,
                Message = $"Error durante el login: {ex.Message}",
                Data = null
            });
        }
    }

    /**
     * Registra un nuevo usuario y envía una contraseña temporal por correo.
     *
     *	@param {RegisterDto} registerDto - Datos personales, RUT, correo y teléfono del usuario.
     *	@returns {Task<RegisterResponse>} Respuesta con el resultado del registro y su mensaje asociado.
     *	@throws {Exception} Si ocurre un error inesperado al guardar el usuario o enviar el correo.
     */
    public async Task<RegisterResponse> Register(RegisterDto registerDto)
    {

        try
        {

            if (!RutHelper.RutValid(registerDto.Rut))
            {
                return new RegisterResponse
                {

                Success = false,
                Message = "El RUT debe ingresarse sin puntos ni guion (ej.: 12456789K)"

                };
            }

            if(_contextDb.Users.Any(x => x.Email == registerDto.Email))
        {
                return new RegisterResponse
            {
                Success = false,
                Message = "El correo electrónico ingresado ya se encuentra registrado"
            };
        }

            if (_contextDb.Users.Any(x => x.Rut == registerDto.Rut))
        {
                return new RegisterResponse
            {
                Success = false,
                Message = "El RUT ingresado ya se encuentra registrado"
            };
        }

            string tempPassword = GenerateTemporaryPassword();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(tempPassword);           

        var newUser = new User
        {
            Name = registerDto.Name,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            Rut = registerDto.Rut,
            Phone = registerDto.PhoneNumber,
            Password = hashedPassword,
            Status = true,
            RoleId = 2
        };

        _contextDb.Users.Add(newUser);
        await _contextDb.SaveChangesAsync();
        await _emailService.SendEmailAsync(
        registerDto.Email,
        "Bienvenido - Tu contraseña temporal",
        tempPassword
        );

            return new RegisterResponse
        {
            Success = true,
            Message = "Usuario registrado exitosamente. Se ha enviado una contraseña temporal al correo ingresado."
        };
    }
        
        catch (Exception ex)
        {
            return new RegisterResponse
            {
                Success = false,
                Message = $"Error en el registro: {ex.Message}"
            };
        }
    }

    /**
     * Cambia la contraseña del usuario autenticado.
     * El usuario debe estar autenticado (token JWT válido) para acceder a esta funcionalidad.
     *
     * Validaciones aplicadas:
     *   - La contraseña actual debe coincidir con la almacenada en la base de datos.
     *   - La nueva contraseña y su confirmación deben ser iguales.
     *   - La nueva contraseña no puede ser igual a la contraseña actual.
     *   - La nueva contraseña debe tener entre 8 y 16 caracteres.
     *   - La nueva contraseña debe contener al menos una letra mayúscula.
     *   - La nueva contraseña debe contener al menos un número.
     *   - La nueva contraseña debe contener al menos un carácter especial (!@#$%^&* etc.).
     *
     *	@param {number} userId - Identificador del usuario obtenido desde el token JWT.
     *	@param {ChangePasswordDto} dto - Contraseña actual, nueva contraseña y confirmación.
     *	@returns {Task<LoginResponse>} Respuesta con el resultado del cambio de contraseña.
     *	@throws {Exception} Si ocurre un error inesperado al verificar o guardar la nueva contraseña.
     */
    public async Task<LoginResponse> ChangePassword(int userId, ChangePasswordDto dto)
    {
        try
        {
            var user = await _contextDb.Users.FindAsync(userId);

            // Verifica que el usuario exista
            if (user == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado",
                    Data = null
                };
            }

            // Verifica que la contraseña actual sea correcta
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.Password))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "La contraseña actual es incorrecta",
                    Data = null
                };
            }

            // Verifica que la nueva contraseña y la confirmación coincidan
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "La nueva contraseña y la confirmación no coinciden",
                    Data = null
                };
            }

            // Verifica que la nueva contraseña no sea igual a la actual
            if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.Password))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "La nueva contraseña no puede ser igual a la contraseña actual",
                    Data = null
                };
            }

            // Verifica que la nueva contraseña tenga entre 8 y 16 caracteres
            if (dto.NewPassword.Length < 8 || dto.NewPassword.Length > 16)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "La nueva contraseña debe tener entre 8 y 16 caracteres",
                    Data = null
                };
            }

            // Verifica que la nueva contraseña contenga al menos una mayúscula, un número y un carácter especial
            if (!dto.NewPassword.Any(char.IsUpper) ||
                !dto.NewPassword.Any(char.IsDigit) ||
                !dto.NewPassword.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "La nueva contraseña debe contener al menos una mayúscula, un número y un carácter especial",
                    Data = null
                };
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _contextDb.SaveChangesAsync();

            return new LoginResponse
            {
                Success = true,
                Message = "Contraseña actualizada exitosamente",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new LoginResponse
            {
                Success = false,
                Message = $"Error al cambiar la contraseña: {ex.Message}",
                Data = null
            };
        }
    }

    private string GenerateTemporaryPassword()
    {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();

    return new string(Enumerable.Repeat(chars, 8)
        .Select(s => s[random.Next(s.Length)])
        .ToArray());
    }
           
}
