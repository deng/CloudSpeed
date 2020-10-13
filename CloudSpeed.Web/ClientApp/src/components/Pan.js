import * as React from 'react';
import { connect } from 'react-redux';
import { useParams } from "react-router-dom";
import { message, Modal } from 'antd';
import { Form, Input } from 'antd';
import { panDownloadUrl } from './../app/util';
import Api from './../app/api';

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
      <p>File name</p>
      <p>{pan.fileName}</p>
      <p>File size</p>
      <p>{pan.fileSize ? `${pan.fileSize / 1000}KB` : ''}</p>
      <p>Release time</p>
      <p>{pan.created ? new Date(pan.created).toLocaleString() : ""}</p>
      <p>Data cid</p>
      <p>{pan.dataCid ? pan.dataCid : "Not yet"}</p>
      <p>Introduction</p>
      <div dangerouslySetInnerHTML={{ __html: pan.description }} />
      {pan.secret && <p> <a onClick={() => setValidating(true)}>Download file</a></p>}
      {!pan.secret  && <p> <a target="_blank" href={`${panDownloadUrl(id)}`}>Download file</a></p>}
      <p><a href='/'>I want to upload it, too</a></p>
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
