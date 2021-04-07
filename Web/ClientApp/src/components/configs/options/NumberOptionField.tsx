import React, {FC} from "react";
import {NumberOption} from "./Option";
import {FormControlLabel, TextField} from "@material-ui/core";

interface TextOptionProps extends NumberOption
{
    onValueChanged: any;
    label: string;
}

const NumberOptionField: FC<TextOptionProps> = ( {name, label, value, onValueChanged}) =>
{
   return (
     <FormControlLabel 
         control={
             <TextField value={value}
                        onChange={(event)=>{onValueChanged({name, value:event.target.value})}}
                        name={name}
                        type={"number"}
             />
                        
         } 
         label={label}
         labelPlacement={"start"}/>
   );
}

export default NumberOptionField;