//show or hide students
$('.showLessons').on('click', function () {
    var clicks = $(this).data('clicks');
    if (clicks) {
        $('.alllessons').hide(250);
    } else {
        $('.alllessons').show(250);
    }
    $(this).data("clicks", !clicks);
});