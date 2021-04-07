import RemoveIcon from "@material-ui/icons/Remove";
import {Button, IconButton, TextField} from "@material-ui/core";
import React from "react";
import AddIcon from "@material-ui/icons/Add";

export default class ListValueComponent extends React.Component
{
    constructor(props) {
        super(props);
        
        this.state = {
            newValue: "",
            data: []
        };
    }
    
    onValueChanged(event)
    {
        this.setState({newValue: event.target.value});
    }
    
    onValueAdded(event)
    {
        let { newValue, data } = this.state;
        
        data.push(newValue);
        newValue = "";
        
        this.setState({newValue, data});
    }
    
    onValueRemoved(index)
    {
         const { data } = this.state;
         
         data.slice(index,1);
         
         this.setState({data});
    }
    
    onValueUpdated(event)
    {
        const { data } = this.data;
        
        data[event.target.id] = event.target.value;
        
        this.setState({data});
    }
    
    onSave(event)
    {
        const { saveCallback = undefined, name } = this.props;
        const { data } = this.state;
        
        if(saveCallback)
        {
            console.log("Saving List Value Component", name, data);
            saveCallback(name, data);
        }
    }
    
    render()
    {
        const {name, removeCallback, newValue} = this.props;
        const { data } = this.state;
        
        const fields = data.map((item, index)=>
        {
            return <div>
                <IconButton style={{
                    borderRadius: "5px",
                    color: "white",
                    backgroundColor: "red",
                    marginRight: "16px"
                }}
                            name={index}
                            onClick={(event)=>
                            {
                                this.onValueRemoved(index);
                            }}>
                    <RemoveIcon/>
                </IconButton>
                
                <TextField id={index} value={item} onChange={this.onValueUpdated.bind(this)}/>
            </div>
        }) 
            
        
        return (
            <div>
                <IconButton style={{
                    borderRadius: "5px",
                    color: "white",
                    backgroundColor: "red",
                    marginRight: "16px"
                }}
                            name={name}
                            onClick={removeCallback.bind(this)}>
                    <RemoveIcon/>
                </IconButton>

                <IconButton style={{
                    borderRadius: "5px",
                    color: "white",
                    backgroundColor: "green",
                    marginRight: "16px"
                }}
                            name={name}
                            onClick={this.onValueAdded.bind(this)}>
                    <AddIcon/>
                </IconButton>
                
                <TextField id="newValue" value={newValue} onChange={this.onValueChanged.bind(this)}/>
                
                <Button onClick={this.onSave.bind(this)}>Save</Button>
                <hr/>
                {fields}
            </div>
        )       
    }
}