using System.Collections.Generic;
using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Player;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Match
{
    public sealed class TeamRotation
    {
        private readonly TeamSide _team;
        private readonly PlayerActor[] _roster;
        private readonly PlayerActor[] _activeBySlot = new PlayerActor[6];
        private readonly PlayerActor _libero;
        private readonly Vector3[] _slotPositions;
        private int _rotationOffset;
        private int _activePlayerCount = 6;

        public TeamRotation(TeamSide team, IReadOnlyList<PlayerActor> sixPlayerRoster, PlayerActor libero, IReadOnlyList<Vector3> slotPositions)
        {
            _team = team;
            _roster = new PlayerActor[6];
            _slotPositions = new Vector3[6];
            for (int i = 0; i < 6; i++)
            {
                _roster[i] = sixPlayerRoster[i];
                _slotPositions[i] = slotPositions[i];
            }

            _libero = libero;
            RefreshCourtAssignments();
        }

        public TeamSide Team => _team;
        public int RotationOffset => _rotationOffset;
        public int ActivePlayerCount => _activePlayerCount;
        public PlayerActor CurrentServer => GetOccupant(_activePlayerCount == 3 ? 2 : 3);

        public PlayerActor GetActiveAtSlot(int slot)
        {
            return slot >= 0 && slot < _activeBySlot.Length ? _activeBySlot[slot] : null;
        }

        public void SetActivePlayerCount(int count)
        {
            _activePlayerCount = count <= 3 ? 3 : 6;
            _rotationOffset = 0;
            RefreshCourtAssignments();
        }

        public void RotateClockwise()
        {
            _rotationOffset = (_rotationOffset + 1) % _activePlayerCount;
            RefreshCourtAssignments();
        }

        public void ResetRotation()
        {
            _rotationOffset = 0;
            RefreshCourtAssignments();
        }

        public void RefreshAssignments()
        {
            RefreshCourtAssignments();
        }

        public void ResetPlayersToHome()
        {
            for (int i = 0; i < _roster.Length; i++)
            {
                if (_roster[i].gameObject.activeSelf)
                {
                    _roster[i].ResetToHome();
                }
            }

            if (_libero != null && _libero.gameObject.activeSelf)
            {
                _libero.ResetToHome();
            }
        }

        private void RefreshCourtAssignments()
        {
            if (_activePlayerCount == 3)
            {
                RefreshThreePlayerAssignments();
                return;
            }

            RefreshSixPlayerAssignments();
        }

        private void RefreshThreePlayerAssignments()
        {
            for (int i = 0; i < 6; i++)
            {
                _activeBySlot[i] = null;
                _roster[i].SetCourtAssignment(-1, HiddenPosition(), false);
            }

            if (_libero != null)
            {
                _libero.SetCourtAssignment(-1, HiddenPosition(), false);
            }

            for (int slot = 0; slot < 3; slot++)
            {
                PlayerActor occupant = GetOccupant(slot);
                occupant.SetCourtAssignment(slot, ThreePlayerPosition(slot), true);
                _activeBySlot[slot] = occupant;
            }
        }

        private void RefreshSixPlayerAssignments()
        {
            int liberoSlot = -1;
            PlayerActor replacedMiddle = null;

            for (int slot = 4; slot < 6; slot++)
            {
                PlayerActor occupant = GetOccupant(slot);
                if (occupant.BaseRole == VolleyballRole.MiddleBlocker)
                {
                    liberoSlot = slot;
                    replacedMiddle = occupant;
                    break;
                }
            }

            for (int slot = 0; slot < 6; slot++)
            {
                PlayerActor occupant = GetOccupant(slot);
                bool active = occupant != replacedMiddle;
                occupant.SetCourtAssignment(slot, _slotPositions[slot], active);
                _activeBySlot[slot] = active ? occupant : null;
            }

            if (_libero != null)
            {
                if (liberoSlot >= 0)
                {
                    _libero.SetCourtAssignment(liberoSlot, _slotPositions[liberoSlot], true);
                    _activeBySlot[liberoSlot] = _libero;
                }
                else
                {
                    _libero.SetCourtAssignment(-1, HiddenPosition(), false);
                }
            }
        }

        private PlayerActor GetOccupant(int slot)
        {
            int modulus = _activePlayerCount == 3 ? 3 : 6;
            int rosterIndex = (slot - _rotationOffset + modulus) % modulus;
            return _roster[rosterIndex];
        }

        private Vector3 ThreePlayerPosition(int slot)
        {
            float sign = _team == TeamSide.Left ? -1f : 1f;
            if (slot == 0) return new Vector3(sign * 2.2f, 1f, 0f);
            if (slot == 1) return new Vector3(sign * 5.6f, 1f, -2.25f);
            return new Vector3(sign * 5.6f, 1f, 2.25f);
        }

        private Vector3 HiddenPosition()
        {
            return _team == TeamSide.Left ? new Vector3(-12f, 1f, 0f) : new Vector3(12f, 1f, 0f);
        }
    }
}
