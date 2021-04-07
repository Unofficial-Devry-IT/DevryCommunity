import React, { useState, useCallback, useRef } from "react";
import {Cron} from "react-js-cron";
import { Input, Divider } from "antd";
import authService from "../api-authorization/AuthorizeService";
import TextField from "@material-ui/core/TextField"
import $ from "jquery";
import TextHelpers from "../../util/encode";

async function createReminder(reminderData)
{
    const { guildId, channelId, schedule, name, contents } = reminderData;
    const token = await authService.getAccessToken();
    
    const payload ={
        GuildId: guildId,
        ChannelId: channelId,
        Name: TextHelpers.encodeText(name),
        Contents: TextHelpers.encodeText(contents),
        Schedule: TextHelpers.encodeText(schedule)
    };
    
    const jsonPayload = JSON.stringify(payload);
    
    console.log("Payload", payload);
    console.log("JSON", jsonPayload);
    
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        headers: {"Authorization": `Bearer ${token}`},
        dataType: "json",
        data: jsonPayload,
        url: "/api/reminder/create",
        success:function(data)
        {
            console.log("Success", data);
        },
        error: function(a, jqXHR, exception)
        {
            console.log(a,jqXHR,exception);
        }
    });
}

export default function CreateReminder(props) {
    const
        {
            guildId   = undefined,
            channelId = undefined
        } = props;
    
    const inputRef = useRef(null)
    const defaultValue = '* * * * *'
    const [value, setValue] = useState(defaultValue);
    const [name, setName] = useState("Default Name");
    const [contents, setContents] = useState();
    const [error, onError] = useState()
    const customSetValue = useCallback(
        (newValue) => {
            setValue(newValue)
            inputRef.current.setValue(newValue)
        },
        [inputRef]
    );
    
    const submit = async ()=>
    {
       const data =
       {
           guildId: guildId,
           channelId: channelId,
           name: name,
           contents: contents,
           schedule: value
       };
       
       var response = await createReminder(data);
       
       if(props.callback)
           props.callback();
    };
    
    return (
        <div>
            <TextField 
                   label="Name"
                   className="mt-3" 
                   placeholder="Reminder Title" 
                   onChange={(event)=>
                            {
                                setName(event.target.value);
                            }} 
                   value={name}/>
                   
            <TextField className="mt-3 mb-3 col-12" 
                       placeholder="Text that shall appear within the reminder message"
                       onChange={(event)=>{
                                    setContents(event.target.value);
                                }} 
                       value={contents} 
                       maxLength={2000} 
                       multiline 
                       rows={5} 
                       variant="outlined"/>
            <div>
                {
                    error &&
                    <div className="alert alert-danger text-center" role="alert">
                        <h4 className="alert-heading">Error</h4>
                        {error.description}
                    </div>
                }


                <Input
                    ref={inputRef}
                    onBlur={(event) => {
                        setValue(event.target.value)
                    }}
                />

                <Divider>OR</Divider>

                <Cron
                    value={value}
                    setValue={customSetValue}
                    onError={onError}
                    humanizeLabels={true}
                    humanizeValue={false}
                />
            </div>
            
            <button className="btn btn-primary" onClick={(event)=>submit()} >Create</button>
            
            {
                props.callback &&
                <button className="btn btn-secondary" onClick={props.callback}>Cancel</button>
            }
        </div>
    )
}