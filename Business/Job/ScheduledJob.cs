using DataAccess;

namespace Business.Job {
    public class ScheduledJob : IScheduledJob {
        private readonly AppDBContext context;
        public ScheduledJob(AppDBContext context) {
            this.context = context;
        }
        public void ProcessFireAndForgetJob() {
            //do your stuff
        }
    }
}
