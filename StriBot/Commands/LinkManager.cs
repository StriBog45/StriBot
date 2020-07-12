using StriBot.TwitchBot.Interfaces;
using System.Collections.Generic;
using TwitchLib.Client.Events;

namespace StriBot.Commands
{
    public class LinkManager
    {
        private ITwitchBot twitchBot;

        public LinkManager(ITwitchBot twitchBot)
        {
            this.twitchBot = twitchBot;
        }

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                #region Информационные
                { "команды", new Command("Команды","Ссылка на список команд",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("https://vk.cc/a6Giqf");}, CommandType.Info)},
                { "dotabuff", new Command("Dotabuff","Ссылка на dotabuff",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("https://ru.dotabuff.com/players/113554714"); }, CommandType.Info)},
                { "vk", new Command("Vk","Наша группа в ВКонтакте",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("https://vk.com/stribog45"); }, CommandType.Info)},
                { "youtube", new Command("Youtube","Архив некоторых записей",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("https://www.youtube.com/channel/UCrp75ozt9Spv5k7oVaRd5MQ"); }, CommandType.Info)},
                { "gg", new Command("GoodGame","Ссылка на дополнительный канал на GoodGame",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("https://goodgame.ru/channel/StriBog45/"); }, CommandType.Info)},
                { "discord", new Command("Discord","Наш discord для связи!",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("https://discord.gg/7Z6HGYZ"); }, CommandType.Info)},
                { "steam", new Command("Steam","Ссылка на мой steam",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("https://steamcommunity.com/id/StriBog45"); }, CommandType.Info)},
                #endregion

                #region Стримеры
                { "daisy", new Command("Daisy","Показывает ссылку на twitch Daisy(roliepolietrolie)",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("Любимая австралийка, обладает хорошим чувством юмора. Не понимает русский, но старается его переводить. А также обожает Dota 2 <3 , twitch.tv/squintee"); },CommandType.Streamers)},
                { "katenok", new Command("Katenok","Показывает ссылку на twitch Katenok",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("Очаровашка Катенок(Ffunnya), улыбчивая и светлая персона! Любит DBD и Dota 2 <3 , twitch.tv/squintee"); }, CommandType.Streamers)},
                { "gohapsp",  new Command("Gohapsp","Показывает ссылку на twitch Gohapsp",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("Специалист по хоррорам, twitch.tv/gohapsp"); }, CommandType.Streamers)},
                { "stone", new Command("Stone","Показывает ссылку на twitch Камушка",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("Самый очаровательный камушек! <3 , twitch.tv/sayyees"); }, CommandType.Streamers)},
                { "бескрыл", new Command("Бескрыл","Показывает ссылку на twitch Бескрыл-а",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("Свежие одиночные игры на прохождение :) Добрый, отзывчивый, не оставит без внимания никого! К слову, он разработчик и у него уже есть своя игра! :) twitch.tv/beskr1l_"); }, CommandType.Streamers)},
                { "wlg", new Command("Welovegames","Показывает ссылку на twitch Welovegames",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("Хранитель убежища, харизматичный Денис! Если вы о нём ещё не знаете, крайне рекомендую посмотреть на его деятельность. p.s. обожаю его смайлы. twitch.tv/welovegames"); }, CommandType.Streamers)},
                { "stryk", new Command("StrykOFF","Показывает ссылку на twitch StrykOFF",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("Владелец таверны, создатель лучших шаверм! Для ламповых посиделок :) twitch.tv/strykoff"); }, CommandType.Streamers)},
                { "tilttena", new Command("Tilttena","Показывает ссылку на twitch Tilttena",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("Горящая Алёна!, twitch.tv/tilttena"); }, CommandType.Streamers)},
                { "bezumnaya", new Command("Bezumnaya","Показывает ссылку на twitch Bezumnaya",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("Безумно любит своих зрителей, twitch.tv/bezumnaya"); }, CommandType.Streamers)},
                { "starval", new Command("Starval","Показывает ссылку на twitch Starval",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("Лера. Киев. Стример. :), twitch.tv/starval"); }, CommandType.Streamers)},
                { "aiana", new Command("Aiana","Показывает ссылку на twitch AianaKim",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("Наша улыбашка-очаровашка Аяна BLELELE  twitch.tv/aianakim"); }, CommandType.Streamers)},
                { "reara", new Command("SyndicateReara","Показывает ссылку на twitch SyndicateReara",
                delegate (OnChatCommandReceivedArgs e) {
                    twitchBot.SendMessage("Незабудь выполнить воинское приветствие striboF twitch.tv/syndicatereara"); }, CommandType.Streamers)}
                #endregion
            };
    }
}