using System.Collections.Generic;
using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Character;
using HaikyuuGame.Gameplay.Match;
using HaikyuuGame.Gameplay.Player;

namespace HaikyuuGame.Gameplay.Teams
{
    public sealed class MatchRosterController
    {
        private readonly IReadOnlyList<PlayerActor> _leftRoster;
        private readonly IReadOnlyList<PlayerActor> _rightRoster;
        private readonly PlayerActor _leftLibero;
        private readonly PlayerActor _rightLibero;
        private readonly TeamRotation _leftRotation;
        private readonly TeamRotation _rightRotation;

        public MatchRosterController(
            IReadOnlyList<PlayerActor> leftRoster,
            IReadOnlyList<PlayerActor> rightRoster,
            PlayerActor leftLibero,
            PlayerActor rightLibero,
            TeamRotation leftRotation,
            TeamRotation rightRotation)
        {
            _leftRoster = leftRoster;
            _rightRoster = rightRoster;
            _leftLibero = leftLibero;
            _rightLibero = rightLibero;
            _leftRotation = leftRotation;
            _rightRotation = rightRotation;
            Current = this;
        }

        public static MatchRosterController Current { get; private set; }
        public string LeftTeamName { get; private set; } = "Karasuno";
        public string RightTeamName { get; private set; } = "Inarizaki";

        public void Apply(TeamSide side, string presetId)
        {
            TeamPreset preset = TeamPresetCatalog.Get(presetId);
            IReadOnlyList<PlayerActor> roster = side == TeamSide.Left ? _leftRoster : _rightRoster;
            PlayerActor libero = side == TeamSide.Left ? _leftLibero : _rightLibero;

            for (int i = 0; i < 6; i++) roster[i].AssignProfile(ResolveProfile(preset, i));

            RuntimeCharacterProfile liberoProfile = !string.IsNullOrEmpty(preset.LiberoId) ? HaikyuuRosterCatalog.Get(preset.LiberoId) : null;
            if (liberoProfile == null) liberoProfile = SupportProfileFactory.Create(preset.DisplayName, VolleyballRole.Libero, 7, preset.SupportStrength);
            libero.AssignProfile(liberoProfile);

            if (side == TeamSide.Left)
            {
                LeftTeamName = preset.DisplayName;
                _leftRotation.RefreshAssignments();
            }
            else
            {
                RightTeamName = preset.DisplayName;
                _rightRotation.RefreshAssignments();
            }
        }

        public void ApplyMatchup(string leftPreset, string rightPreset)
        {
            Apply(TeamSide.Left, leftPreset);
            Apply(TeamSide.Right, rightPreset);
        }

        private static RuntimeCharacterProfile ResolveProfile(TeamPreset preset, int slot)
        {
            string id = preset.StartingSix != null && slot < preset.StartingSix.Length ? preset.StartingSix[slot] : null;
            RuntimeCharacterProfile profile = !string.IsNullOrEmpty(id) ? HaikyuuRosterCatalog.Get(id) : null;
            return profile ?? SupportProfileFactory.Create(preset.DisplayName, TeamPresetCatalog.SupportRoleForSlot(slot), slot + 1, preset.SupportStrength);
        }
    }
}
