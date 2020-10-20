function getValidFile(fileList) {
    if (fileList.length > 0) {
        for (let i = fileList.length - 1; i >= 0; i--) {
            const file = fileList[i];
            if (!file.response.success)
                continue;
            return file;
        }
    }
    return { response: { data: undefined } };
}

export default class Api {
    uploadPan = (values, editorState) => {
        console.warn(values);
        return new Promise((resolve, reject) => {
            const dataFile = getValidFile(values.dataKey.fileList);
            if (!dataFile.response.data) {
                reject("Please upload a file");
                return;
            }
            fetch(`/api/pan`, {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    alipayKey: values.alipayKey ? getValidFile(values.alipayKey.fileList).response.data : undefined,
                    wxpayKey: values.wxpayKey ? getValidFile(values.wxpayKey.fileList).response.data : undefined,
                    dataKey: values.dataKey ? dataFile.response.data : undefined,
                    description: editorState ? editorState.toHTML() : undefined,
                    password: values.password,
                })
            }).then(response => response.json()).then(data => {
                resolve({ ...data });
            }).catch(error => {
                reject(error);
            });
        });
    };
    getPan = id => {
        return new Promise((resolve, reject) => {
            fetch(`/api/pan/${id}`, {
                method: 'GET',
            }).then(response => response.json()).then(data => {
                resolve({ ...data });
            }).catch(error => {
                reject(error);
            });
        });
    };
    validatePan = (id, values) => {
        return new Promise((resolve, reject) => {
            fetch(`/api/pan/${id}/validate`, {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    password: values.password,
                })
            }).then(response => response.json()).then(data => {
                resolve({ ...data });
            }).catch(error => {
                reject(error);
            });
        });
    };
    getDownloadInfoByCid = (values) => {
        console.warn(values);
        return new Promise((resolve, reject) => {
            fetch(`/api/download/${values.cid}`, {
                method: 'GET',
            }).then(response => response.json()).then(data => {
                resolve({ ...data });
            }).catch(error => {
                reject(error);
            });
        });
    };
}
