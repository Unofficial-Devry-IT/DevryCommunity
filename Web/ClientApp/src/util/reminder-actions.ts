import authService from "../components/api-authorization/AuthorizeService";

/**
 * Retrieve reminder data 
 * @param pageNumber (current page the user is viewing)
 * @param pageSize (size per page)
 * @param guildId (should always be the same but required)
 * @param channelId (reminders on a specific channel)
 * @constructor
 */
export async function GetReminderData(pageNumber: number = 1, pageSize: number = 10, guildId: string, channelId: string)
{
    const token = await authService.getAccessToken();
    const response = await fetch(`/api/reminder?PageNumber=${pageNumber}&PageSize=${pageSize}&GuildId=${guildId}&ChannelId=${channelId}`,
        {
            headers: !token ? {} : {"Authorization": `Bearer ${token}`}
        });
    
    return await response.json();
}