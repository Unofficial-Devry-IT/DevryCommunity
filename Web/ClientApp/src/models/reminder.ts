export default interface Reminder
{
    GuildId: string;
    ChannelId: string;
    Schedule: string;
    Name: string;
    Contents: string;
    NextRunTime: Date;
}