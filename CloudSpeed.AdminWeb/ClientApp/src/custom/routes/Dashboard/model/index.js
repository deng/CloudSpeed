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
        type: 'getInfo',
      });
    },
    *getInfo({ }, { put }) {
      yield put({
        type: '@request',
        payload: {
          valueField: 'info',
          url: '/dashboard/info',
        }
      });
    },
  },
  reducers: {}
});
