---
layout: default
title: Stravaig.FeatureFlags - Inception
---

# Inception

The idea for this package came from Mark Seemann's book "Code That Fits in Your Head", chapter 10 section 1. 

![Code That Fits in Your Head by Mark Seemann](book-cover.jpeg)

## Feature Flags

Wikipedia describes [Feature Flags](https://en.wikipedia.org/wiki/Feature_toggle) as:

> A **feature toggle** in software development provides an alternative to maintaining multiple feature branches in source code. A condition within the code enables or disables a feature during runtime. In agile settings the toggle is used in production, to switch on the feature on demand, for some or all the users. Thus, feature toggles do make it easier to release often. Advanced roll out strategies such as canary roll out and A/B testing are easier to handle.
> 
> Even if new releases are not deployed to production continuously, continuous delivery is supported by feature toggles. The feature is integrated into the main branch even before it is completed. The version is deployed into a test environment once, the toggle allows to turn the feature on, and test it. Software integration cycles get shorter, and a version ready to go to production can be provided.
> 
> The third use of the technique is to allow developers to release a version of a product that has unfinished features. These unfinished features are hidden (toggled) so that they do not appear in the user interface. There is less effort to merge features into and out of the productive branch, and hence allows many small incremental versions of software.

## How Mark Seemann uses Feature Flags in the book

In the book he gives and example injecting a `CalendarFlag` into the `Controller` derived class, where the `CalendarFlag` is a value object whose value is ultimately derived from a config variable.

In the code example he gives in the book he reads the config value at app start up and creates a singleton, like this:

```csharp
var calendarFlag = new CalendarFlag(Configuration.GetValue<bool>("EnableCalendar"));
services.AddSingleton(calendarFlag);
```

Which can then be used like this:

```csharp
public class HomeController
{
    private readonly CalendarFlag _calendarFlag;
    public HomeController(CalendarFlag flag)
    {
        _calendarFlag = flag;
    }

    public IActionResult Get()
    {
        if (_calendarFlag.Enabled)
        {
            // Do flag on thing
        }
        else
        {
            // Do flag off thing
        }
    }
}
```

He then goes on to show an example of a test where the `CalendarFlag` class is just created like this `new CalendarFlag(true)` when it is needed (or `false` if the test is for when the flag is off).

The (abridged) code for `CalendarFlag` is:

```csharp
public class CalendarFlag
{
    public CalendarFlag(bool enabled)
    {
        Enabled = enabled;
    }

    public bool Enabled { get; }
}
```

## How to package this in a reusable form?

.NET already has a feature management package, [Microsoft.FeatureManagement](https://www.nuget.org/packages/Microsoft.FeatureManagement). You can inject a `IFeatureManager` into your class and query it via the string name of the feature.

Because of this, I'd taken to creating an `enum` somewhere in a common area of code listing my feature flag names, then making a call like:

```csharp
bool flagIsEnabled = await _featureManager.IsEnabledAsync(nameof(MyEnum.MyFlagName))
```

What if that were easier? What if it was as easy as:

```csharp
bool flagIsEnabled = _myFlag.IsEnabled();
```

What if there was no faffing around getting the name out of an `enum`, or a `const` somewhere.

Since a lot of my code already uses `IFeatureManager` I wanted to be able to continue to use that, but I also wanted the easy of a value object, and the safety of not having to faff around with strings.

So, I created this package that wraps up the `IFeatureManager` in a value object like class[^1]. It is wrapped up in a class that takes the `IFeatureManager` as a parameter in its constructor. 

[^1]: A value objects would ordinarily be a `struct`, but that doesn't work with Microsoft's Dependency Injection framework. It doesn't consider a `struct` to be an injectable thing. Also, injecting an `IFeatureManager` into the object makes this further from a real value object as you wouldn't inject anything into a value object other than its value. The `IFeatureManager` simple allows the object to derive its value. So, in fact, I'm now using the term _Value Object_ quite wrongly! About the only thing that makes it close to a value object any more is that the value it represents is immutable, although it doesn't calculate it until you ask for it the first time. Schrödinger's value object, you might say.

It exposes an `IsEnabledAsync()` method, because that's what `IFeatureManager` has, but is also has an `IsEnabled()` synchronous counterpart. The values they expose are not evaluated until they are called for the first time. The they always return the same value each time, so you don't get mid-way through an operation and have the value flip.

You can set it up to use any of the DI's lifetimes. So if you need a feature flag to be consistent for the lifetime of your application's process (`Singleton`) you can have that. If you can deal with the value changing during the lifetime of the application you can chose a `Scoped` or `Transient` lifetime depending on your needs.

The package contains a source generator so you don't have to create the actual value objects yourself - That would be a lot of repetitive boiler plate code.

The only thing you need to to is to create an `enum` decorate it with the appropriate attribute, tell the Feature Management setup it's there, and then use the feature flag values objects via an interface that was set up for them.

The interface gives you the option to use mock objects in your tests, or to use the fakes that can also be source generated for you.

## Code That Fits in Your Head

Code That Fits in Your head
Heuristics for Software Engineering
by Mark Seemann

Find out more:
* [Book Related downloads](https://www.informit.com/store/code-that-fits-in-your-head-heuristics-for-software-9780137464401)
* [O'Reilly](https://www.oreilly.com/library/view/code-that-fits/9780137464302/)
* [Amazon.co.uk](https://www.amazon.co.uk/Code-That-Fits-Your-Head/dp/0137464401)
* [Amazon.com](https://www.amazon.com/Code-That-Fits-Your-Head/dp/0137464401)
* [Reviews on Good Reads](https://www.goodreads.com/book/show/57345272-code-that-fits-in-your-head)

## Footnotes
