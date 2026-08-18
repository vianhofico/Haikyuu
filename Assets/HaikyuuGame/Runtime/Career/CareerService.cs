using HaikyuuGame.Persistence;

namespace HaikyuuGame.Career
{
    public enum CareerStat
    {
        Attack,
        Serve,
        Set,
        Receive,
        Block,
        Jump,
        Speed
    }

    public sealed class CareerService
    {
        private readonly CareerSaveData _career;

        public CareerService(CareerSaveData career)
        {
            _career = career;
        }

        public bool Train(CareerStat stat, int cost = 1)
        {
            if (_career.trainingPoints < cost)
            {
                return false;
            }

            _career.trainingPoints -= cost;
            switch (stat)
            {
                case CareerStat.Attack: _career.attack = Clamp(_career.attack + 1); break;
                case CareerStat.Serve: _career.serve = Clamp(_career.serve + 1); break;
                case CareerStat.Set: _career.set = Clamp(_career.set + 1); break;
                case CareerStat.Receive: _career.receive = Clamp(_career.receive + 1); break;
                case CareerStat.Block: _career.block = Clamp(_career.block + 1); break;
                case CareerStat.Jump: _career.jump = Clamp(_career.jump + 1); break;
                case CareerStat.Speed: _career.speed = Clamp(_career.speed + 1); break;
            }

            return true;
        }

        public void AdvanceWeek()
        {
            _career.week++;
            _career.trainingPoints += 3;
            if (_career.week > 12)
            {
                _career.week = 1;
                _career.season++;
            }
        }

        private static int Clamp(int value)
        {
            if (value < 1) return 1;
            return value > 100 ? 100 : value;
        }
    }
}
