let currentPage = 2; // Track current page
const pageSize = 6; // Number of articles per page
function confirmDelete(articleId) {
    var result = confirm("Do you want to delete this article ?");
    if (result) {
        deleteArticle(articleId);
    }
}

function deleteArticle(articleId) {
    fetch('/student/article/delete/' + articleId, {
        method: 'DELETE'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert(data.message);
                location.reload();
            } else {
                alert(data.message);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert("Wrong to delete this article.");
        });
}
function loadArticlesByStatus(status, evt, page = 1) {
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(status).style.display = "block";
    evt.currentTarget.className += " active";
    fetch(`/student/article/GetByStatus?status=${status}&page=${page}&pageSize=${pageSize}`)
        .then(response => response.json())
        .then(data => {
            updateArticleList(data.articles);
            // Update pagination
            updatePagination(status, page, Math.ceil(data.allArticleCount / pageSize));
        })
        .catch(error => {
            console.error('Error:', error);
            alert("Failed to load articles.");
        });
}

function updateArticleList(articles) {
    document.getElementById('bookList').innerHTML = '';
    if (articles.length > 0) {
        articles.forEach(article => {
            let articleCard = `<div class="article-card">`;
            articleCard += `<img class="article-card-img" src="${article.imgUrl}" alt="Top Image">
                        <div class="article-information">
                            <label class="article-card-text">Magazine:${article.magazines.title}</label>
                            <h2 class="article-card-text">${article.title}</h2>
                            <p class="article-card-text text-ellipsis">${article.subTitle}</p>
                        </div>
                        <div class="card-button">
                            <div class="d-flex justify-content-between align-items-center">`;
            if (new Date(article.magazines.term.startDate) <= new Date() && new Date(article.magazines.term.endDate) >= new Date() && article.status === false) {
                if (new Date(article.magazines.endDate) >= new Date()) {
                    articleCard += `<a onClick="confirmDelete(${article.articleId})" class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i> Delete</a>`;
                    articleCard += `<a href="/student/article/Update?id=${article.articleId}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Modify</a>`;
                } else {
                    articleCard += `<a href="/student/article/Update?id=${article.articleId}&status=outMagazine" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Modify</a>`;
                }

            } else {
                if (article.status === true) {
                    articleCard += `<a href="/student/article/Update?id=${article.articleId}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i>View</a>`;
                }
                else {
                    articleCard += `<span class="text-danger mx-2">Expired Term</span>`;
                }

            }

            articleCard += `</div></div></div>`;
            document.getElementById('bookList').innerHTML += articleCard;
        });
    }
    else {
        let articleCard = `<div>No Article Here</div>`;
        document.getElementById('bookList').innerHTML += articleCard;
    }
}

function updatePagination(currentStatus, currentPage, totalPages) {
    let paginationContainers = document.querySelectorAll('.pagination');

    paginationContainers.forEach(paginationContainer => {
        paginationContainer.innerHTML = '';

        let prevPageButton = document.createElement('a');
        prevPageButton.href = '#';
        prevPageButton.textContent = '<<';
        prevPageButton.addEventListener('click', function () {
            if (currentPage > 1) {
                loadArticlesByStatus(currentStatus, { currentTarget: document.getElementById(currentStatus) }, currentPage - 1);
            }
        });
        paginationContainer.appendChild(prevPageButton);

        for (let i = 1; i <= totalPages; i++) {
            let pageLink = document.createElement('a');
            pageLink.href = '#';
            pageLink.textContent = i;
            if (i === currentPage) {
                pageLink.className = 'active';
            }
            pageLink.addEventListener('click', function () {
                loadArticlesByStatus(currentStatus, { currentTarget: document.getElementById(currentStatus) }, i);
            });
            paginationContainer.appendChild(pageLink);
        }

        // Add next page button
        let nextPageButton = document.createElement('a');
        nextPageButton.href = '#';
        nextPageButton.textContent = '>>';
        nextPageButton.addEventListener('click', function () {
            if (currentPage < totalPages) {
                loadArticlesByStatus(currentStatus, { currentTarget: document.getElementById(currentStatus) }, currentPage + 1);
            }
        });
        paginationContainer.appendChild(nextPageButton);
    });
}


document.addEventListener("DOMContentLoaded", function () {
    loadArticlesByStatus('All', { currentTarget: document.getElementById('All') });
});