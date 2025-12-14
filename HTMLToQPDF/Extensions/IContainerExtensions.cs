using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Extensions
{
#if DEBUG

    internal static class ContainerExtensions
    {
        private static readonly Random Random = new();

        public static IContainer Debug(this IContainer container, string name) => container.DebugArea(name, $"#{Random.Next(0x1000000):X6}");
    }

#endif
}