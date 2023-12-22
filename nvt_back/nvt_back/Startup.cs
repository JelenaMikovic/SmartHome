using Quartz;
using Quartz.Impl;

namespace nvt_back
{
    public class Startup
    {
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddSingleton<IScheduler>(provider => GetSchedulerAsync(provider).GetAwaiter().GetResult());
        //}

        //private async Task<IScheduler> GetSchedulerAsync(IServiceProvider provider)
        //{
          //  var schedulerFactory = new StdSchedulerFactory();
            //var scheduler = await schedulerFactory.GetScheduler().ConfigureAwait(false);

            //if (scheduler != null)
            //{
              //  await scheduler.Start().ConfigureAwait(false);
                // Log success or additional information
            //}
            //else
            //{
                // Log an error if scheduler is null
              //  Console.WriteLine("Scheduler initialization failed.");
            //}

            //return scheduler;
        //}
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseRouting();
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            });

            Console.WriteLine("ohhohohoh");

            RunOnApplicationStart();
        }

        private void RunOnApplicationStart()
        {
        }
    }

}