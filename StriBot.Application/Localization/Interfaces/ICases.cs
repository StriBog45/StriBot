namespace StriBot.Application.Localization.Interfaces;

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
    /// Именительный мн. ч.
    /// </summary>
    string NominativeMultiple { get; }

    /// <summary>
    /// Родительный мн. ч.
    /// </summary>
    string GenitiveMultiple { get; }

    /// <summary>
    /// Дательный мн. ч.
    /// </summary>
    string DativeMultiple { get; }

    /// <summary>
    /// Винительный мн. ч.
    /// </summary>
    string AccusativeMultiple { get; }

    /// <summary>
    /// Творительный мн. ч.
    /// </summary>
    string InstrumentalMultiple { get; }

    /// <summary>
    /// Предложный мн. ч.
    /// </summary>
    string PrepositionalMultiple { get; }
}