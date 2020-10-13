import modelEnhance from '@/utils/modelEnhance';
import PageHelper from '@/utils/pageHelper';
let LOADED = false;
export default modelEnhance({
  namespace: 'deals',

  state: {
    pageData: PageHelper.create(),
  },

  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/deals/list' && !LOADED) {
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
      const { pageData } = yield select(state => state.deals);
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
          url: '/deals/getList',
          pageInfo: pageData
        }
      });
    },
    *reset({ payload }, { put, select, take }) {
      const { records, success } = payload;
      const { pageData } = yield select(state => state.deals);
      yield put({
        type: '@request',
        payload: {
          notice: true,
          url: '/deals/reset',
          data: records.map(item => item.id)
        }
      });
      yield take('@request/@@end');
      yield put({
        type: 'getPageInfo',
        payload: { pageData }
      });
      success();
    },
  },

  reducers: {}
});
