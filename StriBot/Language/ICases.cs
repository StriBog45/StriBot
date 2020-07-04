namespace StriBot.Language
{
    /// <summary>
    /// Падежи
    /// </summary>
    public interface ICases
    {
        /// <summary>
        /// Именительный
        /// </summary>
        string Nominative { get; }

        /// <summary>
        /// Родительный
        /// </summary>
        string Genitive { get; }

        /// <summary>
        /// Дательный
        /// </summary>
        string Dative { get; }

        /// <summary>
        /// Винительный
        /// </summary>
        string Accusative { get; }

        /// <summary>
        /// Творительный
        /// </summary>
        string Instrumental { get; }
        
        /// <summary>
        /// Предложный
        /// </summary>
        string Prepositional { get; }

        /// <summary>
        /// Именительный
        /// </summary>
        string NominativeMultiple { get; }

        /// <summary>
        /// Родительный
        /// </summary>
        string GenitiveMultiple { get; }

        /// <summary>
        /// Дательный
        /// </summary>
        string DativeMultiple { get; }

        /// <summary>
        /// Винительный
        /// </summary>
        string AccusativeMultiple { get; }

        /// <summary>
        /// Творительный
        /// </summary>
        string InstrumentalMultiple { get; }

        /// <summary>
        /// Предложный
        /// </summary>
        string PrepositionalMultiple { get; }
    }
}