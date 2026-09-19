# EasyDI

A lightweight Dependency Injection (DI) framework for Unity.

EasyDI supports hierarchical contexts, field/property/method injection, tags, singleton/transient resolution, component-based resolution, and decorator chains through `IEasyDIDecore<T>`.

## Package Information

- Package: `com.doantuan.easydi`
- Version: `1.2.5`
- Unity package metadata: `2019.1`
- Repository: https://github.com/wakawa2003/EasyDI/

---

# Installation

## Option 1 — Unity Package Manager

Open **Window → Package Manager → Add package from Git URL...** and use:

```text
https://github.com/wakawa2003/EasyDI-Core-upm.git
```

## Option 2 — Copy the package manually

Copy the `Plugin/EasyDI` folder into your Unity project.

---

# Context Hierarchy

EasyDI resolves dependencies through a hierarchy of contexts:

```text
ProjectContext
    └── SceneContext
          └── GameObjectContext
```

A child context has priority over its parent for normal bindings.

The test project verifies this behavior:

- `ProjectContext` binds `string` to `"string from project context"`.
- The character `GameObjectContext` binds `string` to `"string from character context"`.
- The injected value on the character comes from the character context instead of the project context.

Example from the test:

```csharp
// ProjectContext
ContainerBinding.Bind<string>()
    .To<string>()
    .FromInstance("string from project context");

// Character GameObjectContext
ContainerBinding.Bind<string>()
    .To<string>()
    .FromInstance("string from character context");
```

The nearest matching normal binding wins.

---

# Basic Binding

Register a dependency with `Bind<T>()`.

### 🟢 Recommended

```csharp
ContainerBinding.Bind<string>()
    .To<string>()
    .FromInstance("Hello EasyDI");
```

Then inject it:

```csharp
public class CharacterController : MonoBehaviour
{
    [Inject]
    public string message;
}
```

---

# Tags

Tags allow multiple bindings of the same type to coexist.

### Registration

```csharp
ContainerBinding.Bind<string>(tags.tag1)
    .To<string>()
    .FromInstance("character string tag1");

ContainerBinding.Bind<string>(tags.tag2)
    .To<string>()
    .CustomGetInstance((obj, member) => "scene string tag2");
```

### Injection

```csharp
[Inject(tags.tag1)]
public string stringFieldTag1;

[Inject(tags.tag2)]
public string stringFieldTag2;
```

The test also uses tags for method injection.

---

# Method Injection

`[Inject]` can be placed on methods. Every method parameter is resolved from the container.

### Test example

```csharp
[Inject(tags.tagStringMethod1)]
void Method(string param1)
{
    this.stringInMethod = param1;
}
```

A method can request multiple dependencies:

```csharp
[Inject(tags.tagStringMethod2)]
void Method2(string param1, int intInmethod)
{
    this.stringInMethod = param1;
    this.intInmethod = intInmethod;
}
```

The test registers both dependencies with the same tag:

```csharp
ContainerBinding.Bind<string>(tags.tagStringMethod2)
    .To<string>()
    .FromInstance(stringDefault);

ContainerBinding.Bind<int>(tags.tagStringMethod2)
    .To<int>()
    .FromInstance(characterInstaller.intInMethod);
```

---

# Field, Property and Inherited Injection

EasyDI scans fields, properties, and methods marked with `[Inject]`.

Injection is also searched through the class hierarchy.

The editor test uses:

```csharp
class BaseClass
{
    [Inject] public int BaseField;

    [Inject]
    public void BaseMethod() { }
}

class DerivedClass : BaseClass
{
    [Inject]
    public void DerivedMethod() { }
}
```

The test verifies that all three members are detected.

---

# Singleton

Use `AsSingleton()` when one instance should be reused.

### 🟢 Recommended

```csharp
ContainerBinding.Bind<classIsSingleton>()
    .To<classIsSingleton>()
    .CustomGetInstance((obj, member) =>
    {
        return new classIsSingleton("singleton id");
    })
    .AsSingleton();
```

Both the character and gun test controllers inject the same `classIsSingleton` instance.

The test checks:

