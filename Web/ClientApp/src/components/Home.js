import React, { Component } from 'react';
import {Grid, Paper} from "@material-ui/core";

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
        <Grid container spacing={3}>
            <Grid item xs={12} md={4}>
                <Paper elevation={3} style={{padding: "32px"}}>
                    <h2>Github</h2>
                    <p>
                        This entire website and discord bot is open source. Feel free to contribute if you want! <a href="https://github.com/JBraunsmaJr/Devry-Service-Bot">Link</a><br/>
                        
                        If you discover an issue please open a <a href="https://github.com/JBraunsmaJr/Devry-Service-Bot/issues">ticket</a>!
                    </p>
                </Paper>
            </Grid>
            
            <Grid item xs={12} md={4}>
                <Paper elevation={3} style={{padding: "32px"}}>
                    <h2>Overview</h2>
                    <p>
                        This website is currently under construction. Many features you'd probably expect may not be implemented yet.
                        Below is a list of features currently in-use / available to you
                    </p>
                    
                    <br/><br/>
                    <h3>Current Features</h3>
                    <p>
                        <ul>
                            <li>Can view channels from associated discord server</li>
                            <li>Can create a <code>course</code> area (i.e create category/role/channels for a class)</li>
                            <li>Can create reminders that will appear on certain channels</li>
                        </ul>
                    </p>
                </Paper>
            </Grid>
            
            <Grid item xs={12} md={4}>
                <Paper elevation={3} style={{padding: "32px"}}>
                    <h2>Disclaimer</h2>
                    <p>
                        Registration portion of this site has not been fully fledged out.
                        This means -- we can't quite differenciate between moderators or average user(s).
                    </p>
                    
                    <p>
                        Not ready for general use by the community... Granted the current features available are mostly administrative in nature.
                    </p>
                </Paper>
            </Grid>
        </Grid>
    );
  }
}
