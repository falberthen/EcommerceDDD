namespace EcommerceDDD.Core.Reflection
{
    public static class TypeGetter
    {
        public static Type? GetTypeFromCurrencDomainAssembly(string typeName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()
                .Where(x => x.FullName == typeName || x.Name == typeName))
                .FirstOrDefault();
        }
    }
}
