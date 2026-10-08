namespace RestAPITester.Core.Models;

/// <summary>
/// Entidade que representa um caso de teste para uma operação de endpoint de API REST.
/// </summary>
public class TestCase
{
    /// <summary>
    /// Identificador único do caso de teste.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nome do caso de teste, usado para identificação e descrição do teste.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Identificador da operação de endpoint da API que este caso de teste está testando.
    /// </summary>
    public string EndpointOperationId { get; set; } = string.Empty;

    /// <summary>
    /// Dicionário de valores de parâmetros para a operação de endpoint, onde a chave é o nome do parâmetro e o valor é o valor a ser usado no teste.
    /// </summary>
    public Dictionary<string, object?> ParameterValues { get; set; } = new();

    /// <summary>
    /// Corpo da requisição em formato JSON, se aplicável, para operações que requerem um corpo de requisição (ex: POST, PUT).
    /// </summary>
    public string? RequestBodyJson { get; set; }

    /// <summary>
    /// Código de status HTTP esperado como resultado da execução do caso de teste. Se não for especificado, o teste não verificará o código de status.
    /// </summary>
    public int? ExpectedStatusCode { get; set; }

    /// <summary>
    /// Lista de asserções que devem ser verificadas após a execução do caso de teste, permitindo validações adicionais sobre a resposta da API.
    /// </summary>
    public List<TestAssertion> Assertions { get; set; } = new();

    /// <summary>
    /// Lista de variáveis capturadas da resposta da API, que podem ser usadas em casos de teste subsequentes para encadear testes e validar fluxos de trabalho complexos.
    /// </summary>
    public List<VariableCapture> CapturedVariables { get; set; } = new();

    /// <summary>
    /// Headers adicionados manualmente à requisição (ex: "If-Match": "{{etag}}"),
    /// úteis para APIs com conditional requests que não declaram isso como parâmetro na spec.
    /// </summary>
    public Dictionary<string, string> ExtraHeaders { get; set; } = new();

    /// <summary>
    /// Indica se o cliente HTTP deve seguir redirecionamentos automaticamente. O padrão é true, permitindo que redirecionamentos sejam seguidos. Se definido como false, o cliente não seguirá redirecionamentos e retornará a resposta original.
    /// </summary>
    public bool FollowRedirects { get; set; } = true;
}