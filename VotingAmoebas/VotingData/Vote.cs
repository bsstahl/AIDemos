using System;
using System.Collections.Generic;
using System.Text;

namespace VotingData
{
    public class Vote
    {
        public int Id { get; set; }
        public int VoterId { get; set; }
        public VoteStatus VoteStatus { get; set; }
    }
}
