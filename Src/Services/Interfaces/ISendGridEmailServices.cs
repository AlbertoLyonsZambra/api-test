namespace iCarus.Src.Services.Interfaces;

public interface ISendGridEmailServices
{
    Task<bool> SendEmailAsync(string toEmail, string subject, string tempPassword);
}