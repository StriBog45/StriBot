using StriBot.Commands.Extensions;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using System;
using System.Collections.Generic;

namespace StriBot.Commands
{
    public class ProgressManager
    {
        public CollectionHelper Bosses { get; set; }
        public int Deaths { get; set; } = 0;

        private Action _bossUpdate;
        private Action _deathUpdate;

        public ProgressManager()
        {
            Bosses = new CollectionHelper();
        }

        public void SetConstructorSettings(Action bossUpdate, Action deathUpdate)
        {
            _bossUpdate = bossUpdate;
            _deathUpdate = deathUpdate;
        }

        public Command CreateBossesCommand()
            => new Command("Боссы", "Список убитых боссов!",
                delegate (CommandInfo e)
                {
                    if (Bosses.Count > 0)
                        GlobalEventContainer.Message(Bosses.ToString(), e.Platform);
                    else
                        GlobalEventContainer.Message("Боссов нет", e.Platform);
                }, CommandType.Interactive);

        public Command CreateBossCommand()
            => new Command("Босс", "Добавляет босса", Role.Moderator,
                delegate (CommandInfo e)
                {
                    Bosses.Add(e.ArgumentsAsString);
                    _bossUpdate();
                }, new string[] { "Имя босса" }, CommandType.Interactive);

        public Command CreateDeathsCommand()
            => new Command("Смертей", "Показывает количество смертей",
                delegate (CommandInfo commandInfo)
                {
                    GlobalEventContainer.Message(string.Format("Смертей: {0}", Deaths), commandInfo.Platform);
                }, CommandType.Interactive);

        public Command CreateDeathCommand()
            => new Command("Смерть", "Добавляет смерть", Role.Moderator,
                delegate (CommandInfo e)
                {
                    Deaths++;
                    _deathUpdate();
                    GlobalEventContainer.Message(string.Format("Смертей: {0}", Deaths), e.Platform);
                    GlobalEventContainer.Message("▬▬▬▬▬▬▬▬▬▬ஜ۩۞۩ஜ▬▬▬▬▬▬▬▬▬ ……………..............……...Ｙ Ｏ Ｕ Ｄ Ｉ Ｅ Ｄ…….……….........…..… ▬▬▬▬▬▬▬▬▬▬ஜ۩۞۩ஜ▬▬▬▬▬▬▬▬▬", e.Platform);
                }, CommandType.Interactive);

        public Dictionary<string, Command> CreateCommands()
            => new Dictionary<string, Command>()
            {
                CreateDeathCommand(),
                CreateDeathsCommand(),
                CreateBossCommand(),
                CreateBossesCommand()
            };
    }
}