const labels = [
    'January',
    'February',
    'March',
    'April',
    'May',
    'June',
];

const data = {
    labels: labels,
    datasets: [{
        label: 'My First dataset',
        backgroundColor: 'rgb(255, 99, 132)',
        borderColor: 'rgb(255, 99, 132)',
        data: [0, 10, 5, 2, 20, 30, 45],
    }]
};

const config = {
    type: 'line',
    data: data,
    options: {}
};

function InvokeTicketChart(){
    //Invoke myChart
    var chartExist = Chart.getChart("myChart"); // <canvas> id
    if (chartExist != undefined)
        chartExist.destroy();
    
    const myChart = new Chart(
        document.getElementById('myChart'),
        config
    );
    return true;
}

function ticketByDate(start, end){
    var canvasName = "ticket-by-date";
    chartDestroy(canvasName);
    
    // Bar chart
    new Chart(document.getElementById(canvasName), {
        type: 'bar',
        data: {
            labels: ["28-Mar","29-Mar","30-Mar", "31-Mar", "01-June", "02-June", "03-June"],
            datasets: [
                {
                    label: "Ticket (Counts)",
                    backgroundColor: ["#ffce56", "#ff6384", "#3e95cd", "#8e5ea2","#3cba9f","#e8c3b9","#c45850"],
                    data: [80,68,25,57,50,25,38]
                }
            ]
        },
        options: {
            legend: { display: false },
            title: {
                display: true,
                text: 'Predicted world population (millions) in 2050'
            }
        }
    });
    console.log("Start " + start +" End "+ end);
    return true;
}

function chartDestroy(canvasName){
    var chartExist = Chart.getChart(canvasName); // <canvas> id
    if (chartExist != undefined)
        chartExist.destroy();
}

function salesByStatus(start, end){    
    var canvasName = "sales-by-date";
    chartDestroy(canvasName);
    // Bar chart
    new Chart(document.getElementById(canvasName), {
        type: 'bar',
        data: {
            labels: [
                "SRG","ARG","JO","ALAM","Priti",
                "Now","MICRO","LIS","LIFE","TIME",
                "SATH","PREE","Mar","RAVI","GOWRI",
                "RAJ","VINO", "LAJ", "June", "KART"],
            datasets: [
                {
                    label: "Sales (Rupees)",
                    backgroundColor: [
                        "#ffce56", "#ff6384", "#3e95cd","#8e5ea2","#3cba9f",
                        "#8e5ea2","#3cba9f","#e8c3b9","#c45850", "#8e5ea2",
                        "#e8c3b9","#ffce56", "#ff6384", "#3e95cd", "#8e5ea2",
                        "#8e5ea2","#3cba9f","#e8c3b9","#c45850", "#8e5ea2"],
                    data: [
                        25,35,15,15, 80,
                        38,68,25,5,19,
                        68,25,57,50,25,
                        38,68,25,5,19
                    ]
                }
            ]
        },
        options: {
            legend: { display: false },
            title: {
                display: true,
                text: 'Predicted world population (millions) in 2050'
            }
        }
    });
    console.log("Start " + start +" End "+ end);
    return true;
}
