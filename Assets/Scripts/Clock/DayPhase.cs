namespace Clock
{
    [System.Flags]
    public enum DayPhase
    {
        NONE = 0,
        Early_Morning = 1,
        Late_Morning = 2,
        Early_Afternoon = 4,
        Late_Afternoon = 8,
        Early_Night = 16,
        Late_Night = 32,
        Night_Owl = 64
    }
}