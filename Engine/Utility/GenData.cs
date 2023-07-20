namespace TeleBox.Engine.Utility;

public static class GenData
{
    public static IEnumerable<Type> AllSubclasses(this Type type)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && p is { IsInterface: false, IsAbstract: false });
    }
}