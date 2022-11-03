using System;

namespace Figures;

// Написано на C# 6.0
// Юнит тесты внутри решения

// Как я понял, "Вычисление площади фигуры без знания типа фигуры в compile-time"
// подразумевает использование интерфейса либо наследования

// У меня каждая фигура будет реализовывать интерфейс IFigure, чтобы у неё было определено свойство Area,
// которое можно использовать, не зная конкретный тип фигуры, просто приводя фигуру к интерфейсу

// Как я понял, "Легкость добавления других фигур" также склоняет к использованию интерфейса или наследования.
// Для большей лёгкости можно было бы создать стандартную реализацию методов/свойств интерфейса или базового класса,
// но у фигур вроде бы нет слишком универсальных черт, кроме наличия площади

// "Напишите SQL запрос для выбора всех пар «Имя продукта – Имя категории».
// Если у продукта нет категорий, то его имя все равно должно выводиться."

// Думаю, примерно так:
// Select ProductName, CategoryName
// From Products Left Join Categories
// On Product.Id = Categories.ProductId
// Я не работал с SQL, только изучал на Metanit, поэтому такие запросы я не могу писать на ходу. Нужна практика.


public interface IFigure
{
    public double Area { get; }
}

public class Circle : IFigure
{
    private double radius;
    public double Radius
    {
        get => radius;
        set
        {
            if (double.IsNegative(value) || double.IsNaN(value))
                throw new ArgumentException("Радиус не может быть отрицательным или равным NaN", nameof(value));
            radius = value;
        }
    }
    public double Area
    {
        get
        {
            // Не используем Math.Pow, чтобы работало чуть быстрее
            // Если результат - бесконечность, предполагаем, что пользователь готов с этим работать
            // Можно хранить значение площади в отдельном поле и пересчитывать только тогда, когда меняется радиус
            return Radius * Radius * Math.PI;
        }
    }

    public Circle(double radius) => Radius = radius;
}

public class Triangle : IFigure
{
    // Пусть стороны будут неизменяемы, чтобы не проверять нарушение правила: сумма длин двух сторон больше третьей
    public double SideA { get; }
    public double SideB { get; }
    public double SideC { get; }

    public double Area { get; }
    public bool IsRightTriangle { get; } = false;

    public Triangle(double a, double b, double c)
    {
        if (a <= 0 || b <= 0 || c <= 0 || double.IsNaN(a) || double.IsNaN(b) || double.IsNaN(c))
            throw new ArgumentException("Значение аргумента NaN или <= 0", nameof(a));

        // Используем Epsilon, чтобы точно убедиться, что агрументы правильные
        if (a + b - c < double.Epsilon || a + c - b < double.Epsilon || b + c - a < double.Epsilon)
            throw new ArgumentException("Сумма длин двух сторон должна быть больше длины третьей стороны", nameof(a));

        SideA = a;
        SideB = b;
        SideC = c;

        // Сразу вычислим площадь и сохраним в свойство. Т.к. стороны неизменяемы, можно не пересчитывать
        // Если результат - бесконечность, предполагаем, что пользователь готов с этим работать
        double s = (a + b + c) / 2;
        Area = Math.Sqrt(s * (s - a) * (s - b) * (s - c));

        // И сразу определим, прямоугольный ли треугольник
        if (Math.Abs(a * a - (b * b + c * c)) <= double.Epsilon ||
            Math.Abs(b * b - (a * a + c * c)) <= double.Epsilon ||
            Math.Abs(c * c - (a * a + b * b)) <= double.Epsilon) IsRightTriangle = true;
    }
}


// Класс Calc, как набор статических методов для тех, кто не хочет создавать экземпляры фигур
public static class Calc
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

    public static bool IsRightTriangle(double a, double b, double c)
    {
        if (a <= 0 || b <= 0 || c <= 0 || double.IsNaN(a) || double.IsNaN(b) || double.IsNaN(c))
            throw new ArgumentException("Значение аргумента NaN или <= 0", nameof(a));

        // Используем Epsilon, чтобы точно убедиться, что агрументы правильные
        if (a + b - c < double.Epsilon || a + c - b < double.Epsilon || b + c - a < double.Epsilon)
            throw new ArgumentException("Сумма длин двух сторон должна быть больше длины третьей стороны", nameof(a));

        if (Math.Abs(a * a - (b * b + c * c)) <= double.Epsilon ||
            Math.Abs(b * b - (a * a + c * c)) <= double.Epsilon ||
            Math.Abs(c * c - (a * a + b * b)) <= double.Epsilon) return true;
        return false;
    }
}