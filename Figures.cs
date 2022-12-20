using System;
using System.Data;
using System.Drawing;

namespace Figures;

// Как я понял, "Вычисление площади фигуры без знания типа фигуры в compile-time"
// подразумевает использование интерфейса либо наследования

// У меня каждая фигура будет реализовывать интерфейс IFigure, чтобы у неё было определено свойство Area.
// Это тестируем в "TestInterface".

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

    // Если результат - бесконечность, предполагаем, что пользователь готов с этим работать
    // Можно хранить значение площади в отдельном поле и пересчитывать только тогда, когда меняется радиус
    public double Area => Radius * Radius * Math.PI;

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


// Для "легкости добавления других фигур" с точки зрения внешнего клиента написал следующие классы:
// 1 класс. Правильный многоугольник с любым количеством сторон >= 3 и любой длиной сторон >= 0
public class EquilateralPolygon : IFigure
{
    private int numberOfSides;
    private double lengthOfSide;

    public int NumberOfSides
    {
        get => numberOfSides;
        set
        {
            if (value < 3) throw new ArgumentException("Количество сторон не может быть меньше 3", nameof(value));
            numberOfSides = value;
        }
    }
    public double LengthOfSide
    {
        get => lengthOfSide;
        set
        {
            if (double.IsNegative(value) || double.IsNaN(value))
                throw new ArgumentException("Длина стороны не может быть отрицательной или равной NaN", nameof(value));
            lengthOfSide = value;
        }
    }

    // Если результат - бесконечность, предполагаем, что пользователь готов с этим работать
    public double Area =>
        NumberOfSides * (LengthOfSide * LengthOfSide) / (4 * Math.Tan(180 / NumberOfSides * (Math.PI / 180)));

    public EquilateralPolygon(int numberOfSides, double lengthOfSide)
    {
        NumberOfSides = numberOfSides;
        LengthOfSide = lengthOfSide;
    }
}


// 2 класс. Произвольная фигура на основе любого количества координат >= 1
// В качестве координаты используем System.Drawing.Point
// Этот класс не проверяет корректность координат - эта ответственность на внешнем клиенте
// Разные уязвимые сценарии тестируем в TestArbitraryFigure
public class ArbitraryFigure : IFigure
{
    public Point[] Vertices { get; set; }

    public double Area
    {
        get
        {
            // Здесь можно получить переполнение, т.к. мы не проверяем полученные координаты
            // Можно проверять их сразу в конструкторе или в методе свойства Vertices
            // Но поскольку Vertices - public массив, можно подменить один элемент уже после проверки,
            // и тогда проверка здесь на переполнение будет кстати
            checked
            {
                long res = Vertices[0].Y * Vertices[^1].X - Vertices[0].X * Vertices[^1].Y;
                for (int i = 1; i < Vertices.Length; i++)
                    res += Vertices[i - 1].X * Vertices[i].Y - Vertices[i - 1].Y * Vertices[i].X;
                return Math.Abs(res) / 2;
            }
        }
    }

    public ArbitraryFigure(params Point[] vertices)
    {
        if (vertices == null || vertices.Length == 0) throw new ArgumentException(null, nameof(vertices));
        Vertices = vertices;
    }
}


// 3 класс. Универсальная фигура, заданная массивом любых элементов
// и функцией для вычисления её площади, на основе заданных элементов
// (на случай, если внешний клиент совсем ленивый и не хочет писать свой класс)

// Этот класс самый ненадёжный, т.к. легко ошибиться и передать в elements аргументы
// в неправильном порядке (если для функции важен порядок). Разные уязвимые сценарии рассмотренны в TestUniversalFigure
public class UniversalFigure<T> : IFigure
{
    public T[] Elements { get; }
    public Func<T[], double> CalcArea { get; }

    public double Area => CalcArea(Elements);

    public UniversalFigure(Func<T[], double> calcArea, params T[] elements)
    {
        if (elements == null || elements.Length == 0) throw new ArgumentException(null, nameof(elements));

        CalcArea = calcArea ?? throw new ArgumentException(null, nameof(calcArea));
        Elements = elements;

        // Сразу проверяем работоспособность внешней функции с заданными элементами (чтобы хотя бы не выбрасывала исключения)
        // Работоспособность функции, выдача исключений и соответствие между аргументами и функцией
        // в ответственности внешнего клиента
        CalcArea(Elements);
    }
}


// "Легкость добавления других фигур" с точки зрения разработчика библиотеки склоняет к использованию интерфейса или наследования.
// Для большей лёгкости можно было бы создать стандартную реализацию методов/свойств интерфейса или базового класса,
// но у фигур вроде бы нет слишком универсальных черт, кроме наличия площади и периметра, но их реализация зависит от фигуры


// Класс Calc, как набор статических методов для тех, кто не хочет создавать экземпляры фигур
public static class Calc
{
    public static double GetAreaCircleByRadius(double radius)
    {
        // Если значение отрицательное или не является числом, предполагаем, что аргумент неправильный
        if (double.IsNegative(radius) || double.IsNaN(radius))
            throw new ArgumentException("Значение аргумента отрицательное или NaN", nameof(radius));

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


// "Напишите SQL запрос для выбора всех пар «Имя продукта – Имя категории».
// Если у продукта нет категорий, то его имя все равно должно выводиться."

// Думаю, примерно так:
// Select ProductName, CategoryName
// From Products Left Join Categories
// On Product.Id = Categories.ProductId