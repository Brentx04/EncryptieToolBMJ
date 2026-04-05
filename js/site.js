function showMsg(id, msg, type) {
    const el = document.getElementById(id);
    if (!el) return;
    el.className = 'alert alert-' + type;
    el.textContent = msg;
    el.style.display = 'block';
}

function hideEl(id) {
    const el = document.getElementById(id);
    if (el) el.style.display = 'none';
}

function showEl(id) {
    const el = document.getElementById(id);
    if (el) el.style.display = 'block';
}

function copyText(id) {
    const el = document.getElementById(id);
    if (!el) return;
    const text = el.value !== undefined ? el.value : el.textContent;
    navigator.clipboard.writeText(text).then(() => {
        const btn = document.querySelector('[onclick="copyText(\'' + id + '\')"]');
        if (!btn) return;
        const orig = btn.textContent;
        btn.textContent = 'Gekopieerd!';
        setTimeout(() => { btn.textContent = orig; }, 1500);
    });
}

function downloadText(content, filename) {
    const blob = new Blob([content], { type: 'text/plain' });
    downloadBlob(blob, filename);
}

function setLoading(btn, loading) {
    if (loading) {
        btn.disabled = true;
        btn.dataset.orig = btn.textContent;
        btn.textContent = 'Bezig...';
    } else {
        btn.disabled = false;
        btn.textContent = btn.dataset.orig || btn.textContent;
    }
}

function initTabs(containerSelector) {
    const container = document.querySelector(containerSelector);
    if (!container) return;
    const buttons = container.querySelectorAll('.tabs button');
    const panels = container.querySelectorAll('.tab-panel');

    buttons.forEach((btn, i) => {
        btn.addEventListener('click', () => {
            buttons.forEach(b => b.classList.remove('active'));
            panels.forEach(p => p.classList.remove('active'));
            btn.classList.add('active');
            panels[i].classList.add('active');
        });
    });
}

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('[data-tabs]').forEach(el => initTabs('[data-tabs="' + el.dataset.tabs + '"]'));
    initTabs('body');
});
