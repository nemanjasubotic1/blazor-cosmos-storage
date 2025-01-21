window.showConfirmationModal = function () {
    bootstrap.Modal.getOrCreateInstance(document.getElementById('confirmationModal')).show();
}

window.hideConfirmationModel = function () {
    bootstrap.Modal.getOrCreateInstance(document.getElementById('confirmationModal')).hide();
}