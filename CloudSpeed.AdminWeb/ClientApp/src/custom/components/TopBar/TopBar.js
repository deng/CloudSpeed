import React, { Component } from 'react';
import { Breadcrumb, Row, Col } from 'antd';
import { router } from 'dva';
import Icon from 'components/Icon';
import cx from 'classnames';
import CSSAnimate from 'components/CSSAnimate';
import Mask from 'components/Mask';
import isEqual from 'react-fast-compare';
import 'components/TopBar/style/index.less';
const { Link } = router;

class TopBar extends Component {
  constructor(props) {
    super(props);
    this.state = {
      currentRoute: TopBar.getRouteLevel(props.location.pathname) || []
    };
  }

  static getDerivedStateFromProps(nextProps, prevState) {
    if (!isEqual(nextProps.currentRoute, prevState.currentRoute)) {
      return {
        currentRoute: TopBar.getRouteLevel(nextProps.location.pathname)
      };
    }

    return null;
  }

  static getRouteLevel = pathName => {
    const orderPaths = [];
    pathName.split('/').reduce((prev, next) => {
      const path = [prev, next].join('/');
      orderPaths.push(path);
      return path;
    });

    return orderPaths
      .map(item => window.dva_router_pathMap[item])
      .filter(item => !!item);
  };

  render() {
    const {
      expand,
      toggleRightSide,
      collapsedRightSide,
      onCollapse
    } = this.props;
    const { currentRoute } = this.state;
    const classnames = cx('topbar', {
      'topbar-expand': expand
    });

    return (
      <div className={classnames}>
        <div className="topbar-dropmenu">
        </div>
        <header className="topbar-content">
          {currentRoute.length ? (
            <Breadcrumb>
              <Breadcrumb.Item className="first">
                <CSSAnimate className="inline-block" type="flipInX">
                  {currentRoute[currentRoute.length - 1].title}
                </CSSAnimate>
              </Breadcrumb.Item>
              <Breadcrumb.Item className="icon">
                <Icon type="home" />
              </Breadcrumb.Item>
              <Breadcrumb.Item>
                <Link to="/">主页</Link>
              </Breadcrumb.Item>
              {currentRoute.map((item, index) => (
                <Breadcrumb.Item key={index}>
                  {index === currentRoute.length - 1 ? (
                    <CSSAnimate className="inline-block" type="flipInX">
                      {item.title}
                    </CSSAnimate>
                  ) : (
                    <Link to={item.path}>{item.title}</Link>
                  )}
                </Breadcrumb.Item>
              ))}
            </Breadcrumb>
          ) : null}
        </header>
        <Mask
          visible={expand}
          onClose={onCollapse}
          getContainer={node => node.parentNode}
        />
      </div>
    );
  }
}

export default TopBar;
