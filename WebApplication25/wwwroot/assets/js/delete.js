$(document).ready(function () {
    $('.js-delete').on('click', function () {
        var btn = $(this);
        var result = confirm("Are you sure you want to delete this user?");

        if (result) {
            var userId = btn.data('id');
            var url = '/api/Users?UserID=' + encodeURIComponent(userId);

            fetch(url, { method: 'DELETE' })
                .then(response => {
                    if (response.ok) {
                        // Successful deletion, handle as desired (e.g., remove row from table)
                        console.log('User deleted successfully.');
                    } else {
                        // Error occurred, handle as desired (e.g., display error message)
                        console.error('Failed to delete user.');
                    }
                })
                .catch(error => {
                    console.error('An error occurred during user deletion:', error);
                });
        }
    });
});
