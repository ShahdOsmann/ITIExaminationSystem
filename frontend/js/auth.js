const signupForm = document.querySelector("#signupModal form");

signupForm.addEventListener("submit", async (e) => {
  e.preventDefault();

  const dto = {
    FName: document.getElementById("signupFName").value,
    LName: document.getElementById("signupLName").value,
    Email: document.getElementById("signupEmail").value,
    Password: document.getElementById("signupPassword").value,
    Age: 20,             
    PocketMoney: 1000,   
    GPA: 3,              
    BId: document.getElementById("branchId").value,              
    TrackId: document.getElementById("trackId").value          
  };

  try {
    const res = await fetch("http://localhost:5010/api/student/signup", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(dto)
    });
    const data = await res.json();
    if (res.ok) {
      alert(data.message);
      const signupModal = bootstrap.Modal.getInstance(document.getElementById('signupModal'));
      signupModal.hide();
      const loginModal = new bootstrap.Modal(document.getElementById('loginModal'));
      loginModal.show();
    } else {
      alert(data.error);
    }
  } catch (err) {
    alert("Error: " + err.message);
  }
});

const loginForm = document.querySelector("#loginModal form");

loginForm.addEventListener("submit", async (e) => {
  e.preventDefault();

  const dto = {
    Email: document.getElementById("loginEmail").value,
    Password: document.getElementById("loginPassword").value
  };

  try {
    const res = await fetch("http://localhost:5010/api/student/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(dto)
    });

    const data = await res.json();

    if (res.ok) {
      localStorage.setItem("student", JSON.stringify(data));
      window.location.href = "html/portal.html";
    } else {
      alert(data.error);
    }
  } catch (err) {
    alert("Error: " + err.message);
  }
});
