async function rsaEncrypt() {
    const btn = document.getElementById('rsaEncBtn');
    hideEl('rsaEncError');
    hideEl('rsaEncResultCard');
    setLoading(btn, true);

    try {
        const aesKey    = document.getElementById('encAesKey').value.trim();
        const publicPem = document.getElementById('encPublicPem').value.trim();

        if (!aesKey || !publicPem) throw new Error('Vul de AES-sleutel en publieke sleutel in.');

        const result = await Rsa.encrypt(aesKey, publicPem);
        document.getElementById('rsaEncResult').value = result.resultBase64;
        showEl('rsaEncResultCard');
    } catch (err) {
        showMsg('rsaEncError', `Fout: ${err.message}`, 'error');
    } finally {
        setLoading(btn, false);
    }
}

async function rsaDecrypt() {
    const btn = document.getElementById('rsaDecBtn');
    hideEl('rsaDecError');
    hideEl('rsaDecResultCard');
    setLoading(btn, true);

    try {
        const encKey     = document.getElementById('decEncryptedKey').value.trim();
        const privatePem = document.getElementById('decPrivatePem').value.trim();

        if (!encKey || !privatePem) throw new Error('Vul de versleutelde sleutel en privésleutel in.');

        const result = await Rsa.decrypt(encKey, privatePem);
        document.getElementById('rsaDecResult').value = result.resultBase64;
        showEl('rsaDecResultCard');
    } catch (err) {
        showMsg('rsaDecError', `Fout: ${err.message}`, 'error');
    } finally {
        setLoading(btn, false);
    }
}
