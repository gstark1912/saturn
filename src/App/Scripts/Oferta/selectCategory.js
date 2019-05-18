$(document).ready(function () {
    
    $('#subCategoriesTree').jstree({
        'core': {
            "expand_selected_onload" : false,
            'data': []
        },
        "checkbox": {
            "three_state": false,
        },
        "plugins": ["wholerow", "checkbox"]
    });

    $("#subCategoriesTree").bind(
        "select_node.jstree deselect_node.jstree", function (evt, data) {
            $("#subCategories").val(JSON.stringify($('#subCategoriesTree').jstree(true).get_selected()).replace(/"/g, '').replace(/\[/g, '').replace(/\]/g, ''));
        }
    );

    $("#subCategoriesTree").on("select_node.jstree", function (node, selected) {
        selectParent(selected.node.id);
    });

    $("#subCategoriesTree").on("deselect_node.jstree", function (node, selected) {
        deselectChildren(selected.node.id);
    });

    function selectParent(node) {
        var parentNode = $("#subCategoriesTree").jstree(true).get_parent(node);
        if (parentNode != "#") {
            $("#subCategoriesTree").jstree(true).select_node(parentNode);
            selectParent(parentNode);
        }
    }

    function deselectChildren(node) {
        var childNodes = $("#subCategoriesTree").jstree(true).get_children_dom(node);
        if ($(childNodes).size() > 0) {
            $(childNodes).each(function (x, child) {
                $("#subCategoriesTree").jstree(true).deselect_node(child.id);
                deselectChildren(child.id);
            });
        }
    }

    $("#categorySelector").change(function (e) {
        var categoryId = $(e.target).val();
        if (categoryId != "") {
            $.getJSON("Category/SubCategories", { "category": categoryId }, function (result, status) {
                $('#subCategoriesTree').jstree(true).settings.core.data = JSON.parse(result);
                $('#subCategoriesTree').jstree(true).refresh();
            })
        } else {
            $('#subCategoriesTree').jstree(true).settings.core.data = null;
            $('#subCategoriesTree').jstree(true).refresh();
            
        }
        
        })
          $("#subCategoriesTree").bind("refresh.jstree", function (e, data) { $(this).jstree("close_all");}) 
        
});