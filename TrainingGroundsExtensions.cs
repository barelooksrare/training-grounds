using TrainingGrounds.Accounts;

namespace TrainingGrounds.Extensions
{

    public static class TrainingGroundsExtensions
    {
        public static int GetEnergy(this Player player, Club club)
        {
            var rechargeStartTime = player.RechargeStartTime;
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var timePassed = now - rechargeStartTime;
            var rechargeInterval = club.GameParams.EnergyRechargeMinutes;
            var calculatedEnergy = player.Energy + timePassed / rechargeInterval;
            return (int)Math.Min(calculatedEnergy, club.GameParams.MaxPlayerEnergy);
        }
    }
}