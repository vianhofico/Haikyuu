using System;
using System.Collections.Generic;

namespace HaikyuuGame.Tournament
{
    public readonly struct TournamentPairing
    {
        public TournamentPairing(string left, string right)
        {
            Left = left;
            Right = right;
        }

        public string Left { get; }
        public string Right { get; }
    }

    public static class TournamentBracket
    {
        public static IReadOnlyList<TournamentPairing> Generate(IReadOnlyList<string> teams, int seed)
        {
            List<string> shuffled = new List<string>(teams);
            Random random = new Random(seed);

            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                string temp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = temp;
            }

            List<TournamentPairing> pairings = new List<TournamentPairing>();
            for (int i = 0; i < shuffled.Count; i += 2)
            {
                string right = i + 1 < shuffled.Count ? shuffled[i + 1] : "BYE";
                pairings.Add(new TournamentPairing(shuffled[i], right));
            }

            return pairings;
        }
    }
}
