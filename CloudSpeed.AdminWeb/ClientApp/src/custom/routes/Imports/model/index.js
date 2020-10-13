import modelEnhance from '@/utils/modelEnhance';
import PageHelper from '@/utils/pageHelper';
let LOADED = false;
export default modelEnhance({
  namespace: 'imports',

  state: {
    pageData: PageHelper.create(),
  },

  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/imports/list' && !LOADED) {
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
      const { pageData } = yield select(state => state.imports);
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
          url: '/imports/getList',
          pageInfo: pageData
        }
      });
    },
  },

  reducers: {}
});
