namespace Trinity.Framework.Objects.Memory.Symbols.Types
{
    public enum LeaveGameResult // 2.4.0.36090 @26709176 index:83
    {
        Normal = 0,
        Removed = 1,
        MismatchingProtocolVersion = 2,
        JoinFailed = 3,
        AssertMismatch = 4,
        AccountCreateFailed = 5,
        UsingNdbStorage = 6,
        ShutdownGame = 7,
        HardcoreDeath = 8,
        KickedbyPlayers = 9,
        GameCompleted = 10,
        NotRestrictedContentPlayerLegal = 11,
    }
}