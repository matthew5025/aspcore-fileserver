api_url = window.location.origin + "/api/Files"

function onSubmitClick() {
    uploadFile();
    return false;
}

function updateProgress(evt) {
    if (evt.lengthComputable) {
        let progress = Math.ceil(((evt.loaded + evt.loaded) / evt.total) * 50);
        console.log(progress);
        let progressBar = document.getElementById("progressBar");
        progressBar.style.width = progress.toString(10) + "%";
        progressBar.innerText = progress.toString(10) + "%";
        if (progress === 100) {
            progressBar.innerText = "Processing file...";
        }
    }
}

function uploadFile() {
    let fileInput = document.getElementById('customFile');
    fileInput.disabled = true;
    let submitBtn = document.getElementById("submitBtn");
    submitBtn.disabled = true;
    let singleDownloadCheckBox = document.getElementById("singleDownloadCheck");
    singleDownloadCheckBox.disabled = true;
    let file = fileInput.files[0];
    let xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            console.log(xhr.responseText);
            let response = JSON.parse(xhr.responseText);
            console.log(response);
            let fileId = response["fileId"];
            let fileUrl = api_url + "/" + fileId;
            let qrCodeDiv = document.getElementById("qrcode");
            qrCodeDiv.innerHTML = "";
            new QRCode(qrCodeDiv, {
                text: fileUrl
            });
            document.getElementById("fileUrl").value = fileUrl;
            let qrImage = qrCodeDiv.getElementsByTagName("img")[0];
            qrImage.style.marginLeft = "auto";
            qrImage.style.marginRight = "auto";
            let modalBtn = document.getElementById('mBtn');
            modalBtn.click();
            fileInput.value = null;
            fileInput.disabled = false;
            submitBtn.disabled = false;
            singleDownloadCheckBox.disabled = false;
            let progressBar = document.getElementById("progressBar");
            progressBar.style.width = 0 + "%";
            progressBar.innerText = "";


        }
    }
    xhr.open('POST', api_url);
    xhr.setRequestHeader("FileName", file.name);
    xhr.setRequestHeader("SingleDownload", singleDownloadCheckBox.checked);
    xhr.setRequestHeader("Content-Type", " application/octet-stream")
    xhr.upload.onprogress = updateProgress;
    xhr.send(file);
}

function copyText() {
    let copyText = document.getElementById("fileUrl");
    copyText.select();
    copyText.setSelectionRange(0, 99999); /*For mobile devices*/
    document.execCommand("copy");
    let button = document.getElementById("button-addon2");
    button.innerText = 'Copied!';
}

function resetButtonText() {
    let button = document.getElementById("button-addon2");
    button.innerText = 'Copy URL';

}
