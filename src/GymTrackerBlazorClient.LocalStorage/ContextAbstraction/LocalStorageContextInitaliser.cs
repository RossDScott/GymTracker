using Blazored.LocalStorage;
using System.Reflection;

namespace GymTracker.BlazorClient.LocalStorage.ContextAbstraction;
public static class LocalStorageContextExtensions
{
    public static void Initialise<T>(this T context, ILocalStorageService localStorageService) where T : LocalStorageContext, new()
    {
        context.GetType()
               .GetField("_localStorage", BindingFlags.Instance | BindingFlags.NonPublic)!
               .SetValue(context, localStorageService);

        InitialiseKeys(context, typeof(KeyItem<>), localStorageService);
        InitialiseKeys(context, typeof(KeyListItem<>), localStorageService);
        InitialiseKeys(context, typeof(KeySyncItem<>), localStorageService);
    }

    private static void InitialiseKeys<T>(T context, Type keyType, ILocalStorageService localStorageService) where T : LocalStorageContext, new()
    {
        context.GetType()
            .GetProperties()
            .Where(p => p.PropertyType.IsGenericType && p.CanWrite &&
                        p.PropertyType.GetGenericTypeDefinition() == keyType)
            .ToList()
            .ForEach(item =>
            {
                Type itemType = item.PropertyType.GetGenericArguments()[0];
                Type genericType = keyType.MakeGenericType(itemType);

                ConstructorInfo? constructor = genericType.GetConstructor(new[] { typeof(ILocalStorageService), typeof(string) });
                object? instance = constructor?.Invoke(new object[] { localStorageService, item.Name });
                item.SetValue(context, instance);
            });
    }
}