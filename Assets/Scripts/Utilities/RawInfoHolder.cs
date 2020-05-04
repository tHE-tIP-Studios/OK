namespace Utilities
{
    public struct RawInfoHolder
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Specie;
        public readonly string Location;
        public readonly string Subtitle;
        public readonly string Description;
        public readonly string Months;
        public readonly string Phases;
        public readonly string SpawnChance;
        public readonly string Lifespan;
        public readonly string Stamina;
        public readonly string Fails;
        public readonly string ReelWindow;
        public readonly string WindowIncrease;
        public readonly string BaitInterest;

        public RawInfoHolder(int id, string name, string specie, string location,
            string subtitle, string description, string months, string phases,
            string spawnChance, string lifespan, string stamina, string fails,
            string reelWindow, string windowIncrease, string interest)
        {
            Id = id;
            Name = name;
            Specie = specie;
            Location = location;
            Subtitle = subtitle;
            Description = description;
            Months = months;
            Phases = phases;
            SpawnChance = spawnChance;
            Lifespan = lifespan;
            Stamina = stamina;
            Fails = fails;
            ReelWindow = reelWindow;
            WindowIncrease = windowIncrease;
            BaitInterest = interest;
        }
    }
}