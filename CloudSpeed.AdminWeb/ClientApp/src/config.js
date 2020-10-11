import React from 'react';
import PageLoading from 'components/Loading/PageLoading';
import { normal } from 'components/Notification';
import store from 'cmn-utils/lib/store';

const notice = normal;

const codeMessage = {
  400: 'There was an error in the request. The server did not create or modify the data.',
  401: 'The user does not have permission (wrong token, user name, password).',
  403: 'The user is authorized, but access is prohibited.',
  404: 'The request was made for a nonexistent record and the server did not operate.',
  405: 'Disable method specified in request (method disabled)',
  406: 'The format of the request is not available.',
  410: 'The requested resource is permanently deleted and will no longer be available.',
  422: 'A validation error occurred while creating an object.',
  500: 'Server error, please check the server',
  502: 'Bad Gateway',
  503: 'The service is not available, the server is temporarily overloaded or maintained',
  504: 'gateway timeout',
  0: 'Request error',
  invalidURL: 'Invalid request URL',
  requestCanceled: 'The request was cancelled ahead of time',
  timeout: 'request timeout'
};

export default {
  htmlTitle: 'Speed Cloud - {title}',
  
  notice,

  request: {
    prefix: '/api',
    credentials: 'omit',
    withHeaders: () => ({
      Authorization: `Bearer ${store.getStore("user") ? store.getStore("user").token : ''}`,
    }),

    afterResponse: response => {
      const { status, error } = response;
      if (status) {
        return response;
      } else {
        throw new Error(error);
      }
    },
    errorHandle: err => {
      if (err.name === 'RequestError') {
        if (err.code === 401) return;
        if (err.code && codeMessage[err.code]) {
          notice.error(codeMessage[err.code]);
        } else {
          notice.error(err.text || err.message || err.error);
        }
      }
    }
  },

  exception: {
    global: (err, dispatch) => {
      const errName = err.name;
      if (err.code === 401) {
        store.removeStore('user');
        return;
      }
      if (err.code && codeMessage[err.code]) {
        notice.error(codeMessage[err.code]);
      } else if (err.error) {
        notice.error(err.error);
      }
      if (errName === 'RequestError') {
        console.error(err);
      } else {
        console.error(err);
      }
    },
  },

  pageHelper: {
    requestFormat: pageInfo => {
      const { pageNum, pageSize, filters, sorts } = pageInfo;
      return {
        skip: (pageNum - 1) * pageSize,
        limit: pageSize,
        sortMap: sorts,
        paramMap: filters
      };
    },

    responseFormat: resp => {
      const {
        skip,
        limit,
        total,
        list,
        totalPage
      } = resp.data;

      return {
        pageNum: limit == 0 ? 1 : ((skip / limit) + 1),
        pageSize: limit,
        total: total,
        totalPages: totalPage,
        list: list,
        extraData: resp.data.extraData
      };
    }
  },

  router: {
    loading: <PageLoading loading />
  },

  mock: {
    toSuccess: response => ({
      status: true,
      data: response
    }),

    toError: message => ({
      status: false,
      error: message
    })
  }
};
