namespace StriBot.CustomData
{
    public class AnswerOptions
    {
        private readonly string[] _answersOfBall = { "Бесспорно", "Разумеется", "Никаких сомнений", "Определённо да", "Можешь быть уверен в этом", "Мне кажется — «да»", "Вероятнее всего", "Хорошие перспективы", "Знаки говорят — «да»",
            "Да", "Пока не ясно, попробуй снова", "Спроси позже", "Лучше не рассказывать", "Сейчас нельзя предсказать", "Сконцентрируйся и спроси опять", "Даже не думай", "Мой ответ — «нет»", "По моим данным — «нет»",
            "Перспективы не очень хорошие", "Весьма сомнительно" };
        private readonly string[] _hited = { "в ухо", "по попке PogChamp ", "в плечо", "в пузо", "в ноутбук", "в спину", "в глаз и оставил фингал" };
        private readonly string[] _underpantsType = { "Слипы", "Тонг", "Танга", "Панталоны", "Бикини", "Бразилиано", "Шорты", "Мини-стринги", "Классические трусы", "Хипсстерсы" };
        private readonly string[] _underpantsColor = { "синие", "красные", "желтые", "черные", "розовые", "зеленые", "в горошек", "в цветочек", "в полоску", "прозрачные", "львица", "тигрица", "слоник", "армейка" };
        private readonly string[] _bucket = { "Ромашек", "Тюльпанов", "Алых роз", "Белых роз", "Гладиолусов", "Лилий", "Калл" };
        private readonly string[] _dotaDuelResult = {
            "Побеждает в сухую! striboTea ", 
            "Проиграл в сухую. striboCry ", 
            "Удалось подловить противника с помощью руны. Победа.", 
            "Крипы зажали. Неловко. Поражение. CouldYouNot ",
            "Противник проигнорировал дуэль. CouldYouNot ",
            "Перефармил противника и сломал башню. Победа. POGGERS ",
            "Похоже противник намного опытнее на этом герое. Поражение.",
            "Боже, что сейчас произошло!? Победа! striboLyc ",
            "Жил до конца, умер как герой. Поражение. FeelsRainMan "
        };

        private static string GetRandomAnswer(string[] array)
            => RandomHelper.GetRandomOfArray(array);

        public string GetBallAnswer()
            => GetRandomAnswer(_answersOfBall);

        public string GetHited()
            => GetRandomAnswer(_hited);

        public string GetUnderpantsType()
            => GetRandomAnswer(_underpantsType);

        public string GetUnderpantsColor()
            => GetRandomAnswer(_underpantsColor);

        public string GetBucket()
            => GetRandomAnswer(_bucket);

        public string GetDota2DuelResult()
            => GetRandomAnswer(_dotaDuelResult);
    }
}