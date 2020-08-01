namespace StriBot.CustomData
{
    public class AnswerOptions
    {
        readonly private string[] answersOfBall = new string[]{ "Бесспорно", "Разумеется", "Никаких сомнений", "Определённо да", "Можешь быть уверен в этом", "Мне кажется — «да»", "Вероятнее всего", "Хорошие перспективы", "Знаки говорят — «да»",
            "Да", "Пока не ясно, попробуй снова", "Спроси позже", "Лучше не рассказывать", "Сейчас нельзя предсказать", "Сконцентрируйся и спроси опять", "Даже не думай", "Мой ответ — «нет»", "По моим данным — «нет»",
            "Перспективы не очень хорошие", "Весьма сомнительно" };
        readonly private string[] hited = new string[] { "в ухо", "по попке PogChamp ", "в плечо", "в пузо", "в ноутбук", "в спину", "в глаз и оставил фингал" };
        readonly private string[] underpantsType = new string[] { "Слипы", "Тонг", "Танга", "Панталоны", "Бикини", "Бразилиано", "Шорты", "Мини-стринги", "Классические трусы", "Хипсстерсы" };
        readonly private string[] underpantsColor = new string[] { "синие", "красные", "желтые", "черные", "розовые", "зеленые", "в горошек", "в цветочек", "в полоску", "прозрачные", "львица", "тигрица", "слоник", "армейка" };
        readonly private string[] bucket = new string[] { "Ромашек", "Тюльпанов", "Алых роз", "Белых роз", "Гладиолусов", "Лилий", "Калл" };
        readonly private string[] dotaDuelResult = new string[] {
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

        public string GetRandomAnswer(string[] array)
            => RandomHelper.GetRandomOfArray(array);

        public string GetBallAnswer()
            => GetRandomAnswer(answersOfBall);

        public string GetHited()
            => GetRandomAnswer(hited);

        public string GetUnderpantsType()
            => GetRandomAnswer(underpantsType);

        public string GetUnderpantsColor()
            => GetRandomAnswer(underpantsColor);

        public string GetBucket()
            => GetRandomAnswer(bucket);

        public string GetDota2DuelResult()
            => GetRandomAnswer(dotaDuelResult);
    }
}