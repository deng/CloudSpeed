import React from 'react';
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
    tableItem: {},
  },
  {
    title: 'DealId',
    name: 'dealId',
    tableItem: {},
  },
  {
    title: 'Miner',
    name: 'miner',
    tableItem: {},
  },
  {
    title: 'Status',
    name: 'status',
    tableItem: {},
  },
  {
    title: 'Error',
    name: 'error',
    tableItem: {},
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
