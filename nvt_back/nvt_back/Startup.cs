namespace nvt_back
{
    public class Startup
    {
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