```csharp
if (characterController.classIsSingleton != gunController.classIsSingleton)
{
    Debug.LogError("singleton inject fail!");
}
```

---

# Transient

Use `AsTransient()` when a binding should resolve through its creation function for each injection.

### 🟢 Recommended

```csharp
ContainerBinding.Bind<iAttacker>()
    .To<iAttacker.Temp>()
    .CustomGetInstance((obj, member) => new iAttacker.Temp())
    .AsTransient();
```

The test uses transient bindings for `iAttacker`, `iHealth` decorators, `iDamage` decorators, and `iCharacter` decorators.

---

# Component Resolution

EasyDI also supports resolving Unity components from the current object hierarchy.

### 🟢 From the GameObject and its children

```csharp
ContainerBinding.Bind<Transform>()
    .To<Transform>()
    .FromComponentInChild();
```

### 🟢 From the current GameObject

```csharp
ContainerBinding.Bind<MyComponent>()
    .To<MyComponent>()
    .FromThisGameObject();
```

### 🟢 From the current GameObject and its parents

```csharp
ContainerBinding.Bind<MyComponent>()
    .To<MyComponent>()
    .FromThisAndParent();
```

`FromComponentInChild()` is used by the included demo installers.

---

# Decorators

EasyDI has a dedicated decorator system based on `IEasyDIDecore<T>`.

`Decore<T>()` does **not** behave like a second normal `Bind<T>()`. It builds a decorator chain around an object that implements `IEasyDIDecore<T>`.

The included tests use this to add speed, health, damage, attacker, and character behavior.

## 1. The decorated interface

The interface must inherit from `IEasyDIDecore<T>`:

```csharp
public interface iSpeed : IEasyDIDecore<iSpeed>
{
    public float Speed { get; set; }
}
```

## 2. The root object

The object receiving the decorators must implement the decorated interface and provide its `Decore` / `PrevDecore` members.

From the test:

```csharp
public class characterController : MonoBehaviour, iCharacter, iSpeed, iHealth, iDamage
{
    [Inject]
    iSpeed IEasyDIDecore<iSpeed>.Decore { get; set; }

    iSpeed IEasyDIDecore<iSpeed>.PrevDecore { get; set; }

    public float Speed
    {
        get => (this as iSpeed).Decore == null
            ? 0
            : (this as iSpeed).Decore.Speed;
        set { }
    }
}
```

## 3. The decorator

A decorator normally injects the previous decorated value through `Decore`.

```csharp
public class buffSpeed : iSpeed
{
    [Inject]
    public iSpeed Decore { get; set; }

    public iSpeed PrevDecore { get; set; }

    public float Speed
    {
        get => Decore == null
            ? characterInstaller.buffSpeedValue1
            : Decore.Speed + characterInstaller.buffSpeedValue1;
        set { }
    }
}
```

## 4. Register the decorator

### 🟢 Recommended

```csharp
ContainerBinding.Decore<iSpeed>()
    .To<buffSpeed>()
    .CustomGetInstance((obj, member) => new buffSpeed());
```

---

# Multiple Decorators

Multiple `Decore<T>()` registrations can be added for the same type.

The test registers three character-level speed decorators and one scene-level speed decorator:

```csharp
ContainerBinding.Decore<iSpeed>()
    .To<buffSpeed>()
    .CustomGetInstance((a, b) => new buffSpeed());

ContainerBinding.Decore<iSpeed>()
    .To<buffSpeed>()
    .CustomGetInstance((a, b) => new buffSpeed());

ContainerBinding.Decore<iSpeed>()
    .To<buffSpeed2>()
    .CustomGetInstance((a, b) => new buffSpeed2());
```

Scene context adds another decorator:

```csharp
ContainerBinding.Decore<iSpeed>()
    .To<buffSpeedInScene>()
    .CustomGetInstance((a, b) => new buffSpeedInScene());
```

The test verifies the resulting speed:

```csharp
if (sceneInstaller.buffSpeedValue
    + characterInstaller.buffSpeedValue1 * 2
    + characterInstaller.buffSpeedValue2
    != characterController.Speed)
{
    Debug.LogError("decorator buffSpeed fail!!!");
}
```

