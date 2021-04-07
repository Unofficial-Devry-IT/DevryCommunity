import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { FetchReminders} from "./components/reminders/reminders";
import { Counter } from './components/Counter';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';

import './custom.css'
import {FetchChannels} from "./components/Channel";
import CreateConfigComponent from "./components/configs/create-config";
import CreateCourse from "./components/course/Create-Course";

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/counter' component={Counter} />
        <AuthorizeRoute path='/fetch-data' component={FetchData} />
        <AuthorizeRoute path="/discord" component={FetchChannels}/>
        <AuthorizeRoute path="/reminders" component={FetchReminders}/>
        <AuthorizeRoute path="/config" component={CreateConfigComponent}/>
        <AuthorizeRoute path="/create-course" component={CreateCourse}/>
        <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
      </Layout>
    );
  }
}
