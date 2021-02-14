import React, {Component} from "react";
import authService from "./api-authorization/AuthorizeService";

export class FetchChannels extends Component
{
    static displayName = "Channels";
    
    constructor(props) {
        super(props);
        
        this.state = {
            channelData: {},
            loading: true
        };
    }
    
    componentDidMount() {
        this.populateChannelData();
    }
    
    static renderChannels(channels)
    {
        if(channels && channels.length && channels.length > 0)
            return (
                channels.map(channel=>
                    <div className="card">
                        <div className="card-body">
                            <h5 className="card-title">{channel.Name}</h5>
                            <h6 className="card-subtitle mb-2 text-muted">
                                {channel.ChannelType}
                            </h6>
                            <p className="card-text">{channel.Description}</p>
                        </div>
                    </div>    
                )
            );
        else 
            return(
                <p>No Content</p>
            )
    }
    
    render()
    {
        const { channelData = {}} = this.state;
        const { items = [] } = channelData;
            
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : FetchChannels.renderChannels(items);
            
            return (
                <div>
                    <h1>Unofficial DeVry Discord Channels</h1>
                    <p>Current channels on discord</p>
                    {contents}
                </div>
            )
    }
    
    async populateChannelData()
    {
        const token = await authService.getAccessToken();
        const response = await fetch('api/channel', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        this.setState({channelData: data, loading: false});
    }
}