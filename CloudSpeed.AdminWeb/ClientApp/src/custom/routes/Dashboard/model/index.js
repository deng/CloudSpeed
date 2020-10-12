import modelEnhance from '@/utils/modelEnhance';
import PageHelper from '@/utils/pageHelper';

export default modelEnhance({
  namespace: 'dashboard',

  state: {
    info: {},
    pageData: PageHelper.create(),
  },

  subscriptions: {
    setup({ history, dispatch }) {
      return history.listen(({ pathname }) => {
        if (pathname.indexOf('/dashboard') !== -1) {
          dispatch({
            type: 'init'
          });
        }
      });
    }
  },
  effects: {
    *init({ }, { put }) {
      yield put({
        type: 'getJobsInfo',
      });
      yield put({
        type: 'getDealsInfo',
      });
    },
    *getJobsInfo({ }, { put }) {
      yield put({
        type: '@request',
        payload: {
          valueField: 'jobsData',
          url: '/dashboard/jobs',
        }
      });
    },
    *getDealsInfo({ }, { put }) {
      yield put({
        type: '@request',
        payload: {
          valueField: 'dealsData',
          url: '/dashboard/deals',
        }
      });
    },
  },
  reducers: {}
});
