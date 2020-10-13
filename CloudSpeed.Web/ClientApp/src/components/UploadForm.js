import * as React from 'react';
import { connect } from 'react-redux';
import { Upload, message } from 'antd';
import { InboxOutlined } from '@ant-design/icons';
import { UploadOutlined } from '@ant-design/icons';
import { LoadingOutlined, PlusOutlined } from '@ant-design/icons';
import { Modal } from 'antd';
import { Form, Input, Button } from 'antd';
import BraftEditor from 'braft-editor'
import { Col, Row } from 'antd';
import QRCode from 'qrcode.react';
import { CopyToClipboard } from 'react-copy-to-clipboard';
import { itemUrl } from './../app/util';
import Api from './../app/api';

import 'braft-editor/dist/index.css'

const { Dragger } = Upload;

const api = new Api();

const bcontrols = ['bold', 'italic', 'underline', 'text-color', 'separator', 'link', 'separator', 'media']

const draggerProps = {
    name: 'dataKey',
    multiple: false,
    action: '/api/upload/BigFileUpload',
    showUploadList: true,
    progress: {
        strokeColor: {
            '0%': '#108ee9',
            '100%': '#87d068',
        },
        strokeWidth: 3,
        format: percent => `${parseFloat(percent.toFixed(2))}%`,
    },
};

function beforeUploadSmallFile(file) {
    const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png';
    if (!isJpgOrPng) {
        message.error('You can only upload JPG/PNG file!');
        return false;
    }
    const isLt2M = file.size / 1024 / 1024 < 2;
    if (!isLt2M) {
        message.error('Image must smaller than 2MB!');
        return false;
    }
    return isJpgOrPng && isLt2M;
}

function getBase64(img, callback) {
    const reader = new FileReader();
    reader.addEventListener('load', () => callback(reader.result));
    reader.readAsDataURL(img);
}

const handleSmallFileChange = (info, setStateFunc) => {
    if (info.file.status === 'uploading') {
        //setStateFunc({ imageUrl: "", loading: true, fileList: [] })
    }
    else if (info.file.status === 'done') {
        // Get this url from response in real world.
        let fileList = [...info.fileList];
        fileList = fileList.slice(-1);
        getBase64(info.file.originFileObj, (imageUrl) =>
            setStateFunc({ imageUrl, loading: false, fileList })
        );
    }
    else if (info.file.status === 'error') {
        message.error('Image upload fail!');
        setStateFunc({ imageUrl: "", loading: false, fileList: [] })
    }
};

const UploadForm = () => {
    const [dateKeyList, setDateKeyList] = React.useState([]);
    const [editorState, setEditorState] = React.useState(undefined);
    const [formLoading, setFormLoading] = React.useState(false);
    const [formResult, setFormResult] = React.useState({});
    const [alipayFile, setAlipayFile] = React.useState({ imageUrl: "", loading: false, fileList: [] });
    const [wxpayFile, setWxpayFile] = React.useState({ imageUrl: "", loading: false, fileList: [] });
    const [form] = Form.useForm();

    const onFinish = async values => {
        setFormLoading(true);
        try {
            const data = await api.uploadPan(values, editorState);
            if (data.success) {
                setFormResult({ ...data });
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

    const beforeUploadBigFile = (file, filelist) => {
        console.warn(file, filelist, dateKeyList);
    }

    const handleUploadChange = info => {
        const { status, response } = info.file;
        //console.warn(status);
        setDateKeyList([...info.fileList]);
        if (status !== 'uploading') {
            //console.log(info.file, info.fileList);
        }
        if (status === 'done') {
            if (response.success) {
                message.success(`${info.file.name} file uploaded successfully.`);
                let fileList = [...info.fileList];
                fileList = fileList.slice(-1);
                setDateKeyList(fileList);
            } else {
                message.error(`${info.file.name} file upload failed: ${response.error}`);
                if (info.fileList.length > 1) {
                    setDateKeyList([info.fileList[0]]);
                } else {
                    setDateKeyList([]);
                }
            }
        } else if (status === 'error') {
            message.error(`${info.file.name} file upload failed.`);
            if (info.fileList.length > 1) {
                setDateKeyList([info.fileList[0]]);
            } else {
                setDateKeyList([]);
            }
        } else if (status === 'removed') {
            setDateKeyList([]);
        }
    };

    const renderUploadButton = (loading, cover, title) => (
        <div>
            {loading ? <LoadingOutlined /> : <PlusOutlined />}
            <div style={{ marginTop: 8 }}><img style={{ width: '45px' }} src={cover} alt={title} /></div>
        </div>
    );

    const handleEditorChange = (editorState) => {
        setEditorState(editorState)
    };

    const handleCloseModal = () => {
        setFormResult({});
    };

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
                <Form.Item label="Select file (up to 5g)" name="dataKey" rules={[{ required: true, message: 'Please select a file to upload' }]} required>
                    <Dragger {...draggerProps} onChange={handleUploadChange} beforeUpload={beforeUploadBigFile} fileList={dateKeyList} >
                        <p className="ant-upload-drag-icon">
                            <InboxOutlined />
                        </p>
                        <p className="ant-upload-text">Click or drag file to this area to upload</p>
                    </Dragger>
                </Form.Item>
                <Form.Item label="Document introduction" name="descripton">
                    <BraftEditor
                        className="my-editor"
                        controls={bcontrols}
                        placeholder="Please enter the text"
                        value={editorState}
                        onChange={handleEditorChange}
                    />
                </Form.Item>
                <Form.Item label="Extraction code (limited to 10 digits or letters)" name="password">
                    <Input placeholder="Enter the extraction code, leave blank as unencrypted" />
                </Form.Item>
                <Form.Item>
                    <Button loading={formLoading} type="primary" icon={<UploadOutlined />} htmlType="submit">Publish</Button>
                </Form.Item>
            </Form>
        </React.Fragment>
    );
};

export default connect()(UploadForm);
