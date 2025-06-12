using DryIoc;

namespace StriBot.DryIoc.Interfaces;

public interface IContainerFiller
{
    /// <summary>
    ///     Регистрация зависимости в контейнере
    /// </summary>
    /// <param name="container"> контейнер для регистрации </param>
    void Fill(IContainer container);
}