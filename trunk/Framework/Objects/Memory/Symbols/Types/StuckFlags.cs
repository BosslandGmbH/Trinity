namespace Trinity.Framework.Objects.Memory.Misc
{
    public enum StuckFlags // 2.4.0.36090 @26702288 index:36
    {
        IgnoreCollision = 0,
        NoPathfinding = 1,
        NoSteering = 2,
        ArrivingAtDestination = 3,
        AutoUpdateYaw = 4,
        RemoteMovement = 5,
        NoSnapToGround = 6,
        CheckVertical = 7,
        Snap = 8,
        FromPower = 9,
        WasStuck = 10,
    }
}