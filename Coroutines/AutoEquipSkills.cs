using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework;
using Trinity.Objects;
using Trinity.Reference;
using Zeta.Game;
using Trinity.Technicals;

namespace Trinity.Coroutines
{
    /// <summary>
    /// Handles changing skills while levelling to 70
    /// </summary>
    public class AutoEquipSkills
    {
        private int _lastLevel;

        private static AutoEquipSkills _instance;
        public static AutoEquipSkills Instance
        {
            get { return _instance ?? (_instance = (new AutoEquipSkills())); }
        }

        private DateTime _lastSkillChange = DateTime.MinValue;

        public async Task<bool> Execute()
        {
            if (!Core.Settings.Loot.Pickup.AutoEquipSkills)
                return false;

            if (!Core.Player.IsValid || ZetaDia.Me == null || !ZetaDia.IsInGame || !ZetaDia.Me.IsValid || ZetaDia.Me.CommonData == null || !ZetaDia.Me.CommonData.IsValid || ZetaDia.Me.CommonData.IsDisposed)
                return false;

            var level = ZetaDia.Me.Level;
            if (ZetaDia.Me.Level == 70 || level <= 0)
                return false;

            if (DateTime.UtcNow.Subtract(_lastSkillChange).TotalMinutes < 1)
                return false;

            _lastSkillChange = DateTime.UtcNow;

            if (DateTime.UtcNow.Subtract(TrinityPlugin.BotStartTime).TotalSeconds < 10)
                return false;
       
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
            var build = builds.OrderByDescending(b => b.Level).FirstOrDefault(b => b.Level <= Core.Player.Level);
            if (build == null)
                return false;

            if (_lastBuild != null && build == _lastBuild)
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
            }

            return null;
        }

