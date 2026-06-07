// https://youmightnotneedjquery.com/#ready
function ready(fn) {
    if (document.readyState !== 'loading') {
        fn();
    } else {
        document.addEventListener('DOMContentLoaded', fn);
    }
}

const updateCheckMain = async () => {
    await fetch("/api/version", { cache: "default" })
        .then(r => r.json())
        .then(j => {
            if (!j["current"]) {
                return;
            }
            const mainFooter = document.getElementById("mainFooter");
            if (mainFooter) {
                const newVersionSpan = document.createElement("span");
                newVersionSpan.id = "newVersion";
                const newVersionA = document.createElement("a");
                newVersionA.href = `https://github.com/vladventura/headlight`;
                newVersionA.innerText = `Update found~! v${j["current"]}`;
                newVersionSpan.appendChild(newVersionA);
                mainFooter.appendChild(newVersionSpan);
            }
        });
};

ready(updateCheckMain);
