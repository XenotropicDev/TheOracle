using TheOracle2.Data;

namespace Server.DiceRoller;

public interface IBurnable
{
    void Burn(int? momentum, int? reset);
}
