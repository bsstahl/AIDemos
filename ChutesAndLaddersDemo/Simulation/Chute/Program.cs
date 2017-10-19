using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChutesAndLadders.GamePlay;
using ChutesAndLadders.Entities;
using System.Diagnostics;

namespace Chute
{
    class Program
    {
        static void Main(string[] args)
        {
            var board = new GameBoard();

            #region Demo 1

            board.Demo1a_GreedyAlgorithm();

            #endregion

            #region Demo 2

            // board.Demo2a_RulesEngine();
            // board.Demo2b_ImprovedLinear();

            #endregion

            #region Demo 3

            // Only do demo 3b unless there is time left over

            // board.Demo3a_GeneticAnalysis();
            // board.Demo3b_GeneticEvolution();
            // board.Demo3c_GeneticSuperiority();

            #endregion

            #region Demo 4

            // board.Demo4a_ShortestPath();

            #endregion


            #region Supplemental Demos

            // board.SupplementalDemo_SingleGame();
            // board.SupplementalDemo_Player1Advantage();

            // board.GameActionOutput_SmallSample();

            #endregion
        }
    }
}
