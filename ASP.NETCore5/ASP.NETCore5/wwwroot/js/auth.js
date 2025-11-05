document.addEventListener("DOMContentLoaded", function () {
    const authArea = document.getElementById("authArea");
    updateAuthUI();

    function updateAuthUI() {
        const token = sessionStorage.getItem("jwtToken");
        const username = sessionStorage.getItem("username");

        if (token && username) {
            authArea.innerHTML = `
                <span class="me-3 text-success">Hello, ${username}!</span>
                <button id="logoutBtn" class="btn btn-outline-danger btn-sm">Đăng xuất</button>
            `;
            document.getElementById("logoutBtn").addEventListener("click", function () {
                if (confirm("Bạn có chắc muốn đăng xuất không?")) {
                    sessionStorage.clear();
                    location.href = "/Account/Login";
                }
            });
        } else {
            authArea.innerHTML = `<a href="/Account/Login" class="btn btn-primary btn-sm">Đăng nhập</a>`;
        }
    }

    window.jwtLogin = async function (username, password) {
        try {
            const response = await fetch("/api/jwtauth/login2", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ UserName: username, Password: password })
            });

            if (!response.ok) {
                const error = await response.text();
                alert("Đăng nhập thất bại: " + error);
                return;
            }

            const data = await response.json();
            sessionStorage.setItem("jwtToken", data.token);
            sessionStorage.setItem("username", data.username);

            alert("Đăng nhập thành công!");
            location.href = "/";
        } catch (err) {
            console.error(err);
            alert("Lỗi hệ thống khi đăng nhập");
        }
    };
});
