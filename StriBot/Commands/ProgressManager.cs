using StriBot.Commands.Extensions;
using StriBot.EventConainers;
using StriBot.EventConainers.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace StriBot.Commands
{
    public class ProgressManager
    {
        public int Deaths { get; set; } = 0;

        private Action _bossUpdate;
        private Action _deathUpdate;
        private const string _catalog = "Отчеты";
        private const string _bossesFileName = "Боссы";

        private CollectionHelper _bosses;

        public ProgressManager()
        {
            _bosses = new CollectionHelper();
            LoadBosses();
        }

        private void LoadBosses()
        {
            var path = GetPath(_bossesFileName);
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    var lines = sr.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in lines)
                    {
                        _bosses.Add(line);
                    }
                }

            }
            catch (Exception exception)
            {

            }
        }

        public void SetConstructorSettings(Action bossUpdate, Action deathUpdate)
        {
            _bossUpdate = bossUpdate;
            _deathUpdate = deathUpdate;
            _bossUpdate();
        }

        public Command CreateBossesCommand()
            => new Command("Боссы", "Список убитых боссов!",
                delegate (CommandInfo e)
                {
                    if (_bosses.Count > 0)
                        GlobalEventContainer.Message(_bosses.ToString(), e.Platform);
                    else
                        GlobalEventContainer.Message("Боссов нет", e.Platform);
                }, CommandType.Interactive);

        public Command CreateBossCommand()
            => new Command("Босс", "Добавляет босса", Role.Moderator,
                delegate (CommandInfo commandInfo)
                {
                    if (!string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                    {
                        _bosses.Add(commandInfo.ArgumentsAsString);
                        RecordBoss(commandInfo.ArgumentsAsString);
                        _bossUpdate();

                        GlobalEventContainer.Message($"Босс {commandInfo.ArgumentsAsString} успешно добавлен!", commandInfo.Platform);
                    }
                }, new string[] { "Имя босса" }, CommandType.Interactive);

        private void RecordBoss(string name)
        {
            string path = GetPath(_bossesFileName);
            using (var file = File.AppendText(path))
            {
                file.WriteLine(name);
            }
        }

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

        private static string GetPath(string name)
            => string.Format("{0}\\{1}.txt", _catalog, name);

        public List<string> GetBosses()
            => _bosses;

        public void BossRemoveByIndex(int index)
        {
            _bosses.RemoveAt(index);

            var path = GetPath(_bossesFileName);

            File.WriteAllLines(path, _bosses);

            _bossUpdate();
        }
    }
}