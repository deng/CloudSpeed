import * as React from 'react';
import { connect } from 'react-redux';
import LoginForm from './../../components/LoginForm';
import MyFilesList from './../../components/MyFilesList';

const MyFiles = (props) => {
  const renderLoginForm = () => {
    if (!props.user.token)
      return <LoginForm />;
    return <React.Fragment></React.Fragment>
  };
  const renderMyFilesList = () => {
    if (props.user.token)
      return <MyFilesList />;
    return <React.Fragment></React.Fragment>
  };
  return (
    <div>
      {renderLoginForm()}
      {renderMyFilesList()}
    </div>
  );
};

export default connect(state => state.user)(MyFiles);
