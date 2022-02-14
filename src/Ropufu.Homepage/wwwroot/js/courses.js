$.getJSON(courseApiPath, function (data) {
    var cy = cytoscape({
        container: document.getElementById('cy'),
        boxSelectionEnabled: false,
        autounselectify: true,
        hideEdgesOnViewport: true,
        minZoom: 0.1,
        maxZoom: 2.0,
        wheelSensitivity: 0.2,
        ready: function () {
            this.nodes().forEach(function (node) {
                let p = 1 - Math.exp(-0.2 * node.data("weight"));
                let width = 200 * (1 - p) + 400 * (p);
                let height = 200 * (1 - p) + 400 * (p);
                node.css("width", width);
                node.css("height", height);
            });

            this.layout({
                name: 'cose',
                animate: false,
                fit: true,
                numIter: 2000,
                idealEdgeLength: 100,
                nodeOverlap: 10
            }).run();
        },
        style: [
            {
                selector: 'node',
                style: {
                    'height': 200,
                    'width': 200,
                    'background-color': 'data(background)',
                    'border-width': 1,
                    'border-color': 'black',
                    'label': 'data(name)',
                    'text-wrap': 'wrap',
                    'font-size': 20,
                    'text-valign': 'center',
                    'text-halign': 'center',
                    'color': 'black',
                    'min-zoomed-font-size': 5
                }
            },
            {
                selector: 'edge',
                style: {
                    'curve-style': 'straight',
                    'width': 5,
                    'line-color': '#ffaaaa',
                    'target-arrow-shape': 'vee',
                    'target-arrow-color': '#ffaaaa',
                    'arrow-scale': 2
                }
            }
        ],
        elements: data
    });
});