With the test values:

```text
Scene decorator       = 3
buffSpeed2             = 7
buffSpeed              = 5
buffSpeed              = 5
--------------------------------
Final Speed            = 20
```

This demonstrates that decorator layers can accumulate behavior instead of replacing the underlying object.

---

# `Decore` and `PrevDecore`

`IEasyDIDecore<T>` exposes:

```csharp
T Decore { get; set; }
T PrevDecore { get; set; }
```

During `AddDecore()` the framework:

1. keeps the current root decorator chain;
2. assigns the new decorator as the root's `Decore`;
3. stores the old `Decore` inside the new decorator's `Decore`;
4. assigns the root object to the new decorator's `PrevDecore`.

Conceptually:

```text
Root
  └── Decore -> NewDecorator
                    └── Decore -> OldDecorator
```

The actual implementation also uses `PrevDecore` when removing a decorator from the chain.

---

# Multiple Decorated Interfaces

A class can implement several decorated interfaces.

The test uses this pattern for `iCharacter`, `iHealth`, `iAttacker`, and `iDamage`.

When a class implements several `IEasyDIDecore<T>` interfaces, explicit interface implementation can avoid name collisions:

```csharp
public class buffCharacter : iCharacter
{
    [Inject]
    iHealth IEasyDIDecore<iHealth>.Decore { get; set; }

    iHealth IEasyDIDecore<iHealth>.PrevDecore { get; set; }

    [Inject]
    iDamage IEasyDIDecore<iDamage>.Decore { get; set; }

    iDamage IEasyDIDecore<iDamage>.PrevDecore { get; set; }
}
```

---

# `CustomGetInstance` vs `FromInstance`

This distinction is especially important for decorators.

## 🟢 Recommended for decorators: `CustomGetInstance`

```csharp
ContainerBinding.Decore<iSpeed>()
    .To<buffSpeed>()
    .CustomGetInstance((obj, member) => new buffSpeed());
```

`CustomGetInstance()` calls the factory when EasyDI resolves the dependency.

The test uses this pattern for its decorator chain:

```csharp
ContainerBinding.Decore<iSpeed>()
    .To<buffSpeed>()
    .CustomGetInstance((a, b) => new buffSpeed());

ContainerBinding.Decore<iSpeed>()
    .To<buffSpeed2>()
    .CustomGetInstance((a, b) => new buffSpeed2());
```

## 🔴 Do NOT use `FromInstance()` for these decorator bindings

The test contains these decorator forms only as commented-out code:

```csharp
// ContainerBinding.Decore<iSpeed>()
//     .To<buffSpeed>()
//     .FromInstance(new buffSpeed());

// ContainerBinding.Decore<iSpeed>()
//     .To<buffSpeed2>()
//     .FromInstance(new buffSpeed2());
```

For decorator chains, avoid this pattern when each injection should receive the corresponding newly resolved decorator instance.

`FromInstance()` stores the exact object you provide, so the same object can be returned repeatedly. `CustomGetInstance()` instead creates the object through the registered factory for the resolution.

### Important distinction

`FromInstance()` is **not generally forbidden**. The test suite legitimately uses it for ordinary value bindings:

```csharp
ContainerBinding.Bind<string>()
    .To<string>()
    .FromInstance(stringDefault);

ContainerBinding.Bind<int>()
    .To<int>()
    .FromInstance(intInMethod);
```

The warning is specifically about using `FromInstance()` for decorator objects where per-resolution creation is required.

---

# `AsTransient` and `AsSingleton` with Decorators

The tests explicitly use transient decorators for several interfaces:

```csharp
ContainerBinding.Decore<iHealth>()
    .To<buffHealth>()
    .CustomGetInstance((obj, member) => new buffHealth(buffHeallValue))
    .AsTransient();

ContainerBinding.Decore<iDamage>()
    .To<buffDamage>()
    .CustomGetInstance((obj, member) => new buffDamage())
    .AsTransient();

ContainerBinding.Decore<iCharacter>()
    .To<buffCharacter>()
    .CustomGetInstance((obj, member) => new buffCharacter())
    .AsTransient();
```

