using System;
using Buddy.Coroutines;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Framework;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines
{
    public class AttackCoroutine : ISubroutine
    {
        private States _state;
        private DiaObject _actor;
        private readonly SNOActor _actorId;
        private float _attackRange;
        private SNOPower _attackSkill;

        public AttackCoroutine(SNOActor actorId)
        {
            _actorId = actorId;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        #region ISubroutine

        private bool _isDone;
        public bool IsDone => _isDone;

        public void Reset()
        {
            _isDone = false;
            _state = States.NotStarted;
            _actor = null;
            _attackSkill = SNOPower.None;
            _attackRange = 10;
        }

        public string StatusText { get; set; }

        public void DisablePulse()
        {
        }

        #endregion ISubroutine

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
            get => _state;
            set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Core.Logger.Debug("[Attack] " + value);
                    StatusText = "[Attack] " + value;
                }
                _state = value;
            }
        }

        #endregion State

        public virtual async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

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
                Core.Logger.Log("[AttackCoroutine] Actor not found with id: {0}", _actorId);
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
                Core.Logger.Log("[AttackCoroutine] Moving to actor failed", _actorId);
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

                Core.Logger.Log("[AttackCoroutine] Attacking {0} Dist={1} with skill: {2}", _actor.Name, _actor.Distance, _attackSkill);

                if (ZetaDia.Me.UsePower(_attackSkill, _actor.Position, AdvDia.CurrentWorldDynamicId, _actor.ACDId) ||
                    ZetaDia.Me.UsePower(_attackSkill, _actor.Position, AdvDia.CurrentWorldDynamicId))
                {
                    Core.Logger.Log("[AttackCoroutine] Attack Succeeded! woot!", _actor.Name, _actor.Distance, _attackSkill);
                    break;
                }

                await Coroutine.Yield();
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
            Core.Logger.Log("[AttackCoroutine] Failed", _actor.Name, _actor.Distance, _attackSkill);

            _isDone = true;
            return false;
        }
    }
}
