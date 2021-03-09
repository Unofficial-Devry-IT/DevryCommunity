import React from "react";
import {
    DialogTitle,
    Dialog,
} from "@material-ui/core";

export default class SimpleDialog extends React.Component
{
    constructor(props) {
        super(props);
    }
        
    handleClose()
    {
        this.props.onClose();
    }
    
    render()
    {
        const { open, title = "", children} = this.props;
        return (
                <Dialog
                    className={this.props.className ? this.props.className : ""}    
                    onClose={this.handleClose.bind(this)} 
                    aria-labelledby="simple-dialog-title" 
                    open={open}>
                    <DialogTitle id="simple-dialog-title">{title}</DialogTitle>
                    {children}
                </Dialog>
            );
    }
}
