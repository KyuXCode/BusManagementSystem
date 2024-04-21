var busIdToDelete;
var selectedBusIds;

$(document).ready(function () {
    $("#selectAllCheckbox").on("change", function () {
        $('input[name="busCheckbox"]').prop("checked", this.checked);
    });
});

function submitNewBus() {
    var formData = $("#createBusForm").serialize();

    $.ajax({
        type: "POST",
        url: "/Bus/Create",
        data: formData,
        success: function (response) {
            var createBusModal = new bootstrap.Modal(document.getElementById('createBusModal'));
            createBusModal.hide();
            location.reload();
        },
        error: function (response) {
            console.error("Error while creating the bus:", response);
        },
    });
}

function deleteSelectedBuses() {
    var selectedBusIds = [];
    $('input[name="busCheckbox"]:checked').each(function() {
        selectedBusIds.push($(this).val());
    });

    $('#deleteForm').submit();
}

function deleteBus(busId) {
    if (confirm('Are you sure you want to delete this bus?')) {
        $.ajax({
            url: '/Bus/Delete/' + busId,
            type: 'POST',
            success: function (response) {
                // Reload the page or update the bus list
                location.reload();
            },
            error: function (xhr, status, error) {
                console.error('Error deleting bus:', error);
            }
        });
    }
}


function editBus() {
    var formData = $("#editBusForm").serialize();
    console.log("Bus Id:", formData);

    $.ajax({
        type: "POST",
        url: "/Bus/Edit",
        data: formData,
        success: function (response) {
            var editBusModal = new bootstrap.Modal(document.getElementById('editBusModal'));
            editBusModal.hide();
            location.reload();
        },
        error: function (response) {
            console.error("Error while editing the bus:", response.responseText);
        },
    });
}
