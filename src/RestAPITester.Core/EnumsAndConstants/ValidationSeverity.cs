namespace RestAPITester.Core.EnumsAndConstants
{
    /// <summary>
    /// Enum que representa la severidad de un problema de validación en el flujo de trabajo.
    /// </summary>
    public enum ValidationSeverity 
    {
        /// <summary>
        /// Aviso: Indica que hay un problema que no detendrá la ejecución, pero que podría causar problemas en el futuro.
        /// </summary>
        Warning,

        /// <summary>
        /// Error: Indica que hay un problema crítico que detendrá la ejecución y debe ser corregido antes de continuar.
        /// </summary>
        Error
    }
}
