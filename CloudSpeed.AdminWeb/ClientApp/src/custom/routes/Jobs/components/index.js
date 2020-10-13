import React from 'react';
import { connect } from 'dva';
import { Layout, Button } from 'antd';
import { Modal } from 'antd';
import BaseComponent from 'components/BaseComponent';
import Toolbar from 'components/Toolbar';
import SearchBar from 'components/SearchBar';
import DataTable from 'components/DataTable';
import createColumns from './columns';
import $$ from 'cmn-utils';
import './index.less';
const { Content, Header, Footer } = Layout;
const Pagination = DataTable.Pagination;

@connect(({ jobs, loading }) => ({
  jobs,
  loading: loading.models.jobs
}))
export default class extends BaseComponent {
  state = {
    record: null,
    visible: false,
    rows: [],
    content: null
  };

  render() {
    const { jobs, loading, dispatch } = this.props;
    const { pageData, categories, publish } = jobs;
    const columns = createColumns(this, categories, publish, content => {
      this.setState({
        content: content.toHTML()
      });
    });
    const { rows, record, visible } = this.state;

    const searchBarProps = {
      columns,
      onSearch: values => {
        dispatch({
          type: 'jobs/getPageInfo',
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
      showNum: true,
      isScroll: true,
      selectedRowKeys: rows.map(item => item.id),
      onChange: ({ pageNum, pageSize }) => {
        dispatch({
          type: 'jobs/getPageInfo',
          payload: {
            pageData: pageData.jumpPage(pageNum, pageSize)
          }
        });
      },
      onSelect: (keys, rows) => this.setState({ rows })
    };

    return (
      <Layout className="full-layout jobs-page">
        <Header>
          <Toolbar
            appendLeft={
              <SearchBar group="abc" {...searchBarProps} />
            }
          >
            <Button.Group>
            </Button.Group>
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
