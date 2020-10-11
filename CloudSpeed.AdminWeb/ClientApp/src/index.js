import React from 'react';
import dva, { dynamic, router } from 'dva';
import createLoading from 'dva-loading';
import { createHashHistory } from 'history';
import request from 'cmn-utils/lib/request';
import createRoutes from '@/custom/routes';
import './custom/assets/styles/index.less';
import config from './config';
import { ConfigProvider } from 'antd';
import zh_CN from 'antd/es/locale/zh_CN';
import 'moment/locale/zh-cn';
import { homepage } from '../package.json';
import * as serviceWorker from './serviceWorker';

const { Router } = router;

const app = dva({
  history: createHashHistory({
    basename: homepage.startsWith('/') ? homepage : ''
  })
});

app.use(createLoading());
app.use({ onError: config.exception.global });

request.config(config.request);

require('./custom/__mocks__');
// -> Developer mock data
// if (process.env.NODE_ENV === 'development') {
//   require('./__mocks__');
// }

// -> loading
dynamic.setDefaultLoadingComponent(() => config.router.loading);

app.model(require('./models/global').default);

app.router(({ history, app }) => (
  <ConfigProvider locale={zh_CN}>
    <Router history={history}>{createRoutes(app)}</Router>
  </ConfigProvider>
));

// -> Start
app.start('#root');

// export global
export default {
  app,
  store: app._store,
  dispatch: app._store.dispatch
};

serviceWorker.unregister();
