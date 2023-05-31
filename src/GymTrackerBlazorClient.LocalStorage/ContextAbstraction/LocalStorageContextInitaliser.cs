using Blazored.LocalStorage;
using GymTracker.Domain.Abstractions.Services;
using GymTracker.Domain.Abstractions.Services.ClientStorage;
using System.Reflection;

namespace GymTracker.LocalStorage.ContextAbstraction;
public static class LocalStorageContextExtensions
{
    public static void Initialise<T>(this T context, 
        ILocalStorageService localStorageService,
        IDataBackupService? dataBackupService) 
        where T : LocalStorageContext, new()
    {
        context.GetType()
               .GetField("_localStorage", BindingFlags.Instance | BindingFlags.NonPublic)!
               .SetValue(context, localStorageService);

        InitialiseKeys(context, typeof(IKeyItem<>), typeof(KeyItem<>), localStorageService, dataBackupService);
        InitialiseKeys(context, typeof(IKeyListItem<>), typeof(KeyListItem<>), localStorageService, dataBackupService);

        context.Configure();
    }

    private static void InitialiseKeys<T>(
        T context, 
        Type keyService, 
        Type keyImplementation, 
        ILocalStorageService localStorageService,
        IDataBackupService? dataBackupService) where T : LocalStorageContext, new()
    {
        context.GetType()
            .GetProperties()
            .Where(p => p.PropertyType.IsGenericType && p.CanWrite &&
                        p.PropertyType.GetGenericTypeDefinition() == keyService)
            .ToList()
            .ForEach(item =>
            {
                Type itemType = item.PropertyType.GetGenericArguments()[0];
                Type genericType = keyImplementation.MakeGenericType(itemType);

                ConstructorInfo? constructor = genericType.GetConstructor(new[] { typeof(ILocalStorageService), typeof(IDataBackupService), typeof(string) });
                object? instance = constructor?.Invoke(new object[] { localStorageService, dataBackupService, item.Name });
                item.SetValue(context, instance);
            });
    }
}