import DataItem from "../models/data-item";

function keyValuePair(pair)
{
    
}

function listValuePair(pair)
{
    
}

function dictValuePair(pair)
{
    
}

export default function DataManager(props)
{
    const data = props.data || {};
    
    Object.entries(data).map(([key, value])=>
    {
        switch(value["type"])
        {
            case "text":
            case "number":
                return keyValuePair(value);
            case "list":
                return listValuePair(value);
        }
    });
    
}