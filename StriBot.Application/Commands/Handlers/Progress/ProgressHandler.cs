using System;
using System.Collections.Generic;
using System.IO;
using StriBot.Application.Bot.Enums;
using StriBot.Application.Commands.Enums;
using StriBot.Application.Commands.Extensions;
using StriBot.Application.Commands.Handlers.Progress.Models;
using StriBot.Application.Commands.Models;
using StriBot.Application.Events;
using StriBot.Application.Events.Models;

namespace StriBot.Application.Commands.Handlers.Progress;

public class ProgressHandler
{
    public int Deaths { get; set; }

    private Action _bossUpdate;
    private Action _deathUpdate;
    private const string Catalog = "Отчеты";
    private const string BossesFileName = "Боссы";

    private readonly BossList _bosses;

    public ProgressHandler()
    {
        _bosses = [];
        LoadBosses();
    }

    private void LoadBosses()
    {
        var path = GetPath(BossesFileName);

        if (Directory.Exists(path))
        {
            using var streamReader = new StreamReader(path);
            var lines = streamReader.ReadToEnd()
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                _bosses.Add(line);
            }
        }
    }

    public void SetConstructorSettings(Action bossUpdate, Action deathUpdate)
    {
        _bossUpdate = bossUpdate;
        _deathUpdate = deathUpdate;
        _bossUpdate();
        _deathUpdate();
    }

    private Command CreateBossesCommand()
        => new Command("Боссы", "Список убитых боссов!",
            delegate (CommandInfo e)
            {
                EventContainer.Message(_bosses.Count > 0 ? _bosses.ToString() : "Боссов нет", e.Platform);
            }, CommandType.Interactive);

    private Command CreateBossCommand()
        => new Command("Босс", "Добавляет босса", Role.Moderator,
            delegate (CommandInfo commandInfo)
            {
                if (!string.IsNullOrEmpty(commandInfo.ArgumentsAsString))
                {
                    _bosses.Add(commandInfo.ArgumentsAsString);
                    RecordBoss(commandInfo.ArgumentsAsString);
                    _bossUpdate();

                    EventContainer.Message($"Босс {commandInfo.ArgumentsAsString} успешно добавлен!", commandInfo.Platform);
                }
            }, ["Имя босса"], CommandType.Interactive);

    private static void RecordBoss(string name)
    {
        var path = GetPath(BossesFileName);
        using var file = File.AppendText(path);
        file.WriteLine(name);
    }

    private Command CreateDeathsCommand()
        => new Command("Смертей", "Показывает количество смертей",
            delegate (CommandInfo commandInfo)
            {
                EventContainer.Message($"Смертей: {Deaths}", commandInfo.Platform);
            }, CommandType.Interactive);

    private Command CreateDeathCommand()
        => new Command("Смерть", "Добавляет смерть", Role.Moderator,
            delegate (CommandInfo commandInfo)
            {
                Deaths++;
                _deathUpdate();
                EventContainer.Message($"Смертей: {Deaths}", commandInfo.Platform);
                EventContainer.Message("▬▬▬▬▬▬▬▬▬▬ஜ۩۞۩ஜ▬▬▬▬▬▬▬▬▬ ……………..............……...Ｙ Ｏ Ｕ Ｄ Ｉ Ｅ Ｄ…….……….........…..… ▬▬▬▬▬▬▬▬▬▬ஜ۩۞۩ஜ▬▬▬▬▬▬▬▬▬", commandInfo.Platform);
            }, CommandType.Interactive);

    public Dictionary<string, Command> CreateCommands()
        => new()
        {
            CreateDeathCommand(),
            CreateDeathsCommand(),
            CreateBossCommand(),
            CreateBossesCommand()
        };

    private static string GetPath(string name)
        => $"{Catalog}\\{name}.txt";

    public List<string> GetBosses()
        => _bosses;

    public void BossRemoveByIndex(int index)
    {
        _bosses.RemoveAt(index);

        var path = GetPath(BossesFileName);

        File.WriteAllLines(path, _bosses);

        _bossUpdate();
    }
}