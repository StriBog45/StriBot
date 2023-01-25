﻿using System.Collections.Generic;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Extensions;
using StriBot.Application.Commands.Models;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;

namespace StriBot.Application.Commands.Handlers
{
    public class LinkHandler
    {
        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                #region Информационные

                new Command("Команды", "Ссылка на список команд",
                    delegate(CommandInfo e) { EventContainer.Message("https://vk.cc/a6Giqf", e.Platform); },
                    CommandType.Info),
                new Command("Dotabuff", "Ссылка на dotabuff",
                    delegate(CommandInfo e)
                    {
                        EventContainer.Message("https://ru.dotabuff.com/players/113554714", e.Platform);
                    }, CommandType.Info),
                new Command("Vk", "Наша группа в ВКонтакте",
                    delegate(CommandInfo e) { EventContainer.Message("https://vk.com/stribog45", e.Platform); },
                    CommandType.Info),
                new Command("Youtube", "Архив некоторых записей",
                    delegate(CommandInfo e)
                    {
                        EventContainer.Message("https://www.youtube.com/channel/UCrp75ozt9Spv5k7oVaRd5MQ",
                            e.Platform);
                    }, CommandType.Info),
                new Command("GoodGame", "Ссылка на дополнительный канал на GoodGame",
                    delegate(CommandInfo e)
                    {
                        EventContainer.Message("https://goodgame.ru/channel/StriBog45/", e.Platform);
                    }, CommandType.Info),
                new Command("vkplay", "Ссылка на дополнительный канал на VK Play",
                    delegate(CommandInfo e)
                    {
                        EventContainer.Message("https://vkplay.live/stribog45/", e.Platform);
                    }, CommandType.Info),
                new Command("Discord", "Наш discord для связи!",
                    delegate(CommandInfo e) { EventContainer.Message("https://discord.gg/7Z6HGYZ", e.Platform); },
                    CommandType.Info),
                new Command("Steam", "Ссылка на мой steam",
                    delegate(CommandInfo e)
                    {
                        EventContainer.Message("https://steamcommunity.com/id/StriBog45", e.Platform);
                    }, CommandType.Info),
                new Command("Raiders", "Ссылка для участия в битве StreamRaiders!",
                    delegate(CommandInfo e)
                    {
                        EventContainer.Message("https://www.streamraiders.com/t/stribog45/", e.Platform);
                    }, CommandType.Info),

                #endregion

                #region Стримеры
                new Command("Daisy","Показывает ссылку на twitch Daisy(roliepolietrolie)",
                delegate (CommandInfo e) {
                    EventContainer.Message("Любимая австралийка, с хорошим чувством юмора! Обожает Dota 2 twitch.tv/squintee", 
                        e.Platform); },CommandType.Streamers),
                new Command("Катя","Показывает ссылку на twitch Katenok",
                delegate (CommandInfo e) {
                    EventContainer.Message("Очаровашка Катенок(Ffunnya), улыбчивая и светлая персона! Любит Dota 2, CS:GO, DBD <3 twitch.tv/katenok", e.Platform); }, CommandType.Streamers),
                new Command("Гоха","Показывает ссылку на twitch Gohapsp",
                delegate (CommandInfo e) {
                    EventContainer.Message("Специалист по хоррорам :D twitch.tv/gohapsp", e.Platform); }, CommandType.Streamers),
                new Command("Stone","Показывает ссылку на twitch SayYees",
                delegate (CommandInfo e) {
                    EventContainer.Message("Самый очаровательный камушек! Лера! <3 twitch.tv/sayyees", e.Platform); }, CommandType.Streamers),
                new Command("Лера","Показывает ссылку на twitch Starval",
                delegate (CommandInfo e) {
                    EventContainer.Message("Лера. Киев. Dota Underlords. :) twitch.tv/cyberval", e.Platform); }, CommandType.Streamers),
                new Command("Аяна","Показывает ссылку на twitch AianaKim",
                delegate (CommandInfo e) {
                    EventContainer.Message("Наша улыбашка-очаровашка Аяна twitch.tv/aianakim", e.Platform); }, CommandType.Streamers),
                new Command("Вика","Показывает ссылку на twitch SyndicateReara",
                delegate (CommandInfo e) {
                    EventContainer.Message("Наш любимый модератор <3 twitch.tv/syndicatereara", e.Platform); }, CommandType.Streamers),
                new Command("Денис","Показывает ссылку на twitch Monstro_boy",
                delegate (CommandInfo e) {
                    EventContainer.Message("Молот и наковальня! twitch.tv/monstro_boy", e.Platform); }, CommandType.Streamers),
                new Command("Степа","Показывает ссылку на twitch CyberStepan",
                delegate (CommandInfo e) {
                    EventContainer.Message("Киберспортивный комментатор Dota 2! twitch.tv/cyberstepan", e.Platform); }, CommandType.Streamers),
                new Command("Лина","Показывает ссылку на twitch Anginka",
                delegate (CommandInfo e) {
                    EventContainer.Message("Милашка Ангинка :) twitch.tv/anginka", e.Platform); }, CommandType.Streamers),
                new Command("Baibaka","Показывает ссылку на twitch BaibakaPX",
                delegate (CommandInfo e) {
                    EventContainer.Message("Настя и её бананы! :) twitch.tv/BaibakaPX", e.Platform); }, CommandType.Streamers)
                #endregion
            };
    }
}