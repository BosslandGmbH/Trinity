using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Util;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines
{
    public class AttackCoroutine : ISubroutine
    {
        private States _state;
        private DiaObject _actor;
        private int _actorId;
        private float _attackRange;
        private SNOPower _attackSkill;

        public AttackCoroutine(int actorId)
        {
            _actorId = actorId;
        }

        #region ISubroutine

        private bool _isDone;
        public bool IsDone { get { return _isDone; } }

        public void Reset()
        {
            _isDone = false;
            _state = States.NotStarted;
            _actor = null;
            _attackSkill = SNOPower.None;
            _attackRange = 10;
        }

        public void DisablePulse()
        {

        }

        #endregion

        #region State

        private enum States
        {
            NotStarted,
            Checking,
            Moving,
            Attacking,
            Completed,
            Failed
        }

        private States State
        {
            get { return _state; }
            set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Logger.Debug("[Attack] " + value);
                }
                _state = value;
            }
        }

        #endregion

        public virtual async Task<bool> GetCoroutine()
        {
            switch (State)
            {
                case States.NotStarted:
                    return await NotStarted();

                case States.Checking:
                    return await Checking();

                case States.Moving:
                    return await Moving();

                case States.Attacking:
                    return await Attacking();

                case States.Completed:
                    return await Completed();

                case States.Failed:
                    return await Failed();
            }
            return false;
        }

        private async Task<bool> NotStarted()
        {
            State = States.Checking;
            return false;
        }

        private async Task<bool> Checking()
        {
            _actor = ActorFinder.FindObject(_actorId);
            _attackSkill = SkillHelper.DefaultWeaponPower;
            _attackRange = SkillHelper.DefaultWeaponDistance;

            if (_actor == null)
            {
                Logger.Info("[AttackCoroutine] Actor not found with id: {0}", _actorId);
                State = States.Completed;
                return false;
            }

            if (_actor.Distance > _attackRange)
            {
                State = States.Moving;
                return false;
            }

            State = States.Attacking;
            return false;
        }


        private async Task<bool> Moving()
        {
            if (!await new MoveToPositionCoroutine(AdvDia.CurrentWorldId, _actor.Position, (int)_attackRange).GetCoroutine())
            {
                Logger.Info("[AttackCoroutine] Moving to actor failed", _actorId);
                State = States.Failed;
                return false;
            }

            State = States.Attacking;
            return false;
        }

        private async Task<bool> Attacking()
        {
            var attempts = 0;
            var attemptLimit = 2;

            while (attempts <= attemptLimit)
            {
                attempts++;

                Logger.Info("[AttackCoroutine] Attacking {0} Dist={1} with skill: {2}", _actor.Name, _actor.Distance, _attackSkill);

                if (ZetaDia.Me.UsePower(_attackSkill, _actor.Position, AdvDia.CurrentWorldDynamicId, _actor.ACDId) ||
                    ZetaDia.Me.UsePower(_attackSkill, _actor.Position, AdvDia.CurrentWorldDynamicId))
                {
                    Logger.Info("[AttackCoroutine] Attack Succeeded! woot!", _actor.Name, _actor.Distance, _attackSkill);
                    break;
                }

                await Coroutine.Sleep(50);
            }

            State = attempts > attemptLimit ? States.Failed : States.Completed;
            return false;
        }

        private async Task<bool> Completed()
        {
            _isDone = true;
            return false;
        }

        private async Task<bool> Failed()
        {
            Logger.Info("[AttackCoroutine] Failed", _actor.Name, _actor.Distance, _attackSkill);

            _isDone = true;
            return false;
        }

    }
}
