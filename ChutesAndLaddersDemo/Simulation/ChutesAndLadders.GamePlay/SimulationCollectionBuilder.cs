using ChutesAndLadders.Builders;
using ChutesAndLadders.Entities;
using ChutesAndLadders.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChutesAndLadders.GamePlay
{
    public class SimulationCollectionBuilder: SimulationCollection
    {
        PlayerCollectionBuilder _playerBuilder = new PlayerCollectionBuilder();

        int _maxExecutionCount = 30000;
        bool _outputResults = false;
        string _outputGameActionsFolder = null;

        public void Run()
        {
            base.RunSimulations(_playerBuilder.Build(), _maxExecutionCount, _outputResults, _outputGameActionsFolder);
        }

        public SimulationCollectionBuilder AddPlayer(string playerName, IGameStrategy gameStrategy)
        {
            _playerBuilder.Add(playerName, gameStrategy);
            return this;
        }

        public SimulationCollectionBuilder MaxExecutionCount(int maxExecutionCount)
        {
            _maxExecutionCount = maxExecutionCount;
            return this;
        }

        public SimulationCollectionBuilder OutputResults(bool outputResults)
        {
            _outputResults = outputResults;
            return this;
        }

        public SimulationCollectionBuilder OutputGameActionsFolder(string folder)
        {
            _outputGameActionsFolder = folder;
            return this;
        }
    }
}
