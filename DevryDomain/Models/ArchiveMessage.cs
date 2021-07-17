using System;

namespace DevryDomain.Models
{
    /// <summary>
    /// Message from a channel that got archived
    /// </summary>
    public class ArchiveMessage
    {
        /// <summary>
        /// Discord Message ID shall be used
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// We can't capture the channel ID simply because it won't exist after archival
        /// So we capture the name instead
        /// </summary>
        public string CategoryChannelName { get; set; }
        
        /// <summary>
        /// We can't capture the channel ID simply because it won't exist after archival
        /// So we capture the name instead
        /// </summary>
        public string ChannelName { get; set; }
        
        /// <summary>
        /// Content of the message (textual)
        /// </summary>
        public string Contents { get; set; }
        
        /// <summary>
        /// Author name of the original message
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Time in which the message was created
        /// </summary>
        public DateTimeOffset CreationDate { get; set; }
    }
}