using Blazored.LocalStorage;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using System.Reflection;

namespace GymTracker.LocalStorage.ContextAbstraction;
public static class LocalStorageContextExtensions
{
    public static void Initialise<T>(this T context, 
        ILocalStorageService localStorageService) 
        where T : LocalStorageContext, new()
    {
        context.GetType()
               .GetField("_localStorage", BindingFlags.Instance | BindingFlags.NonPublic)!
               .SetValue(context, localStorageService);

        var keys = new List<IKeyItem>();
        keys.AddRange(InitialiseKeys(context, typeof(IKeyItem<>), typeof(KeyItem<>), localStorageService));
        keys.AddRange(InitialiseKeys(context, typeof(IKeyListItem<>), typeof(KeyListItem<>), localStorageService));

        context.Keys = keys;
        context.Configure();
    }

    private static IEnumerable<IKeyItem> InitialiseKeys<T>(
        T context, 
        Type keyService, 
        Type keyImplementation, 
        ILocalStorageService localStorageService) where T : LocalStorageContext, new()
    {
        var keys = new List<IKeyItem>();
        context.GetType()
            .GetProperties()
            .Where(p => p.PropertyType.IsGenericType && p.CanWrite &&
                        p.PropertyType.GetGenericTypeDefinition() == keyService)
            .ToList()
            .ForEach(item =>
            {
                Type itemType = item.PropertyType.GetGenericArguments()[0];
                Type genericType = keyImplementation.MakeGenericType(itemType);

                ConstructorInfo? constructor = genericType.GetConstructor(new[] { typeof(ILocalStorageService), typeof(string) });
                object? instance = constructor?.Invoke(new object[] { localStorageService, item.Name });
                
                if(instance is not null)
                {
                    item.SetValue(context, instance);
                    keys.Add((IKeyItem)instance);
                }
            });

        return keys;
    }
}