export function fetchActiveHunts() {
    return fetchFromApi("hunts?active=true");
}

export function fetchHunt(id) {
    return fetchFromApi("hunts/" + id);
}

export function completeHunt(id) {
    return postToApi(`hunts/${id}/completed`)
}

export function createHunt(game, type, target) {
    return postToApi("hunts", { game, type, target });
}

export function addEncounters(huntId, count) {
    return postToApi(`hunts/${huntId}/+?count=${count}`);
}

async function postToApi(relative, body) {
    const response = await fetch("https://localhost:7178/" + relative, { body: JSON.stringify(body ?? ""), method: "POST", headers: { "Content-Type": "application/json" } })
    const contentType = response.headers.get("content-type");
    if (contentType && contentType.indexOf("application/json") !== -1) {

        return await response.json();
    }
    return await response.text();
}

async function fetchFromApi(relative) {
    var response = await fetch("https://localhost:7178/" + relative)
    return await response.json();
}
