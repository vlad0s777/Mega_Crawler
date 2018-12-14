namespace Mega.Crawler.Shedules
{
    using Quartz;
    using Quartz.Core;
    using Quartz.Impl;
    using Quartz.Spi;

    public class StructureMapShedulerFactory : StdSchedulerFactory
    {
        private readonly IJobFactory jobFactory;

        public StructureMapShedulerFactory(IJobFactory jobFactory)
        {
            this.jobFactory = jobFactory;
        }

        protected override IScheduler Instantiate(QuartzSchedulerResources rsrcs, QuartzScheduler qs)
        {
            qs.JobFactory = this.jobFactory;

            return base.Instantiate(rsrcs, qs);
        }
    }
}
