import modelEnhance from '@/utils/modelEnhance';
import PageHelper from '@/utils/pageHelper';
import { getQueryObject } from '@/custom/utils';
let LOADED = false;
export default modelEnhance({
  namespace: 'uploads',

  state: {
    pageData: PageHelper.create(),
  },

  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/uploads/list' && !LOADED) {
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
      const { pageData } = yield select(state => state.uploads);
      const { user } = getQueryObject();
      yield put({
        type: 'getPageInfo',
        payload: {
          pageData: !!user ? pageData.startPage(1, 10).filter({ userName: user }, true) : pageData.startPage(1, 10)
        }
      });
    },
    *getPageInfo({ payload }, { call, put }) {
      const { pageData } = payload;
      yield put({
        type: '@request',
        payload: {
          valueField: 'pageData',
          url: '/uploads/getList',
          pageInfo: pageData
        }
      });
    },
    *reset({ payload }, { put, select, take }) {
      const { records, success } = payload;
      const { pageData } = yield select(state => state.uploads);
      yield put({
        type: '@request',
        payload: {
          notice: true,
          url: '/uploads/reset',
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
