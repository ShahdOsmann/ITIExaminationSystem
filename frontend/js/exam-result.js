const params = new URLSearchParams(window.location.search);

const studentId = params.get("studentId");
const examId = params.get("examId");
const grade = params.get("grade");
if (!studentId || !examId) {
    alert("Invalid exam result access.");
    window.location.href = "../index.html";
}

document.getElementById("finalGrade").textContent =
    `Final Grade: ${grade}%`;

fetch(`http://localhost:5010/api/exam/review?studentId=${studentId}&examId=${examId}`)
    .then(res => res.json())
    .then(data => {

        if (!Array.isArray(data)) {
            console.error("Invalid response:", data);
            return;
        }

        const container = document.getElementById("reviewContainer");
        container.innerHTML = "";

        data.forEach((q, index) => {

            const card = document.createElement("div");
            card.className = "review-card p-3 mb-4 border rounded";

            let html = `<h5>Q${index + 1}. ${q.question}</h5>`;

            q.choices.forEach(c => {
                let cls = "choice-item";

                if (c.isCorrect) cls += " choice-correct";
                if (q.studentChoiceId === c.choiceId && !c.isCorrect)
                    cls += " choice-wrong";

                html += `<div class="${cls}">${c.text}</div>`;
            });

            card.innerHTML = html;
            container.appendChild(card);
        });
    })
    .catch(err => console.error(err));
