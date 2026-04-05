async function signData() {
    const btn = document.getElementById('signBtn');
    hideEl('signError');
    hideEl('signResultCard');
    setLoading(btn, true);

    try {
        const data       = document.getElementById('signData').value;
        const privateKey = document.getElementById('signPrivateKey').value.trim();

        if (!data || !privateKey) throw new Error('Vul de tekst en de privésleutel in.');

        const result = await Signature.sign(data, privateKey);
        document.getElementById('signResult').value = result.signatureBase64;
        showEl('signResultCard');
    } catch (err) {
        showMsg('signError', `Fout: ${err.message}`, 'error');
    } finally {
        setLoading(btn, false);
    }
}

async function verifySignature() {
    const btn = document.getElementById('verifyBtn');
    hideEl('verifyError');
    hideEl('verifyResult');
    setLoading(btn, true);

    try {
        const data      = document.getElementById('verifyData').value;
        const signature = document.getElementById('verifySignature').value.trim();
        const publicKey = document.getElementById('verifyPublicKey').value.trim();

        if (!data || !signature || !publicKey) throw new Error('Vul alle velden in.');

        const result = await Signature.verify(data, signature, publicKey);
        const resultEl = document.getElementById('verifyResult');
        resultEl.textContent = result.isValid
            ? 'Handtekening is geldig! Data is authentiek en ongewijzigd.'
            : 'Handtekening is ONGELDIG. Data is gewijzigd of verkeerde sleutel.';
        resultEl.className = `alert alert-${result.isValid ? 'success' : 'error'}`;
        resultEl.style.display = 'block';
    } catch (err) {
        showMsg('verifyError', `Fout: ${err.message}`, 'error');
    } finally {
        setLoading(btn, false);
    }
}
