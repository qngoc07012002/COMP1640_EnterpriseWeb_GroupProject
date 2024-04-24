
CKEDITOR.replace('content');
function coordinateComment(event) {

    var comment = document.querySelector('.comment-input textarea').value;
    var urlParams = new URLSearchParams(window.location.search);
    // Trích xuất articleId từ URL
    var urlParts = window.location.href.split('/');
    var articleId = parseInt(urlParts[urlParts.length - 1]);

    // Kiểm tra xem articleId có là một số hợp lệ không
    if (isNaN(articleId)) {
        console.error('ERROR !!! Invalid articleId.');
        return;
    }
    // Chuyển đổi đối tượng JSON sang chuỗi query string
    var formData = `CommentInput=${encodeURIComponent(comment)}&articleId=${encodeURIComponent(articleId)}`;

    // Gửi request POST tới API endpoint
    fetch('https://gwmagazine.xyz:7112/Coordinate/Comment/UploadPrivate', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: formData
    })
        .then(response => {
            if (response.ok) {

                window.location.reload();
            }
        })
        .catch(error => {
            console.error('ERROR !!! Cannot send comment.:', error);
        });
    document.querySelector('.comment-input textarea').value = '';
}