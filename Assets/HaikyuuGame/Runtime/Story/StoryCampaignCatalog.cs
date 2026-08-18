using System.Collections.Generic;

namespace HaikyuuGame.Story
{
    public readonly struct StoryChapter
    {
        public StoryChapter(int index, string id, string title, string opponent, string objective)
        {
            Index = index;
            Id = id;
            Title = title;
            Opponent = opponent;
            Objective = objective;
        }

        public int Index { get; }
        public string Id { get; }
        public string Title { get; }
        public string Opponent { get; }
        public string Objective { get; }
    }

    public static class StoryCampaignCatalog
    {
        private static readonly StoryChapter[] Chapters =
        {
            new StoryChapter(0, "first_jump", "The First Jump", "Practice Match", "Learn receive, set and spike."),
            new StoryChapter(1, "iron_wall", "Break the Iron Wall", "Date Tech", "Score five attacks against a strong block."),
            new StoryChapter(2, "great_king", "The Great King", "Aoba Johsai", "Survive serve pressure and win the match."),
            new StoryChapter(3, "super_ace", "The Super Ace", "Shiratorizawa", "Receive the left-handed ace and counterattack."),
            new StoryChapter(4, "training_camp", "Tokyo Training Camp", "Fukurodani", "Use three different attack tempos."),
            new StoryChapter(5, "nationals_twins", "The Twins", "Inarizaki", "Stop the twin quick and dual serve."),
            new StoryChapter(6, "battle_garbage_dump", "Connected Court", "Nekoma", "Win a long-rally defensive match."),
            new StoryChapter(7, "little_giant", "The Little Giant", "Kamomedai", "Beat serve-block pressure and the complete small ace."),
            new StoryChapter(8, "top_aces", "Aces of Japan", "All-Star", "Defeat elite ace rotations."),
            new StoryChapter(9, "road_to_top", "Road to the Top", "National Select", "Win the final best-of-three match.")
        };

        public static IReadOnlyList<StoryChapter> All => Chapters;

        public static StoryChapter Get(int index)
        {
            int safe = index < 0 ? 0 : (index >= Chapters.Length ? Chapters.Length - 1 : index);
            return Chapters[safe];
        }
    }
}
