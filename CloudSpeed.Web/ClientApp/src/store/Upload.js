import Api from './../app/api';

const unloadedState = { result: {}, uploading: false, error: undefined };

export const actionCreators = {
    upload: (values, editorState) => (dispatch, getState) => {
        const appState = getState();
        const token = appState.user.user ? appState.user.user.token : undefined;
        console.warn(token);
        new Api().uploadPan(token, values, editorState)
            .then(data => {
                console.warn(data);
                if (data.success) {
                    dispatch({ type: 'UPLOAD_SUCCESS', result: data });
                } else {
                    dispatch({ type: 'UPLOAD_FAILTURE', error: data.error });
                }
            });
        dispatch({ type: 'REQUEST_UPLOAD' });
    },
    close: () => (dispatch, getState) => {
        dispatch({ type: 'CLOSE_UPLOAD' });
    },
};

export const reducer = (state, incomingAction) => {
    if (state === undefined)
        return unloadedState;
    const action = incomingAction;
    switch (action.type) {
        case 'UPLOAD_SUCCESS':
            return { result: { ...action.result }, uploading: false, error: undefined };
        case 'UPLOAD_FAILTURE':
            return { result: {}, uploading: false, error: action.error };
        case 'REQUEST_UPLOAD':
            return { result: {}, uploading: true, error: undefined };
        case 'CLOSE_UPLOAD':
            return { result: {}, uploading: false, error: undefined };
        default:
            return state;
    }
};