using System;
using System.Drawing;

namespace Figures;

// Как я понял, "Вычисление площади фигуры без знания типа фигуры в compile-time"
// подразумевает использование интерфейса либо наследования

// У меня каждая фигура будет реализовывать интерфейс IFigure, чтобы у неё было определено свойство Area.
// Это тестируем в "TestInterface".

// "Напишите SQL запрос для выбора всех пар «Имя продукта – Имя категории».
// Если у продукта нет категорий, то его имя все равно должно выводиться."

// Думаю, примерно так:
// Select ProductName, CategoryName
// From Products Left Join Categories
// On Product.Id = Categories.ProductId


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
    // Чтобы получить треугольник с совершенно другими размерами, проще создать новый,
    // чем постепенно подтягивать каждую сторону, не нарушая это правило
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
        if (a > b && a > c && Math.Abs(a * a - (b * b + c * c)) <= double.Epsilon ||
            b > a && b > c && Math.Abs(b * b - (a * a + c * c)) <= double.Epsilon ||
            c > a && c > b && Math.Abs(c * c - (a * a + b * b)) <= double.Epsilon) IsRightTriangle = true;
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
            // Можно проверять их в методе свойства Vertices
            // Но, поскольку Vertices - public массив, можно подменить элементы по индексу уже после проверки,
            // и тогда проверка на переполнение в этом месте будет кстати
            checked
            {
                long res = Vertices[0].Y * Vertices[^1].X - Vertices[0].X * Vertices[^1].Y;
                for (int i = 1; i < Vertices.Length; i++)
                    res += Vertices[i - 1].X * Vertices[i].Y - Vertices[i - 1].Y * Vertices[i].X;
                return Math.Abs(res) / 2;
            }
        }
    }

    public ArbitraryFigure(params Point[] vertices) => Vertices = vertices;
}


// 3 класс. Произвольная фигура, заданная массивом любых элементов
// и функцией для вычисления её площади, на основе заданных элементов
// (на случай, если внешний клиент совсем ленивый и не хочет писать свой класс)

// Этот класс самый ненадёжный, т.к. легко ошибиться и передать аргументы, не соответствующие друг другу
// Разные уязвимые сценарии рассмотренны в TestGenericArbitraryFigure

// Внешнему клиенту лучше не использовать этот класс.
// На мой взгляд, клиенту проще и разумнее написать для уникальной фигуры свой класс, реализовав интерфейс IFigure
// Без требования "легкости добавления других фигур" я бы не стал добавлять этот класс в библиотеку
public class ArbitraryFigure<T> : IFigure
{
    public T[] Elements { get; set; }
    public Func<T[], double> CalcArea { get; set; }

    public double Area => CalcArea(Elements);

    public ArbitraryFigure(Func<T[], double> calcArea, params T[] elements)
    {
        // Работоспособность функции, выдача исключений и соответствие между аргументами в elements и функцией calcArea
        // в ответственности внешнего клиента
        Elements = elements;
        CalcArea = calcArea;
    }
}


// "Легкость добавления других фигур" с точки зрения разработчика библиотеки склоняет к использованию
// стандартной реализации методов/свойств интерфейса или базового класса,
// но у фигур вроде бы нет слишком универсальных черт, кроме наличия площади и периметра, но их реализация зависит от фигуры
