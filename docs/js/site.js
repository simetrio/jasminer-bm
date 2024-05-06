function getTgModalName() {
    return 'tg-banner-closed-' + new Date().getFullYear() + new Date().getMonth() + new Date().getDate();
}

function openTgModal() {
    window.setTimeout(() => {
        var closed = localStorage.getItem(getTgModalName());
        if (!!closed) {
            return;
        }

        document.getElementById('show-tg-modal').click();
    }, 2000);
}

function closeTgModal() {
    localStorage.setItem(getTgModalName(), 'true');
}