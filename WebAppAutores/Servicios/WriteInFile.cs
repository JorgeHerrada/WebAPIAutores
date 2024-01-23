
namespace WebAppAutores.Servicios
{
    // functions to execute when API start and stop
    public class WriteInFile : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string fileName = "file1.txt";
        private Timer timer;

        public WriteInFile(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5));
            Write("The Process has just started at " + DateTime.Now.ToString("dd//MM/yyyy hh:mm:ss"));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            Write("The process has just FINISHED at " + DateTime.Now.ToString("dd//MM/yyyy hh:mm:ss"));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Write("Process executing: " + DateTime.Now.ToString("dd//MM/yyyy hh:mm:ss"));
        }

        private void Write(string message)
        {
            // write new line in file
            var ruta = $@"{env.ContentRootPath}\wwwroot\{fileName}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true))
            {
                writer.WriteLine(message);
            }
        }
    }
}
