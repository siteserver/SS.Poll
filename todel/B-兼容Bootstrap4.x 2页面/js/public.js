jQuery(document).ready(function($) {
$(".bgImg").each(function(i){$(".bgImg").eq(i).css("background-image","url("+$(this).find("img").attr("src")+")")});


});