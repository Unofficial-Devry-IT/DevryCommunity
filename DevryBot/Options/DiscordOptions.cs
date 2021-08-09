namespace DevryBot.Options
{
    public class DiscordOptions
    {
        public string QueueImage { get; set; }
        public string CompletedImage { get; set; }
        public string WarningImage { get; set; }
        public string ErrorImage { get; set; }
        public string InviteImage { get; set; }
        public string UhOhImage { get; set; }

        /// <summary>
        /// Clear the cache of slash commands
        /// </summary>
        public bool ClearCommands { get; set; } = false;
        
        /// <summary>
        /// Checks to see if discord exists within a link
        /// </summary>
        public string InviteLinkSearchCriteria { get; set; } = "discord";

        /// <summary>
        /// Customizable invitation message.... a call to arms if you will
        /// </summary>
        public string InviteMessage { get; set; } =
            "Spread the word, our trusted scout! Spread the word of our kingdom! Amass an army of knowledge seeking minions! " +
            "Lay waste to the legions of doubt and uncertainty!!";
        
        /// <summary>
        /// Permanent invitation link to use within the service
        /// </summary>
        public string InviteLink { get; set; } = "https://discord.io/unofficial-DevryIT";
        
        /// <summary>
        /// Footer text for invitation messages
        /// </summary>
        public string InviteFooter { get; set; } = "Minions of knowledge! Assembblleeee!";
        
        /// <summary>
        /// Global list of roles to blacklist for any selection-based menu
        /// </summary>
        public string[] BlacklistedRoleNames { get; set; } = new[]
        {
            "@everyone",
            "Admin",
            "Senior Moderators",
            "Junior Moderators",
            "Pollmaster",
            "Professor",
            "Database",
            "Programmer",
            "Motivator",
            "Server Booster",
            "DeVry-SortingHat",
            "Devry-Service-Bot",
            "Devry-Challenge-Bot",
            "Devry-Test-Bot",
            "MathBot",
            "See-All-Channels",
            "Devry",
            "tutor"
        };
    }
}