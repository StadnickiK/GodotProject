using Godot;
using System;

public interface IStatOperations<T>
{
    T Clone(T value);

    T Add(T a, T b);

    T Add(T a, float b);

    T Subtract(T a, T b);

    T Subtract(T a, float b);
    T Multiply(T value, T multiplier);

    T Multiply(T value, float multiplier);
    T Clamp(T value, T min, T max);

    string ToString(T value);
}
