let _aesData = null;
let _rsaData = null;

async function generateAes() {
    const btn = document.getElementById('generateAesBtn');
    hideEl('aesError');
    hideEl('aesResult');
    setLoading(btn, true);

    try {
        const keySize = parseInt(document.getElementById('aesKeySize').value);
        _aesData = await Keys.generateAes(keySize);
        _aesData.keySizeBits = keySize;

        document.getElementById('aesKeyBase64').value = _aesData.keyBase64;
        document.getElementById('aesKeyHex').value    = _aesData.keyHex;
        document.getElementById('aesIvBase64').value  = _aesData.ivBase64;
        document.getElementById('aesIvHex').value     = _aesData.ivHex;

        showEl('aesResult');
    } catch (err) {
        showMsg('aesError', `Fout: ${err.message}`, 'error');
    } finally {
        setLoading(btn, false);
    }
}

function downloadAesKey() {
    if (!_aesData) return;
    const content =
        `AES Sleutel (${_aesData.keySizeBits} bits)\r\n` +
        `Key (Base64): ${_aesData.keyBase64}\r\n` +
        `Key (Hex):    ${_aesData.keyHex}\r\n` +
        `IV  (Base64): ${_aesData.ivBase64}\r\n` +
        `IV  (Hex):    ${_aesData.ivHex}\r\n`;
    downloadText(content, `aes-key-${_aesData.keySizeBits}bit.txt`);
}

async function generateRsa() {
    const btn = document.getElementById('generateRsaBtn');
    hideEl('rsaError');
    hideEl('rsaResult');
    setLoading(btn, true);

    try {
        const keySize = parseInt(document.getElementById('rsaKeySize').value);
        _rsaData = await Keys.generateRsa(keySize);

        document.getElementById('rsaPublicPem').value  = _rsaData.publicKeyPem;
        document.getElementById('rsaPrivatePem').value = _rsaData.privateKeyPem;

        showEl('rsaResult');
    } catch (err) {
        showMsg('rsaError', `Fout: ${err.message}`, 'error');
    } finally {
        setLoading(btn, false);
    }
}

function downloadRsaPublic() {
    if (!_rsaData) return;
    downloadText(_rsaData.publicKeyPem, 'public-key.pem');
}

function downloadRsaPrivate() {
    if (!_rsaData) return;
    downloadText(_rsaData.privateKeyPem, 'private-key.pem');
}
