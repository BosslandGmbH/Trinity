using Trinity.Framework.Avoidance.Handlers;

namespace Trinity.Framework.Avoidance.Structures
{
    public interface IAvoidanceSetting
    {
        string Name { get; set; }
        bool IsEnabled { get; set; }
        bool IsEnabledByDefault { get; set; }
        IAvoidanceHandler Handler { get; set; }
    }
}