using UnityEngine;

public static class Ballistics
{
    public static float StartAngle(float maxHeight, float startSpeed)
    {
        if(startSpeed < MinStartSpeed(maxHeight))
        {
            startSpeed = MinStartSpeed(maxHeight);
        }

        float sinPow2 = (maxHeight * 2 * Mathf.Abs(Physics.gravity.y)) / Mathf.Pow(startSpeed, 2); // Из формулы максимальной высоты снаряда выразили квадратный синус угла полёта снаряда

        float sin = Mathf.Sqrt(sinPow2); // Извлекаем квадратный корень из sinPow2
        
        float angle = Mathf.Asin(sin) * 180f / Mathf.PI; // Перевели синус sin в радианы с помощью arcsin, затем получили угол в градусах

        return angle;
    }

    public static float MinStartSpeed(float maxHeight)
    {
        return Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * maxHeight);
    }

    public static Vector2 StartDirection(float maxHeight, float startSpeed)
    {
        float x = Mathf.Cos(StartAngle(maxHeight, startSpeed) * Mathf.Deg2Rad);

        float y = Mathf.Sin(StartAngle(maxHeight, startSpeed) * Mathf.Deg2Rad);

        return new Vector2(x, y);
    }
}
