import * as React from 'react';
import { connect } from 'react-redux';
import UploadForm from './../../components/UploadForm';

const MyFiles = () => (
  <div>
    <UploadForm />
  </div>
);

export default connect()(MyFiles);
