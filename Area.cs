using System;

namespace Figures;

public static class Area
{
    public static double GetAreaCircleByRadius(double radius)
    {
        // Если значение отрицательное или не является числом, предполагаем, что аргумент неправильный
        if (double.IsNegative(radius) || double.IsNaN(radius)) 
            throw new ArgumentException("Значение аргумента отрицательное или NaN", nameof(radius));

        // Не используем Math.Pow, чтобы работало чуть быстрее
        double result = radius * radius * Math.PI;

        // Если результат - бесконечность, предполагаем, что пользователю хотелось бы получить исключение
        if (double.IsInfinity(result))
            throw new ArgumentException("Слишком большое значение аргумента привело к бесконечности", nameof(radius));
        return result;
    }

    public static double GetAreaTriangleBySides(double a, double b, double c)
    {
        if (a <= 0 || b <= 0 || c <= 0 || double.IsNaN(a) || double.IsNaN(b) || double.IsNaN(c))
            throw new ArgumentException("Значение аргумента NaN или <= 0", nameof(a));

        // Используем Epsilon, чтобы точно убедиться, что агрументы правильные
        if (a + b - c < double.Epsilon || a + c - b < double.Epsilon || b + c - a < double.Epsilon)
            throw new ArgumentException("Сумма длин двух сторон должна быть больше длины третьей стороны", nameof(a));

        double s = (a + b + c) / 2;
        double result = Math.Sqrt(s * (s - a) * (s - b) * (s - c));

        // Если результат - бесконечность или NaN, предполагаем, что пользователю хотелось бы получить исключение
        if (double.IsInfinity(result) || double.IsNaN(result))
            throw new ArgumentException("Аргументы привели к бесконечности или NaN", nameof(a));
        return result;
    }
}