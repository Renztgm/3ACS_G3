let payrollRecords = [];
function showStep2FromForm() {
    event.preventDefault();
    fetchWorkedHours();
    return false;   
}

function fetchWorkedHours() {
    const startDate = document.getElementById("startDate").value;
    const endDate = document.getElementById("endDate").value;
    const cycle = document.getElementById("cycle").value;
    const data = { startDate, endDate, cycle };

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    fetch('/TestPayroll?handler=GetWorkedHours', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify({ startDate, endDate, cycle })
    })

        .then(response => {
            if (!response.ok) {
                throw new Error("Server error: " + response.statusText);
            }
            return response.json();
        })        
        .then(data => {
            if (data.length === 0) {
                console.log("There are no data in the array");
            } else {
                console.log("Array has data");
                console.log("Received payroll data:", data);
            }
            payrollRecords = data; // Save globally
            try {
                if (!Array.isArray(data) || data.length === 0) {
                    document.getElementById("workedHoursData").style.display = "none";
                    document.getElementById("noDataMessage").style.display = "block";
                } else {
                    const tbody = document.getElementById("employeeWorkedHoursBody");
                    tbody.innerHTML = '';
                    data.forEach(row => {
                        const tr = document.createElement("tr");
                        tr.innerHTML = `
                        <td>${row.employeeID}</td>
                        <td>${row.fullName}</td>
                        <td>${Number(row.totalWorkedHours).toFixed(2)} Hrs/${Number(row.scheduledWorkHours).toFixed(2)} Hrs</td>
                        <td>${Number(row.overtimeHours).toFixed(2)}</td>
                        <td>${Number(row.leaves).toFixed(2)}</td>
                        <td>${Number(row.bonus).toFixed(2)}</td>
                        <td>${Number(row.grossSalary).toFixed(2)}</td>
                        <td style="display:none;">${Number(row.sss).toFixed(2)}</td>
                        <td style="display:none;">${Number(row.pagibig).toFixed(2)}</td>
                        <td style="display:none;">${Number(row.philhealth).toFixed(2)}</td>
                        <td style="display:none;">${Number(row.tin).toFixed(2)}</td>
                        <td style="display:none;">${Number(row.hmo).toFixed(2)}</td>
                         <td style="display:none;">${Number(row.loanDeduction).toFixed(2)}</td>
                        <td style="display:none;">${Number(row.totalDeductions).toFixed(2)}</td>
                        <td style="display:none;">${Number(row.netSalary).toFixed(2)}</td>
                    `;
                        tbody.appendChild(tr);
                    });
                    document.getElementById("workedHoursData").style.display = "block";
                    document.getElementById("noDataMessage").style.display = "none";
                    showStep2();
                    //renderStep3EmployeeTable(data); // <-- Step 3
                    generateStep4Summary(data);      // <-- Step 4 this will call the method step 4
                    return data; // <-- ADD THIS LINE!
                }
            } catch (innerErr) {
                console.error("Error inside then block:", innerErr);
            }
        })
        .catch(err => {
            document.getElementById("errorMessage").innerText = err.message;
            document.getElementById("errorMessage").style.display = "block";
        });
}


function showStep1() {
    document.getElementById("step1").classList.add("active");
    document.getElementById("step2").classList.remove("active");
    document.getElementById("step3").classList.remove("active");
    document.getElementById("step4").classList.remove("active");
    updateProgress(25, "Step 1 of 4");
}

function showStep2() {
    console.log("Switching to Step 2");
    const step1 = document.getElementById("step1");
    const step2 = document.getElementById("step2");
    const step3 = document.getElementById("step3");
    const step4 = document.getElementById("step4");

    // Check if the elements exist to avoid errors
    if (step1 && step2 && step3 && step4) {
        // Change the 'active' class to show Step 2
        step1.classList.remove("active");
        step2.classList.add("active");
        step3.classList.remove("active");
        step4.classList.remove("active")

        // Switch content visibility for steps
        document.getElementById("step1").classList.remove("active");
        document.getElementById("step2").classList.add("active");
        document.getElementById("step3").classList.remove("active");
        document.getElementById("step4").classList.remove("active");

        // Update the progress bar (this part is optional)
        updateProgress(50, "Step 2 of 4");
    } else {
        console.error("One or more step elements not found!");
    }
}


