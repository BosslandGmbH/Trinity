namespace Trinity.Settings
{
    internal interface ITrinitySettingEvents
    {
        void OnSave();
        void OnLoaded();
    }
}