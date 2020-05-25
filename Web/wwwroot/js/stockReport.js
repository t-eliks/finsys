function drawGraph(charDataArray, name, company){

    var charDataJsonObject = JSON.parse(charDataArray);
    var charData = [["Data", name]];

    for(i = 0;i < charDataJsonObject.length;i++){
        charData.push([charDataJsonObject[i]['item1'].substring(0,10), charDataJsonObject[i]['item2']]);
    }

    google.charts.load('current', {'packages':['corechart']});
    google.charts.setOnLoadCallback(drawChart);

    function drawChart() {

        var data = google.visualization.arrayToDataTable(charData);

        var options = {
            legend: { position: 'bottom' }
        };

        var chart = new google.visualization.LineChart(document.getElementById(name+company));

        chart.draw(data, options);

    }
}