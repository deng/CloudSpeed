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
    title: 'Cid',
    name: 'cid',
    tableItem: {},
  },
  {
    title: 'JobId',
    name: 'jobId',
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
          <React.Fragment></React.Fragment>
        </DataTable.Oper>
      )
    }
  }
];
