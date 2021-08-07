using UnofficialDevryIT.Architecture.Models;

namespace DevryDomain.Models
{
    public class ChallengeResponse : EntityWithTypedId<ulong>
    {
        public ulong ChallengeId { get; set; }
        public Challenge Challenge { get; set; }
        public string Value { get; set; }
        public double Reward { get; set; } = 0;
        public bool IsCorrect { get; set; } = false;

        public override string ToString()
        {
            return $"Value: {Value}\n" +
                   $"Reward: {Reward}\n" +
                   $"Is Correct: {IsCorrect}\n";
        }
    }
}