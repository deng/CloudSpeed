import React from 'react';
import { connect } from 'dva';
import { Layout } from 'antd';
import { UnorderedListOutlined, DiffOutlined } from '@ant-design/icons';
import { Card, Col, Row, Descriptions } from 'antd';
import { router } from 'dva';
import { ModalForm } from 'components/Modal';
const { Link } = router;
import BaseComponent from 'components/BaseComponent';
import createMinerColumns from './../../Lotus/Miner/components/columns';
import './index.less';
const { Content } = Layout;

@connect(({ dashboard, loading }) => ({
  dashboard,
  loading: loading.models.dashboard
}))
export default class Dashboard extends BaseComponent {
  state = {
    record: null,
    visible: false,
  };
  render() {
    const { dashboard, loading } = this.props;
    const { pageData } = dashboard;
    const minerColumns = createMinerColumns(this);
    const { record, visible } = this.state;

    const modalFormProps = {
      loading,
      record,
      visible,
      title:'查看',
      columns: minerColumns,
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
      <Layout className="full-layout page dashboard-page">
        <Content>
          <Row gutter={16}>
            {
              pageData.list && pageData.list.map((d, i) => (
                <Col sm={24} md={12} lg={8} style={{ marginTop: '10px' }}>
                  <Card title={`${(i + 1)}. ${d.id}`} bordered={false} actions={[
                    <DiffOutlined onClick={e => this.onUpdate(d)}/>,
                    <Link to={"/miner/storage/sector?minerId=" + d.id}>
                      <UnorderedListOutlined />
                    </Link>
                  ]}>
                    <Descriptions column={1} bordered size={'small'}>
                      <Descriptions.Item label={'Sector Size'}>{d['sectorSize']}</Descriptions.Item>
                      <Descriptions.Item label={'Committed'}>{d['committed']}</Descriptions.Item>
                      <Descriptions.Item label={'Proving'}>{d['proving']}</Descriptions.Item>
                      <Descriptions.Item label={'Byte Power'}>{d['bytePower']}</Descriptions.Item>
                      <Descriptions.Item label={'Actual Power'}>{d['actualPower']}</Descriptions.Item>
                      <Descriptions.Item label={'Proving Period Start'}>{d['provingPeriodStart']}</Descriptions.Item>
                      <Descriptions.Item label={'Next Period Start'}>{d['nextPeriodStart']}</Descriptions.Item>
                    </Descriptions>
                  </Card>
                </Col>
              ))
            }
          </Row>
        </Content>
        <ModalForm {...modalFormProps} />
      </Layout>
    );
  }
}
