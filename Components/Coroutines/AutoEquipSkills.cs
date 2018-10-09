using System;
using Trinity.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Game;

namespace Trinity.Components.Coroutines
{
    /// <summary>
    /// Handles changing skills while levelling to 70
    /// </summary>
    public class AutoEquipSkills
    {
        private int _lastLevel;

        private static AutoEquipSkills _instance;
        public static AutoEquipSkills Instance => _instance ?? (_instance = (new AutoEquipSkills()));

        private DateTime _lastSkillChange = DateTime.MinValue;

        public async Task<bool> Execute()
        {
            if (!Core.Settings.Items.AutoEquipSkills)
                return false;

            if (!Core.Player.IsValid || ZetaDia.Me == null || !ZetaDia.IsInGame || !ZetaDia.Me.IsValid || ZetaDia.Me.CommonData == null || !ZetaDia.Me.CommonData.IsValid || ZetaDia.Me.CommonData.IsDisposed)
                return false;

            var level = ZetaDia.Me.Level;
            if (ZetaDia.Me.Level == 70 || level <= 0)
                return false;

            if (DateTime.UtcNow.Subtract(_lastSkillChange).TotalMinutes < 1)
                return false;

            _lastSkillChange = DateTime.UtcNow;

            //if (DateTime.UtcNow.Subtract(Settings.TrinityPlugin.BotStartTime).TotalSeconds < 10)
            //    return false;
       
            if (_lastLevel == level)
                return false;

            if (await Equip())
            {
                _lastLevel = level;
                return false;
            }

            return false;
        }

        private Build _lastBuild;

        private async Task<bool> Equip()
        {           
            var builds = GetBuildsForClass(Core.Player.ActorClass);
            var build = builds?.OrderByDescending(b => b.Level)?.FirstOrDefault(b => b.Level <= Core.Player.Level);
            if (build == null)
                return false;

            if (_lastBuild != null && build == _lastBuild && build.IsEquipped())
                return false;

            if (await EquipBehavior.Instance.Execute(build.Skills, build.Passives))
            {
                _lastBuild = build;
                return true;
            }

            return false;
        }

        public List<Build> GetBuildsForClass(ActorClass actorClass)
        {
            switch (actorClass)
            {
                case ActorClass.Witchdoctor:
                    return WitchDoctorLevelingBuilds;
                case ActorClass.Crusader:
                    return CrusaderLevelingBuilds;
                case ActorClass.DemonHunter:
                    return DemonHunterLevelingBuilds;
                case ActorClass.Wizard:
                    return WizardLevelingBuilds;
                case ActorClass.Barbarian:
                    return BarbarianLevelingBuilds;
                case ActorClass.Monk:
                    return MonkLevelingBuilds;
                case ActorClass.Necromancer:
                    // TODO: Handle Necromancer here!!!
                    break;
            }

            return null;
        }

