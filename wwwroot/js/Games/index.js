const forerunnerDb = new ForerunnerDB();
const db = forerunnerDb.db("games-page");
const collection = db.collection("games").deferredCalls(false);

function onSearchInput() {
    const lowerCaseValue = this.value.toLocaleLowerCase();
    if (!lowerCaseValue) {
        collection.find({}).forEach(r => {
            document.getElementById(r["_id"]).style.display = "table-row";
        });
        return;
    }
    let r = new RegExp(lowerCaseValue, "i");
    const includes = collection.find({
        "name": r
    });
    const notIncludes = collection.find({
        $not: {
            "name": r
        }
    });

    includes.forEach(r => {
        document.getElementById(r["_id"]).style.display = "table-row";
    });
    notIncludes.forEach(r => {
        document.getElementById(r["_id"]).style.display = "none";
    });
}

// https://youmightnotneedjquery.com/#ready
function ready(fn) {
    if (document.readyState !== 'loading') {
        fn();
    } else {
        document.addEventListener('DOMContentLoaded', fn);
    }
}

const fillForerunnerDb = () => {
    const itemsForCollection = [];
    const tableRows = Array.from(targetTable.getElementsByTagName("tbody")[0].getElementsByTagName("tr"));
    tableRows.forEach(tr => {
        itemsForCollection.push({
            _id: tr.id,
            name: tr.cells[0].innerText
        });
    });
    collection.insert(itemsForCollection);
};

const main = () => {
    const targetTable = document.getElementById("games-table");
    fillForerunnerDb();
    document
        .getElementById("searchInput")
        .addEventListener("input", onSearchInput);
};

ready(main);
