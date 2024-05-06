const tgModalName = 'tg-banner-closed-5';

function openTgModal() {
    window.setTimeout(() => {
        var closed = localStorage.getItem(tgModalName);
        if (!!closed) {
            return;
        }

        document.getElementById('show-tg-modal').click();
    }, 2000);
}

function closeTgModal() {
    localStorage.setItem(tgModalName, 'true');
}