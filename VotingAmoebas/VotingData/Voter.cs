using System;
using System.Collections.Generic;
using System.Text;

namespace VotingData
{
    public class Voter: List<Vote>
    {
        public int Id { get; set; }
        public Party PartyAffiliation { get; set; }
        public IEnumerable<Vote> Votes { get; set; }

        public override String ToString() => $"Voter {this.Id:00000} - Party: {this.PartyAffiliation.ToString().PadLeft(10)}";
    }
}
