async function encryptText() {
    await runTextOperation(true);
}

async function decryptText() {
    await runTextOperation(false);
}

async function runTextOperation(isEncrypt) {
    const btn = isEncrypt
        ? document.getElementById('encryptTextBtn')
        : document.getElementById('decryptTextBtn');

    hideEl('textError');
    hideEl('textResultCard');
    setLoading(btn, true);

    try {
        const input = document.getElementById('textInput').value;
        const key   = document.getElementById('textKey').value;
        const iv    = document.getElementById('textIv').value;
        const mode  = document.getElementById('textMode').value;

        if (!input || !key || !iv) throw new Error('Vul alle velden in.');

        const result = isEncrypt
            ? await Aes.encryptText(input, key, iv, mode)
            : await Aes.decryptText(input, key, iv, mode);

        document.getElementById('textResult').value = result.result;
        showEl('textResultCard');
    } catch (err) {
        showMsg('textError', `Fout: ${err.message}`, 'error');
    } finally {
        setLoading(btn, false);
    }
}

async function encryptFile() {
    await runFileOperation(true);
}

async function decryptFile() {
    await runFileOperation(false);
}

async function runFileOperation(isEncrypt) {
    const btn = isEncrypt
        ? document.getElementById('encryptFileBtn')
        : document.getElementById('decryptFileBtn');

    hideEl('fileError');
    hideEl('fileSuccess');
    setLoading(btn, true);

    try {
        const fileInput = document.getElementById('fileInput');
        const key  = document.getElementById('fileKey').value;
        const iv   = document.getElementById('fileIv').value;
        const mode = document.getElementById('fileMode').value;

        if (!fileInput.files.length) throw new Error('Selecteer een bestand.');
        if (!key || !iv) throw new Error('Vul de sleutel en IV in.');

        const file = fileInput.files[0];
        const result = isEncrypt
            ? await Aes.encryptFile(file, key, iv, mode)
            : await Aes.decryptFile(file, key, iv, mode);

        downloadBlob(result.blob, result.filename);
        showMsg('fileSuccess', `Bestand "${result.filename}" wordt gedownload.`, 'success');
    } catch (err) {
        showMsg('fileError', `Fout: ${err.message}`, 'error');
    } finally {
        setLoading(btn, false);
    }
}
