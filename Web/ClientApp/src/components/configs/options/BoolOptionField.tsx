import React, {FC} from "react";
import { BoolOption } from "./Option";
import {FormControl, FormControlLabel, Switch, TextField} from "@material-ui/core";

interface BoolOptionProps extends BoolOption
{
    onValueChanged: any;
    label: string;
}

const BoolOptionField: FC<BoolOptionProps> = ({name, label, value, onValueChanged}) =>
{
    return (
        <FormControl>
            <FormControlLabel
                labelPlacement={"start"}
                control={<Switch checked={value} 
                                 onChange={(event)=>{onValueChanged({name, value: event.target.checked})}} 
                                 name={name}/>}
                label={label}
            />
        </FormControl>
    )  
};

export default BoolOptionField;