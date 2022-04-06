using Discord;

namespace RestoreCord.Utilities;

internal class Miscallenous
{
    internal static Color RandomDiscordColour() => new(new Random().Next(0, 255), new Random().Next(0, 255), new Random().Next(0, 255));
}
