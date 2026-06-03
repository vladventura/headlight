let db;
let collection;

class TypeAhead {
    constructor(dbName, searchBar, table, rowIndexToSearch = 0) {
        this.dbName = dbName;
        this.searchTarget = rowIndexToSearch;
        this.searchBarElement = document.getElementById(searchBar);
        this.tableElement = document.getElementById(table);

        db = new ForerunnerDB().db(dbName);
        collection = db
            .collection(dbName + "-collection")
            .deferredCalls(false);
        collection.remove({});
        collection.save();
    }

    _onSearchBarInput(event) {
        const lowerCaseValue = event.target.value.toLocaleLowerCase();

        if (!lowerCaseValue) {
            collection.find({}).forEach(r => {
                document.getElementById(r["_id"]).style.display = "table-row";
            });
            return;
        }
        let r = new RegExp(lowerCaseValue, "i");

        const includes = collection.find({
            "searchTarget": r
        });
        const notIncludes = collection.find({
            $not: {
                "searchTarget": r
            }
        });

        includes.forEach(r => {
            document.getElementById(r["_id"]).style.display = "table-row";
        });
        notIncludes.forEach(r => {
            document.getElementById(r["_id"]).style.display = "none";
        });
    }

    loadTypeAhead() {
        const tableRows = Array.from(this.tableElement
            .getElementsByTagName("tbody")[0]
            .getElementsByTagName("tr"));
        const rowCount = Number(this.tableElement.dataset.rowcount);
        const itemsForCollection = rowCount <= 0? [] :
            tableRows.map(tr => {
                return {
                    _id: tr.id,
                    searchTarget: tr.cells[this.searchTarget].innerText
                };
            });
        collection.insert(itemsForCollection);
        if (rowCount > 0) {
            this.searchBarElement
                .addEventListener("input", this._onSearchBarInput);
        }
    }
}

window.TypeAhead = TypeAhead;
