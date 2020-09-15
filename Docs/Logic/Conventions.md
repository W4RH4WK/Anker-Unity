# Conventions

Use [ClangFormat](https://clang.llvm.org/docs/ClangFormat.html) for formatting code, the provided configuration file should be used automatically.

Stick to [Microsoft's naming guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/capitalization-conventions).

See [`Docs/VisualStudio`](../VisualStudio) for additional information regarding Visual Studio.

## Omit Braces for Single Lines

Omit braces for conditionals and loops if they contain only one statement.

```csharp
if (Controls.PitchYawRoll.x < 0.0f)
    rates.x *= FlightModelParams.PitchUpRateModifier;

foreach (var indicator in gameObject.GetComponentsInUnit<IndicatorLed>())
    indicator.DisplayStatus(status);
```

Use braces for both branches of a conditional if one branch requires braces.

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

In C#, visibility defaults to the most restricted visibility.
Omit visibility modifiers unless needed.
This also includes Unity messages (e.g. `Start`, `Update`).

```csharp
public class CooldownTimer : MonoBehaviour
{
    public float DefaultPeriod;

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
It is therefore recommended to use Prefabs.

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

## Order of Members

Group members of a class by functionality.
Consider using a separator to enhance readability.

Within such a group, roughly adhere to this order:

- Types
- Constants
- Fields / properties
- Methods

Next, order from most visible to least visible, put non-static members over static ones.

Finally, put *all* Unity messages at the bottom in the following order:

- Awake / OnDestroy
- OnEnable / OnDisable
- Start
- Update
- LateUpdate
- FixedUpdate
- OnTriggerXXX
- OnCollisionXXX
- …

Example:

```csharp
public class PlayerMovement : MonoBehaviour
{
    enum State
    {
        Grounded,
        Jumping,
    }

    State CurrentState = State.Grounded;
    bool IsGrounded => CurrentState == State.Grounded;

    //////////////////////////////////////////////////////////////////////////

    public float MoveSpeed;
    float MoveVelocity;

    void UpdateMoveVelocity() { /* ... */ }

    //////////////////////////////////////////////////////////////////////////

    Rigidbody2D RigidBody;

    void Awake() { /* ... */ }

    void FixedUpdate() { /* ... */ }
}
```

## Awake vs. Start

Use `Awake` for initializations that only access the same GameObject.
Use `Start` for initializations that access other GameObjects (including children).

Consider using `OnEnable` to properly support enabling / disabling of GameObjects.
