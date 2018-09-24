var dateEditor = function (cell, onRendered, success, cancel) {
    //cell - the cell component for the editable cell
    //onRendered - function to call when the editor has been rendered
    //success - function to call to pass the succesfully updated value to Tabulator
    //cancel - function to call to abort the edit and return to a normal cell

    //create and style editor
    var editor = $("<input type='date'></input>");
    editor.css({
        "padding": "3px",
        "width": "100%",
        "box-sizing": "border-box"
    });

    //Set value of editor to the current value of the cell
    editor.val(moment(cell.getValue(), "YYYY/MM/DD").format("YYYY-MM-DD"));

    //set focus on the select box when the editor is selected (timeout allows for editor to be added to DOM)
    onRendered(function () {
        editor.focus();
        editor.css("height", "100%");
    });

    //when the value has been set, trigger the cell to update
    editor.on("change blur", function (e) {
        success(moment(editor.val(), "YYYY-MM-DD").format("YYYY/MM/DD"));
    });

    //return the editor element
    return editor;
};