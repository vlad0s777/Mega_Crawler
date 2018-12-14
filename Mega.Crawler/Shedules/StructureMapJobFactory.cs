namespace Mega.Crawler.Shedules
{
    using System;

    using Quartz;
    using Quartz.Spi;

    using StructureMap;

    public class StructureMapJobFactory : IJobFactory
    {
        private readonly IContainer container;

        public StructureMapJobFactory(IContainer container)
        {
            this.container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                return (IJob)this.container.GetInstance(bundle.JobDetail.JobType);
            }
            catch (Exception e)
            {
                var se = new SchedulerException("Problem instantiating class", e);
                throw se;
            }
        }

        public void ReturnJob(IJob job)
        {
            throw new NotImplementedException();
        }
    }
}
