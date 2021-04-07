import React, {Component} from "react";
import {Button, ButtonGroup, FormControlLabel, Grid, IconButton, Paper, TextField, withStyles} from "@material-ui/core";
import { CreateCourseTypeOption, CreatePublicTypeOption, CreateResourceTypeOption } from "./CourseAreaOptions";
import $ from "jquery";
import authService from "../api-authorization/AuthorizeService";
import RemoveIcon from "@material-ui/icons/Remove";
import { withSnackbar } from "notistack";

const styles = theme =>({
    container: {
        display: "flex",
        flexWrap: "wrap"
    },
    textField: {
        marginLeft: theme.spacing.unit,
        marginRight: theme.spacing.unit,
        width: 280
    },
    cssLabel: {
        "&.Mui-focused": {
            color: "#fec221",
        }
    },
    cssFocused: {}
});

interface CreateCourseState
{
    courseType: string;
    courseTypeDefinition: CreateCourseTypeOption | CreatePublicTypeOption | CreateResourceTypeOption;
    textChannelName: string;
    voiceChannelName: string;
    errors: {}
}

class CreateCourse extends Component<any, CreateCourseState>
{
    constructor(props) 
    {
        super(props);
        
        this.state = {
            courseType : "course",
            textChannelName: "",
            voiceChannelName: "",
            errors: {},
            courseTypeDefinition: 
            {
                courseNumber: "",
                courseName: "",
                courseCategory: "",
                description: "",
                voiceChannels: [
                    "Voice 1",
                    "Voice 2",
                    "Voice 2"
                ],
                textChannels:
                [
                    "General",
                    "I Need Help",
                    "Course Resources",
                    "Course Feedback"
                ]
            }
        }
    }
    
    setCourseType(value)
    {
        let definition;
        
        switch(value)
        {
            default:
            case "course":
                definition = {
                    courseNumber: "",
                    courseName: "",
                    courseCategory: "",
                    description: "",
                    voiceChannels: [
                        "Voice 1",
                        "Voice 2",
                        "Voice 2"
                    ],
                    textChannels: 
                    [
                        "General",
                        "I Need Help",
                        "Course Resources",
                        "Course Feedback"
                    ]
                }
                break;
            case "public":
                definition = {
                    name: ""
                };
                break;
            case "resource":
                definition = {
                    name: "",
                    requireRole: false,
                    roleName: ""
                }
                break;
        }
        
        this.setState({
            courseType: value,
            errors: {},
            courseTypeDefinition: definition
        });
    }
    
    setData(id, value)
    {
        const { courseTypeDefinition} = this.state;
        
        courseTypeDefinition[id] = value;
        
        this.setState({courseTypeDefinition, errors: this.validateDefinition()});
    }
    
    updateState(id, value)
    {
        const state = this.state;
        
        state[id] = value;
        
        this.setState(state);
    }
    
    validateDefinition()
    {
        const { courseTypeDefinition, errors } = this.state;
        
        for(let key of Object.keys(courseTypeDefinition))
        {
            const value = courseTypeDefinition[key];
            
            if(!value)
                errors[key] = "Field is required";
            else
                errors[key] = "";
        }
        
        return errors;
    }
    
    allDefinitionValid()
    {
        const { courseTypeDefinition } = this.state;
        
        for(const key of Object.keys(courseTypeDefinition))
        {
            let value = courseTypeDefinition[key];
            
            if(value === undefined || value === "")
                return false;
        }
        
        return true;
    }

    /**
     * Add the current textChannelName to the list of text channels
     */
    addTextChannel()
    {
        const { courseTypeDefinition, textChannelName } = this.state;
        
        if(!textChannelName)
        {
            alert("Text Channel cannot be empty");
            return;
        }
        
        courseTypeDefinition["textChannels"].push(textChannelName);
        this.setState({courseTypeDefinition, textChannelName: ""});
    }

    /**
     * Add the current voiceChannelName to the list of voice channels
     */
    addVoiceChannel()
    {
        const { courseTypeDefinition, voiceChannelName } = this.state;
        
        if(!voiceChannelName)
        {
            alert("Voice Channel cannot be empty");
            return;
        }
        
        courseTypeDefinition["voiceChannels"].push(voiceChannelName);
        this.setState({courseTypeDefinition, voiceChannelName: ""});
    }
    
