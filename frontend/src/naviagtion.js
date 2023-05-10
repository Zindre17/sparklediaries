import { writable } from "svelte/store";

export const pathStore = writable(getCurrent(), init);

function getCurrent() {
    return location.pathname;
}

function init(setter) {
    const setCurrent = () => setter(getCurrent());

    addEventListener("popstate", setCurrent, false);
    addEventListener("hashchange", setCurrent, false);

    return () => {
        removeEventListener("popstate", setCurrent, false);
        removeEventListener("hashchange", setCurrent, false);
    };
}

export function navigateTo(targetPath, replace = false) {
    let title = document.title;
    let state = null;

    if (replace) {
        history.replaceState(state, title, targetPath);
    } else {
        history.pushState(state, title, targetPath);
    }
    pathStore.set(getCurrent());
}

export function getPathPieces(path) {
    return path.split("/").filter((i) => i !== "");
}

export function matches(a, b) {
    let aPieces = getPathPieces(a);
    let bPieces = getPathPieces(b);

    if (aPieces.length !== bPieces.length) {
        return false;
    }
    for (let i = 0; i < aPieces.length; i++) {
        if (isWildCard(aPieces[i]) || isWildCard(bPieces[i])) {
            continue;
        }
        if (aPieces[i] !== bPieces[i]) {
            return false;
        }
    }

    return true;
}

export function getWildCardValues(template, actual) {
    let tempPieces = getPathPieces(template);
    let actualPieces = getPathPieces(actual);

    let result = {}

    let wildCardIndices = getWildCards(template);
    for (let i = 0; i < wildCardIndices.length; i++) {
        let index = wildCardIndices[i];
        result[tempPieces[index].slice(1)] = actualPieces[index];
    }

    return result;
}

export function getWildCards(template) {
    let pieces = getPathPieces(template);
    let result = [];
    for (let i = 0; i < pieces.length; i++) {
        isWildCard(pieces[i]) && result.push(i)
    }
    return result;
}

export function hasWildCard(template) {
    return getPathPieces(template).some(isWildCard);
}

export function isWildCard(piece) {
    return piece.startsWith("$")
}

export function cleanPath(path) {
    return getPathPieces(path).join("/");
}

export function createRelativePath(root, relative) {
    return "/" + cleanPath(root) + "/" + cleanPath(relative);
}
