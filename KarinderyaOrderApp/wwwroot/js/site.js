document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('[data-confirm-modal]').forEach(modal => {
        modal.addEventListener('show.bs.modal', event => {
            const button = event.relatedTarget;
            if (!button) return;

            modal.querySelector('[data-confirm-name]').textContent = button.dataset.confirmName;
            modal.querySelector('form').action = button.dataset.confirmAction;
        });
    });
});