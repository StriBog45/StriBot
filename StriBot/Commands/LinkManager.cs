using StriBot.Commands.Extensions;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using System.Collections.Generic;

namespace StriBot.Commands
{
    public class LinkManager
    {
        public LinkManager()
        {
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                #region Информационные
                { new Command("Команды","Ссылка на список команд",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("https://vk.cc/a6Giqf", e.Platform);}, CommandType.Info)},
                { new Command("Dotabuff","Ссылка на dotabuff",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("https://ru.dotabuff.com/players/113554714", e.Platform); }, CommandType.Info)},
                { new Command("Vk","Наша группа в ВКонтакте",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("https://vk.com/stribog45", e.Platform); }, CommandType.Info)},
                { new Command("Youtube","Архив некоторых записей",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("https://www.youtube.com/channel/UCrp75ozt9Spv5k7oVaRd5MQ", e.Platform); }, CommandType.Info)},
                { new Command("GoodGame","Ссылка на дополнительный канал на GoodGame",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("https://goodgame.ru/channel/StriBog45/", e.Platform); }, CommandType.Info)},
                { new Command("Discord","Наш discord для связи!",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("https://discord.gg/7Z6HGYZ", e.Platform); }, CommandType.Info)},
                { new Command("Steam","Ссылка на мой steam",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("https://steamcommunity.com/id/StriBog45", e.Platform); }, CommandType.Info)},
                #endregion

                #region Стримеры
                { new Command("Daisy","Показывает ссылку на twitch Daisy(roliepolietrolie)",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("Любимая австралийка, обладает хорошим чувством юмора. Не понимает русский, но старается его переводить. А также обожает Dota 2 <3 , twitch.tv/squintee", 
                        e.Platform); },CommandType.Streamers)},
                { new Command("Katenok","Показывает ссылку на twitch Katenok",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("Очаровашка Катенок(Ffunnya), улыбчивая и светлая персона! Любит DBD и Dota 2 <3 , twitch.tv/squintee", e.Platform); }, CommandType.Streamers)},
                { new Command("Gohapsp","Показывает ссылку на twitch Gohapsp",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("Специалист по хоррорам, twitch.tv/gohapsp", e.Platform); }, CommandType.Streamers)},
                { new Command("Stone","Показывает ссылку на twitch Камушка",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("Самый очаровательный камушек! <3 , twitch.tv/sayyees", e.Platform); }, CommandType.Streamers)},
                { new Command("Бескрыл","Показывает ссылку на twitch Бескрыл-а",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("Свежие одиночные игры на прохождение :) Добрый, отзывчивый, не оставит без внимания никого! К слову, он разработчик и у него уже есть своя игра! :) twitch.tv/beskr1l_",
                        e.Platform); }, CommandType.Streamers)},
                { new Command("Welovegames","Показывает ссылку на twitch Welovegames",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("Хранитель убежища, харизматичный Денис! Если вы о нём ещё не знаете, крайне рекомендую посмотреть на его деятельность. p.s. обожаю его смайлы. twitch.tv/welovegames",
                        e.Platform); }, CommandType.Streamers)},
                { new Command("StrykOFF","Показывает ссылку на twitch StrykOFF",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("Владелец таверны, создатель лучших шаверм! Для ламповых посиделок :) twitch.tv/strykoff", e.Platform); }, CommandType.Streamers)},
                { new Command("Tilttena","Показывает ссылку на twitch Tilttena",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("Горящая Алёна!, twitch.tv/tilttena", e.Platform); }, CommandType.Streamers)},
                { new Command("Bezumnaya","Показывает ссылку на twitch Bezumnaya",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("Безумно любит своих зрителей, twitch.tv/bezumnaya", e.Platform); }, CommandType.Streamers)},
                { new Command("Starval","Показывает ссылку на twitch Starval",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("Лера. Киев. Стример. :), twitch.tv/starval", e.Platform); }, CommandType.Streamers)},
                { new Command("Aiana","Показывает ссылку на twitch AianaKim",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("Наша улыбашка-очаровашка Аяна BLELELE  twitch.tv/aianakim", e.Platform); }, CommandType.Streamers)},
                { new Command("Reara","Показывает ссылку на twitch SyndicateReara",
                delegate (CommandInfo e) {
                    GlobalEventContainer.Message("Незабудь выполнить воинское приветствие striboF twitch.tv/syndicatereara", e.Platform); }, CommandType.Streamers)}
                #endregion
            };
    }
}