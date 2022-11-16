#nullable enable
namespace EcommerceDDD.Core.Reflection
{
    public static class TypeGetter
    {
        public static Type? GetTypeFromCurrentDomainAssembly(string typeName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && t.Name == typeName)
                .FirstOrDefault();
        }
    }
}
