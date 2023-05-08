using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace Stravaig.FeatureFlags;

public class StronglyTypedFeatureBuilder
{
    private readonly IFeatureManagementBuilder _builder;

    public StronglyTypedFeatureBuilder(IFeatureManagementBuilder builder)
    {
        _builder = builder;
    }

    public StronglyTypedFeatureBuilder Add<TEnum>()
        where TEnum : Enum
        => Add(typeof(TEnum));

    public StronglyTypedFeatureBuilder Add(Type enumType)
    {
        if (enumType == null) throw new ArgumentNullException(nameof(enumType));
        if (!enumType.IsEnum) throw new ArgumentException($"Expecting a type representing an enum, but {enumType} was not.", nameof(enumType));

        var enumAttr = enumType.GetCustomAttribute<StronglyTypedFeatureFlagsAttribute>()
                       ?? throw new ArgumentException($"Expecting enum {enumType.FullName} to have a {nameof(StronglyTypedFeatureFlagsAttribute)} attached, but did not.");
        var defaultLifetime = enumAttr.DefaultLifetime;

        string[] names = enumType.GetEnumNames();
        foreach (var name in names)
        {
            MemberInfo member = enumType.GetMember(name)[0];
            var lifetimeAttribute = member.GetCustomAttribute<LifetimeAttribute>();
            var lifetime = lifetimeAttribute?.Lifetime ?? defaultLifetime;

            Type serviceType = GetServiceType(enumType, name);
            Type implementationType = GetImplementationType(enumType, name);
            ServiceLifetime serviceLifetime = GetServiceLifetime(lifetime);

            _builder.Services.Add(new ServiceDescriptor(serviceType, implementationType, serviceLifetime));
        }

        return this;
    }

    private static Type GetImplementationType(Type enumType, string name)
    {
        string? expectedNamespace = enumType.Namespace;
        string expectedName = $"{name}FeatureFlag";
        string fullName = expectedNamespace == null ? name : expectedNamespace + "." + expectedName;
        var result = enumType.Assembly.ExportedTypes.FirstOrDefault(t => t.FullName == fullName);
        return result
               ?? throw new InvalidOperationException($"Expected {enumType.Assembly.FullName} to contain an implementation type called {fullName}");
    }

    private static Type GetServiceType(Type enumType, string name)
    {
        string? expectedNamespace = enumType.Namespace;
        string expectedName = $"I{name}FeatureFlag";
        string fullName = expectedNamespace == null ? name : expectedNamespace + "." + expectedName;
        var result = enumType.Assembly.ExportedTypes.FirstOrDefault(t => t.FullName == fullName);
        return result
               ?? throw new InvalidOperationException($"Expected {enumType.Assembly.FullName} to contain a service type called {fullName}");

    }

    private static ServiceLifetime GetServiceLifetime(Lifetime lifetime) =>
        lifetime switch
        {
            Lifetime.Scoped => ServiceLifetime.Scoped,
            Lifetime.Transient => ServiceLifetime.Transient,
            Lifetime.Singleton => ServiceLifetime.Singleton,
            _ => throw new ArgumentOutOfRangeException(nameof(lifetime)),
        };
}