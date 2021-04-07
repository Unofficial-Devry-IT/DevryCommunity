import authService from "../api-authorization/AuthorizeService";
import "../../util/encode";
import {encodeText} from "../../util/encode";
import $ from "jquery";

export async function GetAllConfigs()
{
    const token = await authService.getAccessToken();
    const response = await fetch(`api/config/configs`, {
        headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    });
    
    return await response.json();
}

export async function FindConfig(id)
{
    const token = await authService.getAccessToken();
    const response = await fetch(`api/config?Id=${id}`, {
        headers: !token ? {} : {"Authorization": `Bearer: ${token}`}
    });
    
    return await response.json();
}

export async function UpdateConfig(dataDict, snackbar)
{
    const token = await authService.getAccessToken();

    /*
    *   We are encoding the json text to ensure nothing gets lost
    *   ... not funny business while crossing the wire
    */

    const payload =
        {
            Id: dataDict["id"],
            ConfigName: dataDict["configName"],
            ConfigType: dataDict["configType"],
            ExtensionData: encodeText(dataDict["extensionData"])
        };

    const json = JSON.stringify(payload);

    $.ajax({
        type: "PUT",
        contentType: "application/json; charset=utf-8",
        headers: {"Authorization": `Bearer ${token}`},
        dataType: "json",
        data: json,
        url: "/api/config",
        success: function(data)
        {
            if(snackbar)
            {
                snackbar("Config Created!",
                {
                    variant: "success"
                });
            }
        },
        error: function(a, jqXHR, exception)
        {
            console.log(a, jqXHR, exception);
            if(snackbar)
            {
                snackbar("Error during creation",
                {
                    variant: "error"
                });
            }
        }
    })
}

export async function CreateConfig(dataDict, snackbar)
{
    const token = await authService.getAccessToken();
    
    /*
    *   We are encoding the json text to ensure nothing gets lost
    *   ... not funny business while crossing the wire
    */
    
    const payload = 
    {
        ConfigName: dataDict["configName"],
        ConfigType: dataDict["configType"],
        ExtensionData: encodeText(dataDict["extensionData"])
    };
    
    const json = JSON.stringify(payload);
    
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        headers: {"Authorization": `Bearer ${token}`},
        dataType: "json",
        data: json,
        url: "/api/config",
        success: function(data)
        {
            if(snackbar)
            {
                snackbar("Config Created!",
                {
                    variant: "success"
                });
            }
        },
        error: function(a, jqXHR, exception)
        {
            console.log(a, jqXHR, exception);
            if(snackbar)
            {
                snackbar("Error during creation", 
                { 
                    variant: "error"
                });
            }
        }
    })    
}