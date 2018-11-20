using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChutesAndLadders.Entities;
using System.Linq;

namespace ChutesAndLadders.Common.Test
{
    [TestClass]
    public class GameBoard_GetLegalEndpoints_Should
    {
        [TestMethod]
        public void ReturnTheCorrectOptions_0_2()
        {
            int boardLocation = 0;
            byte spin = 2;
            var expected = new int[] { 2, 38 };

            var target = new GameBoard();
            var actual = target.GetLegalEndpoints(boardLocation, spin);

            Assert.IsTrue(expected.IsEqualTo(actual));
        }

        [TestMethod]
        public void ReturnTheCorrectOptions_0_3()
        {
            int boardLocation = 0;
            byte spin = 3;
            var expected = new int[] { 3, 39 };

            var target = new GameBoard();
            var actual = target.GetLegalEndpoints(boardLocation, spin);

            Assert.IsTrue(expected.IsEqualTo(actual));
        }

        [TestMethod]
        public void ReturnTheCorrectOptions_46_6()
        {
            int boardLocation = 46;
            byte spin = 6;
            var expected = new int[] { 52, 13, 30, 67, 85 };

            var target = new GameBoard();
            var actual = target.GetLegalEndpoints(boardLocation, spin);

            Assert.IsTrue(expected.IsEqualTo(actual));
        }
    }
}
