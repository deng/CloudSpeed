import React from 'react';
import { connect } from 'dva';
import { PlusOutlined } from '@ant-design/icons';
import { Layout, Button } from 'antd';
import BaseComponent from 'components/BaseComponent';
import Toolbar from 'components/Toolbar';
import SearchBar from 'components/SearchBar';
import DataTable from 'components/DataTable';
import { ModalForm } from 'components/Modal';
import createColumns from './columns';
const { Content, Header, Footer } = Layout;
const Pagination = DataTable.Pagination;

@connect(({ worker, loading }) => ({
  worker,
  loading: loading.models.worker
}))
export default class extends BaseComponent {
  state = {
    record: null,
    visible: false,
    rows: []
  };

  onAddTask = ()=> {
    this.setState({
      record: null,
    });
  };

  render() {
    const { worker, loading, dispatch } = this.props;
    const { pageData } = worker;
    const columns = createColumns(this);
    const { rows, record, visible } = this.state;

    const searchBarProps = {
      columns,
      onSearch: values => {
        dispatch({
          type: 'worker/getPageInfo',
          payload: {
            pageData: pageData.filter(values).jumpPage(1, 10)
          }
        });
      }
    };

    const dataTableProps = {
      loading,
      columns,
      rowKey: 'hostname',
      dataItems: pageData,
      selectType: 'checkbox',
      showNum: false,
      isScroll: true,
      selectedRowKeys: rows.map(item => item.hostname),
      onChange: ({ pageNum, pageSize }) => {
        dispatch({
          type: 'worker/getPageInfo',
          payload: {
            pageData: pageData.jumpPage(pageNum, pageSize)
          }
        });
      },
      onSelect: (keys, rows) => this.setState({ rows })
    };

    return (
      <Layout className="full-layout worker-page">
        <Header>
          <Toolbar>
            <SearchBar group="abc" {...searchBarProps} />
          </Toolbar>
        </Header>
        <Content>
          <DataTable {...dataTableProps} />
        </Content>
        <Footer>
          <Pagination {...dataTableProps} />
        </Footer>
      </Layout>
    );
  }
}