    removeFromList(id, index)
    {
        const { courseTypeDefinition } = this.state;

        if(id in courseTypeDefinition)
        {
            courseTypeDefinition[id].splice(index, 1);
            console.log(courseTypeDefinition[id]);
            
            this.setState({courseTypeDefinition});
        }
        else
            console.log(`${id} does not exist within ${courseTypeDefinition}`);
    }
    
    renderCourse()
    {
        const { classes } = this.props;
        const { courseTypeDefinition, errors } = this.state;
        
        return (
          <React.Fragment>
              
              <p style={{textAlign: "left"}}>Add a new course to the Discord Community. The <code>Course Category</code> should be the TEXT identifier
              of a course. For instance, CARD, CEIS, CIS. <br/> 
              <code>Course Number</code> is the number/text that appears after the <code>Course Category</code> for instance 101, 270C <br/>
              <code>Course Name</code> is the short-name that identifies the course. Such as intro to programming, intro to technology, etc <br/>
              <code>Description</code> has no character limits. This doesn't appear in discord but will eventually be displayed by command. 
                  This should be a description of what the class is but in detail. Yes, if you view it on the site you'll see the description
              </p>
              
              <Grid item xs={12}>
                  <TextField id="courseCategory"
                             name="courseCategory"
                             value={courseTypeDefinition["courseCategory"]}
                             label="Course Category"
                             error={!!errors["courseCategory"]}
                             helperText={errors["courseCategory"]}
                             className={classes.textField}
                             InputLabelProps={{
                                 classes: {
                                     root: classes.cssLabel,
                                     focused: classes.cssFocused
                                 }
                             }}
                             onChange={(event)=>this.setData(event.target.id, event.target.value)}
                  />

                  <TextField id="courseNumber"
                             name="courseNumber"
                             value={courseTypeDefinition["courseNumber"]}
                             error={!!errors["courseNumber"]}
                             helperText={errors["courseNumber"]}
                             label="Course Number"
                             className={classes.textField}
                             InputLabelProps={{
                                 classes: {
                                     root: classes.cssLabel,
                                     focused: classes.cssFocused
                                 }
                             }}
                             onChange={(event)=>this.setData(event.target.id, event.target.value)}
                  />

                  <TextField id="courseName"
                             name="courseName"
                             value={courseTypeDefinition["courseName"]}
                             error={!!errors["courseName"]}
                             helperText={errors["courseName"]}
                             label="Course Name"
                             className={classes.textField}
                             InputLabelProps={{
                                 classes: {
                                     root: classes.cssLabel,
                                     focused: classes.cssFocused
                                 }
                             }}
                             onChange={(event)=>this.setData(event.target.id, event.target.value)}
                  />
              </Grid>

              <Grid item xs={12}>
                  <TextField id="description"
                             name="description"
                             value={courseTypeDefinition["description"]}
                             label="Description"
                             error={!!errors["description"]}
                             helperText={errors["description"]}
                             className={classes.textField}
                             InputLabelProps={{
                                 classes: {
                                     root: classes.cssLabel,
                                     focused: classes.cssFocused
                                 }
                             }}
                             multiline={true}
                             onChange={(event)=>this.setData(event.target.id, event.target.value)}
                  />
              </Grid>
              
              <Grid item xs={12}>
                  <TextField id="textChannels"
                             name="textChannels"
                             label="Text Channel"
                             InputLabelProps={{
                                 classes: {
                                     root: classes.cssLabel,
                                     focused: classes.cssFocused
                                 }
                             }}
                             onChange={(event)=>{this.updateState("textChannelName", event.target.value)}}
                             InputProps={{
                                 endAdornment: <Button onClick={(event)=>
                                 {
                                     this.addTextChannel();
                                 }}>Add</Button>
                             }}/>
                  <br/>
                  <h4 style={{color: "White", marginTop: "10px", marginBottom: "16px"}}>Text Channels</h4>
                  { courseTypeDefinition["textChannels"].map((value, index)=>
                  {
                      return (
                          <React.Fragment>
                              <FormControlLabel control={
                                  <IconButton style={{backgroundColor: "red", marginRight: "24px"}}>
                                      <RemoveIcon
                                          onClick={(event)=>
                                          {
                                              this.removeFromList("textChannels", index);
                                          }}/>
                                  </IconButton>
                              } label={value}/>  
                              <br/>
                          </React.Fragment>
                      );
                  })}
              </Grid>
              
          </React.Fragment>  
        );
    }
    
