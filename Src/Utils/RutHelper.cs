namespace iCarus.Src.Utils;

/// <summary>
/// Clase estática de utilidad para la validación de RUT chileno.
/// </summary>
public static class RutHelper
{

    // <summary>
    /// Metodo que valida si un RUT chileno es válido según el algoritmo del dígito verificador.
    /// El RUT debe ingresarse sin puntos ni guion (ejemplo: 12456789K).
    /// </summary>
    /// <param name="rut">RUT a validar sin puntos ni guion.</param>
    /// <returns>
    /// Retorna true si el RUT es válido, false en caso contrario.
    /// </returns>
    public static bool RutValid(string rut)
    {
        // Verificación de que el RUT no sea nulo o vacío.
        if (string.IsNullOrWhiteSpace(rut))
        return false;

        rut = rut.ToUpper();

        // Separación del cuerpo y dígito verificador del RUT.
        string body = rut.Substring(0, rut.Length-1);
        char dv = rut[rut.Length - 1];

        // Cálculo del dígito verificador según el algoritmo chileno.
        int sum = 0;
        int multiplier = 2;

        for (int i = body.Length - 1; i >= 0; i--)
        {
            sum += (body[i] - '0') * multiplier;
            multiplier++;

            if (multiplier > 7)
            multiplier = 2;
        }

        int remainder = sum % 11;
        int calculateDv = 11 - remainder;

        // Determina el dígito verificador esperado.
        char expectedDv;

        if (calculateDv == 11)
        expectedDv = '0';
        else if (calculateDv == 10)
        expectedDv = 'K';
        else
        expectedDv = (char)(calculateDv + '0');

         // Comparación del dígito verificador ingresado con el calculado.
        return dv == expectedDv;
    }
}