        public List<Build> CrusaderLevelingBuilds = new List<Build>
        {
			//// Level = 1 (L) Punish (Auto)
			new Build {
                Level = 1,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Punish, Runes.Crusader.None }
                },
            },
			//// Level = 2 (R) Shield Bash (Auto)
			new Build {
                Level = 2,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Punish, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.None }
                },
            },
			//// Level = 3 (L) Slash
			//// Level = 3 (1) Shield Glare (Auto)
			new Build {
                Level = 3,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.None }
                },
            },
			//// Level = 4 (1) Shield Glare
			new Build {
                Level = 4,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.None }
                },
            },			
			//// Level = 5 (R) Sweep Attack
			new Build {
                Level = 5,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.None },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.None }
                },
            },
			///// Level = 8	(1)	Iron Skin
            new Build {
                Level = 8,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.None },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.None },
                    {  Skills.Crusader.IronSkin, Runes.Crusader.None }
                },
            },
			//// Level = 9 (L) Slash - Electrify
			//// Level = 9 (2) Provoke (Auto)
			new Build {
                Level = 9,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.None },
					{  Skills.Crusader.Provoke, Runes.Crusader.None }
                },
            },
			//// Level = 10 (P1) Add Passive (Any)
            new Build {
                Level = 10,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.None },
					{  Skills.Crusader.Provoke, Runes.Crusader.None }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness
                }
            },
			//// Level = 11 (R) Sweep Attack - Blazing Sweep
            new Build {
                Level = 11,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.BlazingSweep },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.None },
					{  Skills.Crusader.Provoke, Runes.Crusader.None }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness
                }
            },
			//// Level = 12 (1) Shield Glare - Divine Verdict
            new Build {
                Level = 12,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.BlazingSweep },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.DivineVerdict },
					{  Skills.Crusader.Provoke, Runes.Crusader.None }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness
                }
            },
			//// Level = 13 (2) Steed Charge
            new Build {
                Level = 13,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.BlazingSweep },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.DivineVerdict },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.None }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness
                }
            },
			//// Level = 14 (L) Smite - Shatter
			//// Level = 14 (3) Laws of Valor (Auto)
            new Build {
                Level = 14,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Shatter },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.BlazingSweep },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.DivineVerdict },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.None },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.None }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness
                }
            },
			//// Level = 16 (P1) Righteousness
             new Build {
                Level = 16,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Shatter },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.BlazingSweep },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.DivineVerdict },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.None },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.None }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness
                }
            },
			//// Level = 19 (2) Steed Charge - Spiked Barding
			//// Level = 19 (4) Falling Sword (Auto)
             new Build {
                Level = 19,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Shatter },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.BlazingSweep },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.DivineVerdict },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.SpikedBarding },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.None },
					{  Skills.Crusader.FallingSword, Runes.Crusader.None }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness
                }
            },
			//// Level = 20 (P2) Fanaticism
             new Build {
                Level = 20,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Shatter },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.BlazingSweep },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.DivineVerdict },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.SpikedBarding },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.None },
					{  Skills.Crusader.FallingSword, Runes.Crusader.None }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness,
					Passives.Crusader.Fanaticism
                }
            },
			//// Level = 21 (1) Condemn
			//// Level = 21 (3) Laws of Valor - Invincible
             new Build {
                Level = 21,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Shatter },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.BlazingSweep },
                    {  Skills.Crusader.Condemn, Runes.Crusader.None },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.SpikedBarding },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.Invincible },
					{  Skills.Crusader.FallingSword, Runes.Crusader.None }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness,
					Passives.Crusader.Fanaticism
                }
            },
			//// Level = 24 (4) Falling Sword - Superheated
             new Build {
                Level = 24,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Shatter },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.BlazingSweep },
                    {  Skills.Crusader.Condemn, Runes.Crusader.None },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.SpikedBarding },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.Invincible },
					{  Skills.Crusader.FallingSword, Runes.Crusader.Superheated }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness,
					Passives.Crusader.Fanaticism
                }
            },
			//// Level = 25 (2) Steed Charge - Nightmare
             new Build {
                Level = 25,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Shatter },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.BlazingSweep },
                    {  Skills.Crusader.Condemn, Runes.Crusader.None },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.Nightmare },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.Invincible },
					{  Skills.Crusader.FallingSword, Runes.Crusader.Superheated }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness,
					Passives.Crusader.Fanaticism
                }
            },
			//// Level = 26 (1) Condemn - Vacuum
             new Build {
                Level = 26,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Shatter },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.BlazingSweep },
                    {  Skills.Crusader.Condemn, Runes.Crusader.Vacuum },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.Nightmare },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.Invincible },
					{  Skills.Crusader.FallingSword, Runes.Crusader.Superheated }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness,
					Passives.Crusader.Fanaticism
                }
            },
			//// Level = 27 (P2) Holy Cause
             new Build {
                Level = 27,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Shatter },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.BlazingSweep },
                    {  Skills.Crusader.Condemn, Runes.Crusader.Vacuum },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.Nightmare },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.Invincible },
					{  Skills.Crusader.FallingSword, Runes.Crusader.Superheated }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness,
					Passives.Crusader.HolyCause
                }
            },
			//// Level = 30 (P3) Wrathful
             new Build {
                Level = 30,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Shatter },
                    {  Skills.Crusader.SweepAttack, Runes.Crusader.BlazingSweep },
                    {  Skills.Crusader.Condemn, Runes.Crusader.Vacuum },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.Nightmare },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.Invincible },
					{  Skills.Crusader.FallingSword, Runes.Crusader.Superheated }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness,
					Passives.Crusader.HolyCause,
					Passives.Crusader.Wrathful
                }
            },
			//// Level = 32 (R) Fist of Heaven - Divine Well
             new Build {
                Level = 32,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Shatter },
                    {  Skills.Crusader.FistOfTheHeavens, Runes.Crusader.DivineWell },
                    {  Skills.Crusader.Condemn, Runes.Crusader.Vacuum },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.Nightmare },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.Invincible },
					{  Skills.Crusader.FallingSword, Runes.Crusader.Superheated }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness,
					Passives.Crusader.HolyCause,
					Passives.Crusader.Wrathful
                }
            },
			//// Level = 33 (1) Condemn - Unleashed
             new Build {
                Level = 33,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Shatter },
                    {  Skills.Crusader.FistOfTheHeavens, Runes.Crusader.DivineWell },
                    {  Skills.Crusader.Condemn, Runes.Crusader.Unleashed },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.Nightmare },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.Invincible },
					{  Skills.Crusader.FallingSword, Runes.Crusader.Superheated }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness,
					Passives.Crusader.HolyCause,
					Passives.Crusader.Wrathful
                }
            },
			//// Level = 37 (L) Smite - Surge
             new Build {
                Level = 37,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Surge },
                    {  Skills.Crusader.FistOfTheHeavens, Runes.Crusader.DivineWell },
                    {  Skills.Crusader.Condemn, Runes.Crusader.Unleashed },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.Nightmare },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.Invincible },
					{  Skills.Crusader.FallingSword, Runes.Crusader.Superheated }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness,
					Passives.Crusader.HolyCause,
					Passives.Crusader.Wrathful
                }
            },
			//// Level = 38 (4) Falling Sword - Rise Brothers
             new Build {
                Level = 38,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Surge },
                    {  Skills.Crusader.FistOfTheHeavens, Runes.Crusader.DivineWell },
                    {  Skills.Crusader.Condemn, Runes.Crusader.Unleashed },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.Nightmare },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.Invincible },
					{  Skills.Crusader.FallingSword, Runes.Crusader.RiseBrothers }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness,
					Passives.Crusader.HolyCause,
					Passives.Crusader.Wrathful
                }
            },
			//// Level = 42 (R) Fist of Heaven - Fissure
             new Build {
                Level = 42,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Surge },
                    {  Skills.Crusader.FistOfTheHeavens, Runes.Crusader.Fissure },
                    {  Skills.Crusader.Condemn, Runes.Crusader.Unleashed },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.Nightmare },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.Invincible },
					{  Skills.Crusader.FallingSword, Runes.Crusader.RiseBrothers }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness,
					Passives.Crusader.HolyCause,
					Passives.Crusader.Wrathful
                }
            },
			//// Level = 45 (3) Laws of Valor - Unstoppable Force
             new Build {
                Level = 45,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Smite, Runes.Crusader.Surge },
                    {  Skills.Crusader.FistOfTheHeavens, Runes.Crusader.Fissure },
                    {  Skills.Crusader.Condemn, Runes.Crusader.Unleashed },
					{  Skills.Crusader.SteedCharge, Runes.Crusader.Nightmare },
					{  Skills.Crusader.LawsOfValor, Runes.Crusader.UnstoppableForce },
					{  Skills.Crusader.FallingSword, Runes.Crusader.RiseBrothers }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.Righteousness,
					Passives.Crusader.HolyCause,
					Passives.Crusader.Wrathful
                }
            },
			//// Level = 70 (L) Punish - Celerity
			//// Level = 70 (R) Consecration - Bed of Nails
			//// Level = 70 (1) Steed Charge - Spiked Barding
			//// Level = 70 (2) Justice - Immovable Object
			//// Level = 70 (3) Iron Skin - Reflective Skin
			//// Level = 70 (4) Bombardment - Barrels of Spikes
			//// Level = 70 (P1) Iron Maiden
			//// Level = 70 (P2) Finery
			//// Level = 70 (P3) Fervor 	
			//// Level = 70 (P4) Hold your Ground
             new Build {
                Level = 70,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Punish, Runes.Crusader.Celerity },
                    {  Skills.Crusader.Consecration, Runes.Crusader.BedOfNails },
                    {  Skills.Crusader.SteedCharge, Runes.Crusader.SpikedBarding },
					{  Skills.Crusader.Justice, Runes.Crusader.ImmovableObject },
					{  Skills.Crusader.IronSkin, Runes.Crusader.ReflectiveSkin },
					{  Skills.Crusader.Bombardment, Runes.Crusader.BarrelsOfSpikes }
                },
				Passives = new List<Passive>
                {
                    Passives.Crusader.IronMaiden,
					Passives.Crusader.Finery,
					Passives.Crusader.Fervor,
					Passives.Crusader.HoldYourGround
                }
            },
        };

        public List<Build> DemonHunterLevelingBuilds = new List<Build>
        {
			//// Level = 1 (L) Hungering Arrow
			new Build {
                Level = 1,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.None }
                },
            },
			//// Level = 2 (R) Impale
			new Build {
                Level = 2,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.None },
					{  Skills.DemonHunter.Impale, Runes.DemonHunter.None }
                },
            },
			//// Level = 4 (1) Caltrops
            new Build {
                Level = 4,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.None },
					{  Skills.DemonHunter.Impale, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.None }
                },
            },
			//// Level = 5 (R) Rapid Fire
            new Build {
                Level = 5,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.None },
					{  Skills.DemonHunter.RapidFire, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.None }
                },
            },
			//// Level = 6 (L) Hungering Arrow - Puncturing Arrow
            new Build {
                Level = 6,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.PuncturingArrow },
					{  Skills.DemonHunter.RapidFire, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.None }
                },
            },
			//// Level = 9 (2) Vault
            new Build {
                Level = 9,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.PuncturingArrow },
					{  Skills.DemonHunter.RapidFire, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.None }
                },
            },
			//// Level = 10 (P1) Tactical Advantage
            new Build {
                Level = 10,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.PuncturingArrow },
					{  Skills.DemonHunter.RapidFire, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.TacticalAdvantage
                }
            },
			//// Level = 11 (R) Rapid Fire - Withering Fire
            new Build {
                Level = 11,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.PuncturingArrow },
					{  Skills.DemonHunter.RapidFire, Runes.DemonHunter.WitheringFire },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.TacticalAdvantage
                }
            },
			//// Level = 12 (1) Caltrops - Hooked Spines
            new Build {
                Level = 12,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.PuncturingArrow },
					{  Skills.DemonHunter.RapidFire, Runes.DemonHunter.WitheringFire },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.HookedSpines },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.TacticalAdvantage
                }
            },
			//// Level = 14 (3) Fan of Knives
            new Build {
                Level = 14,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.PuncturingArrow },
					{  Skills.DemonHunter.RapidFire, Runes.DemonHunter.WitheringFire },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.HookedSpines },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.TacticalAdvantage
                }
            },
			//// Level = 16 (2) Vault - Action Shot
            new Build {
                Level = 16,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.PuncturingArrow },
					{  Skills.DemonHunter.RapidFire, Runes.DemonHunter.WitheringFire },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.HookedSpines },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.ActionShot },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.TacticalAdvantage
                }
            },
			//// Level = 17 (L) Hungering Arrow - Serrated Arrow
			//// Level = 17 (1) Companion
            new Build {
                Level = 17,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.SerratedArrow },
					{  Skills.DemonHunter.RapidFire, Runes.DemonHunter.WitheringFire },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.ActionShot },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.TacticalAdvantage
                }
            },
			//// Level = 19 (4) Strafe
            new Build {
                Level = 19,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.SerratedArrow },
					{  Skills.DemonHunter.RapidFire, Runes.DemonHunter.WitheringFire },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.ActionShot },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.None },
					{  Skills.DemonHunter.Strafe, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.TacticalAdvantage
                }
            },
			//// Level = 20 (P1) Thrill of the Hunt
			//// Level = 20 (P2) Cull the Weak
            new Build {
                Level = 20,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.SerratedArrow },
					{  Skills.DemonHunter.RapidFire, Runes.DemonHunter.WitheringFire },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.ActionShot },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.None },
					{  Skills.DemonHunter.Strafe, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.ThrillOfTheHunt,
                    Passives.DemonHunter.CullTheWeak
                }
            },
			//// Level = 22 (R) Multishot
            new Build {
                Level = 22,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.SerratedArrow },
					{  Skills.DemonHunter.Multishot, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.ActionShot },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.None },
					{  Skills.DemonHunter.Strafe, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.ThrillOfTheHunt,
                    Passives.DemonHunter.CullTheWeak
                }
            },
			//// Level = 23 (3) Fan of Knives - Pinpoint Accuracy
            new Build {
                Level = 23,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.SerratedArrow },
					{  Skills.DemonHunter.Multishot, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.ActionShot },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.PinpointAccuracy },
					{  Skills.DemonHunter.Strafe, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.ThrillOfTheHunt,
                    Passives.DemonHunter.CullTheWeak
                }
            },
			//// Level = 25 (4) Preparation - Punishment
            new Build {
                Level = 25,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.SerratedArrow },
					{  Skills.DemonHunter.Multishot, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.ActionShot },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.PinpointAccuracy },
					{  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.ThrillOfTheHunt,
                    Passives.DemonHunter.CullTheWeak
                }
            },
			//// Level = 26 (R) Multishot - Fire at Will
            new Build {
                Level = 26,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.SerratedArrow },
					{  Skills.DemonHunter.Multishot, Runes.DemonHunter.FireAtWill },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.ActionShot },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.PinpointAccuracy },
					{  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.ThrillOfTheHunt,
                    Passives.DemonHunter.CullTheWeak
                }
            },
			//// Level = 28 (3) Sentry - Spitfire Turret
            new Build {
                Level = 28,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.SerratedArrow },
					{  Skills.DemonHunter.Multishot, Runes.DemonHunter.FireAtWill },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.ActionShot },
                    {  Skills.DemonHunter.Sentry, Runes.DemonHunter.SpitfireTurret },
					{  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.ThrillOfTheHunt,
                    Passives.DemonHunter.CullTheWeak
                }
            },
			//// Level = 29 (1) Companion - Bat
            new Build {
                Level = 29,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.SerratedArrow },
					{  Skills.DemonHunter.Multishot, Runes.DemonHunter.FireAtWill },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.BatCompanion },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.ActionShot },
                    {  Skills.DemonHunter.Sentry, Runes.DemonHunter.SpitfireTurret },
					{  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.ThrillOfTheHunt,
                    Passives.DemonHunter.CullTheWeak
                }
            },
			//// Level = 30 (P3) Blood Vengeance
            new Build {
                Level = 30,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.SerratedArrow },
					{  Skills.DemonHunter.Multishot, Runes.DemonHunter.FireAtWill },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.BatCompanion },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.ActionShot },
                    {  Skills.DemonHunter.Sentry, Runes.DemonHunter.SpitfireTurret },
					{  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.ThrillOfTheHunt,
                    Passives.DemonHunter.CullTheWeak,
					Passives.DemonHunter.BloodVengeance
                }
            },
			//// Level = 33 (2) Vault - Tumble
            new Build {
                Level = 33,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.HungeringArrow, Runes.DemonHunter.SerratedArrow },
					{  Skills.DemonHunter.Multishot, Runes.DemonHunter.FireAtWill },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.BatCompanion },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.Tumble },
                    {  Skills.DemonHunter.Sentry, Runes.DemonHunter.SpitfireTurret },
					{  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.ThrillOfTheHunt,
                    Passives.DemonHunter.CullTheWeak,
					Passives.DemonHunter.BloodVengeance
                }
            },
			//// Level = 42 (L) Evasive Fire - Focus
            new Build {
                Level = 42,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.Focus },
					{  Skills.DemonHunter.Multishot, Runes.DemonHunter.FireAtWill },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.BatCompanion },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.Tumble },
                    {  Skills.DemonHunter.Sentry, Runes.DemonHunter.SpitfireTurret },
					{  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.ThrillOfTheHunt,
                    Passives.DemonHunter.CullTheWeak,
					Passives.DemonHunter.BloodVengeance
                }
            },
			//// Level = 55 (R) Multishot - Arsenal
            new Build {
                Level = 55,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.Focus },
					{  Skills.DemonHunter.Multishot, Runes.DemonHunter.Arsenal },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.BatCompanion },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.Tumble },
                    {  Skills.DemonHunter.Sentry, Runes.DemonHunter.SpitfireTurret },
					{  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.ThrillOfTheHunt,
                    Passives.DemonHunter.CullTheWeak,
					Passives.DemonHunter.BloodVengeance
                }
            },
			//// Level = 67 (1) Vengeance - Seethe
            new Build {
                Level = 67,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.Focus },
					{  Skills.DemonHunter.Multishot, Runes.DemonHunter.Arsenal },
                    {  Skills.DemonHunter.Vengeance, Runes.DemonHunter.Seethe },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.Tumble },
                    {  Skills.DemonHunter.Sentry, Runes.DemonHunter.SpitfireTurret },
					{  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.ThrillOfTheHunt,
                    Passives.DemonHunter.CullTheWeak,
					Passives.DemonHunter.BloodVengeance
                }
            },
			//// Level = 70 (L) Impale - Ricochet
			//// Level = 70 (R) Grenade - Tinkerer
			//// Level = 70 (1) Marked for Death - Grim Reaper
			//// Level = 70 (2) Vault - Tumble
			//// Level = 70 (3) Preparation - Punishment
			//// Level = 70 (4) Shadow Power - Any
			//// Level = 70 (P1) Ambush
			//// Level = 70 (P2) Night Stalker
			//// Level = 70 (P3) Perfectionist
			//// Level = 70 (P4) Blood Vengeance (open)
            new Build {
                Level = 70,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.Impale, Runes.DemonHunter.Ricochet },
					{  Skills.DemonHunter.Grenade, Runes.DemonHunter.Tinkerer },
                    {  Skills.DemonHunter.MarkedForDeath, Runes.DemonHunter.GrimReaper },
                    {  Skills.DemonHunter.Vault, Runes.DemonHunter.Tumble },
                    {  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment },
					{  Skills.DemonHunter.ShadowPower, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.Ambush,
                    Passives.DemonHunter.NightStalker,
					Passives.DemonHunter.Perfectionist,
					Passives.DemonHunter.BloodVengeance
                }
            },
        };

        public List<Build> WizardLevelingBuilds = new List<Build>
        {
			//// Level = 1 (L) Magic Missile (auto)
			new Build {
                Level = 1,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.None }
                },
            },
			//// Level = 2 (R) Ray of Frost (auto)
            new Build {
                Level = 2,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.None },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.None }
                },
            },
			//// Level = 4 (1) Frost Nova (auto)
			new Build {
                Level = 4,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.None },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.None },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.None }
                },
            },
			//// Level = 6 (L) Magic Missile - Charged Blast
            new Build {
                Level = 6,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.None },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.None }
                },
            },
			//// Level = 7 (R) Ray of Frost - Cold Blood
            new Build {
                Level = 7,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.ColdBlood },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.None }
                },
            },
			//// Level = 8 (1) Diamond Skin
            new Build {
                Level = 8,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.ColdBlood },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.None }
                },
            },
			//// Level = 9 (2) Wave of Force (auto)
            new Build {
                Level = 9,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.ColdBlood },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.None },
					{  Skills.Wizard.WaveOfForce, Runes.Wizard.None }
                },
            },
			//// Level = 10 (P1) Blur
            new Build {
                Level = 10,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.ColdBlood },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.None },
					{  Skills.Wizard.WaveOfForce, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur
                }
            },
			//// Level = 14 (1) Diamond Skin - Crystal Shell
			//// Level = 14	(3)	Ice Armor (auto)
            new Build {
                Level = 14,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.ColdBlood },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.CrystalShell },
					{  Skills.Wizard.WaveOfForce, Runes.Wizard.None },
					{  Skills.Wizard.IceArmor, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur
                }
            },
			//// Level = 16 (P1) Blur / Glass Cannon
            new Build {
                Level = 16,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.ColdBlood },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.CrystalShell },
					{  Skills.Wizard.WaveOfForce, Runes.Wizard.None },
					{  Skills.Wizard.IceArmor, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur
                }
            },
			//// Level = 17 (3) Storm Armor
            new Build {
                Level = 17,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.ColdBlood },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.CrystalShell },
					{  Skills.Wizard.WaveOfForce, Runes.Wizard.None },
					{  Skills.Wizard.StormArmor, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur
                }
            },
			//// Level = 19 (4) Explosive Blast
            new Build {
                Level = 19,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.ColdBlood },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.CrystalShell },
					{  Skills.Wizard.WaveOfForce, Runes.Wizard.None },
					{  Skills.Wizard.StormArmor, Runes.Wizard.None },
					{  Skills.Wizard.ExplosiveBlast, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur
                }
            },
			//// Level = 20	(4)	Magic Weapon
			//// Level = 20 (P1) Blur
			//// Level = 20 (P2) Glass Cannon
            new Build {
                Level = 20,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.ColdBlood },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.CrystalShell },
					{  Skills.Wizard.WaveOfForce, Runes.Wizard.None },
					{  Skills.Wizard.StormArmor, Runes.Wizard.None },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.GlassCannon
                }
            },
			//// Level = 21 (R) Disintegrate
            new Build {
                Level = 21,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.None },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.CrystalShell },
					{  Skills.Wizard.WaveOfForce, Runes.Wizard.None },
					{  Skills.Wizard.StormArmor, Runes.Wizard.None },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.GlassCannon
                }
            },
			//// Level = 22 (1) Teleport
			//// Level = 22 (2) Familiar
            new Build {
                Level = 22,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.None },
                    {  Skills.Wizard.Teleport, Runes.Wizard.None },
					{  Skills.Wizard.Familiar, Runes.Wizard.None },
					{  Skills.Wizard.StormArmor, Runes.Wizard.None },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.GlassCannon
                }
            },
			//// Level = 23 (3) Storm Armor - Reactive Armor
            new Build {
                Level = 23,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.None },
                    {  Skills.Wizard.Teleport, Runes.Wizard.None },
					{  Skills.Wizard.Familiar, Runes.Wizard.None },
					{  Skills.Wizard.StormArmor, Runes.Wizard.ReactiveArmor },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.GlassCannon
                }
            },
			//// Level = 24 (L) Spectral Blade - Siphoning Blade
			//// Level = 24 (P1) Blur / Glass
			//// Level = 24 (P2) Astral Presence
            new Build {
                Level = 24,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.SpectralBlade, Runes.Wizard.SiphoningBlade },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.None },
                    {  Skills.Wizard.Teleport, Runes.Wizard.None },
					{  Skills.Wizard.Familiar, Runes.Wizard.None },
					{  Skills.Wizard.StormArmor, Runes.Wizard.ReactiveArmor },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.AstralPresence
                }
            },
			//// Level = 26 (1) Teleport - Safe Passage
            new Build {
                Level = 26,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.SpectralBlade, Runes.Wizard.SiphoningBlade },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.None },
                    {  Skills.Wizard.Teleport, Runes.Wizard.SafePassage },
					{  Skills.Wizard.Familiar, Runes.Wizard.None },
					{  Skills.Wizard.StormArmor, Runes.Wizard.ReactiveArmor },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.AstralPresence
                }
            },
			//// Level = 27 (4) Magic Weapon - Electrify
            new Build {
                Level = 27,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.SpectralBlade, Runes.Wizard.SiphoningBlade },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.None },
                    {  Skills.Wizard.Teleport, Runes.Wizard.SafePassage },
					{  Skills.Wizard.Familiar, Runes.Wizard.None },
					{  Skills.Wizard.StormArmor, Runes.Wizard.ReactiveArmor },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.AstralPresence
                }
            },
			//// Level = 30 (R) Disintegrate - Volatility
			//// Level = 30 (2) Familiar - Sparkflint
			//// Level = 30 (P1) Blur
			//// Level = 30 (P3) Glass Cannon
            new Build {
                Level = 30,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.SpectralBlade, Runes.Wizard.SiphoningBlade },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.Volatility },
                    {  Skills.Wizard.Teleport, Runes.Wizard.SafePassage },
					{  Skills.Wizard.Familiar, Runes.Wizard.Sparkflint },
					{  Skills.Wizard.StormArmor, Runes.Wizard.ReactiveArmor },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.AstralPresence,
					Passives.Wizard.GlassCannon
                }
            },
			//// Level = 31 (1) Teleport - Wormhole
            new Build {
                Level = 31,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.SpectralBlade, Runes.Wizard.SiphoningBlade },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.Volatility },
                    {  Skills.Wizard.Teleport, Runes.Wizard.Wormhole },
					{  Skills.Wizard.Familiar, Runes.Wizard.Sparkflint },
					{  Skills.Wizard.StormArmor, Runes.Wizard.ReactiveArmor },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.AstralPresence,
					Passives.Wizard.GlassCannon
                }
            },
			//// Level = 33 (3) Storm Armor - Power of the Storm
            new Build {
                Level = 33,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.SpectralBlade, Runes.Wizard.SiphoningBlade },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.Volatility },
                    {  Skills.Wizard.Teleport, Runes.Wizard.Wormhole },
					{  Skills.Wizard.Familiar, Runes.Wizard.Sparkflint },
					{  Skills.Wizard.StormArmor, Runes.Wizard.PowerOfTheStorm },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.AstralPresence,
					Passives.Wizard.GlassCannon
                }
            },
			//// Level = 34 (L) Hydra - Lightning Hydra
            new Build {
                Level = 34,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Hydra, Runes.Wizard.LightningHydra },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.Volatility },
                    {  Skills.Wizard.Teleport, Runes.Wizard.Wormhole },
					{  Skills.Wizard.Familiar, Runes.Wizard.Sparkflint },
					{  Skills.Wizard.StormArmor, Runes.Wizard.PowerOfTheStorm },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.AstralPresence,
					Passives.Wizard.GlassCannon
                }
            },
			//// Level = 35 (4) Magic Weapon - Force Weapon
            new Build {
                Level = 35,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Hydra, Runes.Wizard.LightningHydra },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.Volatility },
                    {  Skills.Wizard.Teleport, Runes.Wizard.Wormhole },
					{  Skills.Wizard.Familiar, Runes.Wizard.Sparkflint },
					{  Skills.Wizard.StormArmor, Runes.Wizard.PowerOfTheStorm },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.ForceWeapon }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.AstralPresence,
					Passives.Wizard.GlassCannon
                }
            },
			//// Level = 48 (R) Disintergrate - Chaos Nexus
            new Build {
                Level = 48,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Hydra, Runes.Wizard.LightningHydra },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.ChaosNexus },
                    {  Skills.Wizard.Teleport, Runes.Wizard.Wormhole },
					{  Skills.Wizard.Familiar, Runes.Wizard.Sparkflint },
					{  Skills.Wizard.StormArmor, Runes.Wizard.PowerOfTheStorm },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.ForceWeapon }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.AstralPresence,
					Passives.Wizard.GlassCannon
                }
            },
			//// Level = 50 (2) Familiar - Arcanot
            new Build {
                Level = 50,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Hydra, Runes.Wizard.LightningHydra },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.ChaosNexus },
                    {  Skills.Wizard.Teleport, Runes.Wizard.Wormhole },
					{  Skills.Wizard.Familiar, Runes.Wizard.Arcanot },
					{  Skills.Wizard.StormArmor, Runes.Wizard.PowerOfTheStorm },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.ForceWeapon }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
					Passives.Wizard.AstralPresence,
					Passives.Wizard.GlassCannon
                }
            },
			//// Level = 64 (P1) Unwavering Will
            new Build {
                Level = 64,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Hydra, Runes.Wizard.LightningHydra },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.ChaosNexus },
                    {  Skills.Wizard.Teleport, Runes.Wizard.Wormhole },
					{  Skills.Wizard.Familiar, Runes.Wizard.Arcanot },
					{  Skills.Wizard.StormArmor, Runes.Wizard.PowerOfTheStorm },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.ForceWeapon }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.UnwaveringWill,
					Passives.Wizard.AstralPresence,
					Passives.Wizard.GlassCannon
                }
            },
			//// Level = 66 (P3) Audacity
            new Build {
                Level = 66,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Hydra, Runes.Wizard.LightningHydra },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.ChaosNexus },
                    {  Skills.Wizard.Teleport, Runes.Wizard.Wormhole },
					{  Skills.Wizard.Familiar, Runes.Wizard.Arcanot },
					{  Skills.Wizard.StormArmor, Runes.Wizard.PowerOfTheStorm },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.ForceWeapon }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.UnwaveringWill,
					Passives.Wizard.AstralPresence,
					Passives.Wizard.Audacity
                }
            },
			//// Level = 70 (L) Hydra - Blazing Hydra			
			//// Level = 70 (R) Disintegrate - Convergence
			//// Level = 70 (1) Teleport - Wormhole
			//// Level = 70 (2) Energy Armor - Prismatic Armor
			//// Level = 70 (3) Familiar - Arcanot
			//// Level = 70 (4) Magic Weapon - Ignite
			//// Level = 70 (P1) Unwavering Will
			//// Level = 70 (P2) Unstable Anomaly
			//// Level = 70 (P3) Blur
			//// Level = 70 (P4) Galvanizing Ward
            new Build {
                Level = 70,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Hydra, Runes.Wizard.BlazingHydra },
                    {  Skills.Wizard.Disintegrate, Runes.Wizard.Convergence },
                    {  Skills.Wizard.Teleport, Runes.Wizard.Wormhole },
					{  Skills.Wizard.EnergyArmor, Runes.Wizard.PrismaticArmor },
					{  Skills.Wizard.Familiar, Runes.Wizard.Arcanot },
					{  Skills.Wizard.MagicWeapon, Runes.Wizard.Ignite }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.UnwaveringWill,
					Passives.Wizard.UnstableAnomaly,
					Passives.Wizard.Blur,
					Passives.Wizard.GalvanizingWard
                }
            },
        };

        public List<Build> BarbarianLevelingBuilds = new List<Build>
        {
			//// Level = 1 (L) Bash (Auto)
            new Build {
                Level = 1,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Bash, Runes.Barbarian.None }
                },
            },
			//// Level = 2 (R) Hammer of the Ancients (Auto)
            new Build {
                Level = 2,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Bash, Runes.Barbarian.None },
					{  Skills.Barbarian.HammerOfTheAncients, Runes.Barbarian.None }
                },
            },
			//// Level = 4 (1) Ground Stomp (Auto)
            new Build {
                Level = 4,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Bash, Runes.Barbarian.None },
					{  Skills.Barbarian.HammerOfTheAncients, Runes.Barbarian.None },
					{  Skills.Barbarian.GroundStomp, Runes.Barbarian.None }
                },
            },
			//// Level = 5 (R) Rend
            new Build {
                Level = 5,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Bash, Runes.Barbarian.None },
					{  Skills.Barbarian.Rend, Runes.Barbarian.None },
					{  Skills.Barbarian.GroundStomp, Runes.Barbarian.None }
                },
            },
			//// Level = 6 (L) Bash - Frostbite
            new Build {
                Level = 6,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Bash, Runes.Barbarian.Frostbite },
					{  Skills.Barbarian.Rend, Runes.Barbarian.None },
					{  Skills.Barbarian.GroundStomp, Runes.Barbarian.None }
                },
            },
			//// Level = 9 (L) Cleave - Rupture
			//// Level = 9 (2) Overpower
            new Build {
                Level = 9,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Cleave, Runes.Barbarian.Rupture },
					{  Skills.Barbarian.Rend, Runes.Barbarian.None },
					{  Skills.Barbarian.GroundStomp, Runes.Barbarian.None },
					{  Skills.Barbarian.Overpower, Runes.Barbarian.None }
                },
            },
			//// Level = 10 (P1) Ruthless
            new Build {
                Level = 10,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Cleave, Runes.Barbarian.Rupture },
					{  Skills.Barbarian.Rend, Runes.Barbarian.None },
					{  Skills.Barbarian.GroundStomp, Runes.Barbarian.None },
					{  Skills.Barbarian.Overpower, Runes.Barbarian.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless
                }
            },
			//// Level = 11 (R) Rend - Ravage
            new Build {
                Level = 11,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Cleave, Runes.Barbarian.Rupture },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.GroundStomp, Runes.Barbarian.None },
					{  Skills.Barbarian.Overpower, Runes.Barbarian.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless
                }
            },
			//// Level = 12 (1) Ground Stomp - Deafening Crash
            new Build {
                Level = 12,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Cleave, Runes.Barbarian.Rupture },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.GroundStomp, Runes.Barbarian.DeafeningCrash },
					{  Skills.Barbarian.Overpower, Runes.Barbarian.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless
                }
            },
			//// Level = 13 (1) Revenge
            new Build {
                Level = 13,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Cleave, Runes.Barbarian.Rupture },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.Revenge, Runes.Barbarian.None },
					{  Skills.Barbarian.Overpower, Runes.Barbarian.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless
                }
            },
			//// Level = 14 (3) ThreateningShout
            new Build {
                Level = 14,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Cleave, Runes.Barbarian.Rupture },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.Revenge, Runes.Barbarian.None },
					{  Skills.Barbarian.Overpower, Runes.Barbarian.None },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless
                }
            },
			//// Level = 15 (2) Overpower - Storm of Steel
            new Build {
                Level = 15,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Cleave, Runes.Barbarian.Rupture },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.Revenge, Runes.Barbarian.None },
					{  Skills.Barbarian.Overpower, Runes.Barbarian.StormOfSteel },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless
                }
            },
			//// Level = 17 (L) Frenzy - Sidearm
            new Build {
                Level = 17,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.Revenge, Runes.Barbarian.None },
					{  Skills.Barbarian.Overpower, Runes.Barbarian.StormOfSteel },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless
                }
            },
			//// Level = 19 (1) Revenge - Blood Law
			//// Level = 19 (4) Earthquake
            new Build {
                Level = 19,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.Revenge, Runes.Barbarian.BloodLaw },
					{  Skills.Barbarian.Overpower, Runes.Barbarian.StormOfSteel },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.None },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless
                }
            },
			//// Level = 20 (P2) Weapons Master
            new Build {
                Level = 20,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.Revenge, Runes.Barbarian.BloodLaw },
					{  Skills.Barbarian.Overpower, Runes.Barbarian.StormOfSteel },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.None },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless,
					Passives.Barbarian.WeaponsMaster
                }
            },
			//// Level = 22 (2) Battle Rage
            new Build {
                Level = 22,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.Revenge, Runes.Barbarian.BloodLaw },
					{  Skills.Barbarian.BattleRage, Runes.Barbarian.None },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.None },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless,
					Passives.Barbarian.WeaponsMaster
                }
            },
			//// Level = 23 (2) Threatening Shout - Intimidate
            new Build {
                Level = 23,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.Revenge, Runes.Barbarian.BloodLaw },
					{  Skills.Barbarian.BattleRage, Runes.Barbarian.None },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.Intimidate },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless,
					Passives.Barbarian.WeaponsMaster
                }
            },
			//// Level = 24 (4) Earthquake - Giant's Stride
            new Build {
                Level = 24,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.Revenge, Runes.Barbarian.BloodLaw },
					{  Skills.Barbarian.BattleRage, Runes.Barbarian.None },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.Intimidate },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.GiantsStride }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless,
					Passives.Barbarian.WeaponsMaster
                }
            },
			//// Level = 26 (2) Battle Rage - Marauders Rage
            new Build {
                Level = 26,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.Revenge, Runes.Barbarian.BloodLaw },
					{  Skills.Barbarian.BattleRage, Runes.Barbarian.MaraudersRage },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.Intimidate },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.GiantsStride }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless,
					Passives.Barbarian.WeaponsMaster
                }
            },
			//// Level = 27 (1) Furious Charge - Battering Ram
            new Build {
                Level = 27,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.FuriousCharge, Runes.Barbarian.BatteringRam },
					{  Skills.Barbarian.BattleRage, Runes.Barbarian.MaraudersRage },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.Intimidate },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.GiantsStride }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless,
					Passives.Barbarian.WeaponsMaster
                }
            },
			//// Level = 28 (3) Threatening Shout - Falter
            new Build {
                Level = 28,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.FuriousCharge, Runes.Barbarian.BatteringRam },
					{  Skills.Barbarian.BattleRage, Runes.Barbarian.MaraudersRage },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.Falter },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.GiantsStride }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless,
					Passives.Barbarian.WeaponsMaster
                }
            },
			//// Level = 30 (P3) Inspiring Presence
            new Build {
                Level = 30,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.FuriousCharge, Runes.Barbarian.BatteringRam },
					{  Skills.Barbarian.BattleRage, Runes.Barbarian.MaraudersRage },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.Falter },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.GiantsStride }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless,
					Passives.Barbarian.WeaponsMaster,
                    Passives.Barbarian.InspiringPresence
                }
            },
			//// Level = 31 (2) Battle Rage - Ferocity
            new Build {
                Level = 31,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.FuriousCharge, Runes.Barbarian.BatteringRam },
					{  Skills.Barbarian.BattleRage, Runes.Barbarian.Ferocity },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.Falter },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.GiantsStride }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless,
					Passives.Barbarian.WeaponsMaster,
                    Passives.Barbarian.InspiringPresence
                }
            },
			//// Level = 33 (1) Furious Charge - Merciless Assault
            new Build {
                Level = 33,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.FuriousCharge, Runes.Barbarian.MercilessAssault },
					{  Skills.Barbarian.BattleRage, Runes.Barbarian.Ferocity },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.Falter },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.GiantsStride }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless,
					Passives.Barbarian.WeaponsMaster,
                    Passives.Barbarian.InspiringPresence
                }
            },
			//// Level = 39 (4) Earthquake - The Mountains Call
            new Build {
                Level = 39,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
					{  Skills.Barbarian.FuriousCharge, Runes.Barbarian.MercilessAssault },
					{  Skills.Barbarian.BattleRage, Runes.Barbarian.Ferocity },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.Falter },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.TheMountainsCall }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless,
					Passives.Barbarian.WeaponsMaster,
                    Passives.Barbarian.InspiringPresence
                }
            },
			//// Level = 56 (R) Rend - Bloodbath
            new Build {
                Level = 56,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
					{  Skills.Barbarian.Rend, Runes.Barbarian.Bloodbath },
					{  Skills.Barbarian.FuriousCharge, Runes.Barbarian.MercilessAssault },
					{  Skills.Barbarian.BattleRage, Runes.Barbarian.Ferocity },
					{  Skills.Barbarian.ThreateningShout, Runes.Barbarian.Falter },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.TheMountainsCall }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless,
					Passives.Barbarian.WeaponsMaster,
                    Passives.Barbarian.InspiringPresence
                }
            },
			//// Level = 70 (L) Seismic Slam - Shattered Ground
			//// Level = 70 (R) Ancient Spear - Boulder Toss
			//// Level = 70 (1) Leap - Call of Arreat
			//// Level = 70 (2) Battle Rage - Bloodshed
			//// Level = 70 (3) War Cry - Veteran’s Warning
			//// Level = 70 (4) Earthquake - Molten Fury
			//// Level = 70 (P1) Ruthless 				
			//// Level = 70 (P2) Rampage
			//// Level = 70 (P3) Earthen Might 				
			//// Level = 70 (P4) Relentless
            new Build {
                Level = 70,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.SeismicSlam, Runes.Barbarian.ShatteredGround },
					{  Skills.Barbarian.AncientSpear, Runes.Barbarian.BoulderToss },
					{  Skills.Barbarian.Leap, Runes.Barbarian.CallOfArreat },
					{  Skills.Barbarian.BattleRage, Runes.Barbarian.Bloodshed },
					{  Skills.Barbarian.WarCry, Runes.Barbarian.VeteransWarning },
					{  Skills.Barbarian.Earthquake, Runes.Barbarian.MoltenFury }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless,
					Passives.Barbarian.Rampage,
                    Passives.Barbarian.EarthenMight,
					Passives.Barbarian.Relentless
                }
            },
        };

        public List<Build> MonkLevelingBuilds = new List<Build>
        {
			//// Level = 1 (L) Fists of Thunder
            new Build {
                Level = 1,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.None }
                },
            },
			//// Level = 2 (R) Lashing Tail Kick
            new Build {
                Level = 2,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.None },
					{  Skills.Monk.LashingTailKick, Runes.Monk.None }
                },
            },
			//// Level = 4 (1) BlindingFlash
            new Build {
                Level = 4,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.None },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.None },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.None }
                },
            },
			//// Level = 6 (L) Fists of Thunder - 	Thunderclap
            new Build {
                Level = 6,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.Thunderclap },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.None },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.None }
                },
            },
			//// Level = 7 (R) Lashing Tail Kick - Vulture Claw
            new Build {
                Level = 7,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.Thunderclap },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.VultureClawKick },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.None }
                },
            },
			//// Level = 9 (2) Dashing Strike
            new Build {
                Level = 9,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.Thunderclap },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.VultureClawKick },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.None },
                    {  Skills.Monk.DashingStrike, Runes.Monk.None }
                },
            },
			//// Level = 10 (P1) Fleet Footed
            new Build {
                Level = 10,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.Thunderclap },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.VultureClawKick },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.None },
                    {  Skills.Monk.DashingStrike, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.FleetFooted
                }
            },
			//// Level = 12 (1) BlindingFlash - Self Reflection
            new Build {
                Level = 12,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.Thunderclap },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.VultureClawKick },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.SelfReflection },
                    {  Skills.Monk.DashingStrike, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.FleetFooted
                }
            },
			//// Level = 13 (1) Exploding Palm
			//// Level = 13 (P1) Exalted Soul
            new Build {
                Level = 13,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.Thunderclap },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.VultureClawKick },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.None },
                    {  Skills.Monk.DashingStrike, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul
                }
            },
			//// Level = 14 (3) Cyclone Strike
            new Build {
                Level = 14,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.Thunderclap },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.VultureClawKick },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.None },
                    {  Skills.Monk.DashingStrike, Runes.Monk.None },
					{  Skills.Monk.CycloneStrike, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul
                }
            },
			//// Level = 15 (R) Lashing Tail Kick - Sweeping Armada
			//// Level = 15 (2) Dashing Strike - Falling Star
            new Build {
                Level = 15,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.Thunderclap },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.SweepingArmada },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.None },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.CycloneStrike, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul
                }
            },
			//// Level = 17 (L) Crippling Wave - Mangle
            new Build {
                Level = 17,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.SweepingArmada },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.None },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.CycloneStrike, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul
                }
            },
			//// Level = 18 (1) Exploding Palm - Flesh is Weak
            new Build {
                Level = 18,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.SweepingArmada },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.TheFleshIsWeak },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.CycloneStrike, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul
                }
            },
			//// Level = 19 (4) Mantra of Salvation
            new Build {
                Level = 19,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.SweepingArmada },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.TheFleshIsWeak },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.CycloneStrike, Runes.Monk.None },
					{  Skills.Monk.MantraOfSalvation, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul
                }
            },
			//// Level = 20 (P2) Chant of Resonance
            new Build {
                Level = 20,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.SweepingArmada },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.TheFleshIsWeak },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.CycloneStrike, Runes.Monk.None },
					{  Skills.Monk.MantraOfSalvation, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
					Passives.Monk.ChantOfResonance
                }
            },
			//// Level = 21 (3) Cyclone Strike - Eye of the Storm
			//// Level = 21 (4) Mantra of Retribution
            new Build {
                Level = 21,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.SweepingArmada },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.TheFleshIsWeak },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.CycloneStrike, Runes.Monk.EyeOfTheStorm },
					{  Skills.Monk.MantraOfRetribution, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
					Passives.Monk.ChantOfResonance
                }
            },
			//// Level = 25 (R) Wave of Light - Explosive Light
			//// Level = 25 (1) Exploding Palm - Strong Spirit
            new Build {
                Level = 25,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.WaveOfLight, Runes.Monk.ExplosiveLight },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.StrongSpirit },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.CycloneStrike, Runes.Monk.EyeOfTheStorm },
					{  Skills.Monk.MantraOfRetribution, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
					Passives.Monk.ChantOfResonance
                }
            },
			//// Level = 27 (1) Mystic Ally - Water Ally
            new Build {
                Level = 27,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.WaveOfLight, Runes.Monk.ExplosiveLight },
                    {  Skills.Monk.MysticAlly, Runes.Monk.WaterAlly },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.CycloneStrike, Runes.Monk.EyeOfTheStorm },
					{  Skills.Monk.MantraOfRetribution, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
					Passives.Monk.ChantOfResonance
                }
            },
			//// Level = 28 (3) Blinding Flash - Replenishing Light
			//// Level = 28 (4) Mantra of Retribution - Retaliation
            new Build {
                Level = 28,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.WaveOfLight, Runes.Monk.ExplosiveLight },
                    {  Skills.Monk.MysticAlly, Runes.Monk.WaterAlly },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.BlindingFlash, Runes.Monk.ReplenishingLight },
					{  Skills.Monk.MantraOfRetribution, Runes.Monk.Retaliation }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
					Passives.Monk.ChantOfResonance
                }
            },
			//// Level = 30 (P3) Seize the Initiative
            new Build {
                Level = 30,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.WaveOfLight, Runes.Monk.ExplosiveLight },
                    {  Skills.Monk.MysticAlly, Runes.Monk.WaterAlly },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.BlindingFlash, Runes.Monk.ReplenishingLight },
					{  Skills.Monk.MantraOfRetribution, Runes.Monk.Retaliation }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
					Passives.Monk.ChantOfResonance,
					Passives.Monk.SeizeTheInitiative
                }
            },
			//// Level = 35 (4) Mantra of Conviction - Overawe
            new Build {
                Level = 35,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.WaveOfLight, Runes.Monk.ExplosiveLight },
                    {  Skills.Monk.MysticAlly, Runes.Monk.WaterAlly },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.BlindingFlash, Runes.Monk.ReplenishingLight },
					{  Skills.Monk.MantraOfConviction, Runes.Monk.Overawe }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
					Passives.Monk.ChantOfResonance,
					Passives.Monk.SeizeTheInitiative
                }
            },
			//// Level = 38 (4) Mantra of Healing - Circular Breathing
            new Build {
                Level = 38,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.WaveOfLight, Runes.Monk.ExplosiveLight },
                    {  Skills.Monk.MysticAlly, Runes.Monk.WaterAlly },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.BlindingFlash, Runes.Monk.ReplenishingLight },
					{  Skills.Monk.MantraOfHealing, Runes.Monk.CircularBreathing }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
					Passives.Monk.ChantOfResonance,
					Passives.Monk.SeizeTheInitiative
                }
            },
			//// Level = 39 (1) Mystic Ally - Air Ally
            new Build {
                Level = 39,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.WaveOfLight, Runes.Monk.ExplosiveLight },
                    {  Skills.Monk.MysticAlly, Runes.Monk.AirAlly },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.BlindingFlash, Runes.Monk.ReplenishingLight },
					{  Skills.Monk.MantraOfHealing, Runes.Monk.CircularBreathing }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
					Passives.Monk.ChantOfResonance,
					Passives.Monk.SeizeTheInitiative
                }
            },
			//// Level = 46 (3) Sweeping Wind - Inner Storm
            new Build {
                Level = 46,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.WaveOfLight, Runes.Monk.ExplosiveLight },
                    {  Skills.Monk.MysticAlly, Runes.Monk.AirAlly },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.SweepingWind, Runes.Monk.InnerStorm },
					{  Skills.Monk.MantraOfHealing, Runes.Monk.CircularBreathing }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
					Passives.Monk.ChantOfResonance,
					Passives.Monk.SeizeTheInitiative
                }
            },
			//// Level = 57 (R) Wave of Light - Pillar of the Ancients
            new Build {
                Level = 57,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.WaveOfLight, Runes.Monk.PillarOfTheAncients },
                    {  Skills.Monk.MysticAlly, Runes.Monk.AirAlly },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.SweepingWind, Runes.Monk.InnerStorm },
					{  Skills.Monk.MantraOfHealing, Runes.Monk.CircularBreathing }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
					Passives.Monk.ChantOfResonance,
					Passives.Monk.SeizeTheInitiative
                }
            },
			//// Level = 70 (L) Crippling Wave - Mangle
			//// Level = 70 (R) Wave of Light - Explosive Light
			//// Level = 70 (1) Sweeping Wind - Inner Storm
			//// Level = 70 (2) Dashing Strike - Falling Star (or Blinding Speed)
			//// Level = 70 (3) Mystic Ally - Air Ally
			//// Level = 70 (4) Mantra of Salvation (Healing) - Agility (Circular Breathing)
			//// Level = 70 (P1) Beacon of Ytar (or Sixth Sense)
			//// Level = 70 (P2) Seize the Initiative
			//// Level = 70 (P3) Exalted Soul
			//// Level = 70 (P4) Mythic Rhythm (or Chant of Resonance)
            new Build {
                Level = 70,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.WaveOfLight, Runes.Monk.ExplosiveLight },
                    {  Skills.Monk.SweepingWind, Runes.Monk.InnerStorm },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
					{  Skills.Monk.MysticAlly, Runes.Monk.AirAlly },
					{  Skills.Monk.MantraOfSalvation, Runes.Monk.Agility }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.BeaconOfYtar,
					Passives.Monk.SeizeTheInitiative,
					Passives.Monk.ExaltedSoul,
					Passives.Monk.MythicRhythm
                }
            },
        };

        public List<Build> WitchDoctorLevelingBuilds = new List<Build>
        {
			//// Level = 1 (L) Poison Dart (auto)
			new Build {                                  
                Level = 1,              
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.None }
                },
            },
			//// Level = 2 (R) Grasp of dead (auto)
			new Build {                                  
                Level = 2,              
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.GraspOfTheDead, Runes.WitchDoctor.None }
                },
            },
			//// Level = 4 (1) Zombie Dogs (auto)
			new Build {                                  
                Level = 4,              
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.GraspOfTheDead, Runes.WitchDoctor.None },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.None }
                },
            },
			//// Level = 5 (R) Firebats
			new Build {                                  
                Level = 5,              
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.None },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.None }
                },
            },			
			//// Level = 6 (L) Poison Dart - Splinters
			new Build {                                  
                Level = 6,              
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.None },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.None }
                },
            },
			//// Level = 9 (2) Soul Harvest (auto)
			new Build {                                  
                Level = 9,              
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.None },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.None },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.None }
                },
            },
			//// Level = 10 (P1) Jungle Fortitude
            new Build {
                Level = 10,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.None },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.None },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude
                }
            },
			//// Level = 11 (R) Firebats - DireBats
            new Build {
                Level = 11,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.DireBats },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.None },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude
                }
            },
			//// Level = 12 (2) Haunt
            new Build {
                Level = 12,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.DireBats },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.None },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude
                }
            },
			//// Level = 13 (1) Zombie Dogs - Rabid Dogs
            new Build {
                Level = 13,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.DireBats },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude
                }
            },
			//// Level = 14	(3)	Zombie Charger (auto) change to Soul Harvest
            new Build {
                Level = 14,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.DireBats },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.None },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude
                }
            },
			//// Level = 16	(P1) Gruesome Feast
            new Build {
                Level = 16,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.DireBats },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.None },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.GruesomeFeast
                }
            },
			//// Level = 18	(2)	Haunt - Consuming Spirit
            new Build {
                Level = 18,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.DireBats },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ConsumingSpirit },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.GruesomeFeast
                }
            },
			//// Level = 19	(4) Gargantuan (auto)
            new Build {
                Level = 19,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.DireBats },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ConsumingSpirit },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.None },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.GruesomeFeast
                }
            },
			//// Level = 20	(P2) Spiritual Attunement
            new Build {
                Level = 20,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.DireBats },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ConsumingSpirit },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.None },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.GruesomeFeast,
					Passives.WitchDoctor.SpiritualAttunement
                }
            },
			//// Level = 21 (3) Soul Harvest - Siphon
            new Build {
                Level = 21,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.DireBats },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ConsumingSpirit },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Siphon },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.GruesomeFeast,
					Passives.WitchDoctor.SpiritualAttunement
                }
            },
			//// Level = 23 (2) Haunt - Resentful Spirits
			//// Level = 23 (4) Gargantuan - Humongoid
            new Build {
                Level = 23,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.DireBats },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ResentfulSpirits },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Siphon },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.GruesomeFeast,
					Passives.WitchDoctor.SpiritualAttunement
                }
            },
			//// Level = 24 (P1) Zombie Handler
            new Build {
                Level = 24,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.Firebats, Runes.WitchDoctor.DireBats },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ResentfulSpirits },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Siphon },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.ZombieHandler,
					Passives.WitchDoctor.SpiritualAttunement
                }
            },
			//// Level = 27 (R) Locust Swarm - Pestilence
			//// Level = 27 (P2) Pierce the Veil
            new Build {
                Level = 27,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.LocustSwarm, Runes.WitchDoctor.Pestilence },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ResentfulSpirits },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Siphon },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.ZombieHandler,
					Passives.WitchDoctor.PierceTheVeil
                }
            },
			//// Level = 30 (P3) Fetish Sycophant
            new Build {
                Level = 30,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.LocustSwarm, Runes.WitchDoctor.Pestilence },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ResentfulSpirits },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Siphon },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.ZombieHandler,
					Passives.WitchDoctor.PierceTheVeil,
					Passives.WitchDoctor.FetishSycophants
                }
            },
			//// Level = 32 (3) Soul Harvest - Languish
            new Build {
                Level = 32,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.LocustSwarm, Runes.WitchDoctor.Pestilence },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ResentfulSpirits },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Languish },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.ZombieHandler,
					Passives.WitchDoctor.PierceTheVeil,
					Passives.WitchDoctor.FetishSycophants
                }
            },
			//// Level = 39	(3)	Soul Harvest - Soul to Waste
            new Build {
                Level = 39,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.LocustSwarm, Runes.WitchDoctor.Pestilence },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ResentfulSpirits },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.SoulToWaste },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.ZombieHandler,
					Passives.WitchDoctor.PierceTheVeil,
					Passives.WitchDoctor.FetishSycophants
                }
            },
			//// Level = 40	(1)	Zombie Dogs - Burning Dogs
			//// Level = 40 (3) Soul Harvest - Languish / Soul to Waste
            new Build {
                Level = 40,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.LocustSwarm, Runes.WitchDoctor.Pestilence },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.BurningDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ResentfulSpirits },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Languish },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.ZombieHandler,
					Passives.WitchDoctor.PierceTheVeil,
					Passives.WitchDoctor.FetishSycophants
                }
            },			
			//// Level = 45 (P1) Fierce Loyalty
            new Build {
                Level = 45,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.LocustSwarm, Runes.WitchDoctor.Pestilence },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.BurningDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ResentfulSpirits },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Languish },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.FierceLoyalty,
					Passives.WitchDoctor.PierceTheVeil,
					Passives.WitchDoctor.FetishSycophants
                }
            },			
			//// Level = 48	(2)	Haunt - Poisoned Spirits
            new Build {
                Level = 48,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.LocustSwarm, Runes.WitchDoctor.Pestilence },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.BurningDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.PoisonedSpirit },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Languish },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.FierceLoyalty,
					Passives.WitchDoctor.PierceTheVeil,
					Passives.WitchDoctor.FetishSycophants
                }
            },
			//// Level = 54 (1)	Zombie Dogs - Burning Dogs / Leeching Beasts
			
			//// Level = 59	(R)	Locust Swarm - Searing Locusts
            new Build {
                Level = 59,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.LocustSwarm, Runes.WitchDoctor.SearingLocusts },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.BurningDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.PoisonedSpirit },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Languish },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.FierceLoyalty,
					Passives.WitchDoctor.PierceTheVeil,
					Passives.WitchDoctor.FetishSycophants
                }
            },
			//// Level = 68	(P1) Midnight Feast
            new Build {
                Level = 68,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.LocustSwarm, Runes.WitchDoctor.SearingLocusts },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.BurningDogs },
					{  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.PoisonedSpirit },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Languish },
					{  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.MidnightFeast,
					Passives.WitchDoctor.PierceTheVeil,
					Passives.WitchDoctor.FetishSycophants
                }
            },
			//// Level = 70 (L) Haunt - Resentful Spirits
			//// Level = 70 (R) Locust Swarm - Cloud of Insects
			//// Level = 70 (1) Soul Harvest - Languish
			//// Level = 70 (2) Spirit Walk - Severance / Jaunt
			//// Level = 70 (3) Piranhas - Piranhado
			//// Level = 70 (4) Zombie Dogs - Leeching Beasts
			//// Level = 70 (P1) Creeping Death
			//// Level = 70 (P2) Rush of Essence
			//// Level = 70 (P3) Swampland Attunement (Pierce)
			//// Level = 70 (P4) Grave Injustice
            new Build {
                Level = 70,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ResentfulSpirits },
                    {  Skills.WitchDoctor.LocustSwarm, Runes.WitchDoctor.CloudOfInsects },
					{  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Languish },
					{  Skills.WitchDoctor.SpiritWalk, Runes.WitchDoctor.Severance },
					{  Skills.WitchDoctor.Piranhas, Runes.WitchDoctor.Piranhado },
					{  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.LeechingBeasts }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.CreepingDeath,
					Passives.WitchDoctor.RushOfEssence,
					Passives.WitchDoctor.SwamplandAttunement,
					Passives.WitchDoctor.GraveInjustice
                }
            },			
        };


    }

}

