import * as React from 'react';
import { connect } from 'react-redux';
import UploadForm from './UploadForm';

const Home = () => (
  <div>
    <h1>Privacy, speed</h1>
    <p>Private, extreme speed file upload and download support Alipay, WeChat reward</p>
    <UploadForm />
  </div>
);

export default connect()(Home);
