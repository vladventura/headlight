let downloadedData = {};
let cleanData = [[], [], []];

const onSelectChange = () => {
    loadChart();
    refreshFinishedBanner();
}

const getRandomGameData = () => {
    fetch('api/Stats/RandomGame', { cache: "default" })
        .then(r => r.json())
        .then(j => {
            setRandomGame(j);
        });
}

const setRandomGame = (randomGame) => {
    if (randomGame) {
        const { name, platform, status, id } = randomGame;
        document.getElementById("randomgame-name-link").innerText = name;
        document.getElementById("randomgame-name-link").href = '/Games/' + id;
        const description = `Enjoyed best on ${platform} - ${status}`;
        document.getElementById("randomgame-description").innerText = description;
    }
}

const refreshFinishedBanner = () => {
    const chosenKey = document.getElementById("timeFrameSelect").value;
    const { data } = cleanData[chosenKey];
    const finished = data.find(a => a["groupId"] == 3);
    document.getElementById("finished-banner").innerText = finished["value"];
    document.getElementById("finished-description").innerText = finished["originalName"] + " Games";
}

const getChartData = async () => {
    return fetch('/api/Stats')
        .then(r => r.json())
        .then(r => downloadedData = r);
}

const cleanChartData = () => {
    downloadedData["statsTimeFrames"].forEach(stats => {
        const count = stats["statusAnalytics"].map(a => a["count"]).reduce((a, c) => a + c);
        const data = [];
        stats["statusAnalytics"].forEach(a => {
            let pieData = {
                value: a["count"],
                name: a["status"] + "\n" + a["count"],
                groupId: a["statusId"],
                originalName: a["status"]
            };
            data.push(pieData);
        });
        cleanData[stats["timeFrameKey"]] = { count, data };
    });
}

const loadChart = () => {
    const doughnutChartEl = document.getElementById("echarts-doughnut");
    let doughnutChart = echarts.init(doughnutChartEl);
    const chosenKey = document.getElementById("timeFrameSelect").value;
    const { data, count } = cleanData[chosenKey];

    let option = {
        title: {
            text: 'Total: ' + count,
            textStyle: {
                color: "#fff"
            }
        },

        series: [
            {
                type: 'pie',
                radius: ['50%', '70%'],
                avoidLabelOverlap: false,
                name: 'OK',
                label: {
                    show: false,
                    position: 'center'
                },
                emphasis: {
                    label: {
                        show: true,
                        fontSize: '30',
                        textStyle: {
                            color: "#fff"
                        }
                    }
                },
                data: data
            }
        ]
    };
    doughnutChart.setOption(option);
}

const main = async () => {
    getRandomGameData();
    await getChartData();
    cleanChartData();
    loadChart();
    refreshFinishedBanner();
}

main();
