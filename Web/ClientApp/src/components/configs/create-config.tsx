import React, {Component} from "react";
import authService from "../api-authorization/AuthorizeService";
import $ from "jquery";
import {
    Select,
    MenuItem,
    Grid,
    IconButton,
    TextField,
    FormControl,
    FormLabel,
    Paper
} from "@material-ui/core";
import AddIcon from '@material-ui/icons/Add';
import RemoveIcon from '@material-ui/icons/Remove';
import ConfigTypes from "../../constants/config-presets";
import BoolOptionField from "./options/BoolOptionField";
import TextOptionField from "./options/TextOptionField";
import NumberOptionField from "./options/NumberOptionField";

async function createConfig(configData)
{
    const {
        configName,
        configType,
        data
    } = configData;
    
    const payload  = {
        ConfigName: configName,
        ConfigType: configType,
        Data: JSON.stringify(data)
    };
    
    const jsonPayload = JSON.stringify(payload);
    
    console.log("Payload", payload);
    console.log("JSON", jsonPayload);
    
    const token = await authService.getAccessToken();
    
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        headers:{ "Authorization": `Bearer ${token}`},
        dataType: "json",
        data: jsonPayload,
        url: "/api/config/create",
        success:function(data)
        {
            console.log("Success", data);
        },
        error: function(e, jqXHR, exception)
        {
            console.log(e, jqXHR, exception);
        }
    });
}

function getRequiredField(id, options)
{
    for(var i = 0; i < options.length; i++)
    {
        if(options[i].id === id)
            return options[i];
    }

    return undefined;
}

function hasId(id, options)
{
    for(var i = 0; i < options.length; i++)
    {
        if(options[i].id === id)
            return true;
    }
    
    return false;
}

interface CreateConfigState
{
    name: string;
    data: any;
    newFieldName: string;
    newFieldValue: string | number;
    type: string;
}

export default class CreateConfigComponent extends Component<any, CreateConfigState>
{
    static displayName = "Config";
    
    constructor(props) {
        super(props);
        
        this.state = {
            // Basic values that are required
            name: "",
            type: "",
            
            // This shall contain all the editable information pertaining to this config
            data: {},
            
            // Enable user to insert custom fields
            newFieldName: "",
            newFieldValue: ""
        };
    }
    
    onChangeEvent(event)
    {
        const items = this.state;
        items[event.target.id] = event.target.value;
        
        this.setState(items);
    }
    
    onTypeChange(event)
    {
        const oldType = this.state.type;
        const { data } = this.state;
        
        // Ensure the values are different before we start trying to change things
        if(oldType !== undefined && oldType !== "" && oldType !== event.target.value)
        {
            const oldFields = ConfigTypes[oldType];
            
            // Ensure our old fields actually have values
            if(oldFields !== undefined)
                for(let i = 0; i < oldFields.length; i++)
                    delete data[oldFields[i].id];
        }
        
        const newFields = ConfigTypes[event.target.value];
        
        for(let i = 0; i < newFields.length; i++)
            data[newFields[i].id] = newFields[i].defaultValue;
        
        this.setState({type: event.target.value, data});
    }
    
    onAddField(event)
    {
        const { newFieldName, newFieldValue, data } = this.state;
        
        data[newFieldName] = newFieldValue;
        
        this.setState({newFieldName: "", newFieldValue: "", data})
    }
    
    onUpdateField(event)
    {
        const { data } = this.state;
        
        data[event.target.id] = event.target.value;
        
        this.setState({data});
    }
    
    onDataUpdate(values)
    {
        const { data } = this.state;
        
        data[values.name] = values.value;
        
        this.setState({data});
    }
    
    onRemoveField(event)
    {
        const { data } = this.state;
        
        const fields = ConfigTypes[this.state.type];
        
        if(fields.includes(event.target.name)) 
        {
            alert(`Cannot remove ${event.target.name}`)    
            return;
        }
        
        delete data[event.target.name];
        this.setState({data});
    }
    
    render()
    {
        const { name, type, data, newFieldName, newFieldValue } = this.state;
        
        const requiredFields = type !== undefined ? ConfigTypes[type] : [];
        
        const fields = Object.entries(data)
            .map( ([key, value]) =>
            {
                const required = hasId(key, requiredFields);
                
                if(required)
                {
                    const optionDefinition = getRequiredField(key, requiredFields);
                    let field: any = "";
                    
                    if(typeof value === "string")
                        field = <TextOptionField value={value} name={key} label={optionDefinition.label} onValueChanged={this.onDataUpdate.bind(this)}/>;
                    else if(typeof value === "boolean")
                        field = <BoolOptionField onValueChanged={this.onDataUpdate.bind(this)} label={optionDefinition.label} value={value} name={key}/>;
                    else if(typeof value === "number")
                        field = <NumberOptionField onValueChanged={this.onDataUpdate.bind(this)} label={optionDefinition.label} value={value} name={key}/>
                    
                    return <Paper elevation={3} style={{padding: "16px", margin: "16px"}}>
                        {field}
                    </Paper>
                }
                else
                {
                    return <Paper elevation={3} style={{padding: "16px", margin: "16px"}}>
                        <IconButton name={key} onClick={this.onRemoveField.bind(this)} style={{marginTop: "16px", marginRight: "6px", color: "white", backgroundColor: "red"}}>
                            <RemoveIcon/>
                        </IconButton>
                        <TextField id={key} value={value} label={key} onChange={this.onUpdateField.bind(this)}/>    
                    </Paper>
                }
            })
        
        const typeOptions = Object.entries(ConfigTypes).map(([key, value])=>
        {
            return <MenuItem value={key}>{key}</MenuItem>
        });
        
        return (
            <form style={{marginRight: "16px"}}>
                <Grid container spacing={3}>
                    <Grid item xs={12}>
                        <FormControl>
                            <TextField required
                                       id="name"
                                       name="config-name"
                                       label="Config Name"
                                       value={name}
                                       onChange={this.onChangeEvent.bind(this)}/>
                        </FormControl>
                        
                        
                    </Grid>
                    
                    <Grid item xs={12}>
                        <FormControl>
                            <FormLabel id="config-type-label">Config Type</FormLabel>
                            <Select id="type"
                                    style={{width: "200px"}}
                                    labelId="config-type-label"
                                    value={type}
                                    onChange={this.onTypeChange.bind(this)}>
                                {typeOptions}
                            </Select>
                        </FormControl>
                    </Grid>
                    
                    <Grid item xs={6}>
                        <div>
                            <IconButton style={{borderRadius: "5px", color: "white", backgroundColor: "green", marginRight: "16px"}} 
                                        onClick={this.onAddField.bind(this)}>
                                <AddIcon/>
                            </IconButton>
                            <TextField id="newFieldName" label="Item Name" value={newFieldName} onChange={this.onChangeEvent.bind(this)}/>
                            <TextField id="newFieldValue" label="Item Value" value={newFieldValue} onChange={this.onChangeEvent.bind(this)}/>
                        </div>
                    </Grid>
                    
                    <Grid item xs={12}>
                        {fields}
                    </Grid>
                </Grid>
            </form>
        );
    }
}