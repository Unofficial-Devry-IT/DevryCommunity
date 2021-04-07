export default({
    "Command": [
        {
            label: "Name",
            id: "Name",
            type: "text", 
            defaultValue: "", 
            isRequired: true
        },
        {
            id: "IconURL",
            label: "Icon URL", 
            type: "text", 
            defaultValue: "", 
            isRequired: false
        },
        {
            id: "BlacklistedRoles",
            label: "Blacklisted Roles", 
            type: "Roles", 
            defaultValue: [], 
            isRequired: false
        },
        {
            id: "Description",
            label: "Description", 
            type: "text", 
            defaultValue: "", 
            isRequired: false
        },
        {
            id: "AdminOnly",
            label: "Admin Only", 
            type: "boolean", 
            defaultValue: false, 
            isRequired: false
        }
    ],
})