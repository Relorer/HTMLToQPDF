using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLQuestPDF.Extensions
{
#if DEBUG

    internal static class IContainerExtensions
    {
        private static Random random = new Random();

        public static IContainer Debug(this IContainer container, string name) => container.DebugArea(name, String.Format("#{0:X6}", random.Next(0x1000000)));
    }

#endif
}