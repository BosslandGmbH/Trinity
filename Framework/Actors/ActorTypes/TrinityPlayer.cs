using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Actors.Properties;
using Trinity.Technicals;
using Zeta.Game;

namespace Trinity.Framework.Actors.ActorTypes
{
    public class TrinityPlayer : TrinityActor
    {
        public override ActorAttributes Attributes { get; set; }

        public ActorClass ActorClass { get; set; }

        public override void OnCreated()
        {
            Attributes = new PlayerAttributes(FastAttributeGroupId);
            base.Attributes = Attributes;
            UpdateProperties();
        }

        public override void OnUpdated()
        {
            Attributes.Update();          
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            CommonProperties.Populate(this);
            UnitProperties.Populate(this);
            PlayerProperties.Populate(this);
        }

        public override string ToString() => $"{GetType().Name}: AcdId={AcdId}, {ActorClass} {(IsMe ? "ActivePlayer" : string.Empty)}";

    }

}

