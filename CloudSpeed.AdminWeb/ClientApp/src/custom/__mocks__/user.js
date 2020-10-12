export default ({ fetchMock, delay, mock, toSuccess, toError }) => {
  return {
    '/api/user/menu': options => toSuccess([
      {
        name: 'Dashboard',
        icon: 'DashboardOutlined',
        path: '/dashboard',
      },
      {
        name: 'Jobs',
        icon: 'RadarChartOutlined',
        path: '/jobs',
        children: [
          {
            name: 'Powergate Jobs',
            path: '/jobs',
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
        name: 'Imports',
        icon: 'RadarChartOutlined',
        path: '/imports',
        children: [
          {
            name: 'Logs',
            path: '/imports',
          },
        ],
      },
    ], 400)
  }
}