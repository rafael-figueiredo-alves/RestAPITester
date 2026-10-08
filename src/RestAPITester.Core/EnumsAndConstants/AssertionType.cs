namespace RestAPITester.Core.EnumsAndConstants
{
    /// <summary>
    /// Tipo de verificação que pode ser aplicada sobre o resultado da execução de um TestCase.
    /// </summary>
    public enum AssertionType
    {
        /// <summary>
        /// Por código de status HTTP. Exemplo: 200, 404, 500, etc.
        /// </summary>
        StatusCode,

        /// <summary>
        /// Por conteúdo do corpo da resposta. Exemplo: "success", "error", etc.
        /// </summary>
        BodyContains,

        /// <summary>
        /// Por valor de um campo específico no corpo da resposta, usando JSONPath. Exemplo: "$.data.id" para verificar o valor do campo "id" dentro do objeto "data".
        /// </summary>
        JsonPathEquals,
        
        /// <summary>
        /// Por valor de um cabeçalho da resposta. Exemplo: "Content-Type" para verificar o tipo de conteúdo da resposta.
        /// </summary>
        HeaderEquals,
        
        /// <summary>
        /// Por tempo de resposta, verificando se é menor que um valor específico em milissegundos.
        /// </summary>
        ResponseTimeBelowMs
    }
}
