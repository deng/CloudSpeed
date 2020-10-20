import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './page/Upload';
import Pan from './page/Pan';

export default () => (
    <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/pan/:id' component={Pan} />
    </Layout>
);
