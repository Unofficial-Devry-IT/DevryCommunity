import React, {useCallback, useRef} from "react";
import {IconButton, TextField} from "@material-ui/core";
import RemoveIcon from "@material-ui/icons/Remove";
import {Input} from "antd";

export default function TextValueComponent(props)
{
    const {key, removeCallback=undefined} = props;
    const inputRef = useRef(null);
    const [value, setValue] = useState("");
    
    const customSetValue = useCallback((newValue)=>
    {
        setValue(newValue);
        inputRef.current.setValue(newValue);
    }, [inputRef]);
    
    return (
       <div>
           <IconButton style{{
               borderRadius: "5px", 
               color: "white", 
               backgroundColor: "red", 
               marginRight: "16px"
           }} name={key} onClick={(event)=>
           {
               if(removeCallback)
               {
                   removeCallback(event.target.name);
               }
           }}>
               <RemoveIcon/>
           </IconButton>
           
            <TextField  ref={inputRef}
                        label={key}
                        value={value}
                        id={key}
                        onChange={(event)=>{
                          customSetValue(event.target.value);
                      }}
                      />
       </div>  
    );
}