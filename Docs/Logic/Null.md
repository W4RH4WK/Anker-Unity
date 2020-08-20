# NULL

C# provides special operators for dealing with `null`.

- [?? operator](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator)
- [?. and ?[] operators](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-conditional-operators)

These operators must *not* be used with Unity objects.

Upon destroying a Unity object (via `Destroy`), the object is not immediately removed, but the internal state is modified such that `null`-comparisons return true.
Similarly, the Boolean value of a destroyed object is false.

However, the actual reference to this object persists.
The operators described here bypass custom comparison and conversion operators and check the actual reference instead.
In other words, they won't behave as expected with Unity objects.
