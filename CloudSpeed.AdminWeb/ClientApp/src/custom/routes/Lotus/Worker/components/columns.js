import React from 'react';
import DataTable from 'components/DataTable';

export default (self) => [
  {
    title: 'ID',
    name: 'id',
    tableItem: {},
  },
  {
    title: '主机',
    name: 'hostname',
    tableItem: {},
    searchItem: {
      group: 'abc'
    },
  },
  {
    title: '地址',
    name: 'url',
    tableItem: {},
    searchItem: {
      group: 'abc'
    },
  },
  {
    title: 'CPU',
    name: 'cpu',
    tableItem: {},
  },
  {
    title: 'Ram',
    name: 'ram',
    tableItem: {},
  },
  {
    title: 'Vmem',
    name: 'vmem',
    tableItem: {},
  },
  {
    title: 'GPU',
    name: 'gpus',
    tableItem: {},
  },
  {
    title: '是否使用GPU',
    name: 'gpuUsed',
    tableItem: {
      render: (text, record) => (
        <span>{text?"GPU":""}</span>
      )
    },
  },
  {
    title: '操作',
    tableItem: {
      width: 180,
      render: (text, record) => (
        <DataTable.Oper></DataTable.Oper>
      )
    }
  }
];
