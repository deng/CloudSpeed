import * as User from './User';
import * as MyFiles from './MyFiles';
import * as Upload from './Upload';

export const reducers = {
    user: User.reducer,
    myFiles: MyFiles.reducer,
    upload: Upload.reducer
};
