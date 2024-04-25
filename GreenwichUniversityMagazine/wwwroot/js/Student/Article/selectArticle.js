(document).ready(function () {


    // Kiểm tra nếu có tham số 'commented' trong URL, tức là đã comment
    var urlParams = new URLSearchParams(window.location.search);
    if (urlParams.has('commented')) {
        // Nếu có tham số 'commented', gọi hàm cuộn xuống dưới cùng
        scrollToBottom();
    }

    // ham load magazine ( doi du lieu onchange="redirectToMagazine() tren select duoc load thi thuc hien ham nay)
    function redirectToMagazine() {
        // lấy dữ liệu từ select box
        var selectedMagazineId = document.getElementById("magazineselect").value;
        // url mặc định
        var baseUrl = "/student/view/index";

        if (selectedMagazineId) {
            // Nếu có giá trị, thêm vào URL
            window.location.href = baseUrl + "?magazineid=" + selectedMagazineId;
        } else {
            // Nếu không có giá trị, chỉ chuyển đến trang mặc định
            window.location.href = baseUrl;
        }
    }

    function redirectToTerm() {
        var selectedTermId = document.getElementById("termselect").value;
        var baseUrl = "/student/view/index";
        if (selectedTermId) {
            window.location.href = baseUrl + "?termid=" + selectedTermId;
        } else {
            window.location.href = baseUrl;
        }
    }

    function redirectToFaculty() {
        var selectedFacultyId = document.getElementById("facultyselect").value;
        var baseUrl = "/student/view/index";
        if (selectedFacultyId) {
            window.location.href = baseUrl + "?facultyid=" + selectedFacultyId;
        } else {
            window.location.href = baseUrl;
        }
    }

    document.getElementById("searchForm").addEventListener("submit", function (event) {
        // Lấy giá trị của ô tìm kiếm
        var searchString = document.getElementById("searchInput").value;
        // Kiểm tra xem ô tìm kiếm có rỗng không
        if (!searchString.trim()) {
            // Nếu rỗng, hủy việc gửi form
            event.preventDefault();
        } else {
            // Nếu có giá trị, tạo URL tìm kiếm và chuyển hướng đến trang kết quả tìm kiếm
            var baseUrl = "/student/view/index";
            var searchUrl = baseUrl + "?searchString=" + searchString;
            window.location.href = searchUrl;
        }
    });
    var commentSubmitted = false; // Biến trạng thái để kiểm tra xem comment đã được gửi hay chưa

    // Hàm tự động cuộn xuống dưới cùng của trang
    function scrollToBottom() {
        window.scrollTo(0, document.body.scrollHeight);
    }
});
function studentComment(event) {

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
    fetch('https://localhost:7112/Student/Comment/UploadPublic', {
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
            else if (response.status === 400) {
                alert("Please Login!");
                window.location.href = "https://localhost:7112/Student/Home/Login";
            }
        })
        .catch(error => {
            console.error('ERROR !!! Cannot send comment.:', error);
        });
    document.querySelector('.comment-input textarea').value = '';
}