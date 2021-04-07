import React, {Component} from "react";
import authService from "../api-authorization/AuthorizeService";
import CreateReminder from "./create-reminder";

export class FetchReminders extends Component
{
    static displayName = "Reminders";
    
    constructor(props)
    {
        super(props);
        
        this.state = {
          loading: true,
          reminderData: {},
          pageNumber: 1,
          pageSize: 10  
        };
    }
    
    componentDidMount() {
        this.populateReminderData();
    }
    
    static renderReminders(reminders)
    {
        
    }
    
    async onPageUp()
    {
        if(this.state.loading)
            return;
        
        const { pageIndex, totalPages } = this.state.reminderData;
        const { pageSize } = this.state;
        
        if(pageIndex + 1 <= totalPages)
        {
            this.setState({loading:true});
            await this.populateReminderData(pageIndex+1, pageSize);
        }
    }
    
    async onPageDown()
    {
        if(this.state.loading)
            return;
        
        const { pageIndex } = this.state.reminderData;
        const { pageSize } = this.state;
        
        if(pageIndex - 1 >= 1)
        {
            this.setState({loading:true});
            await this.populateReminderData(pageIndex-1, pageSize);
        }
    }
    
    render()
    {
        const { reminderData = {}, pageNumber } = this.state;
        const { items = [] } = reminderData;
        
        let contents = this.state.loading ?
            <p><em>Loading...</em></p>
            : FetchReminders.renderReminders(items);
            
        return(<div>
            <h1>Reminder/Events</h1>
            <p>Typically used to remind users of live lectures</p>
            
            {contents}
            
            {items && items.length &&
                <div className="text-center">
                    <button className="btn btn-primary" onClick={this.onPageDown.bind(this)}>&laquo;</button>
                    <label className="ml-3 mr-3">{pageNumber}/{reminderData.totalPages}</label>
                    <button className="btn btn-primary" onClick={this.onPageUp.bind(this)}>&raquo;</button>
                </div>
            }
            
            <hr/>
            
            <CreateReminder/>
        </div>)
    }
    
    async populateReminderData(pageNumber = 1, pageSize = 10)
    {
        const token = await authService.getAccessToken();
        const response = await fetch(`/api/reminder?PageNumber=${pageNumber}&PageSize=${pageSize}`,
            {
                headers: !token ? {} : {"Authorization": `Bearer ${token}`}
            });
        
        const data = await response.json();
        this.setState({
            reminderData: data,
            loading: false,
            pageNumber,
            pageSize
        })
    }
}