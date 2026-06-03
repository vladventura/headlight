// https://youmightnotneedjquery.com/#ready
function ready(fn) {
    if (document.readyState !== 'loading') {
        fn();
    } else {
        document.addEventListener('DOMContentLoaded', fn);
    }
}

let currentPage = 1;
let observer;

const paginationSetup = () => {
    observer = new IntersectionObserver(e => {
        e.forEach(entr => {
            if (entr.isIntersecting) {
                currentPage += 1;
                const queryParams = (location.search ? location.search + "&" : "?") + `incomingPage=${currentPage}&handler=Rows`;
                const url = `${location.origin}${location.pathname}${queryParams}`;
                fetch(url).then(r => r.text()).then(html => {
                    if (html.trim()) {
                        const table = document.getElementById("searchable-table");
                        const tbody = table.getElementsByTagName("tbody")[0];
                        if (tbody) {
                            tbody.innerHTML += html;
                        }
                    } else {
                        document.getElementById("sentinel").remove();
                        observer.disconnect();
                    }
                });
            }
        });
        }, {
            root: document.getElementById("searchable-table").parentElement,
            rootMargin: "0px 0px 100px 0px"
        }
    );
    observer.observe(document.getElementById("sentinel"));
};


const main = () => {
    const typeAhead = new TypeAhead("tableitems", "searchInput", "searchable-table");
    typeAhead.loadTypeAhead();
    if (document.getElementById("sentinel")) {
        paginationSetup();
    }
};

ready(main);
