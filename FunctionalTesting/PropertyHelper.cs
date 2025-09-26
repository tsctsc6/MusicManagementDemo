namespace FunctionalTesting;

public static class PropertyHelper
{
    public static object? GetProperty(this object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
    }
}
