using System;

namespace Figures.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        // Нечего настраивать
    }

    [TestCase(0, ExpectedResult = 0)]
    [TestCase(double.Epsilon, ExpectedResult = 0)]
    [TestCase(23, ExpectedResult = 1661.9025137490005)]
    [TestCase(1.27, ExpectedResult = 5.067074790974977)]
    [TestCase(972872782742778678578D, ExpectedResult = 2.973459174482516e+42)]
    public double TestGetAreaCircleByRadius(double radius) => Area.GetAreaCircleByRadius(radius);

    [Test]
    public void TestGetAreaCircleByRadiusNotNormalArgument()
    {
        Assert.Throws<ArgumentException>(() => Area.GetAreaCircleByRadius(-654));
        Assert.Throws<ArgumentException>(() => Area.GetAreaCircleByRadius(double.NaN));
        Assert.Throws<ArgumentException>(() => Area.GetAreaCircleByRadius(double.MaxValue));
    }


    [TestCase(2, 4.21, 3.47, ExpectedResult = 3.4443907095450124d)]
    [TestCase(9, 3, 11.9999999999, ExpectedResult = 0.00012727809540592021d)] // Сумма длин 2 сторон почти равна 3 стороне - ОК
    [TestCase(0.00000001, 0.00000001, 0.00000001, ExpectedResult = 4.3301270189221959E-17d)] // Значения близки к 0 - ОК
    public double TestGetAreaTriangleBySides(double a, double b, double c) =>
        Area.GetAreaTriangleBySides(a, b, c);

    [Test]
    public void TestGetAreaTriangleBySidesNotNormalArgument()
    {
        Assert.Throws<ArgumentException>(() => Area.GetAreaTriangleBySides(97, 12, 20)); // Сумма 2 сторон меньше 3 стороны
        Assert.Throws<ArgumentException>(() => Area.GetAreaTriangleBySides(9, 3, 12)); // Сумма 2 сторон равна 3 стороне
        Assert.Throws<ArgumentException>(() => Area.GetAreaTriangleBySides(0, 12, 3));
        Assert.Throws<ArgumentException>(() => Area.GetAreaTriangleBySides(-24, 2, 5));
        Assert.Throws<ArgumentException>(() => Area.GetAreaTriangleBySides(double.NaN, 2, 0.2));
        Assert.Throws<ArgumentException>(() => Area.GetAreaTriangleBySides(double.MaxValue, 1, 4));
    }
}