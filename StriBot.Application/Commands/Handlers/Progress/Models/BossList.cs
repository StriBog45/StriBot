using System.Collections.Generic;

namespace StriBot.Application.Commands.Handlers.Progress.Models;

public class BossList : List<string>
{
    public override string ToString()
        => string.Join(", ", this);
}