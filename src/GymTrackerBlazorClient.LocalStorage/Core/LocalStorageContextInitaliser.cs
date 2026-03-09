using GymTracker.LocalStorage.IndexedDb;
using System.Reflection;

namespace GymTracker.LocalStorage.Core;
public static class LocalStorageContextExtensions
{
    public static void Initialise<T>(this T context,
        IIndexedDbService indexedDbService,
        IServiceProvider serviceProvider)
        where T : LocalStorageContext, new()
    {
        context.GetType()
               .GetField("_indexedDb", BindingFlags.Instance | BindingFlags.NonPublic)!
               .SetValue(context, indexedDbService);

        context.GetType()
               .GetField("_serviceProvider", BindingFlags.Instance | BindingFlags.NonPublic)!
               .SetValue(context, serviceProvider);

        var keys = new List<IKeyItem>();
        keys.AddRange(InitialiseKeys(context, typeof(IKeyItem<>), typeof(IndexedDbKeyItem<>), indexedDbService));
        keys.AddRange(InitialiseKeys(context, typeof(IKeyListItem<>), typeof(IndexedDbKeyListItem<>), indexedDbService));

        context.Keys = keys;
        context.Configure();
        context.InitializeData();
    }

    private static IEnumerable<IKeyItem> InitialiseKeys<T>(
        T context,
        Type keyService,
        Type keyImplementation,
        IIndexedDbService indexedDbService) where T : LocalStorageContext, new()
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

                ConstructorInfo? constructor = genericType.GetConstructor(new[] { typeof(IIndexedDbService), typeof(string) });
                object? instance = constructor?.Invoke(new object[] { indexedDbService, item.Name });

                if (instance is not null)
                {
                    item.SetValue(context, instance);
                    keys.Add((IKeyItem)instance);
                }
            });

        return keys;
    }
}
