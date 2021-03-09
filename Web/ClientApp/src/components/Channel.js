import React, {Component} from "react";
import authService from "./api-authorization/AuthorizeService";
import ChannelTypes from "../constants/channel-types"
import AddAlertIcon from '@material-ui/icons/AddAlert';
import TodayIcon from "@material-ui/icons/Today";
import {IconButton, Modal, Container, Typography, Tooltip} from "@material-ui/core";
import _ from "lodash";
import CreateReminder from "./reminders/create-reminder";
import CollapsableTable from "./CollapsableTable";

export class FetchChannels extends Component
{
    static displayName = "Channels";
    
    constructor(props) {
        super(props);
        
        this.state = {
            channelData: {},
            loading: true,
            pageNumber: 1,
            pageSize: 10,
            modalOpen: false,
            selectedGuidId: undefined,
            selectedChannelId: undefined
        };
        
        _.bindAll(this, 
            "openReminder", 
            "closeReminder", 
            "renderChannels");
    }
    
    componentDidMount() {
        this.populateChannelData();
    }
    
    openReminder(guidId, channelId)
    {
        this.setState({
            modalOpen: true,
            selectedGuidId: guidId,
            selectedChannelId: channelId
        })
    }
    
    closeReminder()
    {
        console.log("closing");
        this.setState({
            modalOpen: false
        });
    }
    
    renderChannels(channels)
    {
        if(channels && channels.length && channels.length > 0)
        {
            const rows = channels.map((channel)=>
            {
                const values = [
                    channel.name,
                    ChannelTypes[channel.channelType]
                ];
                
                const content = (<React.Fragment>
                    <p>{channel.description}</p>
                    <Tooltip title="Create Reminder">
                        <IconButton style={{width: "16"}}
                                    onClick={()=>
                                    {
                                        this.openReminder(channel.guidId, channel.id);
                                    }}>
                            <AddAlertIcon className="text-info"/>
                        </IconButton>
                    </Tooltip>
                    
                    <Tooltip title={"View Reminders"}>
                        <IconButton style={{width: "16"}}
                                    onClick={()=>
                                    {
                                        
                                    }}>
                            <TodayIcon className="text-info"/>
                        </IconButton>
                    </Tooltip>
                </React.Fragment>);
                
                return { values: values, content: content};
            });
            
            const headers = [
                "Name",
                "Type"
            ];
            
            return <CollapsableTable headers={headers} rows={rows}/>
        }
            /*
                return (
                <div className="row">
                    {
                        channels.map(channel=>{
                                console.log(channel);
                                return <div className="col-sm-6 col-md-3">
                                    <div className="card mr-3 mb-3">
    
    
                                        <div className="float-right" style={{position: "absolute", right: "48px"}}>
                                            <div style={{position: "absolute"}}>
                                                <IconButton style={{width: "16"}}
                                                            onClick={()=>
                                                            {
                                                                this.openReminder(channel.guidId, channel.id);
                                                            }}>
                                                    <AddAlertIcon className="text-info"/>
                                                </IconButton>
                                            </div>
                                        </div>
    
                                        <div className="card-header" style={{paddingLeft: 8, paddingRight: 30}}>
                                            {channel.name}
                                        </div>
                                        <div className="card-body">
                                            <h5 className="card-title">{channel.name}</h5>
                                            <h6 className="card-subtitle mb-2 text-muted">
                                                {ChannelTypes[channel.channelType]}
                                            </h6>
                                            <p className="card-text">{channel.description}</p>
                                        </div>
                                    </div>
                                </div>
                            }
                        )
                    }
                </div>
            );            
             */
        else 
            return(
                <p>No Content</p>
            )
    }
    
    async onPageUp()
    {
        if(this.state.loading)
            return;
        
        const { pageIndex, totalPages } = this.state.channelData;
        const { pageSize } = this.state;
        
        if(pageIndex + 1 <= totalPages)
        {
            this.setState({loading: true});
            await this.populateChannelData(pageIndex+1, pageSize);
        }
    }
    
    async onPageDown()
    {
        if(this.state.loading)
            return;

        const { pageIndex } = this.state.channelData;
        const { pageSize } = this.state;
        
        if(pageIndex - 1 >= 1)
        {
            this.setState({loading: true});
            await this.populateChannelData(pageIndex-1, pageSize);
        }
    }
    
    render()
    {
        const { 
            channelData = {}, 
            pageNumber, 
            modalOpen, 
            selectedChannelId, 
            selectedGuidId 
        } = this.state;
        
        const { items = [] } = channelData;
        console.log("Modal Open Value: ", modalOpen);
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderChannels(items);
            
            return (
                <div>
                    <h1>Unofficial DeVry Discord Channels</h1>
                    <p>Current channels on discord</p>
                    {contents}
                    
                    {items && items.length &&
                        <div className="text-center">
                            <button className="btn btn-primary" onClick={this.onPageDown.bind(this)}>&laquo;</button>
                            <label className="ml-3 mr-3">{pageNumber}/{channelData.totalPages}</label>
                            <button className="btn btn-primary" onClick={this.onPageUp.bind(this)}>&raquo;</button>
                        </div>
                    }
                    
                    <Modal style={{
                                    zIndex: 0,
                                    marginTop: "60px"
                                }}
                           open={modalOpen}
                           onClose={this.closeReminder}
                           >
                        <Container maxWidth="md">
                            <Typography component="div" style={{backgroundColor: "#fff", padding: "32px"}}>
                            <CreateReminder guidId={selectedGuidId}
                                            channelId={selectedChannelId}
                                            callback={this.closeReminder.bind(this)}/>
                            </Typography>
                        </Container>
                        
                    </Modal>
                </div>
            )
    }
    
    async populateChannelData(pageNumber=1, pageSize= 10)
    {
        const token = await authService.getAccessToken();
        const response = await fetch(`api/channel?PageNumber=${pageNumber}&PageSize=${pageSize}`, {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        this.setState({
            channelData: data, 
            loading: false,
            pageNumber,
            pageSize
        });
    }
}