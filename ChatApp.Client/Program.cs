using System.Threading.Tasks;

namespace ChatApp.Client
{
    internal class Program
    {
        private static async Task Main()
        {
            var app = new ChatApplication();
            await app.StartClientAsync();
        }
    }
}