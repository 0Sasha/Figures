using System;
using static Figures.Calc;

namespace Figures.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        // Нечего настраивать
    }


    [Test]
    public void TestClasses()
    {
        Assert.Multiple(() =>
        {
            Assert.That(new Circle(0).Area, Is.EqualTo(0));
            Assert.That(new Circle(23).Area, Is.EqualTo(1661.9025137490005));
            Assert.That(new Circle(1.27).Area, Is.EqualTo(5.067074790974977));
            Assert.That(new Circle(972872782742778678578D).Area, Is.EqualTo(2.973459174482516e+42));

            Assert.That(new Triangle(2, 4.21, 3.47).Area, Is.EqualTo(3.4443907095450124d));
            Assert.That(new Triangle(548543, 645557, 753424).Area, Is.EqualTo(173041408016.02628d));
            Assert.That(new Triangle(9, 3, 11.9999999999).Area, Is.EqualTo(0.00012727809540592021d));
            Assert.That(new Triangle(0.00000001, 0.00000001, 0.00000001).Area, Is.EqualTo(4.3301270189221959E-17d));

            Assert.That(new Triangle(3, 4, 5).IsRightTriangle, Is.True);
            Assert.That(new Triangle(3, 4, 5.000000000001).IsRightTriangle, Is.False);
            Assert.That(new Triangle(3, 4, 4.999999999999).IsRightTriangle, Is.False);
        });
    }

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
    [TestCase(9, 3, 11.9999999999, ExpectedResult = 0.00012727809540592021d)] // Сумма длин 2 сторон почти равна 3 стороне - ОК
    [TestCase(0.00000001, 0.00000001, 0.00000001, ExpectedResult = 4.3301270189221959E-17d)] // Значения близки к 0 - ОК
    public double TestGetAreaTriangleBySides(double a, double b, double c) => GetAreaTriangleBySides(a, b, c);

    [Test]
    public void TestGetAreaTriangleBySidesNotNormalArgument()
    {
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(97, 12, 20)); // Сумма 2 сторон меньше 3 стороны
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(9, 3, 12)); // Сумма 2 сторон равна 3 стороне
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
    public void TestTestIsRightTriangleNotNormalArgument()
    {
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(97, 12, 20));
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(9, 3, 12));
        Assert.Throws<ArgumentException>(() => GetAreaTriangleBySides(0, 12, 3));
        Assert.Throws<ArgumentException>(() => IsRightTriangle(-24, 2, 5));
        Assert.Throws<ArgumentException>(() => IsRightTriangle(double.NaN, 2, 0.2));
        Assert.Throws<ArgumentException>(() => IsRightTriangle(double.MaxValue, 1, 4));
    }
}