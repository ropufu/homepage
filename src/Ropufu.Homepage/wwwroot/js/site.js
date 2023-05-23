// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function hideElementById(id) { document.getElementById(id).classList.add("visually-hidden"); }

function unhideElementById(id) { document.getElementById(id).classList.remove("visually-hidden"); }

function invalidateElementById(id, isValid) {
    let x = document.getElementById(id);
    if (isValid) {
        x.classList.remove("is-invalid");
        x.classList.add("is-valid");
    }
    else {
        x.classList.remove("is-valid");
        x.classList.add("is-invalid");
    }
}

function prettyNumber(x) {
    switch (x) {
        case Number.NEGATIVE_INFINITY:
            return "-&infin;";
        case Number.POSITIVE_INFINITY:
            return "&infin;";
    }
    return Number.isNaN(x) ? "??" : x;
}

function parseNumber(x) {
    switch (typeof x) {
        case "string":
            let trimmed = x.trim();
            if (trimmed.length == 0)
                return Number.NaN;
            if (trimmed === String.fromCharCode(8734))
                return Number.POSITIVE_INFINITY;
            if (trimmed.length == 2 && trimmed[0] == "-" && trimmed.charCodeAt(1) == 8734)
                return Number.NEGATIVE_INFINITY;
            return Number(x);
        case "number":
            return x;
        case "object":
            if (x instanceof Number)
                return Number(x);
            return parseNumber(String(x));
        // "boolean", "function", "undefined"
        default:
            return Number.NaN;
    }
}

function invalidateVisibilityByCategory(categoryIds, categoryCounts, fullContainerElementId, emptyContainerElementId) {
    let total = 0;
    for (let i = 0; i < categoryIds.length; ++i) {
        total += categoryCounts[i];
        if (categoryCounts[i] == 0)
            hideElementById(categoryIds[i]);
        else
            unhideElementById(categoryIds[i]);
    }
    if (total == 0) {
        hideElementById(fullContainerElementId);
        unhideElementById(emptyContainerElementId);
    } else {
        unhideElementById(fullContainerElementId);
        hideElementById(emptyContainerElementId);
    }
}

function pageSearchFilter(item, searchTerms, categoryCounts) {
    let isMatch = false;
    let totalLength = 0;
    searchTerms.forEach(function (term) {
        // Ignore empty search terms.
        if (term.length == 0)
            return;
        totalLength += term.length;
        item.keywords.forEach(function (key) {
            if (key.startsWith(term))
                isMatch = true;
        });
    });
    // Display everything if all terms were empty.
    isMatch = isMatch || (totalLength == 0);
    if (!isMatch)
        hideElementById(item.id);
    else {
        unhideElementById(item.id);
        ++categoryCounts[item.cat];
    }
}

function enablePageSearch(categoryIds, categoryCounts, itemsList, fullContainerElementId, emptyContainerElementId) {
    document.addEventListener("DOMContentLoaded", function () {
        unhideElementById("navbar-form");
        let searchBox = document.getElementById("navbar-search");
        searchBox.oninput = function () {
            let searchTerms = searchBox.value.toLowerCase().split(" ");
            categoryCounts.fill(0);
            for (let i = 0; i < itemsList.length; ++i)
                pageSearchFilter(itemsList[i], searchTerms, categoryCounts);
            invalidateVisibilityByCategory(categoryIds, categoryCounts, fullContainerElementId, emptyContainerElementId);
        };
        searchBox.focus();
    });
}

function truncate(element, a, b) {
    let number = parseNumber(element.value);
    if (number < a) {
        number = a;
        element.value = a;
    }
    if (number > b) {
        number = b;
        element.value = b;
    }
    return number;
}
