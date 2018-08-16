using System;

namespace Mega.Services
{
    public class Limitations
    {
        public readonly int attemptLimit;

        public readonly int countLimit;
        public readonly int depthLimit;

        public Limitations(int depthLimit, int countLimit, int attemptLimit)
        {
            this.depthLimit = depthLimit;
            this.countLimit = countLimit;
            this.attemptLimit = attemptLimit;
        }

        public Limitations(string depthLimit, string countLimit, string attemptLimit) : this(Convert.ToInt32(depthLimit), Convert.ToInt32(countLimit),
            Convert.ToInt32(attemptLimit))
        {
        }
    }
}