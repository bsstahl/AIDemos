using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ChutesAndLadders.Strategy.Random.Test
{
    [TestClass]
    public class Engine_GetMove_Should
    {
        [TestMethod]
        public void ReturnOneOfTheValidMoves()
        {
            var rnd = new System.Random();

            var target = new Engine();
            var expected = new int[] { rnd.Next(99), rnd.Next(99), rnd.Next(99) };

            var situation = new Entities.GameSituation() { LegalMoves = expected };
            var actual = target.GetMove(situation);

            Assert.IsTrue(expected.Contains(actual));
        }

        [TestMethod]
        public void SelectAllOfTheValidMovesIfExecutedEnoughTimes()
        {
            var rnd = new System.Random();

            var target = new Engine();
            var expected = new int[] { 0, 1, 2 };
            var s = new bool[] { false, false, false };

            var situation = new Entities.GameSituation() { LegalMoves = expected };

            while (!s[0] || !s[1] || !s[2])
            {
                var actual = target.GetMove(situation);
                s[actual] = true;
            }
        }

        #region Randomness Tests

        // These are really not good tests because they will, on rare occasions,
        // fail even though there is nothing wrong. I could probably calculate the
        // statistical likelyhood of these tests failing even in a perfectly random
        // system if I were so motivated.

        [TestMethod]
        public void EvenlyDistributeTheSelectedMoves_2Options()
        {
            const int executionCount = 100000;

            var rnd = new System.Random();

            var target = new Engine();
            var expected = new int[] { 0, 1 };
            var s = new int[] { 0, 0 };

            var situation = new Entities.GameSituation() { LegalMoves = expected };

            for (int i = 0; i < executionCount; i++)
            {
                var actual = target.GetMove(situation);
                s[actual]++;
            }

            int maxExecutions = (int)(executionCount * 0.6);
            for (int i = 0; i < s.Length; i++)
                Assert.IsTrue(s[i] <= maxExecutions);
        }

        [TestMethod]
        public void EvenlyDistributeTheSelectedMoves_3Options()
        {
            const int executionCount = 100000;

            var rnd = new System.Random();

            var target = new Engine();
            var expected = new int[] { 0, 1, 2 };
            var s = new int[] { 0, 0, 0 };

            var situation = new Entities.GameSituation() { LegalMoves = expected };

            for (int i = 0; i < executionCount; i++)
            {
                var actual = target.GetMove(situation);
                s[actual]++;
            }

            int maxExecutions = (int)(executionCount * 0.4);
            for (int i = 0; i < s.Length; i++)
                Assert.IsTrue(s[i] <= maxExecutions);
        }

        #endregion

    }
}
