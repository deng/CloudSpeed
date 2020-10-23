import Api from './../app/api';

const unloadedState = { user: {}, submitting: false, error: undefined };

export const actionCreators = {
    login: (values) => (dispatch) => {
        new Api().login(values)
            .then(data => {
                if (data.success) {
                    dispatch({ type: 'LOGIN_SUCCESS', user: data.data });
                } else {
                    dispatch({ type: 'LOGOUT_FAILTURE', error: data.error });
                }
            });
        dispatch({ type: 'REQUEST_LOGIN' });
    },
    createMember: (values) => (dispatch) => {
        new Api().createMember(values)
            .then(data => {
                if (data.success) {
                    dispatch({ type: 'CREATE_MEMBER_SUCCESS', user: data.data });
                } else {
                    dispatch({ type: 'CREATE_MEMBER_FAILTURE', error: data.error });
                }
            });
        dispatch({ type: 'REQUEST_CREATE_MEMBER' });
    },
};

export const reducer = (state, incomingAction) => {
    if (state === undefined)
        return unloadedState;
    const action = incomingAction;
    switch (action.type) {
        case 'LOGIN_SUCCESS':
        case 'CREATE_MEMBER_SUCCESS':
            return { user: { ...action.user }, submitting: false, error: undefined };
        case 'LOGOUT_FAILTURE':
        case 'CREATE_MEMBER_FAILTURE':
            return { user: {}, submitting: false, error: action.error };
        case 'REQUEST_LOGIN':
        case 'REQUEST_CREATE_MEMBER':
            return { user: {}, submitting: true };
        default:
            return state;
    }
};