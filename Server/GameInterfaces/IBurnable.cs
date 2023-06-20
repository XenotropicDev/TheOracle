using Dataforged;

namespace Server.DiceRoller;

public interface IBurnable
{
    void Burn(int? momentum, int? reset);
}
