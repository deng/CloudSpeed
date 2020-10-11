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
    // 进入页面加载
    *init({ payload }, { call, put, select }) {
      const { pageData } = yield select(state => state.dashboard);
      yield put({
        type: 'getPageInfo',
        payload: {
          pageData: pageData.startPage(1, 10)
        }
      });
    },
    // 获取分页数据
    *getPageInfo({ payload }, { call, put }) {
      const { pageData } = payload;
      yield put({
        type: '@request',
        payload: {
          valueField: 'pageData',
          url: '/miners/list',
          pageInfo: pageData
        }
      });
    },
  },
  reducers: {}
});
