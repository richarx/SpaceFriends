using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Tools 
{
    public static Vector3 ToVector3(this Vector2 vector, float z = 0.0f)
    {
        return new Vector3(vector.x, vector.y, z);
    }
    
    public static Vector2 ToVector2(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static float Distance(this Vector2 position, Vector2 other)
    {
        return (other - position).magnitude;
    }

    public static Quaternion ToRotation(this Vector2 direction)
    {
        return Quaternion.AngleAxis(DirectionToDegree(direction), Vector3.forward);
    }

    public static Vector2 AddAngleToDirection(this Vector2 direction, float angle)
    {
        float directionAngle = DirectionToDegree(direction);
        float newAngle = directionAngle + angle;
        return DegreeToVector2(newAngle).normalized;
    }
    
    public static Vector2 AddRandomAngleToDirection(this Vector2 direction, float minInclusive, float maxInclusive)
    {
        float directionAngle = DirectionToDegree(direction);
        float newAngle = directionAngle + Random.Range(minInclusive, maxInclusive);
        return DegreeToVector2(newAngle).normalized;
    }

    public static float DirectionToDegree(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
    
    public static float ToDegree(this Vector2 direction)
    {
        return DirectionToDegree(direction);
    }

    public static Vector2 RadianToVector2(float radian, float length = 1.0f)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized * length;
    }

    public static Vector2 DegreeToVector2(float degree, float length = 1.0f)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad).normalized * length;
    }

    public static float RandomPositiveOrNegative(float number = 1.0f)
    {
        int random = (Random.Range(0, 2) * 2) - 1;
        return random * number;
    }

    public static bool RandomBool()
    {
        return RandomPositiveOrNegative() > 0;
    }

    public static Vector2 RandomPositionInRange(Vector2 position, float range)
    {
        return position + (Random.insideUnitCircle * range);
    }
    
    public static Vector2 RandomPositionAtRange(Vector2 position, float range)
    {
        return position + (Random.insideUnitCircle * range);
    }
    
    public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount = 1)
    {
        return list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
    }

    public static float NormalizeValue(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }
    
    public static float NormalizeValueInRange(float value, float min, float max, float rangeMin, float rangeMax)
    {
        return ((rangeMax - rangeMin) * ((value - min) / (max - min))) + rangeMin;
    }

    public static void DrawCone(Vector2 position, Vector2 direction, float angle, float distance, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(position, position + (direction.AddAngleToDirection(angle) * distance));
        Gizmos.DrawLine(position, position + (direction.AddAngleToDirection(-angle) * distance));
    }
}
