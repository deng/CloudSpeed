import modelEnhance from '@/utils/modelEnhance';
import PageHelper from '@/utils/pageHelper';
import { getQueryObject } from '@/custom/utils';
export default modelEnhance({
  namespace: 'sector',

  state: {
    pageData: PageHelper.create(),
    employees: []
  },

  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname.indexOf('/lotus/miner/sector')>=0||pathname.indexOf('/miner/storage/sector')>=0) {
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
      const { pageData } = yield select(state => state.sector);
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
      const { minerId } = getQueryObject();
      yield put({
        type: '@request',
        payload: {
          valueField: 'pageData',
          url: '/sectors/list',
          pageInfo: !!minerId ? pageData.filter({ minerId }, true) : pageData
        }
      });
    },
    // 保存 之后查询分页
    *updateState({ payload }, { call, put, select, take }) {
      const { values, success } = payload;
      const { pageData } = yield select(state => state.sector);
      // put是非阻塞的 put.resolve是阻塞型的
      yield put.resolve({
        type: '@request',
        payload: {
          notice: true,
          url: '/sectors/updateState',
          data: values
        }
      });

      yield put.resolve({
        type: 'getPageInfo',
        payload: { pageData }
      });
      success();
    },
    *remove({ payload }, { call, put, select }) {
      const { records, success } = payload;
      const { pageData } = yield select(state => state.sector);
      yield put.resolve({
        type: '@request',
        payload: {
          notice: true,
          url: '/sectors/remove',
          data: records.map(item => {return {sectorNumber:item.sectorNumber, minerId:item.minerId};})
        }
      });
      yield put.resolve({
        type: 'getPageInfo',
        payload: { pageData }
      });
      success();
    },
    *updateFailedUnrecoverable({ payload }, { call, put, select }) {
      const { records, success } = payload;
      const { pageData } = yield select(state => state.sector);
      yield put.resolve({
        type: '@request',
        payload: {
          notice: true,
          url: '/sectors/updateFailedUnrecoverable',
          data: records.map(item => {return {sectorNumber:item.sectorNumber, minerId:item.minerId};})
        }
      });
      yield put.resolve({
        type: 'getPageInfo',
        payload: { pageData }
      });
      success();
    },
    *upgrade({ payload }, { call, put, select }) {
      const { records, success } = payload;
      const { pageData } = yield select(state => state.sector);
      yield put.resolve({
        type: '@request',
        payload: {
          notice: true,
          url: '/sectors/upgrade',
          data: records.map(item => {return {sectorNumber:item.sectorNumber, minerId:item.minerId};})
        }
      });
      yield put.resolve({
        type: 'getPageInfo',
        payload: { pageData }
      });
      success();
    },
  },
  reducers: {}
});
