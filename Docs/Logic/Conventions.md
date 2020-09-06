# Conventions

Use the provided ClangFormat configuration code styling.

Stick to [Microsoft's naming guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/capitalization-conventions)

See `Docs/VisualStudio` for Visual Studio additional information.

## Omit Braces for Single Lines

Omit braces for conditionals and loops, if they contain only one statement.

```csharp
if (Controls.PitchYawRoll.x < 0.0f)
    rates.x *= FlightModelParams.PitchUpRateModifier;
```

Use braces if the respective other block(s) of a conditional uses braces.

```csharp
if (otherHealth)
{
    otherHealth.ReceiveDamage(Damage);
    OnHit?.Invoke();
}
else if (Target)
{
    OnMiss?.Invoke();
}
```

## Implicit Visibility

In C# visibility defaults to the most restricted visibility.
Omit visibility modifiers unless needed.
This also includes Unity messages (e.g. `Start`, `Update`).

```csharp
public class CooldownTimer
{
    public float DefaultPeriod = 1;

    public void Set(float period) { /* ... */ }

    void Update() { /* ... */ }

    float TimeStamp = 0;
}
```

## Public Fields in MonoBehaviours

Do not initialize public fields of MonoBehaviours in code as these values are overwritten by the Prefab or Scene.
Simply set them in the corresponding Prefab with the Unity Editor.
The values are stored in the Prefab's `.meta` file.

Note that you can also provide [custom presets](https://docs.unity3d.com/Manual/Presets.html).
While such a preset can be used to provide sane default values for components, updating a preset does not propagate the update to any components.
Use Prefabs for this purpose.

## Properties with Backing Fields

Prefix the name of the field with an underscore (`_`).

```csharp
public int Amount {
    get { return _amount; }
    set {
        _amount = value;
        /* ... */
    }
}

int _amount = 5;
```