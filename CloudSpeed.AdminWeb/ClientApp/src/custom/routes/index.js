import { createRoutes } from '@/utils/core';
import BasicLayout from '@/custom/layouts/BasicLayout';
import UserLayout from '@/custom/layouts/UserLayout';
import Page403 from './Pages/403';
import NotFound from './Pages/404';
import Page500 from './Pages/500';
import Dashboard from './Dashboard';
import Jobs from './Jobs';
import Deals from './Deals';
import Imports from './Imports';

/**
 * 主路由配置
 * 
 * path 路由地址
 * component 组件
 * indexRoute 默认显示路由
 * childRoutes 所有子路由
 * NotFound 路由要放到最下面，当所有路由当没匹配到时会进入这个页面
 */
const routesConfig = app => [
  {
    path: '/',
    title: '系统中心',
    component: BasicLayout,
    indexRoute: '/dashboard',
    childRoutes: [
      Dashboard(app),
      Jobs(app),
      Deals(app),
      Imports(app),
      Page403(),
      Page500(),
      NotFound()
    ]
  }
];

export default app => createRoutes(app, routesConfig);
