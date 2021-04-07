import React, {FC} from "react";
import { TextOption } from "./Option";
import {FormControl, FormControlLabel, TextField} from "@material-ui/core";

interface TextOptionProps extends TextOption
{
    onValueChanged: any;
    label: string;
}

const TextOptionField: FC<TextOptionProps> = ({label, name, value, onValueChanged}) =>
{
    return (
      <FormControl>
        <FormControlLabel labelPlacement={"start"} control={<TextField value={value} name={name} onChange={(event)=>{onValueChanged({name, value: event.target.value}); }} />} 
                                              label={label}/>
      </FormControl>    
    );
};

export default TextOptionField;
