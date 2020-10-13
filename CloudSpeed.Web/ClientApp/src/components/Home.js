import * as React from 'react';
import { connect } from 'react-redux';
import UploadForm from './UploadForm';

const Home = () => (
  <div>
    <UploadForm />
  </div>
);

export default connect()(Home);