function showStep3() {
    const data = document.querySelectorAll("#employeeWorkedHoursBody tr");
    const tbody = document.getElementById("overtimeSummaryBody");
    tbody.innerHTML = ''; // Clear previous data

    if (data.length === 0) {
        document.getElementById("noOvertimeDataMessage").style.display = "block";
        document.getElementById("overtimeSummaryData").style.display = "none";
        document.getElementById("step4").style.display = "none";
        return;
    }

    data.forEach(row => {
        const cells = row.children;
        const tr = document.createElement("tr");

        const employeeName = cells[1].textContent;
        const totalWorkedHours = cells[2].textContent;
        const overtimeHours = cells[3].textContent;

        const sssDeduction = Number(cells[7].textContent);
        const pagibigDeduction = Number(cells[8].textContent);
        const philhealthDeduction = Number(cells[9].textContent);
        const tinDeduction = Number(cells[10].textContent);
        const hmoDeduction = Number(cells[11].textContent);
        const loanDeduction = Number(cells[12].textContent);
        const totalDeductions = Number(cells[13].textContent);
        const netSalary = Number(cells[14].textContent);

        // Fetch loan deduction from the payrollRecords array (which contains all records)
        const employeeID = cells[0].textContent; // Assuming employee ID is in the first cell
        const record = payrollRecords.find(r => r.employeeID === employeeID);
        

        tr.innerHTML = `
            <td>${employeeName}</td>
            <td>${totalWorkedHours}</td>
            <td>${overtimeHours}</td>
            <td>${sssDeduction.toFixed(2)}</td>
            <td>${pagibigDeduction.toFixed(2)}</td>
            <td>${philhealthDeduction.toFixed(2)}</td>
            <td>${tinDeduction.toFixed(2)}</td>
            <td>${hmoDeduction.toFixed(2)}</td>
            <td>${loanDeduction.toFixed(2)}</td>
            <td>${totalDeductions.toFixed(2)}</td>
            <td>${netSalary.toFixed(2)}</td>
        `;

        tbody.appendChild(tr);
    });



    document.getElementById("step1").classList.remove("active");
    document.getElementById("step2").classList.remove("active");
    document.getElementById("step3").classList.add("active");
    document.getElementById("step4").classList.remove("active");

    const step1 = document.getElementById("step1");
    const step2 = document.getElementById("step2");
    const step3 = document.getElementById("step3");
    const step4 = document.getElementById("step4");

    // Check if the elements exist to avoid errors
    if (step1 && step2 && step3 && step4) {
        // Change the 'active' class to show Step 3
        step1.classList.remove("active");
        step2.classList.remove("active");
        step3.classList.add("active");
        step4.classList.remove("active")

        // Switch content visibility for steps
        document.getElementById("step1").classList.remove("active");
        document.getElementById("step2").classList.remove("active");
        document.getElementById("step3").classList.add("active");
        document.getElementById("step4").classList.remove("active");

        // Update the progress bar (this part is optional)
        updateProgress(75, "Step 3 of 4");
        console.log("the if else statement step3 is working");
    } else {
        console.error("One or more step elements not found!");
    }

    updateProgress(75, "Step 3 of 4");
    console.log("Step3 pressed!");
}


function showStep4() {
    document.getElementById("step1").classList.remove("active");
    document.getElementById("step2").classList.remove("active");
    document.getElementById("step3").classList.remove("active");
    document.getElementById("step4").classList.add("active");
    generateStep4Summary(payrollRecords); // <-- Now generate the summary here

    updateProgress(100, "Step 4 of 4");
    console.log("Step4 pressed!");
}

function generateStep4Summary(records) {
    const tbody = document.getElementById('summaryTableBody');
    tbody.innerHTML = ''; // Clear previous content

    records.forEach(record => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${record.fullName}</td>
            <td>${Number(record.grossSalary).toFixed(2)}</td>
            <td>${Number(record.totalDeductions).toFixed(2)}</td>
            <td>${Number(record.netSalary).toFixed(2)}</td>
        `;
        tbody.appendChild(tr);
    });
}
        


function updateProgress(percent, label) {
    const bar = document.getElementById("progress-bar");
    bar.style.width = `${percent}%`;
    bar.setAttribute("aria-valuenow", percent);
    bar.innerText = label;
}

function finishProcess() {
    alert("Payroll processing completed!");
    showStep1(); // Reset if desired
}

async function savePayroll() {
    if (payrollRecords.length === 0) {
        alert('No payroll data to save!');
        return;
    }

    // Build the array you want to send
    const payrollData = payrollRecords.map(record => ({
        EmployeeID: record.employeeID,
        PayrollCycle: document.getElementById("cycle").value,
        PayrollStartDate: document.getElementById("startDate").value,
        PayrollEndDate: document.getElementById("endDate").value,
        TotalWorkedHours: record.totalWorkedHours,
        OvertimeHours: record.overtimeHours,    
        Leaves: record.leaves,
        Bonus: record.bonus,
        GrossSalary: record.grossSalary,
        NetSalary: record.netSalary,
        TotalDeductions: record.totalDeductions,
        SSS: record.sss,
        Pagibig: record.pagibig,
        Philhealth: record.philhealth,
        TIN: record.tin,
        HMO: record.hmo,
        LoanDeduction: record.loanDeduction,
        AttendanceID: 6, // <--- TEMPORARY fixed if you are testing (or find actual AttendanceID)
        LeaveID: 1,      // <--- TEMPORARY or real leave ID
        LoanID: 1        // <--- TEMPORARY or real loan ID
    }));

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value; // ADD THIS TOO!

    const response = await fetch('/TestPayroll?handler=SavePayroll', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify(payrollData)

    });

    const result = await response.json();
    if (result.success) {
        console.log(result); 
        alert('Payroll saved successfully!');
        showStep1(); // Reset if desired
    } else {
        
        alert('Failed to save payroll.');
    }
}


