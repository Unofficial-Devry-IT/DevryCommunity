namespace DevryDomain.Models
{
    public class ArchiveChannel
    {
        /// <summary>
        /// Shall use the original discord channel ID as our PK
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Name of the channel
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If applicable -- name of the parent channel
        /// </summary>
        public string ParentName { get; set; }
    }
}