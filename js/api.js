const API_BASE = 'https://localhost:7257';

async function apiPost(path, body) {
    const response = await fetch(API_BASE + path, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    const data = await response.json().catch(() => ({}));
    if (!response.ok) throw new Error(data.error || 'HTTP ' + response.status);
    return data;
}

async function apiPostForm(path, formData) {
    const response = await fetch(API_BASE + path, {
        method: 'POST',
        body: formData
    });
    if (!response.ok) {
        const data = await response.json().catch(() => ({}));
        throw new Error(data.error || 'HTTP ' + response.status);
    }
    const disposition = response.headers.get('Content-Disposition') || '';
    const match = disposition.match(/filename[^;=\n]*=(['"]?)([^'";\n]+)\1/);
    const filename = match ? match[2] : 'download';
    const blob = await response.blob();
    return { blob, filename };
}

function downloadBlob(blob, filename) {
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    URL.revokeObjectURL(url);
}

const Keys = {
    generateAes: (keySize) => apiPost('/api/keys/aes', { keySize }),
    generateRsa: (keySize) => apiPost('/api/keys/rsa', { keySize })
};

const Aes = {
    encryptText: (plainText, keyBase64, ivBase64, mode) =>
        apiPost('/api/aes/encrypt-text', { plainText, keyBase64, ivBase64, mode }),
    decryptText: (cipherTextBase64, keyBase64, ivBase64, mode) =>
        apiPost('/api/aes/decrypt-text', { cipherTextBase64, keyBase64, ivBase64, mode }),
    encryptFile: (file, keyBase64, ivBase64, mode) => {
        const fd = new FormData();
        fd.append('file', file);
        fd.append('keyBase64', keyBase64);
        fd.append('ivBase64', ivBase64);
        fd.append('mode', mode);
        return apiPostForm('/api/aes/encrypt-file', fd);
    },
    decryptFile: (file, keyBase64, ivBase64, mode) => {
        const fd = new FormData();
        fd.append('file', file);
        fd.append('keyBase64', keyBase64);
        fd.append('ivBase64', ivBase64);
        fd.append('mode', mode);
        return apiPostForm('/api/aes/decrypt-file', fd);
    }
};

const Rsa = {
    encrypt: (aesKeyBase64, publicKeyPem) =>
        apiPost('/api/rsa/encrypt', { aesKeyBase64, publicKeyPem }),
    decrypt: (encryptedKeyBase64, privateKeyPem) =>
        apiPost('/api/rsa/decrypt', { encryptedKeyBase64, privateKeyPem })
};

const Hash = {
    hashText: (text, algorithm) => apiPost('/api/hash/text', { text, algorithm }),
    hashFile: (file, algorithm) => {
        const fd = new FormData();
        fd.append('file', file);
        fd.append('algorithm', algorithm);
        return fetch(API_BASE + '/api/hash/file', { method: 'POST', body: fd })
            .then(r => r.json().then(d => { if (!r.ok) throw new Error(d.error); return d; }));
    },
    generateHmac: (message, keyBase64, algorithm) =>
        apiPost('/api/hash/hmac/generate', { message, keyBase64, algorithm }),
    verifyHmac: (message, keyBase64, hmacBase64, algorithm) =>
        apiPost('/api/hash/hmac/verify', { message, keyBase64, hmacBase64, algorithm })
};

const Signature = {
    sign: (data, privateKeyPem) => apiPost('/api/signature/sign', { data, privateKeyPem }),
    verify: (data, signatureBase64, publicKeyPem) =>
        apiPost('/api/signature/verify', { data, signatureBase64, publicKeyPem })
};
