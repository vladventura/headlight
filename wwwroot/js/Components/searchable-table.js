// https://youmightnotneedjquery.com/#ready
function ready(fn) {
    if (document.readyState !== 'loading') {
        fn();
    } else {
        document.addEventListener('DOMContentLoaded', fn);
    }
}

const main = () => {
    const typeAhead = new TypeAhead("tableitems", "searchInput", "searchable-table");
    typeAhead.loadTypeAhead();
};

ready(main);
