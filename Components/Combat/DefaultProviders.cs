namespace Trinity.Components.Combat
{
    public static class DefaultProviders
    {
        public static IPartyProvider Party { get; } = new DefaultPartyProvider();
        public static ITargetingProvider Targeting { get; } = new DefaultTargetingProvider();
        public static ISpellProvider Spells { get; } = new DefaultSpellProvider();
        public static IRoutineProvider Routines { get; } = new DefaultRoutineProvider();
        public static IWeightingProvider Weighting { get; } = new DefaultWeightingProvider();
        public static ILootProvider Loot { get; } = new DefaultLootProvider();
    }
}