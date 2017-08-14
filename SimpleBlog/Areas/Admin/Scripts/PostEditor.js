$(document).ready(function() {

    var tagEditor = $(".post-tag-editor");

    // select tag
    tagEditor.find(".tag-select").on("click", " > li > a", function (e) {
        e.preventDefault();

        var self = $(this);
        var tagParent = self.closest("li");
        tagParent.toggleClass("selected");

        var isSelected = tagParent.hasClass("selected");
        tagParent.find(".selected-input").val(isSelected);
    });

    // add tag
    var addTagButton = tagEditor.find(".add-tag-button");
    var newTagName = tagEditor.find(".new-tag-name");

    addTagButton.click(function(e) {
        e.preventDefault();

        var tagName = newTagName.val();
        if (!tagName) {
            return;
        }

        addTag(newTagName.val());
    });

    function addTag(tagName) {

        console.log(tagEditor.find(".tag-select > li"));
        var newIndex = tagEditor.find(".tag-select > li").length - 1;

        tagEditor
            .find(".tag-select > li.template")
            .clone()
            .removeClass("template")
            .find(".name").text(tagName).end()
            .find(".name-input").attr("name", "Tags[" + newIndex + "].Name").val(tagName).end()
            .find(".selected-input").attr("name", "Tags[" + newIndex + "].IsChecked").val(true).end()
            .appendTo(tagEditor.find(".tag-select"));

        newTagName.val("");

    }

});