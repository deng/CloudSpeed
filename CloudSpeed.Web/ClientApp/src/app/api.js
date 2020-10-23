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
    uploadPan = (token, values, editorState) => {
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
                    Authorization: token ? `Bearer ${token}` : '',
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
    checkMember = (address) => {
        return new Promise((resolve, reject) => {
            fetch(`/api/member/check`, {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    address,
                })
            }).then(response => response.json()).then(data => {
                resolve({ ...data });
            }).catch(error => {
                reject(error);
            });
        });
    };
    createMember = (values) => {
        return new Promise((resolve, reject) => {
            fetch(`/api/member/create`, {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    address: values.address,
                    password: values.password,
                })
            }).then(response => response.json()).then(data => {
                resolve({ ...data });
            }).catch(error => {
                reject(error);
            });
        });
    };
    login = (values) => {
        return new Promise((resolve, reject) => {
            fetch(`/api/member/login`, {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    address: values.address,
                    password: values.password,
                })
            }).then(response => response.json()).then(data => {
                resolve({ ...data });
            }).catch(error => {
                reject(error);
            });
        });
    };
    fetchFiles = (token, skip, limit) => {
        return new Promise((resolve, reject) => {
            fetch(`/api/myfiles`, {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({
                    skip,
                    limit,
                })
            }).then(response => response.json()).then(data => {
                resolve({ ...data });
            }).catch(error => {
                reject(error);
            });
        });
    };
}
