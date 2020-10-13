import React from 'react';
import { connect } from 'dva';
import { Layout } from 'antd';
import { Col, Row } from 'antd';
import Panel from 'components/Panel';
import G2 from 'components/Charts/G2';
import DataSet from '@antv/data-set';
import BaseComponent from 'components/BaseComponent';
import './index.less';
const { Content } = Layout;
const { Chart, Axis, Geom, Tooltip, Legend, Coord, Label } = G2;

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
    const { info } = dashboard;
    return (
      <Layout className="full-layout page dashboard-page">
        <Content>
          <Row gutter={20}>
            <Col md={12}>
              <Panel title="Deals" height={260}>
                {!loading && info.data && info.data.deals && (<Pie data={info.data.deals} />)}
              </Panel>
            </Col>
            <Col md={12}>
              <Panel title="Jobs" height={260}>
                {!loading && info.data && info.data.jobs && (<Pie data={info.data.jobs} />)}
              </Panel>
            </Col>
          </Row>
        </Content>
      </Layout>
    );
  }
}

const Pie = props => {
  const { data } = props;
  if (!data) return (<React.Fragment></React.Fragment>);
  console.warn(Object.keys(data).map(a => { return { item: a, count: data[a] }; }))
  const dv = new DataSet.DataView();
  dv.source(Object.keys(data).map(a => { return { item: a, count: data[a] }; })).transform({
    type: 'percent',
    field: 'count',
    dimension: 'item',
    as: 'percent'
  });
  const cols = {
    percent: {
      formatter: val => {
        val = val * 100 + '%';
        return val;
      }
    }
  };
  return (
    <Chart data={dv} scale={cols} padding={10}>
      <Coord type={'theta'} radius={0.75} innerRadius={0.6} />
      <Axis name="percent" />
      <Legend
        position="right"
        offsetY={-window.innerHeight / 2 + 120}
        offsetX={-100}
      />
      <Tooltip
        showTitle={false}
        itemTpl="<li><span style=&quot;background-color:{color};&quot; class=&quot;g2-tooltip-marker&quot;></span>{name}: {value}</li>"
      />
      <Geom
        type="intervalStack"
        position="percent"
        color="item"
        tooltip={[
          'item*percent',
          (item, percent) => {
            percent = percent * 100 + '%';
            return {
              name: item,
              value: percent
            };
          }
        ]}
        style={{ lineWidth: 1, stroke: '#fff' }}
      >
        <Label
          content="percent"
          formatter={(val, item) => {
            return item.point.item + ': ' + val;
          }}
        />
      </Geom>
    </Chart>
  );
};