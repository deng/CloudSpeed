import modelEnhance from '@/utils/modelEnhance';
import PageHelper from '@/utils/pageHelper';
let LOADED = false;
export default modelEnhance({
  namespace: 'jobs',

  state: {
    pageData: PageHelper.create(),
  },

  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/jobs/list' && !LOADED) {
          LOADED = true;
          dispatch({
            type: 'init'
          });
        }
      });
    }
  },

  effects: {
    *init({ }, { put, select }) {
      const { pageData } = yield select(state => state.jobs);
      yield put({
        type: 'getPageInfo',
        payload: {
          pageData: pageData.startPage(1, 10)
        }
      });
    },
    *getPageInfo({ payload }, { call, put }) {
      const { pageData } = payload;
      yield put({
        type: '@request',
        payload: {
          valueField: 'pageData',
          url: '/jobs/getList',
          pageInfo: pageData
        }
      });
    },
  },

  reducers: {}
});
