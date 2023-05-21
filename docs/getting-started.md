# Getting Started

To start using `Stravaig.FeatureFlags` in your solution, first add the [Stravaig.FeatureFlags Nuget package](https://www.nuget.org/packages/Stravaig.FeatureFlags) to the project where you want to set up the feature flags. This should be a fairly central location as they will likely need to be accessible from anywhere in your application. It will also have to be accessible from the project that sets up the dependency injection.

Create a `enum` that represents each of your feature flags. For example,

```csharp
public enum ApplicationFeatures
{
    SendEmail,
    SendSms,
    NewShippingCalculation,
}
```

Add the `[StronglyTypedFeatureFlags]` attribute to the enum, you'll have to add `using Stravaig.FeatureFlags` as well. So the code will now look like this:

```csharp
using Stravaig.FeatureFlags;

namespace MyExampleApplication;

[StronglyTypedFeatureFlags]
public enum ApplicationFeatures
{
    SendEmail,
    SendSms,
    NewShippingCalculation,
}
```

Finally, add the feature flags to the service container in the Microsoft Dependency Injection container, like this:

```csharp
using Microsoft.FeatureManagement;
using Stravaig.FeatureFlags;

// services is your IServiceCollection.
// Configuration is your IConfiguration.
// ApplicationFeatures is your enum. (See above)
services.AddFeatureManagement(Configuration)
    .SetupStronglyTypedFeatures(opts =>
    {
        opts.Add<ApplicationFeatures>();
    });
```

You can add as many enums as you like.

## Using the feature flag

Each enum value will translate into a class and interface that is injected into the dependency injection container. Therefore, anywhere you can inject into a class, you can inject a feature flag.

The feature flag interface will have the name `I<name>FeatureFlag` where `<name>` is the value name in the enum. So, for example your class might be set up like this:

```csharp
namespace MyExampleApplication.Services;

public class EmailService
{
    private readonly ISmtpClient _smtpClient;
    private readonly ISendEmailFeatureFlag _sendEmail;

    public EmailService(ISmtpClient client, ISendEmailFeatureFlag sendEmail)
    {
        _smptClient = smtpClient;
        _sendEmail = sendEmail;
    }

    public Task SendEmailAsync(string subject, string recipient, string body)
    {
        if (!await _sendEmail.IsEnabledAsync())
            return;
        
        // Do stuff for sending email.
    }
}
```

## Setting lifetimes

Each feature flag can have its own lifetime, or all feature flags can have the same lifetime, or you can set a default lifetime and override it on a case by case basis.

Say your `SendEmail` and `SendSms` feature flags are in place to prevent emails and SMS messages being sent from development or testing environments, you might want to set them with a Singleton lifetime as they won't change during the lifetime of your application. However, your `NewShippingCalculation` is to be applied at a specific point in time you could set that to `Transient` or `Scoped`.

* `Singleton`: The feature flag value will remain the same for the lifetime of the application.
* `Scoped`: The feature flag will remain the same for the lifetime of the scope it was created in. If this is an ASP.NET application the scope will be the lifetime of the incoming HTTP request, so the value will be consistent throughout the handling of a single request, but may be different for subsequent requests.
* `Transient`: The feature flag will be created for each place it is injected. That means that difference classes will have their own feature flag with its own value.

To set the default lifetime, you add a property to the `[StronglyTypedFeatureFlag]` attribute on the enum. If you don't set it, the default will be `Scoped`.

```csharp
[StronglyTypedFeatureFlags(DefaultLifetime = Lifetime.Singleton)]
public enum ApplicationFeatures
{
    // ...
}
```

If you want to set the lifetime on a case-by-case basis then you can set it on each individual value, like this:

```csharp
[StronglyTypedFeatureFlags]
public enum ApplicationFeatures
{
    [Lifetime(Lifetime.Singleton)]
    SendEmail,

    [Lifetime(Lifetime.Singleton)]
    SendSms,

    [Lifetime(Lifetime.Scoped)]
    NewShippingCalculation,
}
```

You can also mix the two styles. e.g. If most of your flags are one lifetime, but some are different:

```csharp
[StronglyTypedFeatureFlags(DefaultLifetime = Lifetime.Singleton)]
public enum ApplicationFeatures
{
    SendEmail,
    SendSms,

    [Lifetime(Lifetime.Scoped)]
    NewShippingCalculation,
}
```

## Naming your Feature Flags

* Give the feature flag a Pascal cased name, just as you would any enum value.
* You should not end the name with `FeatureFlag` or `Flag` or `Feature`.
* If you set up multiple enums for feature flags, avoid clashing names.

If you name the the values of the enum something like `MyFeatureFlag` then the generated interfaces and classes will have redundant parts to their names as they have `FeatureFlag` appended. So if you do that you'll be injecting an interface called `IMyFeatureFlagFeatureFlag`.

If names clash because you have multiple enums set up and they have duplicate names then this can manifest in a couple of ways.

1. If the enums are in different namespaces then you'll get multiple classes and interfaces that resolve to the same feature. However they may end up with different lifetimes and values which could cause confusion.
2. If the enums are in the same namespace then you'll get a compilation error as duplicate classes and interfaces will be created.