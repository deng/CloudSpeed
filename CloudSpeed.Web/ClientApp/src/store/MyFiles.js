import Api from './../app/api';

const unloadedState = { files: [], isLoading: false, page: 1, pageSize: 10, total: 0, error: undefined };

export const actionCreators = {
    requestMyFiles: (page, pageSize) => (dispatch, getState) => {
        const appState = getState();
        new Api().fetchFiles(appState.user.user.token, (page - 1) * pageSize, pageSize)
            .then(data => {
                if (data.success) {
                    dispatch({ type: 'RECEIVE_MYFILES_SUCCESS', page, pageSize, files: data.data.list, total: data.data.total });
                } else {
                    dispatch({ type: 'RECEIVE_MYFILES_FAILTURE', error: data.error });
                }
            });
        dispatch({ type: 'REQUEST_MYFILES', page, pageSize });
    }
};

export const reducer = (state, incomingAction) => {
    if (state === undefined)
        return unloadedState;
    const action = incomingAction;
    switch (action.type) {
        case 'REQUEST_MYFILES':
            return { ...state, isLoading: true, page: action.page, pageSize: action.pageSize, error: undefined };
        case 'RECEIVE_MYFILES_SUCCESS':
            return { ...state, isLoading: false, page: action.page, pageSize: action.pageSize, total: action.total, files: action.files, error: undefined };
        case 'RECEIVE_MYFILES_FAILTURE':
            return { ...state, isLoading: false, error: action.error };
        default:
            return state;
    };
};