import React from 'react';
import { connect } from 'dva';
import { Layout } from 'antd';
import BaseComponent from 'components/BaseComponent';
import Toolbar from 'components/Toolbar';
import SearchBar from 'components/SearchBar';
import DataTable from 'components/DataTable';
import { ModalForm } from 'components/Modal';
import createColumns from './columns';
const { Content, Header, Footer } = Layout;
const Pagination = DataTable.Pagination;

@connect(({ miner, loading }) => ({
  miner,
  loading: loading.models.miner
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
    const { miner, loading, dispatch } = this.props;
    const { pageData } = miner;
    const columns = createColumns(this);
    const { rows, record, visible } = this.state;

    const searchBarProps = {
      columns,
      onSearch: values => {
        dispatch({
          type: 'miner/getPageInfo',
          payload: {
            pageData: pageData.filter(values).jumpPage(1, 10)
          }
        });
      }
    };

    const dataTableProps = {
      loading,
      columns,
      rowKey: 'id',
      dataItems: pageData,
      selectType: 'checkbox',
      showNum: false,
      isScroll: true,
      selectedRowKeys: rows.map(item => item.id),
      onChange: ({ pageNum, pageSize }) => {
        dispatch({
          type: 'miner/getPageInfo',
          payload: {
            pageData: pageData.jumpPage(pageNum, pageSize)
          }
        });
      },
      onSelect: (keys, rows) => this.setState({ rows })
    };

    const modalFormProps = {
      loading,
      record,
      visible,
      title:'查看',
      columns,
      formOpts:{
        formItemLayout: {
          labelCol: { span: 7 },
          wrapperCol: { span: 16 }
        },
      },
      modalOpts: {
          width: 800
      },
      onCancel: () => {
        this.setState({
          record: null,
          visible: false
        });
      },
    };

    return (
      <Layout className="full-layout miner-page">
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
        <ModalForm {...modalFormProps} />
      </Layout>
    );
  }
}
