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

@connect(({ job, loading }) => ({
  job,
  loading: loading.models.job
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
    const { job, loading, dispatch } = this.props;
    const { pageData } = job;
    const columns = createColumns(this);
    const { rows, record, visible } = this.state;

    const searchBarProps = {
      columns,
      onSearch: values => {
        dispatch({
          type: 'job/getPageInfo',
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
          type: 'job/getPageInfo',
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
      // 新增、修改都会进到这个方法中，
      // 可以使用主键或是否有record来区分状态
      onSubmit: values => {
        dispatch({
          type: 'job/save',
          payload: {
            values,
            success: () => {
              this.setState({
                record: null,
                visible: false
              });
            }
          }
        });
      },
    };

    return (
      <Layout className="full-layout job-page">
        <Header>
          <Toolbar appendLeft={
              <Button.Group>
                <Button type="primary" icon={<PlusOutlined />} onClick={this.onAdd}>
                  新增
                </Button>
              </Button.Group>
            }>
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
