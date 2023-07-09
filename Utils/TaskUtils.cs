
using System.Threading.Tasks;

namespace Fractural.Utils
{
    public static class TaskUtils
    {
        public static Task CompletedTask => Task.Delay(0);
    }
}