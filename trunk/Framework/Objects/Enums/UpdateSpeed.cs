namespace Trinity.Framework.Objects.Enums
{
    public enum UpdateSpeed
    {
        /// <summary>
        /// Default, Only Once
        /// </summary>
        Once = -1,

        /// <summary>
        /// Once per tick
        /// </summary>
        RealTime = 0,

        /// <summary>
        /// Every 50ms
        /// </summary>
        Ultra = 50,

        /// <summary>
        /// Every 200ms
        /// </summary>
        Fast = 200,

        /// <summary>
        /// Every 500ms
        /// </summary>
        Normal = 500,
        
        /// <summary>
        /// Every 1000ms
        /// </summary>
        Slow = 1000,

        /// <summary>
        /// Every 2000ms
        /// </summary>
        VerySlow = 2000,

        /// <summary>
        /// Every 5000ms
        /// </summary>
        ExtremelySlow = 5000


    }
}
