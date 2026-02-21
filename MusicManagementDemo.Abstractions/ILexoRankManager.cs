using System.Collections.Frozen;
using System.Numerics;

namespace MusicManagementDemo.Abstractions;

public interface ILexoRankManager
{
    FrozenDictionary<char, BigInteger> CharacterToBigIntegerMap { get; init; }
    char[] IntToCharacterMap { get; init; }
    int BaseNumber { get; init; }
    string Between(string prev, string next);
}
