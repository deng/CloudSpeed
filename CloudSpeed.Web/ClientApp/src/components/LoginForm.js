import * as React from 'react';
import { message } from 'antd';
import { Form, Input, Button } from 'antd';
import { connect } from 'react-redux';
import MetaMaskOnboarding from '@metamask/onboarding';
import * as UserStore from '../store/User';

import Api from './../app/api';

const layout = {
    labelCol: { span: 8 },
    wrapperCol: { span: 8 },
};
const tailLayout = {
    wrapperCol: { offset: 8, span: 8 },
};

const api = new Api();

const LoginForm = (props) => {
    const [buttonInstallText, setButtonInstallText] = React.useState('Click here to install MetaMask');
    const [buttonInstallTextDisabled, setButtonInstallTextDisabled] = React.useState(false);
    const [connectedMetaMask, setConnectedMetaMask] = React.useState(false);
    const [newMember, setNewMember] = React.useState(false);
    const [account, setAccount] = React.useState(undefined);

    const onLoginFinish = async values => {
        props.login({ ...values, address: account });
    };

    const onLoginFinishFailed = errorInfo => {
        console.log('Failed:', errorInfo);
    };

    const onNewMemberFinish = async values => {
        props.createMember({ ...values, address: account });
    };

    const onNewMemberFinishFailed = errorInfo => {
        console.log('Failed:', errorInfo);
    };

    const onboarding = new MetaMaskOnboarding({ forwarderOrigin: window.location.href });

    const onClickConnect = async () => {
        try {
            const { ethereum } = window;
            const accounts = await ethereum.request({ method: 'eth_requestAccounts' });
            if (accounts && accounts.length > 0) {
                setAccount(accounts[0]);
                const checkAccount = await api.checkMember(accounts[0]);
                setConnectedMetaMask(true);
                setNewMember(!checkAccount.data);
            }
        } catch (error) {
            console.error(error);
        }
    };

    const onClickInstall = () => {
        setButtonInstallText('Onboarding in progress');
        setButtonInstallTextDisabled(true);
        onboarding.startOnboarding();
    };

    const isMetaMaskInstalled = () => {
        const { ethereum } = window;
        console.warn(ethereum);
        return Boolean(ethereum && ethereum.isMetaMask);
    };

    const renderButton = () => {
        if (isMetaMaskInstalled())
            return (<Button type="primary" htmlType="button" onClick={onClickConnect}>Connect with MetaMask</Button>);
        return (<Button type="primary" htmlType="button" disabled={buttonInstallTextDisabled} onClick={onClickInstall}>{buttonInstallText}</Button>);
    };

    return (
        <React.Fragment>
            {
                !connectedMetaMask && (
                    <Form
                        {...layout}
                    >
                        <Form.Item {...tailLayout}>
                            {renderButton()}
                        </Form.Item>
                    </Form>
                )
            }
            {
                connectedMetaMask && !newMember && (
                    <Form
                        {...layout}
                        initialValues={{ remember: true }}
                        onFinish={onLoginFinish}
                        onFinishFailed={onLoginFinishFailed}
                    >
                        <Form.Item
                            label="Password"
                            name="password"
                            rules={[{ required: true, message: 'Please input your password!' }]}
                            hasFeedback
                        >
                            <Input.Password />
                        </Form.Item>

                        <Form.Item {...tailLayout}>
                            <Button loading={props.submitting} type="primary" htmlType="submit">Login</Button>
                        </Form.Item>
                    </Form>
                )
            }
            {
                connectedMetaMask && newMember && (
                    <Form
                        {...layout}
                        initialValues={{ remember: true }}
                        onFinish={onNewMemberFinish}
                        onFinishFailed={onNewMemberFinishFailed}
                    >
                        <Form.Item
                            name="password"
                            label="Password"
                            rules={[{ required: true, message: 'Please input your password!', }]}
                            hasFeedback
                        >
                            <Input.Password />
                        </Form.Item>
                        <Form.Item
                            name="confirm"
                            label="Confirm Password"
                            dependencies={['password']}
                            hasFeedback
                            rules={[
                                {
                                    required: true,
                                    message: 'Please confirm your password!',
                                },
                                ({ getFieldValue }) => ({
                                    validator(rule, value) {
                                        if (!value || getFieldValue('password') === value) {
                                            return Promise.resolve();
                                        }
                                        return Promise.reject('The two passwords that you entered do not match!');
                                    },
                                }),
                            ]}
                        >
                            <Input.Password />
                        </Form.Item>
                        <Form.Item {...tailLayout}>
                            <Button loading={props.submitting} type="primary" htmlType="submit">Create Password</Button>
                        </Form.Item>
                    </Form>
                )
            }
        </React.Fragment>
    );
};

export default connect(state => state.user, UserStore.actionCreators)(LoginForm);