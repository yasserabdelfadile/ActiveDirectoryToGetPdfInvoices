


 // Declare the variable outside the $(document).ready() function
$(document).ready(function () {

    var dataTable = $('#example').DataTable({
                    "lengthMenu": [10, 25, 50, 75, 100],
                    "pageLength": 10,
                    "order": [],
                    "language": {
                        "search": "Search:",
                        "class":'btn btn-sm btn-outline-light bi bi-search mb-4',
                        "paginate": {
                            "previous": "<",
                            "next": ">"
                        }
                    } 
     });

    // Handle apply filter button click
    $('#applyFilter').click(function () {
        var startDate = $('#startDate').val();
        var endDate = $('#endDate').val();

        // Redirect to PrintPDF action with selected dates
        window.location.href = "/Home/Index?startDate=" + startDate + "&endDate=" + endDate;
    });

// Handle download of selected files
     $('#downloadSelected').click(function () {
            var selectedFiles = [];
            $('input[name="selectedFiles"]:checked').each(function () {
                selectedFiles.push($(this).val());
            });
            if (selectedFiles.length > 0) {
                window.location.href = "/Home/DownloadSelected?ids=" + selectedFiles.join(',');
            }
     });


    //handel check box to select all
    // Handle checkbox to select all
    $('#selectall').click(function () {
        var isChecked = $(this).prop('checked');
        $('input[name="selectedFiles"]').prop('checked', isChecked);
    });


    // Show filter pop-up
    function showFilterPopup() {
        $('#filterPopup').show();
    }

    // Apply date filter
    function applyDateFilter() {
        var startDate = $('#filterStartDate').val();
        var endDate = $('#filterEndDate').val();

        // Hide filter pop-up
        $('#filterPopup').hide();

        // Apply date filter using the DataTable API
        dataTable.column(4).search(startDate + '-' + endDate).draw();
    }


});
