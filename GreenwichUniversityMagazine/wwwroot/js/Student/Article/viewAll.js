document.addEventListener("DOMContentLoaded", function () {
    var maxTitleWords = 15; // Số từ tối đa cho Title
    var maxSubtitleWords = 20; // Số từ tối đa cho SubTitle

    limitWords('.card-text-1 h3', maxTitleWords);
    limitWords('.card-text-1 p', maxSubtitleWords);

    function limitWords(selector, maxWords) {
        var elements = document.querySelectorAll(selector);
        elements.forEach(function (element) {
            var words = element.textContent.trim().split(/\s+/);
            if (words.length > maxWords) {
                element.textContent = words.slice(0, maxWords).join(" ") + '...';
            }
        });
    }
});


document.addEventListener("DOMContentLoaded", function () {
    const pagination = document.getElementById("pagination");
    pagination.addEventListener("click", function (event) {
        event.preventDefault();
        const target = event.target;
        if (target.classList.contains("pagination-item-link")) {
            const page = target.getAttribute("data-page");
            let url;
            const currentUrl = new URL(window.location.href);
            const magazineId = currentUrl.searchParams.get("magazineid");
            const termId = currentUrl.searchParams.get("termid");
            const facultyId = currentUrl.searchParams.get("facultyid");
            const searchString = currentUrl.searchParams.get("searchString");

            if (searchString) {
                url = `${window.location.pathname}?searchString=${searchString}&page=${page}`;
            } else if (magazineId) {
                url = `${window.location.pathname}?magazineid=${magazineId}&page=${page}`;
            } else if (termId) {
                url = `${window.location.pathname}?termid=${termId}&page=${page}`;

            } else if (facultyId) {
                url = `${window.location.pathname}?facultyid=${facultyId}&page=${page}`;
            } else {
                url = `${window.location.pathname}?page=${page}`;
            }
            window.location.href = url;
        }
    });
});

$(document).ready(function () {
    // Giới hạn số từ trong card title
    $("#articleContainer .card-title").each(function () {
        var maxWords = 5;
        var title = $(this).text().trim().split(/\s+/);
        if (title.length > maxWords) {
            var limitedTitle = title.slice(0, maxWords).join(" ");
            $(this).text(limitedTitle + "...");
        }
    });

    // Giới hạn số từ trong card text
    $("#articleContainer .card-text").each(function () {
        var maxWords = 15;
        var text = $(this).text().trim().split(/\s+/);
        if (text.length > maxWords) {
            var limitedText = text.slice(0, maxWords).join(" ");
            $(this).text(limitedText + "...");
        }
    });
});

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




