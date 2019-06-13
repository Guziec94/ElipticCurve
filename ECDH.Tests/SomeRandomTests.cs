using ECDH;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

namespace ECDH.Tests
{
    [TestClass]
    public class SomeRandomTests
    {
        [TestMethod]
        public void NeutralPointAddition()
        {
            BigInteger a = 3;
            BigInteger p = 7;
            Point p1 = Point.NeutralElement(a, p);
            Point p2 = new Point(3, 5, a, p);
            Point p3 = p1 + p2;
            Assert.IsTrue(p2 == p3);
        }

        [TestMethod]
        public void AdditionShouldReturnNeutralPoint()
        {
            BigInteger a = 3;
            BigInteger p = 7;
            Point p1 = new Point(3, 3, a, p);
            Point p2 = new Point(3, 5, a, p);
            Point p3 = p1 + p2;
            Assert.IsTrue(p3 == Point.NeutralElement(a, p));
        }

        [TestMethod]
        public void PointAddition1()
        {
            Point p1 = new Point(4, 3, 1, 5);
            Point p2 = new Point(0, 4, 1, 5);
            Point p3 = p1 + p2;// (2, 4)
            Point expectedResult = new Point(2, 4, 0, 0);
            Assert.IsTrue(expectedResult == p3);
        }

        [TestMethod]
        public void PointAddition2()
        {
            Point p1 = new Point(3, 1, 1, 5);
            Point p2 = new Point(3, 1, 1, 5);
            Point p3 = p1 + p2;// (0,1)
            Point expectedResult = new Point(0, 1, 0, 0);
            Assert.IsTrue(expectedResult == p3);
        }

        [TestMethod]
        public void CheckIfStaticPointsBelongsToCurve()
        {
            BigInteger a = 6;
            BigInteger p = 11;
            ElipticCurve curve = new ElipticCurve(a, 4, p);
            Point p1 = new Point(7, 9, a, p);
            Point p2 = new Point(3, 4, a, p);

            bool result = curve.CheckIfPointBelongsToCurve(p1);
            Assert.IsTrue(result);

            result = curve.CheckIfPointBelongsToCurve(p2);
            Assert.IsTrue(result);

            result = curve.CheckIfPointBelongsToCurve(p1 + p1);
            Assert.IsTrue(result);

            result = curve.CheckIfPointBelongsToCurve(p1 + p2);
            Assert.IsTrue(result);

            result = curve.CheckIfPointBelongsToCurve(Point.NeutralElement(a, p));
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GenerateCurveAndPointCheck()
        {
            ElipticCurve curve = new ElipticCurve(64);
            curve.GenerateRandomCurve();

            Point p1 = curve.GenerateRandomPointOnCurve();
            Assert.IsTrue(curve.CheckIfPointBelongsToCurve(p1));

            Point p2 = curve.GenerateRandomPointOnCurve();
            Assert.IsTrue(curve.CheckIfPointBelongsToCurve(p1));

            Assert.IsTrue(curve.CheckIfPointBelongsToCurve(p1 + p1));

            Assert.IsTrue(curve.CheckIfPointBelongsToCurve(p1 + p2));
        }

        [TestMethod]
        public void ScalarMultiplicationTests()
        {
            ElipticCurve curve = new ElipticCurve(64);
            curve.GenerateRandomCurve();
            Point randomPoint = curve.GenerateRandomPointOnCurve();

            Point testPoint = curve.ScalarMultiplication(randomPoint, 1);
            Assert.IsTrue(curve.CheckIfPointBelongsToCurve(testPoint));

            testPoint = curve.ScalarMultiplication(randomPoint, 5);
            Assert.IsTrue(curve.CheckIfPointBelongsToCurve(testPoint));

            testPoint = curve.ScalarMultiplication(randomPoint, 100);
            Assert.IsTrue(curve.CheckIfPointBelongsToCurve(testPoint));

            testPoint = curve.ScalarMultiplication(randomPoint, BigInteger.Parse("999999999999999999999999999999999999999999999999999999999999"));

            Assert.IsTrue(curve.CheckIfPointBelongsToCurve(testPoint));
        }
    }
}
