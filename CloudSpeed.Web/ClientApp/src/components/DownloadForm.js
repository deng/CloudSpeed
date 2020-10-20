import * as React from 'react';
import { connect } from 'react-redux';
import { message } from 'antd';
import { DownloadOutlined } from '@ant-design/icons';
import { Modal } from 'antd';
import { Form, Input, Button } from 'antd';
import QRCode from 'qrcode.react';
import { CopyToClipboard } from 'react-copy-to-clipboard';
import { Descriptions, Badge } from 'antd';
import { itemUrl } from './../app/util';
import Api from './../app/api';

const api = new Api();

const DownloadForm = () => {
    const [formLoading, setFormLoading] = React.useState(false);
    const [formResult, setFormResult] = React.useState({});
    const [form] = Form.useForm();

    const onFinish = async values => {
        setFormLoading(true);
        try {
            const data = await api.getDownloadInfoByCid(values);
            if (data.success) {
                setFormResult({ ...data.data });
            } else {
                message.error(data.error);
            }
        } catch (error) {
            message.error(error + '');
        }
        setFormLoading(false);
    };

    const onFinishFailed = errorInfo => {
        console.log('Failed:', errorInfo);
    };

    const handleCloseModal = () => {
        setFormResult({});
    };
    console.warn(formResult);
    return (
        <React.Fragment>
            <Modal
                title="Publish successfully"
                visible={formResult.success}
                onCancel={handleCloseModal}
                footer={[
                    <Button key="submit" onClick={handleCloseModal}>
                        Close
                    </Button>,
                ]}
            >
                <p>Please use the link below to download or access the file</p>
                <p><Input value={`${itemUrl(formResult.data)}`} suffix={<CopyToClipboard text={`${itemUrl(formResult.data)}`} onCopy={() => message.success(`copy successfully.`)}>
                    <Button>Click Copy</Button>
                </CopyToClipboard>} /></p>
                <p>Right click (or long press) the picture below to save the picture and forward it</p>
                <p style={{ textAlign: "center" }}><QRCode size={250} value={`${itemUrl(formResult.data)}`} /></p>
            </Modal>
            <Form
                form={form}
                layout="vertical"
                onFinish={onFinish}
                onFinishFailed={onFinishFailed}
            >
                <Form.Item label="Get Data from Filecoin" name="cid">
                    <Input placeholder="Please input the data cid" />
                </Form.Item>
                <Form.Item>
                    <Button loading={formLoading} type="primary" icon={<DownloadOutlined />} htmlType="submit">Get Data from Filecoin</Button>
                </Form.Item>
            </Form>
            { formResult.localStroeInfo && (
                <Descriptions title="Local Store Info" bordered>
                    <Descriptions.Item label="File Name" span={3}>{formResult.localStroeInfo.fileName}</Descriptions.Item>
                    <Descriptions.Item label="Mime Type" span={3}>{formResult.localStroeInfo.mimeType}</Descriptions.Item>
                    <Descriptions.Item label="File Size" span={3}>{formResult.localStroeInfo.fileSize}</Descriptions.Item>
                    <Descriptions.Item label="Miner" span={3}>{formResult.localStroeInfo.miner}</Descriptions.Item>
                    <Descriptions.Item label="Publisher" span={3}>{formResult.localStroeInfo.publisher}</Descriptions.Item>
                    <Descriptions.Item label="Date" span={3}>{formResult.localStroeInfo.date ? new Date(formResult.localStroeInfo.date).toLocaleString() : ''}</Descriptions.Item>
                    <Descriptions.Item label="Download" span={3}><a href={itemUrl(formResult.localStroeInfo.logId)}>{formResult.localStroeInfo.logId ? 'Download file': ''}</a> </Descriptions.Item>
                </Descriptions>
            )}
            { formResult.retrievalOrderInfos && formResult.retrievalOrderInfos.map(oi => (
                <Descriptions title="Filecoin Order Info" bordered>
                    <Descriptions.Item label="Miner" span={3}>{oi.miner}</Descriptions.Item>
                    <Descriptions.Item label="Miner PeerId" span={3}>{oi.minerPeerId}</Descriptions.Item>
                    <Descriptions.Item label="Offer Min Price" span={3}>{oi.offerMinPrice}</Descriptions.Item>
                    <Descriptions.Item label="Offer Size" span={3}>{oi.offerSize}</Descriptions.Item>
                    <Descriptions.Item label="Err" span={3}>{oi.err}</Descriptions.Item>
                </Descriptions>
            ))}
        </React.Fragment>
    );
};

export default connect()(DownloadForm);
