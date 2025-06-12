namespace StriBot.Application.Commands.Handlers.Raffle.Models;

public class RaffleParticipant
{
    public string Nick { get; }

    public string Link { get; }

    public RaffleParticipant(string nick, string link)
    {
        Nick = nick;
        Link = link;
    }
}