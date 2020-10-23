import * as React from 'react';
import { connect } from 'react-redux';
import { Table, Space } from 'antd';
import { itemUrl } from './../app/util';
import * as MyFilesStore from '../store/MyFiles';

const MyFilesList = (props) => {
    React.useEffect(() => {
        props.requestMyFiles(props.page, props.pageSize);
    }, []);
    const fetch = async (page, pageSize) => {
        props.requestMyFiles(page, pageSize);
    };
    const columns = [
        {
            title: 'File Name',
            dataIndex: 'fileName',
            key: 'fileName',
            render: text => <a>{text}</a>,
        },
        {
            title: 'Date',
            dataIndex: 'created',
            key: 'created',
            render: (text, record) => (
                <Space size="middle">
                    {new Date(record.created).toLocaleString()}
                </Space>
            ),
        },
        {
            title: 'CID',
            dataIndex: 'dataCid',
            key: 'dataCid',
            ellipsis: true,
        },
        {
            title: 'File Size',
            dataIndex: 'fileSize',
            key: 'fileSize',
        },
        {
            title: 'Format',
            dataIndex: 'format',
            key: 'format',
        },
        {
            title: 'Action',
            key: 'action',
            render: (text, record) => (
                <Space size="middle">
                    <a target="_blank" href={itemUrl(record.id)}>Download</a>
                </Space>
            ),
        },
    ];
    return (
        <Table columns={columns} dataSource={props.files.map(i => { return { ...i, key: i.id }; })} pagination={{
            total: props.total,
            pageSize: props.pageSize,
            showTotal: (total, range) => `${range[0]}-${range[1]} of ${total} items`,
            showSizeChanger: false,
            onChange: fetch
        }} />
    );
};

export default connect(state => state.myFiles, MyFilesStore.actionCreators)(MyFilesList);