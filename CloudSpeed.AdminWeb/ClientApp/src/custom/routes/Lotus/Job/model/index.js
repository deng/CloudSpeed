import modelEnhance from '@/utils/modelEnhance';
import PageHelper from '@/utils/pageHelper';
import $$ from 'cmn-utils';
export default modelEnhance({
  namespace: 'job',

  state: {
    pageData: PageHelper.create(),
    employees: []
  },

  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/lotus/job') {
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
      const { pageData } = yield select(state => state.job);
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
          url: '/jobs/list',
          pageInfo: pageData
        }
      });
    },
    // 保存 之后查询分页
    *save({ payload }, { call, put, select, take }) {
      const { values, success } = payload;
      const { pageData } = yield select(state => state.job);
      // put是非阻塞的 put.resolve是阻塞型的
      yield put.resolve({
        type: '@request',
        payload: {
          notice: true,
          url: '/sectors/pledge',
          data: values
        }
      });
      yield $$.delay(2000);
      yield put.resolve({
        type: 'getPageInfo',
        payload: { pageData }
      });
      success();
    },
  },
  reducers: {}
});
