using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Scripting.Utils;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Modules;

namespace Trinity.Framework.Objects
{
    public enum ActorWeightType
    {
        None,
        Shrine,
        Trash,
        Elite,
        Boss,
        Goblin,
        HealthGlobe
    }

    public class WeightSettingsHelper
    {
        public static List<WeightSettings> GetDefaults()
        {
            var types = ActorWeightType.None.ToList<ActorWeightType>(true);
            var defaults = types.Select(t => new WeightSettings(t)).ToList();
            return defaults;
        }

        public static ActorWeightType GetWeightType(TrinityActor actor)
        {
            switch (actor.Type)
            {
                case TrinityObjectType.Shrine:
                    return ActorWeightType.Shrine;
                case TrinityObjectType.HealthGlobe:
                    return ActorWeightType.HealthGlobe;
                case TrinityObjectType.Unit:
                    if (actor.IsElite)
                        return ActorWeightType.Elite;
                    if (actor.IsBoss)
                        return ActorWeightType.Boss;
                    if (actor.IsTreasureGoblin)
                        return ActorWeightType.Goblin;
                    break;
            }
            return default(ActorWeightType);
        }
    }

    [DataContract(Namespace = "")]
    public sealed class WeightSettings : NotifyBase
    {
        private string _formula;
        private Func<float> _compiledFormula;
        private int _order;
        private string _name;
        private bool _isEnabled;
        private ActorWeightType _actorWeightType;
        private DataRetriever _retriever;
        public string _finalFormula;

        public WeightSettings()
        {
            LoadDefaults();
        }

        public WeightSettings(ActorWeightType type)
        {
            LoadDefaults();
            ActorWeightType = type;
            Name = type.ToString();
        }

        [DataMember(EmitDefaultValue = false)]
        public ActorWeightType ActorWeightType
        {
            get { return _actorWeightType; }
            set { SetField(ref _actorWeightType, value); }
        }

        [DataMember(EmitDefaultValue = false)]
        public int Order
        {
            get { return _order; }
            set { SetField(ref _order, value); }
        }

        [DataMember(EmitDefaultValue = false)]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetField(ref _isEnabled, value); }
        }

        [DataMember(EmitDefaultValue = false)]
        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }

        [DataMember(EmitDefaultValue = false)]
        public string Formula
        {
            get { return _formula; }
            set { SetField(ref _formula, value); }
        }

        [IgnoreDataMember]
        public TrinityActor Actor { get; set; }

        [IgnoreDataMember]
        public double Base { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public TrinityObjectType ObjectType { get; set; }

        public void Compile()
        {
            var compiler = new CompiledExpression<float>();
            _retriever = new DataRetriever();
            compiler.RegisterType("Data", _retriever);
            _finalFormula = FormatFormula(Formula);
            compiler.StringToParse = _finalFormula;
            _compiledFormula = compiler.Compile();
        }

        private string FormatFormula(string formula)
        {
            var result = formula.Replace("Base", "Data.Base");
            result = result.Replace("Actor", "Data.Actor");
            result = result.Replace("Player", "Data.Player");
            return result;
        }

        public class DataRetriever
        {
            public double Base { get; set; }
            public TrinityActor Actor { get; set; }
            public PlayerCache Player { get; set; }
        }

        public double Eval(TrinityActor actor, double currentWeight)
        {
            if (string.IsNullOrEmpty(Formula))
                return 0;

            if (_compiledFormula == null)
                Compile();

            _retriever.Base = currentWeight;
            _retriever.Actor = actor;
            _retriever.Player = Core.Player;
            return _compiledFormula();
        }
    }

}

