import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import {createMuiTheme, MuiThemeProvider} from "@material-ui/core";
//import registerServiceWorker from './registerServiceWorker';
import { SnackbarProvider } from "notistack";

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

const THEME = createMuiTheme(
{
            palette: 
            {
                type: "dark"
            }
        });

ReactDOM.render(
  <BrowserRouter basename={baseUrl}>
        <MuiThemeProvider theme={THEME}>
            <SnackbarProvider maxSnack={5}>
                <App />
            </SnackbarProvider>
        </MuiThemeProvider>
  </BrowserRouter>,
  rootElement);

// Uncomment the line above that imports the registerServiceWorker function
// and the line below to register the generated service worker.
// By default create-react-app includes a service worker to improve the
// performance of the application by caching static assets. This service
// worker can interfere with the Identity UI, so it is
// disabled by default when Identity is being used.
//
//registerServiceWorker();

