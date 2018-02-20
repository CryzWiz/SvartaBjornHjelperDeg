$(function() {

    Morris.Area({
        element: 'morris-area-chart',
        data: [{
            period: '2018 Q1',
            Samtaler: 2666,
            Chatbot: null,
            Manuelle: 2647
        }, {
            period: '2018 Q2',
            Samtaler: 2778,
            Chatbot: 2294,
            Manuelle: 2441
        }, {
            period: '2018 Q3',
            Samtaler: 4912,
            Chatbot: 1969,
            Manuelle: 2501
        }, {
            period: '2018 Q4',
            Samtaler: 3767,
            Chatbot: 3597,
            Manuelle: 5689
        }, {
            period: '2019 Q1',
            Samtaler: 6810,
            Chatbot: 1914,
            Manuelle: 2293
        }, {
            period: '2019 Q2',
            Samtaler: 5670,
            Chatbot: 4293,
            Manuelle: 1881
        }, {
            period: '2019 Q3',
            Samtaler: 4820,
            Chatbot: 3795,
            Manuelle: 1588
        }, {
            period: '2019 Q4',
            Samtaler: 15073,
            Chatbot: 5967,
            Manuelle: 5175
        }, {
            period: '2020 Q1',
            Samtaler: 10687,
            Chatbot: 4460,
            Manuelle: 2028
        }, {
            period: '2020 Q2',
            Samtaler: 8432,
            Chatbot: 5713,
            Manuelle: 1791
        }],
        xkey: 'period',
        ykeys: ['Samtaler', 'Chatbot', 'Manuelle'],
        labels: ['Samtaler', 'Chatbot', 'Manuelle'],
        pointSize: 2,
        hideHover: 'auto',
        resize: true
    });

    Morris.Donut({
        element: 'morris-donut-chart',
        data: [{
            label: "Standard svar",
            value: 12
        }, {
            label: "Chatbot",
            value: 30
        }, {
            label: "Manuell Chat",
            value: 20
        }],
        resize: true
    });

    Morris.Bar({
        element: 'morris-bar-chart',
        data: [{
            y: 'User 1',
            a: 100,
            b: 90
        }, {
            y: 'User 2',
            a: 75,
            b: 65
        }, {
            y: 'User 3',
            a: 50,
            b: 40
        }, {
            y: 'User 4',
            a: 75,
            b: 65
        }, {
            y: 'User 5',
            a: 50,
            b: 40
        }, {
            y: 'User 6',
            a: 75,
            b: 65
        }, {
            y: 'User 7',
            a: 100,
            b: 90
        }],
        xkey: 'y',
        ykeys: ['a', 'b'],
        labels: ['Løst', 'Ikke løst'],
        hideHover: 'auto',
        resize: true
    });
    
});
