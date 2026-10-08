// Adicione a esta enumeração os tipos de nós do fluxo de trabalho que você deseja suportar. Cada tipo de nó representa uma ação ou operação específica no fluxo de trabalho.

namespace RestAPITester.Core.EnumsAndConstants
{
    /// <summary>
    /// Tipos de nós do fluxo de trabalho.
    /// </summary>
    public enum WorkflowNodeType
    {
        /// <summary>
        /// Nó inicial do fluxo de trabalho. Não faz nada, só serve como ponto de partida.
        /// </summary>
        Start,

        /// <summary>
        /// Nó que representa uma chamada de API. Reaproveita um TestCase existente.
        /// </summary>
        ApiCall,

        /// <summary>
        /// Nó que representa um atraso (delay) no fluxo de trabalho. O fluxo espera um tempo definido antes de continuar.
        /// </summary>
        Delay,

        /// <summary>
        /// Nó que representa uma condição. Dependendo do resultado da condição, o fluxo pode seguir por caminhos diferentes.
        /// </summary>
        Condition,

        /// <summary>
        /// Nó que representa um loop. O fluxo repete um conjunto de nós um número definido de vezes.
        /// </summary>
        Loop,

        /// <summary>
        /// Nó que representa a definição de uma variável. Permite armazenar valores para uso posterior no fluxo de trabalho.
        /// </summary>
        SetVariable,

        /// <summary>
        /// Nó que representa uma nota. Permite adicionar comentários ou informações adicionais no fluxo de trabalho.
        /// </summary>
        Note
    }
}
