
// Get all user from api

document.addEventListener("DOMContentLoaded", () => {
    // Fetch user data from the API
    fetch("https://localhost:7133/GetUsers")
        .then(response => response.json())
        .then(data => {
            const userTableBody = document.getElementById("userTableBody");
            // Iterate through the data and create table rows
            data.forEach(user => {
                const row = document.createElement("tr");
                row.innerHTML = `
                    <td>${user.name}</td>
                    <td>${user.emailId}</td>
                    <td>${user.mobileNumber}</td>
                    <td>${user.userId}</td>
                `;
                userTableBody.appendChild(row);
            });
        })
        .catch(error => console.error("Error:", error));
});


// Add New user and check Validation

