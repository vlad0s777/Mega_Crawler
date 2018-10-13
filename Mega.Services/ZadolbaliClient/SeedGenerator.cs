namespace Mega.Services.ZadolbaliClient
{
    using System;

    public class SeedGenerator
    {
        public int Seed { get; }

        public SeedGenerator(Random random)
        {
            this.Seed = random.Next();
        }
    }
}
