var _exchange_rate = sessionStorage.getItem('GBPUSD');
var _val = (Math.round(_exchange_rate * 100) / 100).toFixed(2);

var fxChartData = {
    series: [_val],
    chart: {
        height: 350,
        type: 'radialBar',
        offsetY: 0
    },
    colors: ['#ff1500', '#0011ff'],
    plotOptions: {
        radialBar: {
            startAngle: -135,
            endAngle: 135,
            dataLabels: {
                name: {
                    fontSize: '16px',
                    color: undefined,
                    offsetY: 120
                },
                value: {
                    offsetY: 76,
                    fontSize: '22px',
                    color: undefined,
                    formatter: function (val) {
                        return `£${_val} - $1`;
                    }
                }
            }
        }
    },
    fill: {
        type: 'gradient',
        gradient: {
            shade: 'dark',
            shadeIntensity: 0.15,
            inverseColors: false,
            opacityFrom: 1,
            opacityTo: 1,
            stops: [0, 50, 65, 91]
        },
    },
    stroke: {
        dashArray: 4
    },
    labels: ['GBP - USD'],
};

var fxChart = new ApexCharts(document.querySelector("#chart6"), fxChartData);
fxChart.render();

var transactionOverviewGraphData = document.querySelector("#graphDataValues").value.split(',');

var options = {
    series: [
        {
            name: "Transactions",
            data: transactionOverviewGraphData
        }
    ],
    chart: {
        height: 300,
        type: 'line',
        zoom: {
            enabled: false,
        },
        dropShadow: {
            enabled: true,
            color: '#000',
            top: 18,
            left: 7,
            blur: 16,
            opacity: 0.2
        },
        toolbar: {
            show: false
        }
    },
    colors: ['#255cd3'],
    dataLabels: {
        enabled: false,
    },
    stroke: {
        width: [3, 3],
        curve: 'smooth'
    },
    grid: {
        show: false,
    },
    markers: {
        colors: ['#255cd3'],
        size: 5,
        strokeColors: '#ffffff',
        strokeWidth: 2,
        hover: {
            sizeOffset: 2
        }
    },
    xaxis: {
        categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        labels: {
            style: {
                colors: '#8c9094'
            }
        }
    },
    yaxis: {
        min: 0,
        max: 35,
        labels: {
            style: {
                colors: '#8c9094'
            }
        }
    },
    legend: {
        position: 'top',
        horizontalAlign: 'right',
        floating: true,
        offsetY: 0,
        labels: {
            useSeriesColors: true
        },
        markers: {
            width: 10,
            height: 10,
        }
    }
};


var chart = new ApexCharts(document.querySelector("#activities-chart"), options);
chart.render();
