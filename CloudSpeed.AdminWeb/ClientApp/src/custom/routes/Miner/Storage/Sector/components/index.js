import React from 'react';
import { connect } from 'dva';
import { DeleteOutlined, RedoOutlined, CloseOutlined } from '@ant-design/icons';
import { Layout, Button } from 'antd';
import { Modal } from 'antd';
import $$ from 'cmn-utils';
import BaseComponent from 'components/BaseComponent';
import Toolbar from 'components/Toolbar';
import SearchBar from 'components/SearchBar';
import DataTable from 'components/DataTable';
import { ModalForm } from 'components/Modal';
import createColumns from './columns';
const { Content, Header, Footer } = Layout;
const Pagination = DataTable.Pagination;

@connect(({ sector, loading }) => ({
  sector,
  loading: loading.models.sector
}))
export default class extends BaseComponent {
  state = {
    record: null,
    visible: false,
    rows: []
  };

  handleDelete = records => {
    const { rows } = this.state;

    this.props.dispatch({
      type: 'sector/remove',
      payload: {
        records,
        success: () => {
          // 如果操作成功，在已选择的行中，排除删除的行
          this.setState({
            rows: rows.filter(
              item => !records.some(jtem => jtem.sectorNumber === item.sectorNumber)
            )
          });
        }
      }
    });
  };

  onFailedUnrecoverable = record => {
    if (!record) return;
    if ($$.isArray(record) && !record.length) return;

    const content = `您是否要更改这${
      $$.isArray(record) ? record.length : ''
    }项的扇区状态为“FailedUnrecoverable”？`;

    Modal.confirm({
      title: '注意',
      content,
      onOk: () => {
        this.handleFailedUnrecoverable($$.isArray(record) ? record : [record]);
      },
      onCancel() {}
    });
  };

  handleFailedUnrecoverable = records => {
    this.props.dispatch({
      type: 'sector/updateFailedUnrecoverable',
      payload: {
        records,
        success: () => {}
      }
    });
  };

  onUpgrade = record => {
    if (!record) return;
    if ($$.isArray(record) && !record.length) return;

    const content = `您是否要Upgrade这${
      $$.isArray(record) ? record.length : ''
    }项吗？`;

    Modal.confirm({
      title: '注意',
      content,
      onOk: () => {
        this.handleUpgrade($$.isArray(record) ? record : [record]);
      },
      onCancel() {}
    });
  };

  handleUpgrade = records => {
    this.props.dispatch({
      type: 'sector/upgrade',
      payload: {
        records,
        success: () => {}
      }
    });
  };

  canDelete = record => {
    return !(record.canDelete || false);
  };

  canUpgrade = record => {
    return !(record.canUpgrade || false);
  };

  render() {
    const { sector, loading, dispatch } = this.props;
    const { pageData } = sector;
    const columns = createColumns(this, pageData.extraData);
    const { rows, record, visible } = this.state;
    const searchBarProps = {
      columns,
      onSearch: values => {
        dispatch({
          type: 'sector/getPageInfo',
          payload: {
            pageData: pageData.filter(values).jumpPage(1, 10)
          }
        });
      }
    };

    const dataTableProps = {
      loading,
      columns,
      rowKey: 'sectorNumber',
      dataItems: pageData,
      selectType: 'checkbox',
      showNum: false,
      isScroll: true,
      selectedRowKeys: rows.map(item => item.sectorNumber),
      onChange: ({ pageNum, pageSize }) => {
        dispatch({
          type: 'sector/getPageInfo',
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
      modalOpts: {
        width: 700
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
          type: 'sector/updateState',
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
      }
    };

    return (
      <Layout className="full-layout sector-page">
        <Header>
          <Toolbar
            appendLeft={
              <Button.Group>
                <Button disabled={!rows.filter(a=>a.canDelete).length} icon={<CloseOutlined />} onClick={e => this.onFailedUnrecoverable(rows)}>
                  FailedUnrecoverable
                </Button>
                <Button disabled={!rows.filter(a=>a.canUpgrade).length} icon={<RedoOutlined />} onClick={e => this.onUpgrade(rows)}>
                  Upgrade
                </Button>
                <Button
                  disabled={!rows.filter(a => a.canDelete).length}
                  onClick={e => this.onDelete(rows)}
                  icon={<DeleteOutlined />}
                >
                  删除
                </Button>
              </Button.Group>
            }
            pullDown={<SearchBar type="grid" {...searchBarProps} />}
          >
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
