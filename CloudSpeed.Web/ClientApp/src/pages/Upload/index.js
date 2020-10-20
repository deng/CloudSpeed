import * as React from 'react';
import { connect } from 'react-redux';
import UploadForm from './../../components/UploadForm';

const Upload = () => (
  <div>
    <UploadForm />
  </div>
);

export default connect()(Upload);
