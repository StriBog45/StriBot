﻿using System;

namespace StriBot.CustomData
{
    public class CustomArray
    {
        readonly private string[] answersOfBall = new string[]{ "Бесспорно", "Разумеется", "Никаких сомнений", "Определённо да", "Можешь быть уверен в этом", "Мне кажется — «да»", "Вероятнее всего", "Хорошие перспективы", "Знаки говорят — «да»",
            "Да", "Пока не ясно, попробуй снова", "Спроси позже", "Лучше не рассказывать", "Сейчас нельзя предсказать", "Сконцентрируйся и спроси опять", "Даже не думай", "Мой ответ — «нет»", "По моим данным — «нет»",
            "Перспективы не очень хорошие", "Весьма сомнительно" };
        readonly private string[] hited = new string[] { "в ухо", "по попке PogChamp ", "в плечо", "в пузо", "в ноутбук", "в спину", "в глаз и оставил фингал" };
        readonly private string[] underpantsType = new string[] { "Слипы", "Тонг", "Танга", "Панталоны", "Бикини", "Бразилиано", "Шорты", "Мини-стринги", "Классические трусы", "Хипсстерсы" };
        readonly private string[] underpantsColor = new string[] { "синие", "красные", "желтые", "черные", "розовые", "зеленые", "в горошек", "в цветочек", "в полоску", "прозрачные", "львица", "тигрица", "слоник", "армейка" };
        readonly private string[] bucket = new string[] { "Ромашек", "Тюльпанов", "Алых роз", "Белых роз", "Гладиолусов", "Лилий", "Калл" };
        
        public CustomArray() 
        {
        }

        public string GetRandomOfArray(string[] array)
            => RandomHelper.GetRandomOfArray(array);

        public string GetBallAnswer()
            => GetRandomOfArray(answersOfBall);

        public string GetHited()
            => GetRandomOfArray(hited);

        public string GetUnderpantsType()
            => GetRandomOfArray(underpantsType);

        public string GetUnderpantsColor()
            => GetRandomOfArray(underpantsColor);

        public string GetBucket()
            => GetRandomOfArray(bucket);
    }
}