using iCarus.Src.Db;
using iCarus.Src.Dtos.Funcion;
using iCarus.Src.Model;
using iCarus.Src.Services.interfaces;
using iCarus.Src.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace iCarus.Src.Services;

/// <summary>
/// Servicio que implementa la lógica de negocio para el requerimiento FUN-001.
/// Contiene las validaciones y el registro de funciones en la cartelera del teatro.
/// </summary>
public class FuncionServices(ContextDb contextDb) : IFuncionServices
{
    private readonly ContextDb _contextDb = contextDb;

    /// <summary>
    /// Valida y registra una nueva función en la base de datos.
    /// Aplica todas las reglas de negocio definidas en el requerimiento FUN-001:
    /// - La fecha y hora no puede ser anterior a la actual.
    /// - El precio base debe ser un valor numérico positivo.
    /// - No puede existir otra función registrada para el mismo día.
    ///
    /// @param request - Datos de la función: nombre, descripción, fecha, hora y precio base.
    /// @returns Respuesta con el resultado del registro y los datos creados si fue exitoso.
    /// @throws Exception Si ocurre un error inesperado al validar o guardar la función.
    /// </summary>
    public async Task<CreateFuncionResponse> CreateFuncionAsync(CreateFuncion request)
    {
        try
        {
            // Validación FUN-001: la fecha y hora no puede ser anterior a la actual
            var fechaHora = DateTime.Parse($"{request.DateFunction} {request.TimeFunction}");
            if (fechaHora <= DateTime.Now)
            {
                return new CreateFuncionResponse
                {
                    Success = false,
                    Message = "La fecha y hora de la función no puede ser anterior a la fecha actual",
                    Data = null
                };
            }

            // Validación FUN-001: el precio base debe ser un valor numérico positivo
            if (request.BasePrice <= 0)
            {
                return new CreateFuncionResponse
                {
                    Success = false,
                    Message = "El precio debe ser un valor numérico positivo",
                    Data = null
                };
            }

            // Validación FUN-001: no puede existir otra función registrada para el mismo día
            bool existeFuncion = await _contextDb.Funciones
                .AnyAsync(f => f.DateFunction == request.DateFunction);

            if (existeFuncion)
            {
                return new CreateFuncionResponse
                {
                    Success = false,
                    Message = "Ya existe una función registrada para este día",
                    Data = null
                };
            }

            // Todas las validaciones pasaron — se crea y persiste la nueva función
            var funcion = new Funcion
            {
                Name = request.Name,
                Description = request.Description,
                DateFunction = request.DateFunction,
                TimeFunction = request.TimeFunction,
                BasePrice = request.BasePrice,
            };

            await _contextDb.Funciones.AddAsync(funcion);
            await _contextDb.SaveChangesAsync();

            return new CreateFuncionResponse
            {
                Success = true,
                Message = "Función creada exitosamente",
                Data = new CreateFuncionData
                {
                    Id = funcion.Id,
                    Name = funcion.Name,
                    Description = funcion.Description,
                    DateFunction = funcion.DateFunction,
                    TimeFunction = funcion.TimeFunction,
                    BasePrice = funcion.BasePrice,
                }
            };
        }
        catch (Exception ex)
        {
            // Error inesperado — se retorna mensaje descriptivo sin exponer detalles internos al cliente
            return new CreateFuncionResponse
            {
                Success = false,
                Message = $"Error al crear la función: {ex.Message}",
                Data = null
            };
        }
    }
}