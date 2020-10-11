import modelEnhance from '@/utils/modelEnhance';
import PageHelper from '@/utils/pageHelper';
export default modelEnhance({
  namespace: 'miner',

  state: {
    pageData: PageHelper.create(),
  },

  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/lotus/miner') {
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
      const { pageData } = yield select(state => state.miner);
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
