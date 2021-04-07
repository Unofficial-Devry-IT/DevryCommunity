import ChannelTypes from "../constants/channel-types"

export default class Channel
{
    GuildId: string = "";
    Id: string = "";
    Name: string = "";
    Description: string = "";
    Position: number = 0;
    ChannelType: number = 0;

    /**
     * Text representation of channel type
     */
    getChannelText(): string
    {
        return ChannelTypes[this.ChannelType];
    }
}