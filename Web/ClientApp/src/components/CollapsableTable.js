import React from "react";
import {
    Box,
    Collapse,
    IconButton,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Typography,
    Paper,
    makeStyles
} from "@material-ui/core";

import KeyboardArrowDownIcon from "@material-ui/icons/KeyboardArrowDown";
import KeyboardArrowUpIcon from "@material-ui/icons/KeyboardArrowUp";

const useRowStyles = makeStyles({
    root: {
        "& > *":{
            borderBottom: "unset"
        }
    }
});

function Row(props)
{
    const { row, headers = [] } = props;
    const [ open, setOpen] = React.useState(false);
    const classes = useRowStyles();
    return(
        <React.Fragment>
            <TableRow className={classes.root}>
                <TableCell>
                    <IconButton aria-label="expand row" size="small" onClick={()=>setOpen(!open)}>
                        {open ? <KeyboardArrowUpIcon/> : <KeyboardArrowDownIcon/>}
                    </IconButton>
                </TableCell>
                { row.values && row.values.length &&
                    row.values.map(text=><TableCell align="right">{text}</TableCell>)
                }
            </TableRow>

            <TableRow>
                <TableCell style={{paddingbottom: 0, paddingTop: 0}} colSpan={headers.length+1}>
                    <Collapse in={open} timeout="auto" unmountOnExit>
                        <Box margin={1}>
                            {row.content}
                        </Box>
                    </Collapse>
                </TableCell>
            </TableRow>
        </React.Fragment>
    )
}

export default function CollapsableTable(props)
{
    return(
        <TableContainer component={Paper}>
            <Table aria-labelledby="collapsible table">
                <TableHead>
                    <TableRow>
                        <TableCell/>
                        {props.headers.map(text=><TableCell align="right">{text}</TableCell>)}
                    </TableRow>
                </TableHead>
                <TableBody>
                    {props.rows.map((row)=>(
                        <Row key={row.name} row={row}/>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    )
}