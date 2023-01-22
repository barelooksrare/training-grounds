using System;
using TrainingGrounds.Accounts;
using TrainingGrounds.Utils;


namespace TrainingGrounds.Extensions
{

    public static class TrainingGroundsExtensions
    {
        public static int GetEnergy(this Player player, Club club)
        {
            var rechargeStartTime = player.RechargeStartTime;
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var timePassed = now - rechargeStartTime;
            var rechargeInterval = club.GameParams.EnergyRechargeMinutes * 60;
            var calculatedEnergy = player.Energy + timePassed / rechargeInterval;
            return (int)Math.Min(calculatedEnergy, club.GameParams.MaxPlayerEnergy);
        }
        
        public static TimeSpan GetTimeUntilCharge(this Player player, Club club)
        {
            var energy = player.GetEnergy(club);
            if (energy == club.GameParams.MaxPlayerEnergy) return TimeSpan.Zero;
            var rechargeStartTime = player.RechargeStartTime;
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var timePassed = (now - rechargeStartTime) % (club.GameParams.EnergyRechargeMinutes * 60);
            var timeRemaining = club.GameParams.EnergyRechargeMinutes * 60 - timePassed;
            return TimeSpan.FromSeconds(timeRemaining);
        }

        public static DisplayGameParams GetDisplayGameParams(this Club club)
        {
            return new DisplayGameParams(club);
        }
    }
}