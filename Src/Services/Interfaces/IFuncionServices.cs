using iCarus.Src.Dtos.Funcion;

namespace iCarus.Src.Services.interfaces;

public interface IFuncionServices
{
    Task<CreateFuncionResponse> CreateFuncionAsync(CreateFuncion request);
}
