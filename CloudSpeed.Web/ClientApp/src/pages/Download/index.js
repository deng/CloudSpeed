import * as React from 'react';
import { connect } from 'react-redux';
import DownloadForm from './../../components/DownloadForm';

const Download = () => (
  <div>
    <DownloadForm />
  </div>
);

export default connect()(Download);
