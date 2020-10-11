import React from 'react';
import DataTable from 'components/DataTable';
import Button from 'components/Button';
import { Typography } from 'antd';
import { router } from 'dva';
const { Link } = router;

export default (self) => [
  {
    title: '_',
    name: 'id',
  },
  {
    title: 'ID',
    name: 'actorAddress',
    tableItem: {},
    searchItem: {
      group: 'abc'
    },
    formItem: {      
      type: 'custom',
      render: (record, form) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {record.actorAddress}
        </Typography.Text>
      )
    },
  },
  {
    title: '扇区大小',
    name: 'sectorSize',
    tableItem: {},
    formItem: {      
      type: 'custom',
      render: (record, form) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {record.sectorSize}
        </Typography.Text>
      )
    },
  },
  {
    title: 'Byte Power',
    name: 'bytePower',
    tableItem: {},
    formItem: {      
      type: 'custom',
      render: (record, form) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {record.bytePower}
        </Typography.Text>
      )
    },
  },
  {
    title: 'Actual Power',
    name: 'actualPower',
    tableItem: {},
    formItem: {      
      type: 'custom',
      render: (record, form) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {record.actualPower}
        </Typography.Text>
      )
    },
  },
  {
    title: 'Committed',
    name: 'committed',
    tableItem: {},
    formItem: {      
      type: 'custom',
      render: (record, form) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {record.committed}
        </Typography.Text>
      )
    },
  },
  {
    title: 'Proving',
    name: 'proving',
    tableItem: {},
    formItem: {      
      type: 'custom',
      render: (record, form) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {record.proving}
        </Typography.Text>
      )
    },
  },
  {
    title: 'Expected Block Win Rate',
    name: 'expectedBlockWinRate',
    formItem: {      
      type: 'custom',
      render: (record, form) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {record.expectedBlockWinRate}
        </Typography.Text>
      )
    },
  },
  {
    title: 'Miner Balance',
    name: 'minerBalance',
    formItem: {      
      type: 'custom',
      render: (record, form) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {record.minerBalance}
        </Typography.Text>
      )
    },
  },
  {
    title: 'Worker Balance',
    name: 'workerBalance',
    formItem: {      
      type: 'custom',
      render: (record, form) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {record.workerBalance}
        </Typography.Text>
      )
    },
  },
  {
    title: 'Market',
    name: 'marketBalance',
    formItem: {      
      type: 'custom',
      render: (record, form) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {record.marketBalance}
        </Typography.Text>
      )
    },
  },
  {
    title: 'Proving Period Start',
    name: 'provingPeriodStart',
    formItem: {      
      type: 'custom',
      render: (record, form) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {record.provingPeriodStart}
        </Typography.Text>
      )
    },
  },
  {
    title: 'Next Period Start',
    name: 'nextPeriodStart',
    formItem: {      
      type: 'custom',
      render: (record, form) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {record.nextPeriodStart}
        </Typography.Text>
      )
    },
  },
  {
    title: '扇区',
    name: 'sectors',
    tableItem1: {
      render: (text, record) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {text}
        </Typography.Text>
      )
    },
    formItem1: {      
      type: 'custom',
      render: (record, form) => (
        <Typography.Text className="ant-form-text pre-wrap-text" type="secondary">
          {record.sectors}
        </Typography.Text>
      )
    },
  },
  {
    title: '操作',
    tableItem: {
      width: 180,
      render: (text, record) => (
        <DataTable.Oper>
          <Button tooltip="查看" onClick={e => self.onUpdate(record)}>
            查看信息
          </Button>
          <Link to={"/miner/storage/sector?minerId=" + record.id}>
            管理扇区
          </Link>
        </DataTable.Oper>
      )
    }
  }
];