Use the lifetime explicitly when the lifetime is important to the design.

---

# `Where(...)`

Bindings can optionally define a condition with `Where(...)`.

The API is:

```csharp
ContainerBinding.Bind<MyType>()
    .To<MyTypeImpl>()
    .Where((instance, member) => true)
    .CustomGetInstance((instance, member) => new MyTypeImpl());
```

The predicate receives:

- the object currently being injected;
- the member being injected.

---

# Android / Managed Stripping

`Decore<T>()` relies on `IEasyDIDecore<T>` and reflection-based member discovery.

For Android builds, use a low stripping level when decorator code is required at runtime.

### 🟢 Recommended

```text
Managed Stripping Level: Minimal
```

or:

```text
Managed Stripping Level: Low
```

### 🔴 Avoid for decorator builds

```text
Managed Stripping Level: Medium
Managed Stripping Level: High
```

Higher stripping levels can remove members that EasyDI needs to discover at runtime. If `Decore<T>()` works in the Unity Editor but fails in an Android build, check the Managed Stripping Level first.

---

# Quick Reference

| API | Usage |
|---|---|
| 🟢 `Bind<T>()` | Register a normal dependency |
| 🟢 `Decore<T>()` | Add a decorator to an `IEasyDIDecore<T>` chain |
| 🟢 `To<T>()` | Select the concrete implementation |
| 🟢 `CustomGetInstance(...)` | Create the resolved object through a factory |
| 🟢 `FromInstance(...)` | Good for fixed/shared values and instances |
| 🔴 `Decore<T>().FromInstance(...)` | Avoid when each decorator resolution needs a new corresponding instance |
| 🟢 `AsSingleton()` | Reuse the resolved instance |
| 🟢 `AsTransient()` | Resolve through the creation path for each injection |
| 🟢 `[Inject]` | Field/property/method injection |
| 🟢 `[Inject("tag")]` | Tagged injection |
| 🟢 `FromComponentInChild()` | Resolve a Unity component from the object/children |
| 🟢 `FromThisGameObject()` | Resolve a Unity component from the current GameObject |
| 🟢 `FromThisAndParent()` | Resolve a Unity component from the current GameObject/parents |

---

# Test / Example Sources

The repository contains concrete usage examples in:

- `Editor/Unit test/Tests Playmode/TestDI.cs`
- `Editor/Unit test/Tests Playmode/Test/characterInstaller.cs`
- `Editor/Unit test/Tests Playmode/Test/sceneInstaller.cs`
- `Editor/Unit test/Tests Playmode/Test/characterController.cs`
- `Editor/Unit test/Tests Playmode/Test/Interfaces/`
- `Examples/Scripts/`

The main Play Mode test checks:

- normal injection;
- context override;
- tagged injection;
- singleton reuse;
- method injection;
- decorator chaining;
- decorator value accumulation;
- reflection-based inherited injection discovery.

---

# Example: Full Decorator Setup From the Tests

### Root interface

```csharp
public interface iSpeed : IEasyDIDecore<iSpeed>
{
    public float Speed { get; set; }
}
```

### Root object

```csharp
public class characterController : MonoBehaviour, iSpeed
{
    [Inject]
    iSpeed IEasyDIDecore<iSpeed>.Decore { get; set; }

    iSpeed IEasyDIDecore<iSpeed>.PrevDecore { get; set; }

    public float Speed =>
        (this as iSpeed).Decore == null
            ? 0
            : (this as iSpeed).Decore.Speed;
}
```

### Decorator

```csharp
public class buffSpeed : iSpeed
{
    [Inject]
    public iSpeed Decore { get; set; }

    public iSpeed PrevDecore { get; set; }

    public float Speed =>
        Decore == null
            ? 5
            : Decore.Speed + 5;
}
```

### Registration

```csharp
ContainerBinding.Decore<iSpeed>()
    .To<buffSpeed>()
    .CustomGetInstance((obj, member) => new buffSpeed());

ContainerBinding.Decore<iSpeed>()
    .To<buffSpeed2>()
    .CustomGetInstance((obj, member) => new buffSpeed2());
```

---

# License

See the repository for the current license information.
