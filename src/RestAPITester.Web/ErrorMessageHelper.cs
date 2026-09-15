namespace RestAPITester.Web;

/// <summary>
/// Adiciona contexto amigável a mensagens de erro técnicas comuns
/// (ex: falha de rede no navegador, que geralmente indica CORS).
/// </summary>
public static class ErrorMessageHelper
{
    public static string Friendly(string? errorMessage)
    {
        if (string.IsNullOrEmpty(errorMessage)) return string.Empty;

        if (errorMessage.Contains("Failed to fetch", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("NetworkError", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("ERR_CONNECTION_REFUSED", StringComparison.OrdinalIgnoreCase))
        {
            return $"{errorMessage} — isso costuma acontecer por bloqueio de CORS ou pelo proxy local não estar rodando. Tente ativar/verificar o \"Usar proxy local\".";
        }

        return errorMessage;
    }
}