        public List<Build> CrusaderLevelingBuilds = new List<Build>
        {
            new Build {
                Level = 6,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Roar },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.None }
                },
            },
            new Build {
                Level = 7,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Roar },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.ShatteredShield }
                },
            },
            new Build {
                Level = 7,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Roar },
                    {  Skills.Crusader.IronSkin, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.ShatteredShield }
                },
            },
            new Build {
                Level = 9,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.IronSkin, Runes.Crusader.None },
                    {  Skills.Crusader.Provoke, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.ShatteredShield }
                },
            },
            new Build {
                Level = 9,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.IronSkin, Runes.Crusader.None },
                    {  Skills.Crusader.Provoke, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.ShatteredShield }
                },            
                Passives = new List<Passive>
                {
                    Passives.Crusader.Fervor
                }
            },
            new Build {
                Level = 12,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.DivineVerdict },
                    {  Skills.Crusader.Provoke, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.ShatteredShield }
                },
                Passives = new List<Passive>
                {
                    Passives.Crusader.Fervor
                }
            },
            new Build {
                Level = 16,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.DivineVerdict },
                    {  Skills.Crusader.Provoke, Runes.Crusader.Cleanse },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.ShatteredShield },
                    {  Skills.Crusader.LawsOfValor, Runes.Crusader.ShatteredShield }
                },
                Passives = new List<Passive>
                {
                    Passives.Crusader.Fervor
                }
            },
            new Build {
                Level = 18,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.Uncertainty },
                    {  Skills.Crusader.Provoke, Runes.Crusader.Cleanse },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.ShatteredShield },
                    {  Skills.Crusader.LawsOfValor, Runes.Crusader.ShatteredShield }
                },
                Passives = new List<Passive>
                {
                    Passives.Crusader.Fervor
                }
            },
            new Build {
                Level = 20,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.Uncertainty },
                    {  Skills.Crusader.Provoke, Runes.Crusader.Cleanse },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.ShatteredShield },
                    {  Skills.Crusader.LawsOfValor, Runes.Crusader.ShatteredShield }
                },
                Passives = new List<Passive>
                {
                    Passives.Crusader.Fervor,
                    Passives.Crusader.Fanaticism
                }
            },
            new Build {
                Level = 20,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.ShieldGlare, Runes.Crusader.Uncertainty },
                    {  Skills.Crusader.Provoke, Runes.Crusader.Cleanse },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.ShatteredShield },
                    {  Skills.Crusader.LawsOfValor, Runes.Crusader.ShatteredShield },
                    {  Skills.Crusader.IronSkin, Runes.Crusader.SteelSkin }
                },
                Passives = new List<Passive>
                {
                    Passives.Crusader.Fervor,
                    Passives.Crusader.Fanaticism
                }
            },
            new Build {
                Level = 25,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.Condemn, Runes.Crusader.Vacuum },
                    {  Skills.Crusader.AkaratsChampion, Runes.Crusader.None },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.ShatteredShield },
                    {  Skills.Crusader.LawsOfValor, Runes.Crusader.FrozenInTerror },
                    {  Skills.Crusader.IronSkin, Runes.Crusader.SteelSkin }
                },
                Passives = new List<Passive>
                {
                    Passives.Crusader.Fervor,
                    Passives.Crusader.Fanaticism
                }
            },
            new Build {
                Level = 30,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Crusader.Slash, Runes.Crusader.Electrify },
                    {  Skills.Crusader.Condemn, Runes.Crusader.Vacuum },
                    {  Skills.Crusader.AkaratsChampion, Runes.Crusader.FireStarter },
                    {  Skills.Crusader.ShieldBash, Runes.Crusader.ShatteredShield },
                    {  Skills.Crusader.LawsOfValor, Runes.Crusader.FrozenInTerror },
                    {  Skills.Crusader.IronSkin, Runes.Crusader.SteelSkin }
                },
                Passives = new List<Passive>
                {
                    Passives.Crusader.Fervor,
                    Passives.Crusader.Fanaticism,
                    Passives.Crusader.Indestructible
                }
            },
        };

        public List<Build> DemonHunterLevelingBuilds = new List<Build>
        {
            new Build {
                Level = 4,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EntanglingShot, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Impale, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.None }
                },
            },
            new Build {
                Level = 4,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EntanglingShot, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Impale, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.None }
                },
            },
            new Build {
                Level = 7,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EntanglingShot, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Impale, Runes.DemonHunter.Impact },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.None }
                },
            },
            new Build {
                Level = 9,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EntanglingShot, Runes.DemonHunter.ChainGang },
                    {  Skills.DemonHunter.Impale, Runes.DemonHunter.Impact },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.SmokeScreen, Runes.DemonHunter.None }
                },
            },
            new Build {
                Level = 10,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EntanglingShot, Runes.DemonHunter.ChainGang },
                    {  Skills.DemonHunter.Impale, Runes.DemonHunter.Impact },
                    {  Skills.DemonHunter.Caltrops, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.SmokeScreen, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.ThrillOfTheHunt
                }
            },
            new Build {
                Level = 14,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.None},
                    {  Skills.DemonHunter.Impale, Runes.DemonHunter.Impact },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.SmokeScreen, Runes.DemonHunter.Displacement },
                    {  Skills.DemonHunter.Preparation, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.BloodVengeance
                }
            },
            new Build {
                Level = 18,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.None},
                    {  Skills.DemonHunter.Chakram, Runes.DemonHunter.TwinChakrams },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Preparation, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.BloodVengeance
                }
            },
            new Build {
                Level = 20,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.None},
                    {  Skills.DemonHunter.Chakram, Runes.DemonHunter.TwinChakrams },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.Preparation, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.ShadowPower, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.BloodVengeance,
                    Passives.DemonHunter.NightStalker
                }
            },
            new Build {
                Level = 23,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.Hardened},
                    {  Skills.DemonHunter.Chakram, Runes.DemonHunter.TwinChakrams },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.PinpointAccuracy },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.SpiderCompanion },
                    {  Skills.DemonHunter.Preparation, Runes.DemonHunter.None },
                    {  Skills.DemonHunter.ShadowPower, Runes.DemonHunter.NightBane }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.BloodVengeance,
                    Passives.DemonHunter.NightStalker
                }
            },
            new Build {
                Level = 25,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.None},
                    {  Skills.DemonHunter.Chakram, Runes.DemonHunter.TwinChakrams },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.PinpointAccuracy },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.SpiderCompanion },
                    {  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment },
                    {  Skills.DemonHunter.MarkedForDeath, Runes.DemonHunter.None }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.Brooding,
                    Passives.DemonHunter.NightStalker
                }
            },
            new Build {
                Level = 27,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.None},
                    {  Skills.DemonHunter.Chakram, Runes.DemonHunter.TwinChakrams },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.PinpointAccuracy },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.SpiderCompanion },
                    {  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment },
                    {  Skills.DemonHunter.MarkedForDeath, Runes.DemonHunter.Contagion }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.Brooding,
                    Passives.DemonHunter.NightStalker
                }
            },
            new Build {
                Level = 30,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.None},
                    {  Skills.DemonHunter.Chakram, Runes.DemonHunter.TwinChakrams },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.PinpointAccuracy },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.BatCompanion },
                    {  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment },
                    {  Skills.DemonHunter.MarkedForDeath, Runes.DemonHunter.Contagion }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.Brooding,
                    Passives.DemonHunter.NightStalker,
                    Passives.DemonHunter.Archery,
                }
            },
            new Build {
                Level = 32,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.None},
                    {  Skills.DemonHunter.Chakram, Runes.DemonHunter.TwinChakrams },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.BladedArmor },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.BatCompanion },
                    {  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment },
                    {  Skills.DemonHunter.MarkedForDeath, Runes.DemonHunter.Contagion }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.Brooding,
                    Passives.DemonHunter.NightStalker,
                    Passives.DemonHunter.Archery,
                }
            },
            new Build {
                Level = 32,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.None},
                    {  Skills.DemonHunter.Chakram, Runes.DemonHunter.TwinChakrams },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.BladedArmor },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.BatCompanion },
                    {  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment },
                    {  Skills.DemonHunter.MarkedForDeath, Runes.DemonHunter.Contagion }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.Brooding,
                    Passives.DemonHunter.NightStalker,
                    Passives.DemonHunter.Archery,
                }
            },
            new Build {
                Level = 42,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.Focus},
                    {  Skills.DemonHunter.Chakram, Runes.DemonHunter.TwinChakrams },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.BladedArmor },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.BatCompanion },
                    {  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment },
                    {  Skills.DemonHunter.MarkedForDeath, Runes.DemonHunter.Contagion }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.Brooding,
                    Passives.DemonHunter.NightStalker,
                    Passives.DemonHunter.Archery,
                }
            },
            new Build {
                Level = 59,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.DemonHunter.EvasiveFire, Runes.DemonHunter.Focus},
                    {  Skills.DemonHunter.Chakram, Runes.DemonHunter.TwinChakrams },
                    {  Skills.DemonHunter.FanOfKnives, Runes.DemonHunter.BladedArmor },
                    {  Skills.DemonHunter.Companion, Runes.DemonHunter.WolfCompanion },
                    {  Skills.DemonHunter.Preparation, Runes.DemonHunter.Punishment },
                    {  Skills.DemonHunter.MarkedForDeath, Runes.DemonHunter.Contagion }
                },
                Passives = new List<Passive>
                {
                    Passives.DemonHunter.Brooding,
                    Passives.DemonHunter.NightStalker,
                    Passives.DemonHunter.Archery,
                }
            },
        };

        public List<Build> WizardLevelingBuilds = new List<Build>
        {
            new Build {
                Level = 6,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.MagicMissile, Runes.Wizard.ChargedBlast },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.None },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.None }
                },
            },
            new Build {
                Level = 10,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.ShockPulse, Runes.Wizard.ExplosiveBolts },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.ColdBlood },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.None },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.None }
                },
            },
            new Build {
                Level = 11,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.ShockPulse, Runes.Wizard.ExplosiveBolts },
                    {  Skills.Wizard.RayOfFrost, Runes.Wizard.ColdBlood },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.None },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur
                }
            },
            new Build {
                Level = 13,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.ShockPulse, Runes.Wizard.ExplosiveBolts },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.Obliteration },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.Shatter },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur
                }
            },
            new Build {
                Level = 15,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.ShockPulse, Runes.Wizard.ExplosiveBolts },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.Obliteration },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.Shatter },
                    {  Skills.Wizard.IceArmor, Runes.Wizard.None },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.CrystalShell }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur
                }
            },
            new Build {
                Level = 20,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.ShockPulse, Runes.Wizard.FireBolts },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.Obliteration },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.ColdSnap },
                    {  Skills.Wizard.StormArmor, Runes.Wizard.None },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.CrystalShell },
                    {  Skills.Wizard.SlowTime, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur
                }
            },
            new Build {
                Level = 21,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.ShockPulse, Runes.Wizard.FireBolts },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.Obliteration },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.ColdSnap },
                    {  Skills.Wizard.StormArmor, Runes.Wizard.None },
                    {  Skills.Wizard.DiamondSkin, Runes.Wizard.CrystalShell },
                    {  Skills.Wizard.MagicWeapon, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
                    Passives.Wizard.Prodigy,
                }
            },
            new Build {
                Level = 23,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Electrocute, Runes.Wizard.ChainLightning },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.Obliteration },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.ColdSnap },
                    {  Skills.Wizard.StormArmor, Runes.Wizard.None },
                    {  Skills.Wizard.Familiar, Runes.Wizard.None },
                    {  Skills.Wizard.MagicWeapon, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
                    Passives.Wizard.Prodigy,
                }
            },
            new Build {
                Level = 24,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Electrocute, Runes.Wizard.ChainLightning },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.Obliteration },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.ColdSnap },
                    {  Skills.Wizard.StormArmor, Runes.Wizard.ReactiveArmor },
                    {  Skills.Wizard.Familiar, Runes.Wizard.None },
                    {  Skills.Wizard.MagicWeapon, Runes.Wizard.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
                    Passives.Wizard.Prodigy,
                }
            },
            new Build {
                Level = 28,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Electrocute, Runes.Wizard.ChainLightning },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.Obliteration },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.ColdSnap },
                    {  Skills.Wizard.StormArmor, Runes.Wizard.ReactiveArmor },
                    {  Skills.Wizard.Familiar, Runes.Wizard.None },
                    {  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
                    Passives.Wizard.Prodigy,
                }
            },
            new Build {
                Level = 29,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Electrocute, Runes.Wizard.ChainLightning },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.Obliteration },
                    {  Skills.Wizard.FrostNova, Runes.Wizard.ColdSnap },
                    {  Skills.Wizard.EnergyArmor, Runes.Wizard.None },
                    {  Skills.Wizard.Familiar, Runes.Wizard.None },
                    {  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
                    Passives.Wizard.Prodigy,
                }
            },
            new Build {
                Level = 31,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Electrocute, Runes.Wizard.ChainLightning },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.Obliteration },
                    {  Skills.Wizard.Archon, Runes.Wizard.None },
                    {  Skills.Wizard.EnergyArmor, Runes.Wizard.None },
                    {  Skills.Wizard.Familiar, Runes.Wizard.Sparkflint },
                    {  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
                    Passives.Wizard.Prodigy,
                    Passives.Wizard.GlassCannon,
                }
            },
            new Build {
                Level = 33,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Electrocute, Runes.Wizard.ChainLightning },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.Obliteration },
                    {  Skills.Wizard.Archon, Runes.Wizard.None },
                    {  Skills.Wizard.EnergyArmor, Runes.Wizard.Absorption },
                    {  Skills.Wizard.Familiar, Runes.Wizard.Sparkflint },
                    {  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
                    Passives.Wizard.Prodigy,
                    Passives.Wizard.GlassCannon,
                }
            },
            new Build {
                Level = 38,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Electrocute, Runes.Wizard.ChainLightning },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.Obliteration },
                    {  Skills.Wizard.Archon, Runes.Wizard.Combustion },
                    {  Skills.Wizard.EnergyArmor, Runes.Wizard.Absorption },
                    {  Skills.Wizard.Familiar, Runes.Wizard.Sparkflint },
                    {  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
                    Passives.Wizard.Prodigy,
                    Passives.Wizard.GlassCannon,
                }
            },
            new Build {
                Level = 43,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Electrocute, Runes.Wizard.ChainLightning },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.Obliteration },
                    {  Skills.Wizard.Archon, Runes.Wizard.Combustion },
                    {  Skills.Wizard.EnergyArmor, Runes.Wizard.PinpointBarrier },
                    {  Skills.Wizard.Familiar, Runes.Wizard.Sparkflint },
                    {  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
                    Passives.Wizard.Prodigy,
                    Passives.Wizard.GlassCannon,
                }
            },
            new Build {
                Level = 46,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Electrocute, Runes.Wizard.SurgeOfPower },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.Obliteration },
                    {  Skills.Wizard.Archon, Runes.Wizard.Combustion },
                    {  Skills.Wizard.EnergyArmor, Runes.Wizard.PinpointBarrier },
                    {  Skills.Wizard.Familiar, Runes.Wizard.Sparkflint },
                    {  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
                    Passives.Wizard.Prodigy,
                    Passives.Wizard.GlassCannon,
                }
            },
            new Build {
                Level = 57,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Electrocute, Runes.Wizard.SurgeOfPower },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.FrozenOrb },
                    {  Skills.Wizard.Archon, Runes.Wizard.Combustion },
                    {  Skills.Wizard.EnergyArmor, Runes.Wizard.PinpointBarrier },
                    {  Skills.Wizard.Familiar, Runes.Wizard.Sparkflint },
                    {  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
                    Passives.Wizard.Prodigy,
                    Passives.Wizard.GlassCannon,
                }
            },
            new Build {
                Level = 63,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Wizard.Electrocute, Runes.Wizard.SurgeOfPower },
                    {  Skills.Wizard.ArcaneOrb, Runes.Wizard.FrozenOrb },
                    {  Skills.Wizard.Archon, Runes.Wizard.ImprovedArchon },
                    {  Skills.Wizard.EnergyArmor, Runes.Wizard.PinpointBarrier },
                    {  Skills.Wizard.Familiar, Runes.Wizard.Sparkflint },
                    {  Skills.Wizard.MagicWeapon, Runes.Wizard.Electrify }
                },
                Passives = new List<Passive>
                {
                    Passives.Wizard.Blur,
                    Passives.Wizard.Prodigy,
                    Passives.Wizard.GlassCannon,
                }
            },
        };

        public List<Build> BarbarianLevelingBuilds = new List<Build>
        {
            new Build {
                Level = 5,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Cleave, Runes.Barbarian.None },
                    {  Skills.Barbarian.GroundStomp, Runes.Barbarian.None },
                    {  Skills.Barbarian.Rend, Runes.Barbarian.None }
                },
            },
            new Build {
                Level = 10,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Cleave, Runes.Barbarian.Rupture },
                    {  Skills.Barbarian.GroundStomp, Runes.Barbarian.None },
                    {  Skills.Barbarian.Overpower, Runes.Barbarian.None },
                    {  Skills.Barbarian.Rend, Runes.Barbarian.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless
                }
            },
            new Build {
                Level = 11,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.None },
                    {  Skills.Barbarian.GroundStomp, Runes.Barbarian.None },
                    {  Skills.Barbarian.Overpower, Runes.Barbarian.None },
                    {  Skills.Barbarian.Rend, Runes.Barbarian.Ravage }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Ruthless
                }
            },
            new Build {
                Level = 15,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.None },
                    {  Skills.Barbarian.GroundStomp, Runes.Barbarian.DeafeningCrash },
                    {  Skills.Barbarian.Overpower, Runes.Barbarian.StormOfSteel },
                    {  Skills.Barbarian.Revenge, Runes.Barbarian.None },
                    {  Skills.Barbarian.Rend, Runes.Barbarian.Ravage }
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.NervesOfSteel
                }
            },
            new Build {
                Level = 16,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.None },
                    {  Skills.Barbarian.GroundStomp, Runes.Barbarian.DeafeningCrash },
                    {  Skills.Barbarian.Overpower, Runes.Barbarian.StormOfSteel },
                    {  Skills.Barbarian.Revenge, Runes.Barbarian.None },
                    {  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.WeaponsMaster
                }
            },
            new Build {
                Level = 17,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
                    {  Skills.Barbarian.GroundStomp, Runes.Barbarian.DeafeningCrash },
                    {  Skills.Barbarian.Overpower, Runes.Barbarian.StormOfSteel },
                    {  Skills.Barbarian.Revenge, Runes.Barbarian.None },
                    {  Skills.Barbarian.Rend, Runes.Barbarian.Ravage },
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.WeaponsMaster
                }
            },
            new Build {
                Level = 20,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
                    {  Skills.Barbarian.GroundStomp, Runes.Barbarian.DeafeningCrash },
                    {  Skills.Barbarian.Overpower, Runes.Barbarian.StormOfSteel },
                    {  Skills.Barbarian.Revenge, Runes.Barbarian.BloodLaw },
                    {  Skills.Barbarian.ThreateningShout, Runes.Barbarian.None },
                    {  Skills.Barbarian.Earthquake, Runes.Barbarian.None },
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.WeaponsMaster,
                    Passives.Barbarian.InspiringPresence
                }
            },
            new Build {
                Level = 25,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
                    {  Skills.Barbarian.GroundStomp, Runes.Barbarian.WrenchingSmash },
                    {  Skills.Barbarian.Overpower, Runes.Barbarian.StormOfSteel },
                    {  Skills.Barbarian.Revenge, Runes.Barbarian.BestServedCold },
                    {  Skills.Barbarian.CallOfTheAncients, Runes.Barbarian.None },
                    {  Skills.Barbarian.Earthquake, Runes.Barbarian.GiantsStride },
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Bloodthirst,
                    Passives.Barbarian.InspiringPresence
                }
            },
            new Build {
                Level = 30,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
                    {  Skills.Barbarian.GroundStomp, Runes.Barbarian.WrenchingSmash },
                    {  Skills.Barbarian.WrathOfTheBerserker, Runes.Barbarian.None },
                    {  Skills.Barbarian.Revenge, Runes.Barbarian.BestServedCold },
                    {  Skills.Barbarian.CallOfTheAncients, Runes.Barbarian.None },
                    {  Skills.Barbarian.Earthquake, Runes.Barbarian.GiantsStride },
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Bloodthirst,
                    Passives.Barbarian.Superstition,
                    Passives.Barbarian.InspiringPresence
                }
            },
            new Build {
                Level = 31,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
                    {  Skills.Barbarian.GroundStomp, Runes.Barbarian.WrenchingSmash },
                    {  Skills.Barbarian.WrathOfTheBerserker, Runes.Barbarian.None },
                    {  Skills.Barbarian.Revenge, Runes.Barbarian.BestServedCold },
                    {  Skills.Barbarian.CallOfTheAncients, Runes.Barbarian.TheCouncilRises },
                    {  Skills.Barbarian.Earthquake, Runes.Barbarian.GiantsStride },
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Bloodthirst,
                    Passives.Barbarian.Superstition,
                    Passives.Barbarian.InspiringPresence
                }
            },
            new Build {
                Level = 36,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
                    {  Skills.Barbarian.GroundStomp, Runes.Barbarian.WrenchingSmash },
                    {  Skills.Barbarian.WrathOfTheBerserker, Runes.Barbarian.ArreatsWail },
                    {  Skills.Barbarian.Revenge, Runes.Barbarian.Retribution },
                    {  Skills.Barbarian.CallOfTheAncients, Runes.Barbarian.TheCouncilRises },
                    {  Skills.Barbarian.Earthquake, Runes.Barbarian.ChillingEarth },
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Bloodthirst,
                    Passives.Barbarian.Superstition,
                    Passives.Barbarian.InspiringPresence
                }
            },
            new Build {
                Level = 40,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Barbarian.Frenzy, Runes.Barbarian.Sidearm },
                    {  Skills.Barbarian.BattleRage, Runes.Barbarian.MaraudersRage },
                    {  Skills.Barbarian.WrathOfTheBerserker, Runes.Barbarian.ArreatsWail },
                    {  Skills.Barbarian.Revenge, Runes.Barbarian.BestServedCold },
                    {  Skills.Barbarian.CallOfTheAncients, Runes.Barbarian.DutyToTheClan },
                    {  Skills.Barbarian.Earthquake, Runes.Barbarian.TheMountainsCall },
                },
                Passives = new List<Passive>
                {
                    Passives.Barbarian.Bloodthirst,
                    Passives.Barbarian.BerserkerRage,
                    Passives.Barbarian.WeaponsMaster
                }
            },
        };

        public List<Build> MonkLevelingBuilds = new List<Build>
        {
            new Build {
                Level = 4,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.None },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.None },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.None }
                },
            },
            new Build {
                Level = 6,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.Thunderclap },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.None },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.None }
                },
            },
            new Build {
                Level = 6,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.FistsOfThunder, Runes.Monk.Thunderclap },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.VultureClawKick },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.None }
                },
            },
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
                    Passives.Monk.Resolve
                }
            },
            new Build {
                Level = 11,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.None },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.VultureClawKick },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.None },
                    {  Skills.Monk.DashingStrike, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.Resolve
                }
            },
            new Build {
                Level = 12,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.None },
                    {  Skills.Monk.LashingTailKick, Runes.Monk.VultureClawKick },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.SelfReflection },
                    {  Skills.Monk.DashingStrike, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.Resolve
                }
            },
            new Build {
                Level = 15,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.None },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.None },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.SelfReflection },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
                    {  Skills.Monk.CycloneStrike, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul
                }
            },
            new Build {
                Level = 17,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.None },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.SelfReflection },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
                    {  Skills.Monk.SevenSidedStrike, Runes.Monk.None },
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul
                }
            },
            new Build {
                Level = 18,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.TheFleshIsWeak },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.SelfReflection },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
                    {  Skills.Monk.SevenSidedStrike, Runes.Monk.None },
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul
                }
            },
            new Build {
                Level = 20,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.TheFleshIsWeak },
                    {  Skills.Monk.BlindingFlash, Runes.Monk.MystifyingLight },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
                    {  Skills.Monk.SevenSidedStrike, Runes.Monk.None },
                    {  Skills.Monk.MantraOfSalvation, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
                    Passives.Monk.SeizeTheInitiative,
                }
            },
            new Build {
                Level = 22,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.TheFleshIsWeak },
                    {  Skills.Monk.MysticAlly, Runes.Monk.None },
                    {  Skills.Monk.DashingStrike, Runes.Monk.WayOfTheFallingStar },
                    {  Skills.Monk.SevenSidedStrike, Runes.Monk.None },
                    {  Skills.Monk.MantraOfSalvation, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
                    Passives.Monk.SeizeTheInitiative,
                }
            },
            new Build {
                Level = 24,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.TheFleshIsWeak },
                    {  Skills.Monk.MysticAlly, Runes.Monk.None },
                    {  Skills.Monk.DashingStrike, Runes.Monk.BlindingSpeed },
                    {  Skills.Monk.SevenSidedStrike, Runes.Monk.SuddenAssault },
                    {  Skills.Monk.MantraOfSalvation, Runes.Monk.HardTarget }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
                    Passives.Monk.SeizeTheInitiative,
                }
            },
            new Build {
                Level = 25,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.StrongSpirit },
                    {  Skills.Monk.MysticAlly, Runes.Monk.None },
                    {  Skills.Monk.DashingStrike, Runes.Monk.BlindingSpeed },
                    {  Skills.Monk.SevenSidedStrike, Runes.Monk.SuddenAssault },
                    {  Skills.Monk.MantraOfSalvation, Runes.Monk.HardTarget }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
                    Passives.Monk.SeizeTheInitiative,
                }
            },
            new Build {
                Level = 30,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.Mangle },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.StrongSpirit },
                    {  Skills.Monk.MysticAlly, Runes.Monk.WaterAlly },
                    {  Skills.Monk.DashingStrike, Runes.Monk.BlindingSpeed },
                    {  Skills.Monk.SevenSidedStrike, Runes.Monk.SuddenAssault },
                    {  Skills.Monk.MantraOfConviction, Runes.Monk.None }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
                    Passives.Monk.SeizeTheInitiative,
                    Passives.Monk.Determination,
                }
            },
            new Build {
                Level = 39,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.Monk.CripplingWave, Runes.Monk.RisingTide },
                    {  Skills.Monk.ExplodingPalm, Runes.Monk.StrongSpirit },
                    {  Skills.Monk.MysticAlly, Runes.Monk.AirAlly },
                    {  Skills.Monk.DashingStrike, Runes.Monk.BlindingSpeed },
                    {  Skills.Monk.SevenSidedStrike, Runes.Monk.Pandemonium },
                    {  Skills.Monk.MantraOfConviction, Runes.Monk.Overawe }
                },
                Passives = new List<Passive>
                {
                    Passives.Monk.ExaltedSoul,
                    Passives.Monk.SeizeTheInitiative,
                    Passives.Monk.Determination,
                }
            },
        };

        public List<Build> WitchDoctorLevelingBuilds = new List<Build>
        {
            new Build {                                  
                Level = 3,              
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.CorpseSpiders, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.GraspOfTheDead, Runes.WitchDoctor.None }
                },
            },
            new Build {
                Level = 6,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.PoisonDart, Runes.WitchDoctor.Splinters },
                    {  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.GraspOfTheDead, Runes.WitchDoctor.None }
                }
            },
            new Build {
                Level = 10,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.CorpseSpiders, Runes.WitchDoctor.LeapingSpiders },
                    {  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.GraspOfTheDead, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude
                }
            },
            new Build {
                Level = 12,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.CorpseSpiders, Runes.WitchDoctor.LeapingSpiders },
                    {  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
                    {  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.None },
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude
                }
            },
            new Build {
                Level = 14,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.CorpseSpiders, Runes.WitchDoctor.LeapingSpiders },
                    {  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
                    {  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.ZombieCharger, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude
                }
            },
            new Build {
                Level = 15,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.CorpseSpiders, Runes.WitchDoctor.LeapingSpiders },
                    {  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
                    {  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.SwallowYourSoul },
                    {  Skills.WitchDoctor.GraspOfTheDead, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude
                }
            },
            new Build {
                Level = 15,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.CorpseSpiders, Runes.WitchDoctor.LeapingSpiders },
                    {  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
                    {  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.SwallowYourSoul },
                    {  Skills.WitchDoctor.SpiritWalk, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude
                }
            },
            new Build {
                Level = 15,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.CorpseSpiders, Runes.WitchDoctor.LeapingSpiders },
                    {  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
                    {  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ConsumingSpirit },
                    {  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.SwallowYourSoul },
                    {  Skills.WitchDoctor.SpiritWalk, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.None }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude
                }
            },

            new Build {
                Level = 21,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.Firebomb, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
                    {  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ConsumingSpirit },
                    {  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.SwallowYourSoul },
                    {  Skills.WitchDoctor.SpiritWalk, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude,
                    Passives.WitchDoctor.SpiritualAttunement
                }
            },

            new Build {
                Level = 24,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.Firebomb, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.RabidDogs },
                    {  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ResentfulSpirits },
                    {  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Siphon },
                    {  Skills.WitchDoctor.AcidCloud, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude,
                    Passives.WitchDoctor.ZombieHandler
                }
            },

            new Build {
                Level = 28,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.Firebomb, Runes.WitchDoctor.FlashFire },
                    {  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.LeechingBeasts },
                    {  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ResentfulSpirits },
                    {  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Siphon },
                    {  Skills.WitchDoctor.AcidCloud, Runes.WitchDoctor.None },
                    {  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.Humongoid }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude,
                    Passives.WitchDoctor.ZombieHandler
                }
            },

            new Build {
                Level = 30,
                Skills = new Dictionary<Skill, Rune>
                {
                    {  Skills.WitchDoctor.Firebomb, Runes.WitchDoctor.FlashFire },
                    {  Skills.WitchDoctor.SummonZombieDogs, Runes.WitchDoctor.LeechingBeasts },
                    {  Skills.WitchDoctor.Haunt, Runes.WitchDoctor.ResentfulSpirits },
                    {  Skills.WitchDoctor.SoulHarvest, Runes.WitchDoctor.Siphon },
                    {  Skills.WitchDoctor.AcidCloud, Runes.WitchDoctor.LobBlobBomb },
                    {  Skills.WitchDoctor.Gargantuan, Runes.WitchDoctor.RestlessGiant }
                },
                Passives = new List<Passive>
                {
                    Passives.WitchDoctor.JungleFortitude,
                    Passives.WitchDoctor.ZombieHandler,
                    Passives.WitchDoctor.FetishSycophants
                }
            },

        };


    }

}

