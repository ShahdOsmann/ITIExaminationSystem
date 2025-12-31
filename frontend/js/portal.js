const student = JSON.parse(localStorage.getItem("student"));
if (!student) {
    alert("Please login first");
    window.location.href = "../index.html";
}
document.getElementById("generateExamBtn").addEventListener("click", function () {
    const courseName = document.getElementById("courseSelect").value;
    const numMCQ = 2;
    const numTF = 2;

    const student = JSON.parse(localStorage.getItem("student"));
    if (!student) {
        alert("Please login first!");
        return;
    }

    window.location.href =
        `exam.html?courseName=${courseName}&numMCQ=${numMCQ}&numTF=${numTF}&studentId=${student.id}`;
});

if (student) {
    document.querySelector(".navbar-nav .nav-item:first-child strong")
        .textContent = student.fName;
}
const logoutBtn = document.getElementById("logoutBtn");

if (logoutBtn) {
    logoutBtn.addEventListener("click", () => {
        localStorage.removeItem("student");

        window.location.href = "../index.html";
    });
}
