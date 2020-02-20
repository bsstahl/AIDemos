using System;
using System.Collections.Generic;
using System.Text;

namespace VotingData
{
    public class VotingDataRepository : IVotingDataRepository
    {
        const string _fileName = @"house-votes-84.data";

        private readonly string _dataFolderPath;

        public VotingDataRepository(string dataFolderPath)
        {
            _dataFolderPath = Environment.ExpandEnvironmentVariables(dataFolderPath);
        }

        public IEnumerable<Voter> GetAllVoters()
        {
            string filePath = System.IO.Path.Combine(_dataFolderPath, _fileName);
            var result = new List<Voter>();

            int i = 0;
            var data = System.IO.File.ReadAllLines(filePath);
            foreach (var record in data)
            {
                var recordData = record.Split(',');
                result.Add(new Voter()
                {
                    Id = i,
                    PartyAffiliation = (Party)Enum.Parse(typeof(Party), recordData[0]),
                    Votes = this.ParseVotes(i, recordData)
                });

                i++;
            }

            return result;
        }

        private IEnumerable<Vote> ParseVotes(int voterId, string[] recordData)
        {
            var results = new List<Vote>();
            for (int i = 1; i < recordData.Length; i++)
            {
                results.Add(new Vote() 
                { 
                    Id = i,
                    VoterId = voterId,
                    VoteStatus = this.ParseVoteStatus(recordData[i])
                });
            }
            return results;
        }

        private VoteStatus ParseVoteStatus(string voteIndicator)
        {
            var result = VoteStatus.Unknown;
            if (voteIndicator.ToLower().StartsWith("y"))
                result = VoteStatus.Aye;
            else if (voteIndicator.ToLower().StartsWith("n"))
                result = VoteStatus.Nay;
            return result;
        }
    }
}
