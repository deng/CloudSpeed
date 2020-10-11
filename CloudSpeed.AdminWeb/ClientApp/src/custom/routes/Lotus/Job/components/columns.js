import React from 'react';
import DataTable from 'components/DataTable';
import { taskTypeDict } from '@/custom/utils';

export default (self) => [
  {
    title: '任务数量',
    name: 'count',
    formItem: {
      type:'number',
      initialValue:1,
      rules: [{ required: true, message: '请输入任务数量' }]
    },
  },
  {
    title: 'ID',
    name: 'id',
    tableItem: {},
  },
  {
    title: 'Sector Number',
    name: 'sectorNumber',
    tableItem: {},
  },
  {
    title: 'Worker',
    name: 'worker',
    tableItem: {},
  },
  {
    title: 'Hostname',
    name: 'hostname',
    tableItem: {},
  },
  {
    title: 'Url',
    name: 'url',
    tableItem: {},
    searchItem: {
      group: 'abc'
    },
  },
  {
    title: 'Task',
    name: 'task',
    dict: taskTypeDict,
    tableItem: {},
    searchItem: {
      group: 'abc',
      type: 'select',
      mode: "multiple",
      width: 150,
      maxTagCount: 0,
      maxTagPlaceholder: o => { return `已选择${o.length}项`; },
    },
  },
  {
    title: 'Time',
    name: 'time',
    tableItem: {},
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
