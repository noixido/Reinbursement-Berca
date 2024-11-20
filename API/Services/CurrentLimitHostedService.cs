using API.Context;

namespace API.Services
{
    public class CurrentLimitHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public CurrentLimitHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // satu hari setelah sever jalan
            var now = DateTime.Now;
            var timeUntilMidnight = DateTime.Today.AddDays(1) - now;
            _timer = new Timer(ExecuteTask, null, timeUntilMidnight, TimeSpan.FromDays(1));

            // tiap menit
            //_timer = new Timer(ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            // tiap 10 detik
            //_timer = new Timer(ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));


            return Task.CompletedTask;
        }

        private async void ExecuteTask(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MyContext>();
                var service = new CurrentLimit(context);
                Console.WriteLine($"Task executed at {DateTime.UtcNow}"); // Log untuk memverifikasi
                await service.UpdateCurrentLimitsAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
