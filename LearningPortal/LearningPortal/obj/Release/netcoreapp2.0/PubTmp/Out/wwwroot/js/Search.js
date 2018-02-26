var typingTimer;
var doneTypingInterval = 600;
var $input = $('.search');

//on keyup, start the countdown
$input.on('keyup', function () {
    clearTimeout(typingTimer);
    typingTimer = setTimeout(doneTyping, doneTypingInterval);
});

//on keydown, clear the count down
$input.on('keydown', function () {
    clearTimeout(typingTimer);
});