    renderPublic()
    {
        const { classes } = this.props;
        return (
            <React.Fragment>
                <code>Not Implemented</code>
            </React.Fragment>
        );
    }
    
    renderResource()
    {
        const { classes } = this.props;
        return (
            <React.Fragment>
                <code>Not Implemented</code>
            </React.Fragment>
        );
    }
    
    createData()
    {
        const { courseType, courseTypeDefinition } = this.state;
        let data = {};
        
        switch(courseType)
        {
            case "course":
                data = 
                {
                    CourseNumber: courseTypeDefinition["courseNumber"],
                    CourseName: courseTypeDefinition["courseName"],
                    CourseCategory: courseTypeDefinition["courseCategory"],
                    Description: courseTypeDefinition["description"],
                    VoiceChannels: courseTypeDefinition["voiceChannels"],
                    TextChannels: courseTypeDefinition["textChannels"]
                };
                break;
        }
        
        return data;
    }
    
    async createChannel()
    {
        const { courseType } = this.state;
        const token = await authService.getAccessToken();
        const jsonPayload = JSON.stringify(this.createData());
        console.log(jsonPayload); 
        const self = this;
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            headers: { "Authorization": `Bearer ${token}`},
            dataType: "json",
            data: jsonPayload,
            url: "/api/course/create-course",
            success:function(data)
            {
                console.log("success", data);
                self.setCourseType(courseType);
                self.props.enqueueSnackbar("Channels have been created!", 
                {
                    variant: "success"   
                });
            },
            error: function(e, jqXHR, exception)
            {
                console.log(e, jqXHR, exception);
                self.props.enqueueSnackbar("Error during creation",
                {
                    variant: "error"
                });
            }
        });
    }
    
    render()
    {
        const { courseType } = this.state;
        
        let renderArea;
        
        switch(courseType)
        {
            case "course":
                renderArea = this.renderCourse();
                break;
            case "resource":
                renderArea = this.renderResource();
                break;
            case "public":
                renderArea = this.renderPublic();
                break;
        }
        
        const buttons = [
            {
                name: "Course", 
                type: "course"
            }, 
            {
                name: "Resource", 
                type:"resource"
            }, 
            {
                name: "Public Area",
                type: "public"
            }].map((value, index)=>
        {
           const currentlySelected = courseType === value.type;
           
           return (
               <Button
                   color={currentlySelected ? "secondary" : "primary"}
                   onClick={(event)=>{this.setCourseType(value.type)}}>
                   {value.name}
               </Button>
           )
        });
        
        return(
            <Paper elevation={2} style={{padding:"32px"}}>
                <h2 style={{textAlign: "center", color: "white"}}>Channel Creation</h2>
                
                <Grid container 
                      direction="column" 
                      alignItems="center"
                      spacing={3}>
                    
                    <Grid item xs={12}>
                        <ButtonGroup variant="contained" color="primary" aria-label="contained primary button group">
                            {buttons}
                        </ButtonGroup>
                    </Grid>
                    
                    <Grid item xs={12}>
                        {renderArea}
                        
                        <br/><Button variant="contained" 
                                     color="secondary"
                                     disabled={!this.allDefinitionValid()}
                                     onClick={(event)=> {
                                         this.createChannel();
                                     }}
                                     style={{marginTop: "16px", marginBottom: "16px"}}>Create</Button>
                    </Grid>
                </Grid>
            </Paper>
        )        
    }
}

// @ts-ignore
export default withStyles(styles)(withSnackbar(CreateCourse));