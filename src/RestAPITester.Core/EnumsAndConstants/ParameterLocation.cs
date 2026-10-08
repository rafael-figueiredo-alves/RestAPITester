namespace RestAPITester.Core.EnumsAndConstants
{
    /// <summary>
    /// Localização de um parâmetro em uma requisição HTTP (query, path, header, cookie).
    /// </summary>
    public enum ParameterLocation
    {
        /// <summary>
        /// Indica que o parâmetro está localizado na query string da URL.
        /// </summary>
        Query,

        /// <summary>
        /// Indica que o parâmetro está localizado no caminho (path) da URL.
        /// </summary>
        Path,

        /// <summary>
        /// Indica que o parâmetro está localizado no cabeçalho (header) da requisição HTTP.
        /// </summary>
        Header,

        /// <summary>
        /// Indica que o parâmetro está localizado em um cookie da requisição HTTP.
        /// </summary>
        Cookie
    }
}
