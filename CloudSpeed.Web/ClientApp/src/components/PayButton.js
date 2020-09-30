import * as React from 'react';
import { Upload, message } from 'antd';
import { LoadingOutlined, PlusOutlined } from '@ant-design/icons';

function getBase64(img, callback) {
  const reader = new FileReader();
  reader.addEventListener('load', () => callback(reader.result));
  reader.readAsDataURL(img);
}

function beforeUpload(file) {
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

export default class PayButton extends React.PureComponent {
  state = {
    loading: false,
    imageUrl: '',
  };

  handleChange = (info) => {
    console.warn(info);
    if (info.file.status === 'uploading') {
      this.setState({ loading: true });
      return;
    }
    else if (info.file.status === 'done') {
      // Get this url from response in real world.
      getBase64(info.file.originFileObj, (imageUrl) =>
        this.setState({
          imageUrl,
          loading: false,
        }),
      );
    }
    else if (info.file.status === 'error') {
      message.error('Image upload fail!');
      this.setState({ loading: false });
    }
  };

  render() {
    const { loading, imageUrl } = this.state;
    const { cover, title, name } = this.props;
    const uploadButton = (
      <div>
        {loading ? <LoadingOutlined /> : <PlusOutlined />}
        <div style={{ marginTop: 8 }}><img style={{ width: '45px' }} src={cover} alt={title} /></div>
      </div>
    );
    return (
      <Upload
        name={name}
        listType="picture-card"
        className="avatar-uploader"
        action='/api/upload/SmallFileUpload'
        showUploadList={false}
        beforeUpload={beforeUpload}
        onChange={this.handleChange}
      >
        {imageUrl ? <img src={imageUrl} alt={title} style={{ width: '100%' }} /> : uploadButton}
      </Upload>
    );
  }
}