let currentPage = 1;
let observer;
let sentinelCopy;

let isSortable = false;
let isPaginateable = false;

const getIsSortable = () => isSortable;
const setIsSortable = (b) => isSortable = b;

// Yes this is ridiculous
const arrowUpSvg =
    `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-arrow-up-short" viewBox="0 0 16 16">
  <path fill-rule="evenodd" d="M8 12a.5.5 0 0 0 .5-.5V5.707l2.146 2.147a.5.5 0 0 0 .708-.708l-3-3a.5.5 0 0 0-.708 0l-3 3a.5.5 0 1 0 .708.708L7.5 5.707V11.5a.5.5 0 0 0 .5.5"/>
</svg>`;
const arrowDownSvg =
    `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-arrow-down-short" viewBox="0 0 16 16">
  <path fill-rule="evenodd" d="M8 4a.5.5 0 0 1 .5.5v5.793l2.146-2.147a.5.5 0 0 1 .708.708l-3 3a.5.5 0 0 1-.708 0l-3-3a.5.5 0 1 1 .708-.708L7.5 10.293V4.5A.5.5 0 0 1 8 4"/>
</svg>`;

let currentHeader = "";
let sortDirection = "";

const getCurrentHeader = () => currentHeader;
const getSortDirection = () => sortDirection;

const setCurrentHeader = (h) => {
    currentHeader = h;
};

const setSortDirection = (sd) => {
    sortDirection = sd;
};

// https://youmightnotneedjquery.com/#ready
function ready(fn) {
    if (document.readyState !== 'loading') {
        fn();
    } else {
        document.addEventListener('DOMContentLoaded', fn);
    }
}

const destroyObserver = () => {
    if (observer) {
        observer.disconnect();
    }
    if (document.getElementById("sentinel")) {
        document.getElementById("sentinel").remove();
    }
};

const clearData = () => {
    destroyObserver();
    currentPage = 0;
    const table = document.getElementById("searchable-table");
    const tbody = table.getElementsByTagName("tbody")[0];
    if (tbody) {
        tbody.innerHTML = "";
        table.scrollTop = 0;
    }
    if (isPaginateable) {
        const tableParent = table.parentElement;
        if (!tableParent.querySelector("#sentinel")) {
            let newSentinel = document.createElement("div");
            newSentinel.id = "sentinel";
            newSentinel.className = "loading-sentinel";
            newSentinel.innerText = "...";
            tableParent.appendChild(newSentinel);
        }
    }
};

const getSortQueryString = () => { 
    if (!getIsSortable() || !getCurrentHeader()) {
        return "";
    }
    let sortField = getCurrentHeader().split('-')[0];
    return `sortField=${sortField}&sortDirection=${getSortDirection()}`
};

const pullData = () => {
    currentPage += 1;
    let sortQueryString = getSortQueryString();
    if (sortQueryString) {
        sortQueryString = "&" + sortQueryString;
    }
    let queryParams = (location.search ? location.search + "&" : "?") + `incomingPage=${currentPage}&handler=Rows${sortQueryString}`;
    const url = `${location.origin}${location.pathname}${queryParams}`;
    fetch(url).then(r => r.text()).then(html => {
        if (html.trim()) {
            const table = document.getElementById("searchable-table");
            const tbody = table.getElementsByTagName("tbody")[0];
            if (tbody) {
                tbody.innerHTML += html;
            }
        } else {
            destroyObserver();
        }
    });
};

const paginationSetup = () => {
    isPaginateable = true;
    const searchableTable = document.getElementById("searchable-table");
    const rowCount = Number(searchableTable.dataset.rowcount ?? 0);
    if (rowCount < 50) {
        destroyObserver();
        return;
    }
    observer = new IntersectionObserver(e => {
        e.forEach(entr => {
            if (entr.isIntersecting) {
                pullData();
            }
        });
        }, {
            root: document.getElementById("searchable-table").parentElement,
            rootMargin: "0px 0px 100px 0px"
        }
    );
    observer.observe(document.getElementById("sentinel"));
};

const sortSetup = () => {
    setIsSortable(true);
    sentinelCopy = document.getElementById("sentinel").cloneNode();
    [... document.getElementsByClassName("clickable-header")]
        .forEach(div => {
            div.addEventListener("click", function () {
                let activeHeader = getCurrentHeader();
                let activeSortDirection = getSortDirection();
                let arrowEl = document.getElementById(this.id + "-arrow");
                if (activeHeader == this.id) {
                    switch (activeSortDirection) {
                        case "Asc":
                            setSortDirection("Desc");
                            arrowEl.innerHTML = arrowDownSvg;
                            break;
                        case "Desc":
                            setSortDirection("");
                            setCurrentHeader("");
                            arrowEl.innerHTML = "";
                            break;
                        default:
                            setSortDirection("Asc");
                            arrowEl.innerHTML = arrowUpSvg;
                            break;
                    }
                } else {
                    if (getCurrentHeader()) {
                        document.getElementById(getCurrentHeader() + "-arrow").innerHTML = "";
                    }
                    setCurrentHeader(this.id);
                    setSortDirection("Asc");
                    arrowEl.innerHTML = arrowUpSvg;
                }
                clearData();
                paginationSetup();
            });
        });
};

const main = () => {
    if (document.getElementById("sentinel")) {
        paginationSetup();
    }
    if (document.getElementsByClassName("clickable-header").length > 0) {
        sortSetup();
    }
};

ready(main);
