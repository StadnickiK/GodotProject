using Godot;

public class FloatStatOperations : IStatOperations<float>
{
    public float Add(float a, float b) => a + b;

    public float Multiply(float value, float multiplier)
        => value * multiplier;

    public float Clamp(float value, float min, float max)
        => Mathf.Clamp(value, min, max);

    public float Clone(float value)
    {
        return value;
    }

    public string ToString(float value)
    {
        return value.ToString();
    }

    public float Subtract(float a, float b) => a - b;


}
