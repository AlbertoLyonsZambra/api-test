using SendGrid;
using SendGrid.Helpers.Mail;
using iCarus.Src.Services.Interfaces;

namespace iCarus.Src.Services;
/// <summary>
/// Clase que implementa la interfaz ISendGridEmailServices para el envío de correos electrónicos
/// mediante la API de SendGrid. Utiliza plantillas dinámicas para el contenido del correo.
/// </summary>
/// <param name="config">
/// La configuración de la aplicación,
/// utilizada para obtener las credenciales de SendGrid.
/// </param>
public class SendGridEmailServices(IConfiguration config) : ISendGridEmailServices
{
    /// <summary>
    /// Campo privado para almacenar la configuración de la aplicación,
    /// que contiene las credenciales de SendGrid (ApiKey, FromEmail, FromName).
    /// </summary>
    private readonly IConfiguration _config = config;

     /// <summary>
    /// Implementación del método SendEmailAsync definido en la interfaz ISendGridEmailServices.
    /// Envía un correo electrónico al usuario recién registrado con su contraseña temporal,
    /// utilizando una plantilla dinámica de SendGrid.
    /// </summary>
    /// <param name="toEmail">Correo electrónico destino.</param>
    /// <param name="subject">Asunto del correo electrónico.</param>
    /// <param name="tempPassword">Contraseña temporal generada para el usuario.</param>
    /// <returns>
    /// Retorna true si el correo fue enviado exitosamente, false en caso contrario.
    /// </returns>
    public async Task<bool> SendEmailAsync(string toEmail, string subject, string tempPassword)
    {
        try
        {
             // Obtención de las credenciales de SendGrid desde la configuración de la aplicación.
            var apiKey = _config["SendGrid:ApiKey"];
            var fromEmail = _config["SendGrid:FromEmail"];
            var fromName = _config["SendGrid:FromName"];

            // Creación del cliente de SendGrid y configuración del remitente y destinatario.
            var client = new SendGrid.SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(toEmail);

            // Construcción del mensaje de correo con la plantilla dinámica de SendGrid.
            var msg = new SendGridMessage();

            msg.SetFrom(from);
            msg.AddTo(to);

            msg.SetTemplateId("d-45c1e1278b494fdfab11fece6e192305");
            msg.SetTemplateData(new
            {
                TEMP_PASSWORD = tempPassword
            });

            var response = await client.SendEmailAsync(msg);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar el correo electronico: {ex.Message}");
            return false;
        }    
    }
}