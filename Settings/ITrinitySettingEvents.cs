namespace Trinity.Config
{
    internal interface ITrinitySettingEvents
    {
        void OnSave();
        void OnLoaded();
    }
}