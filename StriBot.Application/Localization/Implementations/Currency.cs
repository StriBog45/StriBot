using System;
using System.IO;
using System.Linq;
using ProtoBuf;
using StriBot.Application.Localization.Interfaces;
using StriBot.Application.Localization.Models;

namespace StriBot.Application.Localization.Implementations;

public class Currency : ICases
{
    private const string Catalog = "Currencies";
    private Cases _cases;

    public Currency()
    {
        _cases = new Cases();

        GetCurrencies();
    }

    public string Nominative => _cases.Nominative?.ToLower();

    public string Genitive => _cases.Genitive?.ToLower();

    public string Dative => _cases.Dative?.ToLower();

    public string Accusative => _cases.Accusative?.ToLower();

    public string Instrumental => _cases.Instrumental?.ToLower();

    public string Prepositional => _cases.Prepositional?.ToLower();

    public string NominativeMultiple => _cases.NominativeMultiple?.ToLower();

    public string GenitiveMultiple => _cases.GenitiveMultiple?.ToLower();

    public string DativeMultiple => _cases.DativeMultiple?.ToLower();

    public string AccusativeMultiple => _cases.AccusativeMultiple?.ToLower();

    public string InstrumentalMultiple => _cases.InstrumentalMultiple?.ToLower();

    public string PrepositionalMultiple => _cases.PrepositionalMultiple?.ToLower();

    public static string[] GetCurrencies()
    {
        var result = Array.Empty<string>();

        if (Directory.Exists(Catalog))
        {
            result = Directory.GetFiles(Catalog)
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();
        }
        else
        {
            Directory.CreateDirectory(Catalog);
        }

        return result;
    }

    public void LoadCurrency(string currencyName)
    {
        using (var file = File.OpenRead($"{Catalog}//{currencyName}.bin"))
        {
            _cases = Serializer.Deserialize<Cases>(file);
        }
    }

    public bool CreateCurrency(string currencyName, Cases cases)
    {
        var result = false;

        if (IsValidFileName(currencyName))
        {
            using (var file = File.Create($"{Catalog}//{currencyName}.bin"))
            {
                Serializer.Serialize(file, cases);
            }

            _cases = cases;

            result = true;
        }

        return result;
    }

    private static bool IsValidFileName(string fileName)
    {
        var invalidFileNameChars = Path.GetInvalidFileNameChars();

        foreach (var invalidChar in invalidFileNameChars)
        foreach (var symbol in fileName)
            if (symbol == invalidChar)
                return false;

        return true;
    }
}