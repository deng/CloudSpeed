import React from 'react';
import { Tooltip } from 'antd';
import DataTable from 'components/DataTable';
import Icon from 'components/Icon';
import Button from 'components/Button';

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
    title: 'Updated',
    name: 'updated',
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
    title: 'DealId',
    name: 'dealId',
    tableItem: {
      render: (text, record) => (
        <Tooltip title={text}>
          <span>{text && text.slice(-6)}</span>
        </Tooltip>
      )
    },
  },
  {
    title: 'Miner',
    name: 'miner',
    tableItem: {},
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
    title: 'Status',
    name: 'status',
    tableItem: {},
    dict: [{ code: 'None', codeName: 'None' },
    { code: 'Processing', codeName: 'Processing' },
    { code: 'Success', codeName: 'Success' },
    { code: 'Failed', codeName: 'Failed' }],
    searchItem: {
      type: 'select',
      group: 'abc'
    }
  },
  {
    title: 'Error',
    name: 'error',
    tableItem: {
      render: (text, record) => (
        <Tooltip title={text}>
          <span>{text.split(' ')[0]}</span>
        </Tooltip>
      )
    },
  },
  {
    title: 'Action',
    tableItem: {
      width: 180,
      render: (text, record) => (
        <DataTable.Oper>
          <Button tooltip="Reset" onClick={e => self.onReset(record)}>
            <Icon type="reset" />
          </Button>
        </DataTable.Oper>
      )
    }
  }
];
