import {dynamicWrapper, createRoute} from '@/utils/core';

const routesConfig = (app) => ({
  path: '/miner/storage/sector',
  title: '扇区列表',
  component: dynamicWrapper(app, [import('./model')], () => import('./components')),
  exact: true
});

export default (app) => createRoute(app, routesConfig);
