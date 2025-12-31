const student = JSON.parse(localStorage.getItem("student"));

if (!student) {
    alert("Please login first");
    window.location.href = "../index.html";
}

 
const params = new URLSearchParams(window.location.search);
const courseName = params.get("courseName");
const numMCQ = params.get("numMCQ");
const numTF = params.get("numTF");
const studentId = params.get("studentId");
if (!studentId) {
    alert("Session expired. Please login again.");
    window.location.href = "../index.html";
}

const container = document.getElementById("examContainer");
const timerEl = document.getElementById("timer");

let currentExamId = null;

fetch(`http://localhost:5010/api/exam/generate?courseName=${courseName}&numMCQ=${numMCQ}&numTF=${numTF}`)
    .then(res => res.json())
    .then(data => {

        if (!Array.isArray(data) || data.length === 0) {
            container.innerHTML = `<div class="alert alert-warning">No questions available.</div>`;
            return;
        }

        currentExamId = data[0].examId;

        data.forEach(q => {

            const qDiv = document.createElement("div");
            qDiv.className = "question-box mb-4 p-4 border rounded shadow-sm";
            qDiv.dataset.questionId = q.questionId;

            qDiv.innerHTML = `<h5 class="mb-3">${q.content} (${q.points} pts)</h5>`;

            q.choices.forEach(c => {
                qDiv.innerHTML += `
                    <div class="form-check mb-2">
                        <input class="form-check-input" type="radio"
                               name="q${q.questionId}"
                               value="${c.choiceId}">
                        <label class="form-check-label">
                            ${c.choiceContent}
                        </label>
                    </div>
                `;
            });

            container.appendChild(qDiv);
        });
    })
    .catch(err => {
        console.error(err);
        container.innerHTML = `<div class="alert alert-danger">Error loading exam</div>`;
    });

const EXAM_DURATION_MINUTES = 60;
let remainingTime = EXAM_DURATION_MINUTES * 60;

const timerInterval = setInterval(() => {
    const minutes = Math.floor(remainingTime / 60);
    const seconds = remainingTime % 60;

    timerEl.textContent = `${minutes}:${seconds.toString().padStart(2, "0")}`;

    if (remainingTime <= 0) {
        clearInterval(timerInterval);
        alert("Time is up! Exam will be submitted automatically.");
        submitExam();
    }

    remainingTime--;
}, 1000);

document.getElementById("submitExamBtn").addEventListener("click", submitExam);

function submitExam() {

    clearInterval(timerInterval);

    const answers = [];

    document.querySelectorAll(".question-box").forEach(qBox => {
        const qId = qBox.dataset.questionId;
        const selected = qBox.querySelector("input[type='radio']:checked");
        if (selected) {
            answers.push({
                questionId: parseInt(qId),
                choiceId: parseInt(selected.value)
            });
        }
    });

    fetch("http://localhost:5010/api/exam/submit", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            studentId: parseInt(studentId),
            examId: currentExamId,
            answers: answers
        })
    })
    .then(res => res.json())
    .then(data => {
        window.location.href =
            `exam-result.html?studentId=${studentId}&examId=${currentExamId}&grade=${data.grade}`;
    })
    .catch(err => console.error(err));
}

