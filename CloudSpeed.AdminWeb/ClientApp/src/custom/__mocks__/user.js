export default ({ fetchMock, delay, mock, toSuccess, toError }) => {
  return {
    '/api/user/menu': options => toSuccess([
      {
        name: 'Dashboard',
        icon: 'DashboardOutlined',
        path: '/dashboard',
      },
      {
        name: 'Uploads',
        icon: 'RadarChartOutlined',
        path: '/uploads',
        children: [
          {
            name: 'Upload Logs',
            path: '/uploads/list',
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
            path: '/jobs/list',
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
            path: '/deals/list',
          },
        ],
      },
    ], 400)
  }
}