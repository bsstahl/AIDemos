using System.Collections.Generic;

namespace VotingData
{
    public interface IVotingDataRepository
    {
        IEnumerable<Voter> GetAllVoters();
    }
}