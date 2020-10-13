import React from 'react';
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
    title: 'Updated',
    name: 'updated',
    tableItem: {
      render: (text, record) => (
        <span>{new Date(text).toLocaleString()}</span>
      )
    },
  },
  {
    title: 'Path',
    name: 'path',
    tableItem: {},
  },
  {
    title: 'Status',
    name: 'status',
    tableItem: {},
  },
  {
    title: 'Total',
    name: 'total',
    tableItem: {},
  },
  {
    title: 'Success',
    name: 'success',
    tableItem: {},
  },
  {
    title: 'Failed',
    name: 'failed',
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
          <React.Fragment></React.Fragment>
        </DataTable.Oper>
      )
    }
  }
];
