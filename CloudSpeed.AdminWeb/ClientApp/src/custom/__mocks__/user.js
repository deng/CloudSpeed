export default ({ fetchMock, delay, mock, toSuccess, toError }) => {
  return {
    '/api/user/menu': options => toSuccess([
      {
        name: 'Dashboard',
        icon: 'DashboardOutlined',
        path: '/dashboard',
      },
      {
        name: 'Files',
        icon: 'RadarChartOutlined',
        path: '/files',
        children: [
          {
            name: 'Logs',
            path: '/files/logs',
          },
        ],
      },
      {
        name: 'Jobs',
        icon: 'RadarChartOutlined',
        path: '/jobs',
        children: [
          {
            name: 'Powergate Jobs',
            path: '/jobs/',
          },
        ],
      },
      {
        name: 'Deals',
        icon: 'RadarChartOutlined',
        path: '/deals',
        children: [
          {
            name: 'Lotus Deals',
            path: '/deals',
          },
        ],
      },
      {
        name: 'Storages',
        icon: 'RadarChartOutlined',
        path: '/storages',
        children: [
          {
            name: 'Logs',
            path: '/storages/logs',
          },
          {
            name: 'Import',
            path: '/storages/import',
          },
        ],
      },
    ], 400)
  }
}