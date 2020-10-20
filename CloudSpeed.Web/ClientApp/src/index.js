import 'bootstrap/dist/css/bootstrap-grid.min.css';
import 'antd/dist/antd.css';
import './nav.css'
import './custom.css'
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Route } from 'react-router';
import { Provider } from 'react-redux';
import { ConnectedRouter } from 'connected-react-router';
import { createBrowserHistory } from 'history';
import configureStore from './store/configureStore';
import { ConfigProvider } from 'antd';
import en_US from 'antd/es/locale/en_US';
//import App from './App';
//import App from './Home';
import registerServiceWorker from './registerServiceWorker';
import Layout from './components/Layout';
import Upload from './pages/Upload';
import Pan from './pages/Pan';
import MyFiles from './pages/MyFiles';
import Download from './pages/Download';

// Create browser history to use in the Redux store
const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const history = createBrowserHistory({ basename: baseUrl });

// Get the application-wide store instance, prepopulating with state from the server where available.
const store = configureStore(history);

ReactDOM.render(
    <Provider store={store}>
        <ConnectedRouter history={history}>
            <ConfigProvider locale={en_US}>
                <Layout>
                    <Route exact path='/' component={Upload} />
                    <Route exact path='/pan/item-:id' component={Pan} />
                    <Route exact path='/download' component={Download} />
                    <Route exact path='/myfiles' component={MyFiles} />
                </Layout>
            </ConfigProvider>
        </ConnectedRouter>
    </Provider>,
    document.getElementById('root'));

registerServiceWorker();
