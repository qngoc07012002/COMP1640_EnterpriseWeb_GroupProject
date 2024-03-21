CKEDITOR.replace('content');
let files = [];
let filesDelete = [];
function openFileUploader() {
    document.getElementById('file-input').click();
}

document.getElementById('file-input').addEventListener('change', handleFileSelect);

function handleFileSelect(event) {
    const selectedFiles = event.target.files;
    for (let i = 0; i < selectedFiles.length; i++) {
        const file = selectedFiles[i];
        const fileName = file.name;
        const fileType = fileName.split('.').pop().toUpperCase();
        if (['DOC', 'DOCX', 'PDF', 'CSV'].includes(fileType)) {
            files.push(file);
            addUploadItem(file);
        }
    }
}

function addUploadItem(file) {
    const uploadContainer = document.getElementById('upload-container');
    const uploadItem = document.createElement('div');
    uploadItem.classList.add('upload-item');
    uploadItem.innerHTML = `
                <span>${file.name}</span>
              `;
    uploadContainer.appendChild(uploadItem);
    uploadItem.addEventListener('click', function () {
        cancelUpload(file, uploadItem);
    });
}

function cancelUpload(file, uploadItem) {
    const index = files.indexOf(file);
    if (index !== -1) {
        files.splice(index, 1);
    }
    uploadItem.remove();
}

var x, i, j, l, ll, selElmnt, a, b, c;
/* Look for any elements with the class "custom-select": */
x = document.getElementsByClassName("custom-select");
l = x.length;
for (i = 0; i < l; i++) {
    selElmnt = x[i].getElementsByTagName("select")[0];
    ll = selElmnt.length;
    /* For each element, create a new DIV that will act as the selected item: */
    a = document.createElement("DIV");
    a.setAttribute("class", "select-selected");
    a.innerHTML = selElmnt.options[selElmnt.selectedIndex].innerHTML;
    x[i].appendChild(a);
    /* For each element, create a new DIV that will contain the option list: */
    b = document.createElement("DIV");
    b.setAttribute("class", "select-items select-hide");
    for (j = 1; j < ll; j++) {
        /* For each option in the original select element,
        create a new DIV that will act as an option item: */
        c = document.createElement("DIV");
        c.innerHTML = selElmnt.options[j].innerHTML;
        c.addEventListener("click", function (e) {
            /* When an item is clicked, update the original select box,
            and the selected item: */
            var y, i, k, s, h, sl, yl;
            s = this.parentNode.parentNode.getElementsByTagName("select")[0];
            sl = s.length;
            h = this.parentNode.previousSibling;
            for (i = 0; i < sl; i++) {
                if (s.options[i].innerHTML == this.innerHTML) {
                    s.selectedIndex = i;
                    h.innerHTML = this.innerHTML;
                    y = this.parentNode.getElementsByClassName("same-as-selected");
                    yl = y.length;
                    for (k = 0; k < yl; k++) {
                        y[k].removeAttribute("class");
                    }
                    this.setAttribute("class", "same-as-selected");
                    break;
                }
            }
            h.click();
        });
        b.appendChild(c);
    }
    x[i].appendChild(b);
    a.addEventListener("click", function (e) {
        /* When the select box is clicked, close any other select boxes,
        and open/close the current select box: */
        e.stopPropagation();
        closeAllSelect(this);
        this.nextSibling.classList.toggle("select-hide");
        this.classList.toggle("select-arrow-active");
    });
}
function confirmAndRemove(resourceId) {
    var result = confirm("Do you want to delete this file ?");
    if (result) {
        removeResource(resourceId);
    }
}
function removeResource(resourceId) {
    var element = document.getElementById(resourceId);
    element.remove();
    filesDelete.push(resourceId);
    var filesDeleteInput = document.getElementById('filesDeleteInput');
    filesDeleteInput.value = filesDelete;
}
function closeAllSelect(elmnt) {
    /* A function that will close all select boxes in the document,
    except the current select box: */
    var x, y, i, xl, yl, arrNo = [];
    x = document.getElementsByClassName("select-items");
    y = document.getElementsByClassName("select-selected");
    xl = x.length;
    yl = y.length;
    for (i = 0; i < yl; i++) {
        if (elmnt == y[i]) {
            arrNo.push(i)
        } else {
            y[i].classList.remove("select-arrow-active");
        }
    }
    for (i = 0; i < xl; i++) {
        if (arrNo.indexOf(i)) {
            x[i].classList.add("select-hide");
        }
    }
}

var loadFile = function (event) {
    var reader = new FileReader();
    reader.onload = function () {
        var output = document.getElementById('output');
        output.src = reader.result;
    };
    reader.readAsDataURL(event.target.files[0]);
};

/* If the user clicks anywhere outside the select box,
then close all select boxes: */
document.addEventListener("click", closeAllSelect);

function submitComment(event) {

    var comment = document.querySelector('.comment-input textarea').value;
    var urlParams = new URLSearchParams(window.location.search);
    var articleId = parseInt(urlParams.get('id'));

    // Chuyển đổi đối tượng JSON sang chuỗi query string
    var formData = `CommentInput=${encodeURIComponent(comment)}&articleId=${encodeURIComponent(articleId)}`;

    // Gửi request POST tới API endpoint
    fetch('https://localhost:7112/Student/Comment/UploadPrivate', {
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