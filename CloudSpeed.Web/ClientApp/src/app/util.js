export function itemUrl(id) {
    return `${window.location.origin}/pan/item-${id}`;
}

export function panDownloadUrl(id, password) {
    if (password)
        return `${window.location.origin}/api/pan/${id}/download?password=${password}`;
    return `${window.location.origin}/api/pan/${id}/download`;
}
