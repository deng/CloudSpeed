import React from 'react';
import DataTable from 'components/DataTable';
import { Button, Select, Input } from 'antd';
import { mergeSectorStateDict, sectorStateDict, updatingStateDict } from '@/custom/utils';

export default (self, states) => [
  {
    title: 'ID',
    name: 'sectorNumber',
    tableItem: {},
    formItem: {
      type: 'input',
      bordered: false,
      readonly: 'readonly',
    },
    searchItem: {
      group: 'abc'
    },
  },
  {
    title: 'MinerID',
    name: 'minerId',
    tableItem: {},
    searchItem: {
      group: 'abc',
    },
    formItem: {
      type: 'input',
      bordered: false,
      readonly: 'readonly',
    },
  },
  {
    title: '状态',
    name: 'state',
    dict: mergeSectorStateDict(states),
    searchItem: {
      group: 'abc',
      type: 'select',
      mode: "multiple",
      width: 250,
      maxTagCount: 0,
      maxTagPlaceholder: o => { return `已选择${o.length}项`; },
    },
  },
  {
    title: '状态',
    name: 'state',
    dict: sectorStateDict,
    tableItem: {},
  },
  {
    title: '状态',
    name: 'state',
    dict: updatingStateDict,
    formItem: {
      type: 'custom',
      rules: [{
        required: true, 
        message: '请选择一个状态' 
      }],
      render: (record, form, otherProps) => {
        return form.getFieldDecorator('state', otherProps)(
          <Select>
            {record.updatingStates.map((dic, i) => (
              <Select.Option key={dic} value={dic} title={dic}>
                {dic}
              </Select.Option>
            ))}
          </Select>
        );
      },
    },
  },
  {
    title: 'SSet',
    name: 'sSet',
    tableItem: {
      render: (text, record) => (
        <span>{text?"Y":""}</span>
      )
    },
  },
  {
    title: 'Active',
    name: 'active',
    tableItem: {
      render: (text, record) => (
        <span>{text?"Y":""}</span>
      )
    },
  },
  {
    title: 'TktH',
    name: 'tktH',
    tableItem: {},
  },
  {
    title: 'SeedH',
    name: 'seedH',
    tableItem: {},
  },
  {
    title: 'Deals',
    name: 'deals',
    tableItem: {},
  },
  {
    title: 'Upgrade',
    name: 'toUpgrade',
    tableItem: {
      render: (text, record) => (
        <span>{text?"Y":""}</span>
      )
    },
  },
  {
    title: '操作',
    tableItem: {
      width: 180,
      render: (text, record) => (
        <DataTable.Oper>
          <Button tooltip="更新状态" disabled={record.updatingStates.length === 0 ? 'disabled' : ''} onClick={e => self.onUpdate(record)}>更新状态</Button>
          <Button tooltip="删除" disabled={self.canDelete(record)?'disabled':''} onClick={e => self.onDelete(record)}>删除</Button>
        </DataTable.Oper>
      )
    }
  }
];
