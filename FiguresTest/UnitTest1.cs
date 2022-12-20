using System;
using System.Drawing;
using static Figures.Calc;

namespace Figures.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        // ������ �����������
    }


    [Test] // ��������� ���� � �����������
    public void TestClasses()
    {
        Assert.Multiple(() =>
        {
            Assert.That(new Circle(0).Area, Is.EqualTo(0));
            Assert.That(new Circle(23).Area, Is.EqualTo(1661.9025137490005));
            Assert.That(new Circle(1.27).Area, Is.EqualTo(5.067074790974977));
            Assert.That(new Circle(972872782742778678578D).Area, Is.EqualTo(2.973459174482516e+42));
            Assert.That(new Circle(double.MaxValue).Area, Is.EqualTo(double.PositiveInfinity));

            Assert.Throws<ArgumentException>(() => new Circle(-1));
            Assert.Throws<ArgumentException>(() => new Circle(double.NaN));

            Assert.That(new Triangle(2, 4.21, 3.47).Area, Is.EqualTo(3.4443907095450124d));
            Assert.That(new Triangle(548543, 645557, 753424).Area, Is.EqualTo(173041408016.02628d));
            Assert.That(new Triangle(9, 3, 11.9999999999).Area, Is.EqualTo(0.00012727809540592021d));
            Assert.That(new Triangle(0.00000001, 0.00000001, 0.00000001).Area, Is.EqualTo(4.3301270189221959E-17d));
            Assert.That(new Triangle(double.MaxValue, double.MaxValue, double.MaxValue).Area, Is.EqualTo(double.PositiveInfinity));

            Assert.That(new Triangle(3, 4, 5).IsRightTriangle, Is.True);
            Assert.That(new Triangle(3, 4, 5.000000000001).IsRightTriangle, Is.False);
            Assert.That(new Triangle(3, 4, 4.999999999999).IsRightTriangle, Is.False);

            Assert.Throws<ArgumentException>(() => new Triangle(9, 3, 12));
            Assert.Throws<ArgumentException>(() => new Triangle(0, 12, 3));
            Assert.Throws<ArgumentException>(() => new Triangle(-24, 2, 5));
            Assert.Throws<ArgumentException>(() => new Triangle(double.NaN, 2, 0.2));
        });
    }


    [Test] // ��������� ���������� ������������� � ����� ����������� ������ >= 3 � ����� ������ ������ >= 0
    public void TestEquilateralPolygon()
    {
        EquilateralPolygon polygon = new(3, 0); // ��� ������� � ������� ������
        Assert.That(polygon.Area, Is.EqualTo(0));

        polygon.LengthOfSide = 0.1;
        Assert.That(polygon.Area, Is.EqualTo(0.0043301270189221959d));

        polygon.LengthOfSide = 3;
        Assert.That(polygon.Area, Is.EqualTo(3.8971143170299753d));

        Assert.That(Math.Round(polygon.Area, 12),                    // ���������� ������ ������� �� ���� �������
            Is.EqualTo(Math.Round(new Triangle(3, 3, 3).Area, 12))); // ������� �� ������� ��������, ���������� ���������

        polygon.NumberOfSides = 5;
        Assert.That(polygon.Area, Is.EqualTo(15.484296605300704d));

        polygon = new EquilateralPolygon(4, 2);
        Assert.That(Math.Round(polygon.Area, 12), Is.EqualTo(4)); // ������� �� ������� ��������, ���������
        // ��������, ����� ����� ������� ��������� � ������� ������� ���� �������

        polygon.NumberOfSides = int.MaxValue;
        Assert.That(polygon.Area, Is.EqualTo(double.PositiveInfinity));


        Assert.Throws<ArgumentException>(() => new EquilateralPolygon(2, 2)); // 2 �������
        Assert.Throws<ArgumentException>(() => new EquilateralPolygon(-1, 2)); // -1 �������
        Assert.Throws<ArgumentException>(() => new EquilateralPolygon(4, -3)); // ������������� �����
    }


    [Test] // ��������� ������������ ������ �� ������ ������ ���������� ��������� >= 1
    public void TestArbitraryFigure()
    {
        var figure = new ArbitraryFigure(new Point(3, 4)); // ������ �����
        Assert.That(figure.Area, Is.EqualTo(0));

        figure = new ArbitraryFigure(new Point(3, 4), new Point(3, 8)); // �������
        Assert.That(figure.Area, Is.EqualTo(0));

        figure = new ArbitraryFigure(new Point(0, 0), new Point(0, 4), new Point(3, 0)); // ������������� �����������
        Assert.That(figure.Area, Is.EqualTo(new Triangle(3, 4, 5).Area));

        figure = new ArbitraryFigure(new Point(0, 0), new Point(0, 4), new Point(0, 16)); // �������������� ����������� - �����
        Assert.That(figure.Area, Is.EqualTo(0)); // �������� 0 ���
                                                 // ����� �������� �������� ��������� � ������������ � �������� ����������
        
        figure = new ArbitraryFigure(new Point(0, 0), new Point(2, 0), new Point(2, 2), new Point(0, 2)); // �������
        Assert.That(figure.Area, Is.EqualTo(4));

        figure = new ArbitraryFigure(new Point(3, 4), new Point(5, 11), new Point(12, 8), new Point(9, 5), new Point(5, 6));
        Assert.That(figure.Area, Is.EqualTo(30));

        figure = new ArbitraryFigure(new Point(int.MinValue, int.MaxValue),                // �������� �����������
            new Point(int.MinValue, int.MinValue), new Point(int.MaxValue, int.MaxValue));
        Assert.Throws<OverflowException>(() => { var x = figure.Area; }); // �������� ������������ � ������ �������� ���
                                                                          // ����� �������� �������� ��������� � ������������

        Assert.Throws<ArgumentNullException>(() => new ArbitraryFigure(null));
        Assert.Throws<ArgumentException>(() => new ArbitraryFigure(Array.Empty<Point>()));
    }


    [Test] // ��������� ������������� ������, �������� �������� ����� ��������� � �������� ��� ���������� � �������
    public void TestUniversalFigure()
    {
        // ����� ������� ��� ���������� ������� ������������ �� ��� ��������
        Func<double[], double> myFunc = new((arrSides) =>
        {
            if (arrSides.Length != 3)
                throw new ArgumentException("����� ������� �� ������������� �������.", nameof(arrSides));

            double a = arrSides[0];
            double b = arrSides[1];
            double c = arrSides[2];

            double s = (a + b + c) / 2;
            return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        });

        // ����� ����������� ����� ��������� � ��������
        var myFigure = new UniversalFigure<double>(myFunc, 3, 4, 5);
        Assert.That(myFigure.Area, Is.EqualTo(new Triangle(3, 4, 5).Area));

        // ������� ������������ ������ ����� ���� ��������� ������
        myFigure.Elements[0] = 74;
        Assert.That(myFigure.Area, Is.EqualTo(double.NaN)); // �������� double.NaN ��� ����� �������� �������� ������ �������

        // �������� �������
        myFunc = new((arrSides) =>
        {
            if (arrSides.Length != 3)
                throw new ArgumentException("����� ������� �� ������������� �������.", nameof(arrSides));

            double a = arrSides[0];
            double b = arrSides[1];
            double c = arrSides[2];

            if (a <= 0 || b <= 0 || c <= 0 || double.IsNaN(a) || double.IsNaN(b) || double.IsNaN(c))
                throw new ArgumentException("�������� ��������� NaN ��� <= 0", nameof(a));

            if (a + b - c < double.Epsilon || a + c - b < double.Epsilon || b + c - a < double.Epsilon)
                throw new ArgumentException("����� ���� ���� ������ ������ ���� ������ ����� ������� �������", nameof(a));

            double s = (a + b + c) / 2;
            return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        });
        myFigure.CalcArea = myFunc;
        Assert.Throws<ArgumentException>(() => { var x = myFigure.Area; }); // ������ ����������� ���������� ������ �������

        // �������� � ����������� ���������� - ������� �������� ���������� � ������������
        Assert.Throws<ArgumentException>(() => myFigure = new UniversalFigure<double>(myFunc, 3, 4, 5, 6));


        // ����� ����������� ����� ��������� � ����� ����� ����
        myFunc = new((elements) =>
        {
            if (elements.Length != 3)
                throw new ArgumentException("����� ������� �� ������������� �������.", nameof(elements));

            double side1 = elements[0];
            double side2 = elements[1];
            double angleBetween = elements[2];

            return side1 * side2 / 2 * Math.Sin(angleBetween / 180 * Math.PI);
        });
        myFigure = new UniversalFigure<double>(myFunc, 3, 4, 90);
        Assert.That(myFigure.Area, Is.EqualTo(new Triangle(3, 4, 5).Area));

        // � ���� �������� ����� ��������� � �������� ���������� -
        // �������� ������� ����, � ����� �������, � ���� �� ������ �� ����
        // ������� ������� ����� �� ������������ ����� ����� � ��������,
        // ��� ������� ����� ������� ����������, � ��� ����������� ����������, ���������� �� ������� ����������
        myFigure = new UniversalFigure<double>(myFunc, 90, 3, 4);
        Assert.That(myFigure.Area, !Is.EqualTo(new Triangle(3, 4, 5).Area)); // ������� �� �����


        // ����� ���� �� ����� ����������
        myFunc = new((elements) =>
        {
            if (elements.Length != 1)
                throw new ArgumentException("����� ������� �� ������������� �������.", nameof(elements));

            return elements[0] * elements[0] / (4 * Math.PI);
        });
        myFigure = new UniversalFigure<double>(myFunc, 2 * Math.PI);
        Assert.That(myFigure.Area, Is.EqualTo(new Circle(1).Area));


        // ����� ������� �� 2 �����������. ������ ���������� Point[]
        Func<Point[], double> myPointFunc = new((elements) =>
        {
            if (elements.Length != 2)
                throw new ArgumentException("����� ������� �� ������������� �������.", nameof(elements));

            return Math.Abs((elements[0].X - elements[1].X) * (elements[0].Y - elements[1].Y));
        });
        var myPointFigure = new UniversalFigure<Point>(myPointFunc, new Point(-4, 0), new Point(0, 4));
        Assert.That(myPointFigure.Area, Is.EqualTo(16));


        // ��������� ������ � �������� ����������. ������� 1 ���������� ������ 2.
        Assert.Throws<ArgumentException>(() => new UniversalFigure<Point>(myPointFunc, new Point(-4, 0)));

        // ��������� ������ ���������� � ������������
        Assert.Throws<ArgumentNullException>(() => new UniversalFigure<Point>(null, new Point(-4, 0), new Point(0, 4)));
        Assert.Throws<ArgumentException>(() => new UniversalFigure<Point>(myPointFunc, Array.Empty<Point>()));
    }


    [Test] // ��������� ���������
    public void TestInterface()
    {
        IFigure figure;
        Random random = new();
        double ExpectedResult;

        // �� �� �����, ����� ��� ������ ����� � ���������� figure
        if (random.Next(0, 2) == 1)
        {
            figure = new Circle(23);
            ExpectedResult = 1661.9025137490005;
        }
        else
        {
            figure = new Triangle(3, 4, 5);
            ExpectedResult = 6;
        }

        // �� �� ���� ���������� ������ � ���������� ��������� �������
        Assert.That(figure.Area, Is.EqualTo(ExpectedResult));
    }


    // ��������� ����������� ������ � ������ Calc

    [TestCase(0, ExpectedResult = 0)]
    [TestCase(double.Epsilon, ExpectedResult = 0)]
    [TestCase(23, ExpectedResult = 1661.9025137490005)]
    [TestCase(1.27, ExpectedResult = 5.067074790974977)]
    [TestCase(972872782742778678578D, ExpectedResult = 2.973459174482516e+42)]
    public double TestGetAreaCircleByRadius(double radius) => GetAreaCircleByRadius(radius);

    [Test]
    public void TestGetAreaCircleByRadiusNotNormalArgument()
    {
        Assert.Throws<ArgumentException>(() => GetAreaCircleByRadius(-654));
        Assert.Throws<ArgumentException>(() => GetAreaCircleByRadius(double.NaN));
        Assert.Throws<ArgumentException>(() => GetAreaCircleByRadius(double.MaxValue));
    }


    [TestCase(2, 4.21, 3.47, ExpectedResult = 3.4443907095450124d)]
    [TestCase(548543, 645557, 753424, ExpectedResult = 173041408016.02628d)]
    [TestCase(9, 3, 11.9999999999, ExpectedResult = 0.00012727809540592021d)] // ����� ���� 2 ������ ����� ����� 3 ������� - ��
    [TestCase(0.00000001, 0.00000001, 0.00000001, ExpectedResult = 4.3301270189221959E-17d)] // �������� ������ � 0 - ��
    public double TestGetAreaTriangleBySides(double a, double b, double c) => GetAreaTriangleBySides(a, b, c);

    [Test]
    public void TestGetAreaTriangleBySidesNotNormalArgument()
    {
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(97, 12, 20)); // ����� 2 ������ ������ 3 �������
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(9, 3, 12)); // ����� 2 ������ ����� 3 �������
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(0, 12, 3));
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(-24, 2, 5));
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(double.NaN, 2, 0.2));
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(double.MaxValue, 1, 4));
    }


    [TestCase(3, 4, 5, ExpectedResult = true)]
    [TestCase(3, 4, 5.000000000001, ExpectedResult = false)]
    [TestCase(3, 4, 4.999999999999, ExpectedResult = false)]
    public bool TestIsRightTriangle(double a, double b, double c) => IsRightTriangle(a, b, c);

    [Test]
    public void TestIsRightTriangleNotNormalArgument()
    {
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(97, 12, 20));
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(9, 3, 12));
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(0, 12, 3));
        Assert.Throws<ArgumentException>(() => IsRightTriangle(-24, 2, 5));
        Assert.Throws<ArgumentException>(() => IsRightTriangle(double.NaN, 2, 0.2));
        Assert.Throws<ArgumentException>(() => IsRightTriangle(double.MaxValue, 1, 4));
    }


    [TestCase(3, 0, ExpectedResult = 0)]
    [TestCase(3, 0.1, ExpectedResult = 0.0043301270189221959d)]
    [TestCase(5, 3, ExpectedResult = 15.484296605300704d)]
    [TestCase(int.MaxValue, 3, ExpectedResult = double.PositiveInfinity)]
    public double TestGetAreaEquilateralPolygon(int numberOfSides, double lengthOfSide) =>
        GetAreaEquilateralPolygon(numberOfSides, lengthOfSide);

    [Test]
    public void TestGetAreaEquilateralPolygonNotNormalArgument()
    {
        Assert.Throws<ArgumentException>(() => GetAreaEquilateralPolygon(2, 0));
        Assert.Throws<ArgumentException>(() => GetAreaEquilateralPolygon(4, -0.001));
    }

    [Test]
    public void TestGetAreaArbitraryFigure()
    {
        Assert.Multiple(() =>
        {
            Assert.That(GetAreaArbitraryFigure(new Point(3, 4)), Is.EqualTo(0));

            Assert.That(GetAreaArbitraryFigure(new Point(0, 0), new Point(0, 4), new Point(3, 0)), Is.EqualTo(6));

            Assert.That(GetAreaArbitraryFigure(new Point(0, 0),
                new Point(2, 0), new Point(2, 2), new Point(0, 2)), Is.EqualTo(4));

            Assert.That(GetAreaArbitraryFigure(new Point(3, 4),
                new Point(5, 11), new Point(12, 8), new Point(9, 5), new Point(5, 6)), Is.EqualTo(30));

            Assert.Throws<ArgumentNullException>(() => GetAreaArbitraryFigure(null));
            Assert.Throws<ArgumentException>(() => GetAreaArbitraryFigure(Array.Empty<Point>()));
            Assert.Throws<OverflowException>(() => GetAreaArbitraryFigure(new Point(int.MinValue, int.MaxValue),
                new Point(int.MinValue, int.MinValue), new Point(int.MaxValue, int.MaxValue)));
        });
    }
}