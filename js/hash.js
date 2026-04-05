function checkHashWarning(selectId, warningId) {
    const algo = document.getElementById(selectId).value;
    if (algo === 'MD5') {
        showMsg(warningId, 'MD5 is onveilig — gebruik alleen voor educatieve doeleinden. Gebruik SHA-256 of hoger in productie.', 'warning');
    } else if (algo === 'SHA1') {
        showMsg(warningId, 'SHA-1 wordt als zwak beschouwd. Gebruik SHA-256 of hoger.', 'warning');
    } else {
        hideEl(warningId);
    }
}

async function hashText() {
    const btn = document.getElementById('hashTextBtn');
    hideEl('hashTextError');
    hideEl('hashTextResultCard');
    setLoading(btn, true);

    try {
        const text = document.getElementById('hashTextInput').value;
        const algo = document.getElementById('hashTextAlgo').value;
        if (!text) throw new Error('Vul een tekst in.');

        const result = await Hash.hashText(text, algo);
        document.getElementById('hashTextResult').value = result.hash;
        document.getElementById('hashTextResultLabel').textContent = `Hash (${result.algorithm})`;
        showEl('hashTextResultCard');
    } catch (err) {
        showMsg('hashTextError', `Fout: ${err.message}`, 'error');
    } finally {
        setLoading(btn, false);
    }
}

async function hashFile() {
    const btn = document.getElementById('hashFileBtn');
    hideEl('hashFileError');
    hideEl('hashFileResultCard');
    hideEl('hashFileMatchResult');
    setLoading(btn, true);

    try {
        const fileInput = document.getElementById('hashFileInput');
        const algo      = document.getElementById('hashFileAlgo').value;
        const expected  = document.getElementById('hashFileExpected').value.trim();

        if (!fileInput.files.length) throw new Error('Selecteer een bestand.');

        const result = await Hash.hashFile(fileInput.files[0], algo);
        document.getElementById('hashFileResult').value = result.hash;
        document.getElementById('hashFileResultLabel').textContent = `Hash (${result.algorithm})`;
        showEl('hashFileResultCard');

        if (expected) {
            const match = result.hash.toLowerCase() === expected.toLowerCase();
            const matchEl = document.getElementById('hashFileMatchResult');
            matchEl.textContent = match
                ? 'Hash komt overeen! Het bestand is ongewijzigd.'
                : 'Hash komt NIET overeen! Het bestand is mogelijk gewijzigd of beschadigd.';
            matchEl.className = `alert alert-${match ? 'success' : 'error'}`;
            matchEl.style.display = 'block';
        }
    } catch (err) {
        showMsg('hashFileError', `Fout: ${err.message}`, 'error');
    } finally {
        setLoading(btn, false);
    }
}

async function generateHmac() {
    const btn = document.getElementById('hmacGenBtn');
    hideEl('hmacGenError');
    hideEl('hmacGenResultCard');
    setLoading(btn, true);

    try {
        const message  = document.getElementById('hmacGenMsg').value;
        const keyBase64 = document.getElementById('hmacGenKey').value.trim();
        const algo     = document.getElementById('hmacGenAlgo').value;

        if (!message || !keyBase64) throw new Error('Vul het bericht en de sleutel in.');

        const result = await Hash.generateHmac(message, keyBase64, algo);
        document.getElementById('hmacGenResult').value = result.hmac;
        showEl('hmacGenResultCard');
    } catch (err) {
        showMsg('hmacGenError', `Fout: ${err.message}`, 'error');
    } finally {
        setLoading(btn, false);
    }
}

async function verifyHmac() {
    const btn = document.getElementById('hmacVerBtn');
    hideEl('hmacVerError');
    hideEl('hmacVerResult');
    setLoading(btn, true);

    try {
        const message   = document.getElementById('hmacVerMsg').value;
        const keyBase64  = document.getElementById('hmacVerKey').value.trim();
        const hmacValue = document.getElementById('hmacVerValue').value.trim();
        const algo      = document.getElementById('hmacVerAlgo').value;

        if (!message || !keyBase64 || !hmacValue) throw new Error('Vul alle velden in.');

        const result = await Hash.verifyHmac(message, keyBase64, hmacValue, algo);
        const resultEl = document.getElementById('hmacVerResult');
        resultEl.textContent = result.isValid
            ? 'HMAC is geldig — bericht is authentiek en ongewijzigd.'
            : 'HMAC is ongeldig — bericht is gewijzigd of verkeerde sleutel.';
        resultEl.className = `alert alert-${result.isValid ? 'success' : 'error'}`;
        resultEl.style.display = 'block';
    } catch (err) {
        showMsg('hmacVerError', `Fout: ${err.message}`, 'error');
    } finally {
        setLoading(btn, false);
    }
}
