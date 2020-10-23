import React from 'react';
import { Tooltip } from 'antd';
import DataTable from 'components/DataTable';

export default (self) => [
  {
    title: 'Created',
    name: 'created',
    tableItem: {
      render: (text, record) => (
        <span>{new Date(text).toLocaleString()}</span>
      )
    },
  },
  {
    title: 'Cid',
    name: 'cid',
    tableItem: {
      render: (text, record) => (
        <Tooltip title={text}>
          <span>{text && text.slice(-6)}</span>
        </Tooltip>
      )
    },
  },
  {
    title: 'File Nme',
    name: 'fileName',
    tableItem: {},
  },
  {
    title: 'Format',
    name: 'format',
    tableItem: {},
  },
  {
    title: 'File Size',
    name: 'fileSize',
    tableItem: {},
  },
  {
    title: 'User Name',
    name: 'userName',
    tableItem: {
      render: (text, record) => (
        <Tooltip title={text}>
          <span>{text && text.slice(-6)}</span>
        </Tooltip>
      )
    },
    searchItem: {
      group: 'abc'
    }
  },
  {
    title: 'Action',
    tableItem: {
      width: 180,
      render: (text, record) => (
        <DataTable.Oper>
        </DataTable.Oper>
      )
    }
  }
];
