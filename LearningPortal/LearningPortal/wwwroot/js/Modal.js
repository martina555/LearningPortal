$(".createModalLink").click(function () {
    $('#myModal').modal('show');
});

function hideModal() {
    $('#myModal').modal('hide');
    $('.name').val("");
}