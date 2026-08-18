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
            IReadOnlyList<PlayerActor> roster = RosterFor(side);
            PlayerActor libero = LiberoFor(side);

            for (int i = 0; i < 6; i++)
            {
                roster[i].AssignProfile(ResolveProfile(preset, i));
            }

            RuntimeCharacterProfile liberoProfile = !string.IsNullOrEmpty(preset.LiberoId)
                ? HaikyuuRosterCatalog.Get(preset.LiberoId)
                : null;
            if (liberoProfile == null)
            {
                liberoProfile = SupportProfileFactory.Create(preset.DisplayName, VolleyballRole.Libero, 7, preset.SupportStrength);
            }

            libero.AssignProfile(liberoProfile);
            SetTeamName(side, preset.DisplayName);
            RotationFor(side).RefreshAssignments();
        }

        public void ApplyMatchup(string leftPreset, string rightPreset)
        {
            Apply(TeamSide.Left, leftPreset);
            Apply(TeamSide.Right, rightPreset);
        }

        public void ApplyCustomLineup(
            TeamSide side,
            string teamName,
            IReadOnlyList<RuntimeCharacterProfile> startingSix,
            RuntimeCharacterProfile liberoProfile)
        {
            IReadOnlyList<PlayerActor> roster = RosterFor(side);
            for (int i = 0; i < 6; i++)
            {
                RuntimeCharacterProfile profile = startingSix != null && i < startingSix.Count
                    ? startingSix[i]
                    : null;
                if (profile != null)
                {
                    roster[i].AssignProfile(profile);
                }
            }

            if (liberoProfile != null)
            {
                LiberoFor(side).AssignProfile(liberoProfile);
            }

            SetTeamName(side, string.IsNullOrEmpty(teamName) ? "Custom Team" : teamName);
            RotationFor(side).RefreshAssignments();
        }

        public void AssignProfileToSlot(TeamSide side, int rosterSlot, RuntimeCharacterProfile profile)
        {
            if (profile == null || rosterSlot < 0 || rosterSlot >= 6)
            {
                return;
            }

            RosterFor(side)[rosterSlot].AssignProfile(profile);
            RotationFor(side).RefreshAssignments();
        }

        public void AssignLiberoProfile(TeamSide side, RuntimeCharacterProfile profile)
        {
            if (profile == null)
            {
                return;
            }

            LiberoFor(side).AssignProfile(profile);
            RotationFor(side).RefreshAssignments();
        }

        private IReadOnlyList<PlayerActor> RosterFor(TeamSide side)
        {
            return side == TeamSide.Left ? _leftRoster : _rightRoster;
        }

        private PlayerActor LiberoFor(TeamSide side)
        {
            return side == TeamSide.Left ? _leftLibero : _rightLibero;
        }

        private TeamRotation RotationFor(TeamSide side)
        {
            return side == TeamSide.Left ? _leftRotation : _rightRotation;
        }

        private void SetTeamName(TeamSide side, string name)
        {
            if (side == TeamSide.Left)
            {
                LeftTeamName = name;
            }
            else
            {
                RightTeamName = name;
            }
        }

        private static RuntimeCharacterProfile ResolveProfile(TeamPreset preset, int slot)
        {
            string id = preset.StartingSix != null && slot < preset.StartingSix.Length ? preset.StartingSix[slot] : null;
            RuntimeCharacterProfile profile = !string.IsNullOrEmpty(id) ? HaikyuuRosterCatalog.Get(id) : null;
            return profile ?? SupportProfileFactory.Create(
                preset.DisplayName,
                TeamPresetCatalog.SupportRoleForSlot(slot),
                slot + 1,
                preset.SupportStrength);
        }
    }
}
