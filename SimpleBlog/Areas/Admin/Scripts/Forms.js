$(document).ready(function() {

    $("a[data-post]").click(function(e) {
        e.preventDefault();

        var self = $(this);
        var message = self.data("post");

        if (message && !confirm(message)) {
            return;
        }

        var antyForgeryToken = $("#anti-forgery-form input");
        var antyForgeryInput = $("<input type='hidden' name=''>").attr("name", antyForgeryToken.attr("name")).val(antyForgeryToken.val());

        $("<form>")
            .attr("method", "post")
            .attr("action", self.attr("href"))
            .append(antyForgeryInput)
            .appendTo(document.body)
            .submit();
    });

    $("[data-slug]").each(function() {
        var self = $(this);

        var sendSlugFrom = $(self.data("slug"));
        sendSlugFrom.keyup(function() {
            var slug = sendSlugFrom.val();

            slug = slug.replace(/[^a-zA-Z0-9\s]/g, "");
            slug = slug.toLowerCase();
            slug = slug.replace(/\s+/g, "-");

            if (slug.charAt(slug.length -1) == "-") {
                slug = slug.substr(0, (slug.length - 1));
            }

            self.val(slug);
        });

    });


});