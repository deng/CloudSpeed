import * as React from 'react';
import { connect } from 'react-redux';
import { useParams } from "react-router-dom";
import { Button, message, Modal } from 'antd';
import { Form, Input } from 'antd';
import { Descriptions } from 'antd';
import { panDownloadUrl } from './../../app/util';
import Api from './../../app/api';

const api = new Api();

const Pan = () => {
  const [form] = Form.useForm();
  let { id } = useParams();
  const [pan, setPan] = React.useState({})
  const [validating, setValidating] = React.useState(false)

  React.useEffect(() => {
    async function fetchData() {
      const pan = await api.getPan(id);
      if (pan.success) {
        setPan(pan.data);
      }
      console.warn(pan);
    };
    fetchData();
  }, [id]);

  const handleCloseModal = () => {
    setValidating(false);
  };

  const onValidate = async (values) => {
    const validate = await api.validatePan(id, values);
    if (validate.data) {
      message.success('Validation successful, Start downloading file');
      setValidating(false);
      let blobUrl = `${panDownloadUrl(id, values.password)}`;
      const filename = pan.fileName;
      const aElement = document.createElement('a');
      document.body.appendChild(aElement);
      aElement.style.display = 'none';
      aElement.href = blobUrl;
      aElement.download = filename;
      aElement.click();
      document.body.removeChild(aElement);
    } else {
      message.error('Validation failed');
    }
  };

  return (
    <div>
      <Descriptions bordered>
        <Descriptions.Item label="File Name" span={3}>{pan.fileName}</Descriptions.Item>
        <Descriptions.Item label="Mime Type" span={3}>{pan.mimeType}</Descriptions.Item>
        <Descriptions.Item label="File Size" span={3}>{pan.fileSize}</Descriptions.Item>
        <Descriptions.Item label="Date" span={3}>{pan.created ? new Date(pan.created).toLocaleString() : ''}</Descriptions.Item>
        <Descriptions.Item label="Data cid" span={3}>{pan.dataCid ? pan.dataCid : "Not yet"}</Descriptions.Item>
        <Descriptions.Item label="Introduction" span={3}> <div dangerouslySetInnerHTML={{ __html: pan.description }} /></Descriptions.Item>
        <Descriptions.Item label="" span={3}>
          {pan.secret && <p> <Button type="primary" onClick={() => setValidating(true)} htmlType="submit">Download file</Button></p>}
          {!pan.secret && <p> <a target="_blank" href={`${panDownloadUrl(id)}`}>Download file</a></p>}
          <p><a href='/'>I want to upload it, too</a></p>
        </Descriptions.Item>
      </Descriptions>

      <Modal
        visible={validating}
        title="Input extraction code"
        okText="Verify and download"
        cancelText="Cancel"
        onCancel={handleCloseModal}
        onOk={() => {
          form
            .validateFields()
            .then(values => {
              onValidate(values);
              console.log('Validate :', values);
            })
            .catch(info => {
              console.log('Validate Failed:', info);
            });
        }}
      >
        <Form
          form={form}
          layout="vertical"
        >
          <Form.Item
            name="password"
            label="Extractor"
            rules={[{ required: true, message: 'Please enter extraction code' }]}
          >
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}
export default connect()(Pan);
