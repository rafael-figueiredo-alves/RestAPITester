namespace RestAPITester.Core.EnumsAndConstants;

/// <summary>
/// Representa os diferentes métodos HTTP que podem ser utilizados em requisições REST.
/// </summary>
public enum HttpMethodType
{
    /// <summary>
    /// Representa o método HTTP GET, utilizado para recuperar informações de um recurso.
    /// </summary>
    Get,

    /// <summary>
    /// Representa o método HTTP POST, utilizado para enviar dados para um recurso e criar uma nova entidade.
    /// </summary>
    Post,

    /// <summary>
    /// Representa o método HTTP PUT, utilizado para atualizar completamente um recurso existente.
    /// </summary>
    Put,

    /// <summary>
    /// Representa o método HTTP PATCH, utilizado para atualizar parcialmente um recurso existente.
    /// </summary>
    Patch,

    /// <summary>
    /// Representa o método HTTP DELETE, utilizado para remover um recurso existente.
    /// </summary>
    Delete,

    /// <summary>
    /// Representa o método HTTP HEAD, utilizado para recuperar apenas os cabeçalhos de um recurso, sem o corpo da resposta.
    /// </summary>
    Head,

    
    /// <summary>
    /// Representa o método HTTP OPTIONS, utilizado para recuperar as opções de comunicação disponíveis para um recurso.
    /// </summary>
    Options
}